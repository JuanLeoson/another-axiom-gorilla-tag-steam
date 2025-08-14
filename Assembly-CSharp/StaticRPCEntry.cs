using System;

// Token: 0x02000308 RID: 776
public class StaticRPCEntry
{
	// Token: 0x060012B6 RID: 4790 RVA: 0x00066DC3 File Offset: 0x00064FC3
	public StaticRPCEntry(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		this.placeholder = placeholder;
		this.code = code;
		this.lookupMethod = lookupMethod;
	}

	// Token: 0x04001A51 RID: 6737
	public NetworkSystem.StaticRPCPlaceholder placeholder;

	// Token: 0x04001A52 RID: 6738
	public byte code;

	// Token: 0x04001A53 RID: 6739
	public NetworkSystem.StaticRPC lookupMethod;
}
