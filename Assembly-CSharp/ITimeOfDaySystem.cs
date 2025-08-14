using System;

// Token: 0x020007D1 RID: 2001
public interface ITimeOfDaySystem
{
	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x0600322E RID: 12846
	double currentTimeInSeconds { get; }

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x0600322F RID: 12847
	double totalTimeInSeconds { get; }
}
