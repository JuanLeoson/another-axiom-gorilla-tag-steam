using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F2D RID: 3885
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x06006053 RID: 24659 RVA: 0x001E9BD0 File Offset: 0x001E7DD0
		protected void Awake()
		{
			this.edibleState = 0;
			this.previousEdibleState = 0;
			this.ownerRig = base.GetComponentInParent<VRRig>();
			this.isLocal = (this.ownerRig != null && this.ownerRig.isOfflineVRRig);
			this.isHandSlot = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand);
			this.isLeftHand = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand);
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001E9C5C File Offset: 0x001E7E5C
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("EdibleWearable \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < this.edibleStateInfos.Length; i++)
			{
				this.edibleStateInfos[i].gameObject.SetActive(i == this.edibleState);
			}
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x001E9CD6 File Offset: 0x001E7ED6
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x001E9CF4 File Offset: 0x001E7EF4
		protected virtual void LateUpdateLocal()
		{
			if (this.edibleState == this.edibleStateInfos.Length - 1)
			{
				if (!this.isNonRespawnable && Time.time > this.lastFullyEatenTime + this.respawnTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
				}
				if (this.isNonRespawnable && Time.time > this.lastFullyEatenTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
					GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
					{
						false,
						this.isLeftHand
					});
				}
			}
			else if (Time.time > this.lastEatTime + this.biteCooldown)
			{
				Vector3 b = base.transform.TransformPoint(this.edibleBiteOffset);
				bool flag = false;
				float num = this.biteDistance * this.biteDistance;
				if (!GorillaParent.hasInstance)
				{
					return;
				}
				if ((GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - b).sqrMagnitude < num)
				{
					flag = true;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!flag)
					{
						if (vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						if ((vrrig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - b).sqrMagnitude < num)
						{
							flag = true;
						}
					}
				}
				if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
				{
					this.edibleState++;
					this.lastEatTime = Time.time;
					this.lastFullyEatenTime = Time.time;
				}
				this.wasInBiteZoneLastFrame = flag;
			}
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
		}

		// Token: 0x06006057 RID: 24663 RVA: 0x001E9F38 File Offset: 0x001E8138
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x06006058 RID: 24664 RVA: 0x001E9F68 File Offset: 0x001E8168
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x001E9F94 File Offset: 0x001E8194
		protected virtual void OnEdibleHoldableStateChange()
		{
			if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
			}
			if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.edibleState].gameObject.SetActive(true);
			}
			if (this.edibleState > 0 && this.edibleState < this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState].sound, this.volume);
			}
			if (this.edibleState == this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState - 1].sound, this.volume);
			}
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.isLocal && this.isHandSlot)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHand, amplitude, fixedDeltaTime);
			}
		}

		// Token: 0x04006BBB RID: 27579
		[Tooltip("Check when using non cosmetic edible items like honeycomb")]
		public bool isNonRespawnable;

		// Token: 0x04006BBC RID: 27580
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x04006BBD RID: 27581
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x04006BBE RID: 27582
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04006BBF RID: 27583
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x04006BC0 RID: 27584
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x04006BC1 RID: 27585
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x04006BC2 RID: 27586
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04006BC3 RID: 27587
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04006BC4 RID: 27588
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x04006BC5 RID: 27589
		private VRRig ownerRig;

		// Token: 0x04006BC6 RID: 27590
		private bool isLocal;

		// Token: 0x04006BC7 RID: 27591
		private bool isHandSlot;

		// Token: 0x04006BC8 RID: 27592
		private bool isLeftHand;

		// Token: 0x04006BC9 RID: 27593
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04006BCA RID: 27594
		private int edibleState;

		// Token: 0x04006BCB RID: 27595
		private int previousEdibleState;

		// Token: 0x04006BCC RID: 27596
		private float lastEatTime;

		// Token: 0x04006BCD RID: 27597
		private float lastFullyEatenTime;

		// Token: 0x04006BCE RID: 27598
		private bool wasInBiteZoneLastFrame;

		// Token: 0x02000F2E RID: 3886
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04006BCF RID: 27599
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04006BD0 RID: 27600
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
