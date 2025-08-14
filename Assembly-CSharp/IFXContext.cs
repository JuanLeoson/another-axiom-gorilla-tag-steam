using System;

// Token: 0x02000A4B RID: 2635
public interface IFXContext
{
	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x06004076 RID: 16502
	FXSystemSettings settings { get; }

	// Token: 0x06004077 RID: 16503
	void OnPlayFX();
}
