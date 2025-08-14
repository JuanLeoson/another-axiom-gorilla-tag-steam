using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02000EE2 RID: 3810
	[Flags]
	public enum EEdCosBrowserAntiClippingFilter
	{
		// Token: 0x040068F3 RID: 26867
		None = 0,
		// Token: 0x040068F4 RID: 26868
		NameTag = 1,
		// Token: 0x040068F5 RID: 26869
		LeftArm = 2,
		// Token: 0x040068F6 RID: 26870
		RightArm = 4,
		// Token: 0x040068F7 RID: 26871
		Chest = 8,
		// Token: 0x040068F8 RID: 26872
		HuntComputer = 16,
		// Token: 0x040068F9 RID: 26873
		Badge = 32,
		// Token: 0x040068FA RID: 26874
		BuilderWatch = 64,
		// Token: 0x040068FB RID: 26875
		All = 127
	}
}
