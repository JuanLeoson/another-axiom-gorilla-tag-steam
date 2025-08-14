using System;

// Token: 0x02000A4D RID: 2637
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x06004079 RID: 16505
	FXSystemSettings settings { get; }

	// Token: 0x0600407A RID: 16506
	void OnPlayFX(T parems);
}
