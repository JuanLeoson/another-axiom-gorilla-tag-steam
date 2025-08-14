using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000436 RID: 1078
public class DrumsItem : MonoBehaviour, ISpawnable
{
	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0008C7FA File Offset: 0x0008A9FA
	// (set) Token: 0x06001A44 RID: 6724 RVA: 0x0008C802 File Offset: 0x0008AA02
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001A45 RID: 6725 RVA: 0x0008C80B File Offset: 0x0008AA0B
	// (set) Token: 0x06001A46 RID: 6726 RVA: 0x0008C813 File Offset: 0x0008AA13
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001A47 RID: 6727 RVA: 0x0008C81C File Offset: 0x0008AA1C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
		for (int j = 0; j < this.drumsAS.Length; j++)
		{
			this.myRig.AssignDrumToMusicDrums(j + this.onlineOffset, this.drumsAS[j]);
		}
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x0008C8C4 File Offset: 0x0008AAC4
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x0008C8EC File Offset: 0x0008AAEC
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, QueryTriggerInteraction.Collide);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x0008CB07 File Offset: 0x0008AD07
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x0008CB30 File Offset: 0x0008AD30
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				NetworkView netView = this.myRig.netView;
				if (netView != null)
				{
					netView.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].GTPlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x0400227F RID: 8831
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x04002280 RID: 8832
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x04002281 RID: 8833
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x04002282 RID: 8834
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x04002283 RID: 8835
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x04002284 RID: 8836
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x04002285 RID: 8837
	private bool rightHandIn;

	// Token: 0x04002286 RID: 8838
	private bool leftHandIn;

	// Token: 0x04002287 RID: 8839
	private float volToPlay;

	// Token: 0x04002288 RID: 8840
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04002289 RID: 8841
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x0400228A RID: 8842
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x0400228B RID: 8843
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x0400228C RID: 8844
	public LayerMask drumsTouchable;

	// Token: 0x0400228D RID: 8845
	private float sphereRadius;

	// Token: 0x0400228E RID: 8846
	private Vector3 spherecastSweep;

	// Token: 0x0400228F RID: 8847
	private int collidersHitCount;

	// Token: 0x04002290 RID: 8848
	private List<RaycastHit> hitList = new List<RaycastHit>(20);

	// Token: 0x04002291 RID: 8849
	private Drum tempDrum;

	// Token: 0x04002292 RID: 8850
	private bool drumHit;

	// Token: 0x04002293 RID: 8851
	private RaycastHit nullHit;

	// Token: 0x04002294 RID: 8852
	public int onlineOffset;

	// Token: 0x04002295 RID: 8853
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	private VRRig myRig;
}
