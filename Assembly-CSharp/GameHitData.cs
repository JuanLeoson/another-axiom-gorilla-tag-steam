using System;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
public struct GameHitData
{
	// Token: 0x04002D70 RID: 11632
	public GameEntityId hitEntityId;

	// Token: 0x04002D71 RID: 11633
	public GameEntityId hitByEntityId;

	// Token: 0x04002D72 RID: 11634
	public int hitTypeId;

	// Token: 0x04002D73 RID: 11635
	public Vector3 hitEntityPosition;

	// Token: 0x04002D74 RID: 11636
	public Vector3 hitPosition;

	// Token: 0x04002D75 RID: 11637
	public Vector3 hitImpulse;

	// Token: 0x04002D76 RID: 11638
	public int hitAmount;
}
