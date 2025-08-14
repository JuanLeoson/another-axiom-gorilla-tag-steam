using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000426 RID: 1062
public class FriendshipCharm : HoldableObject
{
	// Token: 0x060019B0 RID: 6576 RVA: 0x0008A144 File Offset: 0x00088344
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x0008A158 File Offset: 0x00088358
	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * GTPlayer.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x0008A1B0 File Offset: 0x000883B0
	public void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		this.isBroken = false;
		this.UpdatePosition();
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x0008A1D7 File Offset: 0x000883D7
	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x0008A200 File Offset: 0x00088400
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x0008A288 File Offset: 0x00088488
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x0008A2C4 File Offset: 0x000884C4
	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x0008A304 File Offset: 0x00088504
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.UpdatePosition();
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x0400220B RID: 8715
	[SerializeField]
	private InteractionPoint interactionPoint;

	// Token: 0x0400220C RID: 8716
	[SerializeField]
	private Transform rightHandHoldAnchor;

	// Token: 0x0400220D RID: 8717
	[SerializeField]
	private Transform leftHandHoldAnchor;

	// Token: 0x0400220E RID: 8718
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x0400220F RID: 8719
	[SerializeField]
	private Transform lineStart;

	// Token: 0x04002210 RID: 8720
	[SerializeField]
	private Transform lineEnd;

	// Token: 0x04002211 RID: 8721
	[SerializeField]
	private Transform releasePosition;

	// Token: 0x04002212 RID: 8722
	[SerializeField]
	private float breakBraceletLength;

	// Token: 0x04002213 RID: 8723
	[SerializeField]
	private LayerMask breakItemLayerMask;

	// Token: 0x04002214 RID: 8724
	private Transform parent;

	// Token: 0x04002215 RID: 8725
	private bool isBroken;
}
