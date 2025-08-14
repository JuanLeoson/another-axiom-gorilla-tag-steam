using System;

// Token: 0x02000A70 RID: 2672
internal interface ITickSystemTick
{
	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x06004132 RID: 16690
	// (set) Token: 0x06004133 RID: 16691
	bool TickRunning { get; set; }

	// Token: 0x06004134 RID: 16692
	void Tick();
}
