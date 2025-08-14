using System;

// Token: 0x02000A4F RID: 2639
public interface IFXEffectContext<T> where T : IFXEffectContextObject
{
	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x06004085 RID: 16517
	T effectContext { get; }

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x06004086 RID: 16518
	FXSystemSettings settings { get; }
}
