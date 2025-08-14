using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class PropHuntGrabbableProp : HoldableObject
{
	// Token: 0x06000943 RID: 2371 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00032754 File Offset: 0x00030954
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.handFollower.SwitchHand(flag);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00032790 File Offset: 0x00030990
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x04000AE5 RID: 2789
	public PropHuntHandFollower handFollower;

	// Token: 0x04000AE6 RID: 2790
	public Vector3 offset;

	// Token: 0x04000AE7 RID: 2791
	public List<InteractionPoint> interactionPoints;
}
