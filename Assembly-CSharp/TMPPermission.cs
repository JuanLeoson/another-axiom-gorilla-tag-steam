using System;
using Newtonsoft.Json;

// Token: 0x020008AE RID: 2222
[Serializable]
public class TMPPermission
{
	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x060037AA RID: 14250 RVA: 0x0012080C File Offset: 0x0011EA0C
	// (set) Token: 0x060037AB RID: 14251 RVA: 0x00120814 File Offset: 0x0011EA14
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x060037AC RID: 14252 RVA: 0x0012081D File Offset: 0x0011EA1D
	// (set) Token: 0x060037AD RID: 14253 RVA: 0x00120825 File Offset: 0x0011EA25
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x060037AE RID: 14254 RVA: 0x0012082E File Offset: 0x0011EA2E
	// (set) Token: 0x060037AF RID: 14255 RVA: 0x00120836 File Offset: 0x0011EA36
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
