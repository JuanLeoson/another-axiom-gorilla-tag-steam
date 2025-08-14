using System;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x04000EA7 RID: 3751
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x04000EA8 RID: 3752
	public Transform colorPoint;

	// Token: 0x020001F1 RID: 497
	public enum ColorMode
	{
		// Token: 0x04000EAA RID: 3754
		None,
		// Token: 0x04000EAB RID: 3755
		Red,
		// Token: 0x04000EAC RID: 3756
		Green,
		// Token: 0x04000EAD RID: 3757
		Blue,
		// Token: 0x04000EAE RID: 3758
		Black,
		// Token: 0x04000EAF RID: 3759
		Clear
	}
}
