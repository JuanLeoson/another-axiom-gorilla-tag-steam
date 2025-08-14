using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x020008A5 RID: 2213
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x060037A5 RID: 14245 RVA: 0x001207EA File Offset: 0x0011E9EA
	// (set) Token: 0x060037A6 RID: 14246 RVA: 0x001207F2 File Offset: 0x0011E9F2
	public List<Permission> Permissions { get; set; }

	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x060037A7 RID: 14247 RVA: 0x001207FB File Offset: 0x0011E9FB
	// (set) Token: 0x060037A8 RID: 14248 RVA: 0x00120803 File Offset: 0x0011EA03
	public AgeStatusType AgeStatus { get; set; }
}
