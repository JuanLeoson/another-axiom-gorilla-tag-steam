using System;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class HoldableHandle : InteractionPoint
{
	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001A6D RID: 6765 RVA: 0x0008D7F7 File Offset: 0x0008B9F7
	public new HoldableObject Holdable
	{
		get
		{
			return this.holdable;
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001A6E RID: 6766 RVA: 0x0008D7FF File Offset: 0x0008B9FF
	public CapsuleCollider Capsule
	{
		get
		{
			return this.handleCapsuleTrigger;
		}
	}

	// Token: 0x040022C3 RID: 8899
	[SerializeField]
	private HoldableObject holdable;

	// Token: 0x040022C4 RID: 8900
	[SerializeField]
	private CapsuleCollider handleCapsuleTrigger;
}
