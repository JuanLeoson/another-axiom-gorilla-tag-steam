using System;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class SnowballGrabZone : HoldableObject
{
	// Token: 0x060005EC RID: 1516 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00022764 File Offset: 0x00020964
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		SnowballThrowable snowballThrowable;
		((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	// Token: 0x0400070D RID: 1805
	[GorillaSoundLookup]
	public int materialIndex;
}
