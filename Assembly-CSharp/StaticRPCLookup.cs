using System;
using System.Collections.Generic;

// Token: 0x02000309 RID: 777
public class StaticRPCLookup
{
	// Token: 0x060012B7 RID: 4791 RVA: 0x00066DE0 File Offset: 0x00064FE0
	public void Add(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		int count = this.entries.Count;
		this.entries.Add(new StaticRPCEntry(placeholder, code, lookupMethod));
		this.eventCodeEntryLookup.Add(code, count);
		this.placeholderEntryLookup.Add(placeholder, count);
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x00066E26 File Offset: 0x00065026
	public NetworkSystem.StaticRPC CodeToMethod(byte code)
	{
		return this.entries[this.eventCodeEntryLookup[code]].lookupMethod;
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00066E44 File Offset: 0x00065044
	public byte PlaceholderToCode(NetworkSystem.StaticRPCPlaceholder placeholder)
	{
		return this.entries[this.placeholderEntryLookup[placeholder]].code;
	}

	// Token: 0x04001A54 RID: 6740
	public List<StaticRPCEntry> entries = new List<StaticRPCEntry>();

	// Token: 0x04001A55 RID: 6741
	private Dictionary<byte, int> eventCodeEntryLookup = new Dictionary<byte, int>();

	// Token: 0x04001A56 RID: 6742
	private Dictionary<NetworkSystem.StaticRPCPlaceholder, int> placeholderEntryLookup = new Dictionary<NetworkSystem.StaticRPCPlaceholder, int>();
}
