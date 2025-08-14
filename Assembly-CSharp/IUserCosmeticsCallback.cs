using System;

// Token: 0x02000A5D RID: 2653
internal interface IUserCosmeticsCallback
{
	// Token: 0x060040AE RID: 16558
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x17000621 RID: 1569
	// (get) Token: 0x060040AF RID: 16559
	// (set) Token: 0x060040B0 RID: 16560
	bool PendingUpdate { get; set; }
}
