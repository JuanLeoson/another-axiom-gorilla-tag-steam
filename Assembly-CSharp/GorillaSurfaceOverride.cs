using System;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x04002717 RID: 10007
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x04002718 RID: 10008
	public float extraVelMultiplier = 1f;

	// Token: 0x04002719 RID: 10009
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x0400271A RID: 10010
	[HideInInspector]
	[NonSerialized]
	public float slidePercentageOverride = -1f;

	// Token: 0x0400271B RID: 10011
	public bool sendOnTapEvent;

	// Token: 0x0400271C RID: 10012
	public bool disablePushBackEffect;
}
