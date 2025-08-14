using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
[Serializable]
public class GestureNode
{
	// Token: 0x04000C69 RID: 3177
	public bool track;

	// Token: 0x04000C6A RID: 3178
	public GestureHandState state;

	// Token: 0x04000C6B RID: 3179
	public GestureDigitFlexion flexion;

	// Token: 0x04000C6C RID: 3180
	public GestureAlignment alignment;

	// Token: 0x04000C6D RID: 3181
	[Space]
	public GestureNodeFlags flags;
}
