using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02000EE4 RID: 3812
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x0400690C RID: 26892
		None = 0,
		// Token: 0x0400690D RID: 26893
		NoParts = 1,
		// Token: 0x0400690E RID: 26894
		Holdable = 2,
		// Token: 0x0400690F RID: 26895
		Functional = 4,
		// Token: 0x04006910 RID: 26896
		Wardrobe = 8,
		// Token: 0x04006911 RID: 26897
		Store = 16,
		// Token: 0x04006912 RID: 26898
		FirstPerson = 32,
		// Token: 0x04006913 RID: 26899
		All = 63
	}
}
