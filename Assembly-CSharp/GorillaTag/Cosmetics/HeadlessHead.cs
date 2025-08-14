using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F2F RID: 3887
	public class HeadlessHead : HoldableObject
	{
		// Token: 0x0600605B RID: 24667 RVA: 0x001EA158 File Offset: 0x001E8358
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.ownerRig.isOfflineVRRig;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
			this.baseLocalPosition = base.transform.localPosition;
			this.hasFirstPersonRenderer = (this.firstPersonRenderer != null);
		}

		// Token: 0x0600605C RID: 24668 RVA: 0x001EA1DC File Offset: 0x001E83DC
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("HeadlessHead \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
		}

		// Token: 0x0600605D RID: 24669 RVA: 0x001EA235 File Offset: 0x001E8435
		private void OnDisable()
		{
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x001EA248 File Offset: 0x001E8448
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

		// Token: 0x0600605F RID: 24671 RVA: 0x001EA266 File Offset: 0x001E8466
		protected virtual void LateUpdateLocal()
		{
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
		}

		// Token: 0x06006060 RID: 24672 RVA: 0x001EA2A4 File Offset: 0x001E84A4
		protected virtual void LateUpdateReplicated()
		{
			int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
			this.isHeld = (num != 0);
			this.isHeldLeftHand = ((num & 2) != 0);
		}

		// Token: 0x06006061 RID: 24673 RVA: 0x001EA2F0 File Offset: 0x001E84F0
		protected virtual void LateUpdateShared()
		{
			if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
			{
				this.blendingFromPosition = base.transform.position;
				this.blendingFromRotation = base.transform.rotation;
				this.blendFraction = 0f;
			}
			Quaternion quaternion;
			Vector3 vector;
			if (this.isHeldLeftHand)
			{
				quaternion = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
				vector = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else if (this.isHeld)
			{
				quaternion = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
				vector = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else
			{
				quaternion = base.transform.parent.rotation;
				vector = base.transform.parent.TransformPoint(this.baseLocalPosition);
			}
			if (this.blendFraction < 1f)
			{
				this.blendFraction += Time.deltaTime / this.blendDuration;
				quaternion = Quaternion.Lerp(this.blendingFromRotation, quaternion, this.blendFraction);
				vector = Vector3.Lerp(this.blendingFromPosition, vector, this.blendFraction);
			}
			base.transform.rotation = quaternion;
			base.transform.position = vector;
			if (this.hasFirstPersonRenderer)
			{
				float x = base.transform.lossyScale.x;
				this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * x);
			}
			this.wasHeld = this.isHeld;
			this.wasHeldLeftHand = this.isHeldLeftHand;
		}

		// Token: 0x06006062 RID: 24674 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x001EA4F7 File Offset: 0x001E86F7
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			this.isHeld = true;
			this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
			EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x001EA52B File Offset: 0x001E872B
		public override void DropItemCleanup()
		{
			this.isHeld = false;
			this.isHeldLeftHand = false;
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001EA53C File Offset: 0x001E873C
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
			{
				return false;
			}
			if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
			{
				return false;
			}
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
			this.isHeld = false;
			this.isHeldLeftHand = false;
			return true;
		}

		// Token: 0x04006BD1 RID: 27601
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;

		// Token: 0x04006BD2 RID: 27602
		[SerializeField]
		private Vector3 offsetFromLeftHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04006BD3 RID: 27603
		[SerializeField]
		private Vector3 offsetFromRightHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04006BD4 RID: 27604
		[SerializeField]
		private Quaternion rotationFromLeftHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04006BD5 RID: 27605
		[SerializeField]
		private Quaternion rotationFromRightHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04006BD6 RID: 27606
		private Vector3 baseLocalPosition;

		// Token: 0x04006BD7 RID: 27607
		private VRRig ownerRig;

		// Token: 0x04006BD8 RID: 27608
		private bool isLocal;

		// Token: 0x04006BD9 RID: 27609
		private bool isHeld;

		// Token: 0x04006BDA RID: 27610
		private bool isHeldLeftHand;

		// Token: 0x04006BDB RID: 27611
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04006BDC RID: 27612
		[SerializeField]
		private MeshRenderer firstPersonRenderer;

		// Token: 0x04006BDD RID: 27613
		[SerializeField]
		private float firstPersonHiddenRadius;

		// Token: 0x04006BDE RID: 27614
		[SerializeField]
		private Transform firstPersonHideCenter;

		// Token: 0x04006BDF RID: 27615
		[SerializeField]
		private Transform holdAnchorPoint;

		// Token: 0x04006BE0 RID: 27616
		private bool hasFirstPersonRenderer;

		// Token: 0x04006BE1 RID: 27617
		private Vector3 blendingFromPosition;

		// Token: 0x04006BE2 RID: 27618
		private Quaternion blendingFromRotation;

		// Token: 0x04006BE3 RID: 27619
		private float blendFraction;

		// Token: 0x04006BE4 RID: 27620
		private bool wasHeld;

		// Token: 0x04006BE5 RID: 27621
		private bool wasHeldLeftHand;

		// Token: 0x04006BE6 RID: 27622
		[SerializeField]
		private float blendDuration = 0.3f;
	}
}
