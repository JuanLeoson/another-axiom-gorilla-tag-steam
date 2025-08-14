using System;

// Token: 0x02000A6F RID: 2671
internal interface ITickSystemPre
{
	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x0600412F RID: 16687
	// (set) Token: 0x06004130 RID: 16688
	bool PreTickRunning { get; set; }

	// Token: 0x06004131 RID: 16689
	void PreTick();
}
