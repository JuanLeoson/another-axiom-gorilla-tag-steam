using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000758 RID: 1880
public class HoverboardHandle : HoldableObject
{
	// Token: 0x06002F18 RID: 12056 RVA: 0x000F95A0 File Offset: 0x000F77A0
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x000F9610 File Offset: 0x000F7810
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
		Quaternion localRotation;
		Vector3 localPosition;
		if (!this.parentVisual.IsHeld)
		{
			localRotation = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
			localPosition = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		}
		else
		{
			localRotation = transform.InverseTransformRotation(this.parentVisual.transform.rotation);
			localPosition = transform.InverseTransformPoint(this.parentVisual.transform.position);
		}
		this.parentVisual.SetIsHeld(flag, localPosition, localRotation, this.parentVisual.boardColor);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x000F96E9 File Offset: 0x000F78E9
	public override void DropItemCleanup()
	{
		if (this.parentVisual.gameObject.activeSelf)
		{
			this.parentVisual.DropFreeBoard();
		}
		this.parentVisual.SetNotHeld();
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x000F9714 File Offset: 0x000F7914
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
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.parentVisual.IsLeftHanded);
		this.parentVisual.SetNotHeld();
		return true;
	}

	// Token: 0x04003B1B RID: 15131
	[SerializeField]
	private HoverboardVisual parentVisual;

	// Token: 0x04003B1C RID: 15132
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04003B1D RID: 15133
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04003B1E RID: 15134
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04003B1F RID: 15135
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04003B20 RID: 15136
	private int noHapticsUntilFrame = -1;
}
