using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x020008C0 RID: 2240
public class VerifyAgeResponse
{
	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x060037E6 RID: 14310 RVA: 0x00120996 File Offset: 0x0011EB96
	// (set) Token: 0x060037E7 RID: 14311 RVA: 0x0012099E File Offset: 0x0011EB9E
	public SessionStatus Status { get; set; }

	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x060037E8 RID: 14312 RVA: 0x001209A7 File Offset: 0x0011EBA7
	// (set) Token: 0x060037E9 RID: 14313 RVA: 0x001209AF File Offset: 0x0011EBAF
	[Nullable(2)]
	public Session Session { [NullableContext(2)] get; [NullableContext(2)] set; }

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x060037EA RID: 14314 RVA: 0x001209B8 File Offset: 0x0011EBB8
	// (set) Token: 0x060037EB RID: 14315 RVA: 0x001209C0 File Offset: 0x0011EBC0
	public KIDDefaultSession DefaultSession { get; set; }
}
