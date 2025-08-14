using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000751 RID: 1873
public class FreeHoverboardHandle : HoldableObject
{
	// Token: 0x06002EEE RID: 12014 RVA: 0x000F8A30 File Offset: 0x000F6C30
	private void Awake()
	{
		this.hasParentBoard = (this.parentFreeBoard != null);
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000F8A44 File Offset: 0x000F6C44
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

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000F8AB4 File Offset: 0x000F6CB4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
			Quaternion rot = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 pos = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, pos, rot, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion rot2 = flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight;
		Vector3 pos2 = flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight;
		GTPlayer.Instance.GrabPersonalHoverboard(flag, pos2, rot2, VRRig.LocalRig.playerColor);
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x00002628 File Offset: 0x00000828
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04003AF0 RID: 15088
	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	// Token: 0x04003AF1 RID: 15089
	private bool hasParentBoard;

	// Token: 0x04003AF2 RID: 15090
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04003AF3 RID: 15091
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04003AF4 RID: 15092
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04003AF5 RID: 15093
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04003AF6 RID: 15094
	private int noHapticsUntilFrame = -1;
}
