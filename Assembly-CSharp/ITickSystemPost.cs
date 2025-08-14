using System;

// Token: 0x02000A71 RID: 2673
internal interface ITickSystemPost
{
	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x06004135 RID: 16693
	// (set) Token: 0x06004136 RID: 16694
	bool PostTickRunning { get; set; }

	// Token: 0x06004137 RID: 16695
	void PostTick();
}
