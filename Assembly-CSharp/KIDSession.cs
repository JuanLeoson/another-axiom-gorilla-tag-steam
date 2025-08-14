using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x020008B0 RID: 2224
[Serializable]
public class KIDSession
{
	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x060037B2 RID: 14258 RVA: 0x0012083F File Offset: 0x0011EA3F
	// (set) Token: 0x060037B3 RID: 14259 RVA: 0x00120847 File Offset: 0x0011EA47
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x060037B4 RID: 14260 RVA: 0x00120850 File Offset: 0x0011EA50
	// (set) Token: 0x060037B5 RID: 14261 RVA: 0x00120858 File Offset: 0x0011EA58
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x060037B6 RID: 14262 RVA: 0x00120861 File Offset: 0x0011EA61
	// (set) Token: 0x060037B7 RID: 14263 RVA: 0x00120869 File Offset: 0x0011EA69
	public Guid SessionId { get; set; }

	// Token: 0x1700055B RID: 1371
	// (get) Token: 0x060037B8 RID: 14264 RVA: 0x00120872 File Offset: 0x0011EA72
	// (set) Token: 0x060037B9 RID: 14265 RVA: 0x0012087A File Offset: 0x0011EA7A
	public string KUID { get; set; }

	// Token: 0x1700055C RID: 1372
	// (get) Token: 0x060037BA RID: 14266 RVA: 0x00120883 File Offset: 0x0011EA83
	// (set) Token: 0x060037BB RID: 14267 RVA: 0x0012088B File Offset: 0x0011EA8B
	public string etag { get; set; }

	// Token: 0x1700055D RID: 1373
	// (get) Token: 0x060037BC RID: 14268 RVA: 0x00120894 File Offset: 0x0011EA94
	// (set) Token: 0x060037BD RID: 14269 RVA: 0x0012089C File Offset: 0x0011EA9C
	public List<Permission> Permissions { get; set; }

	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x060037BE RID: 14270 RVA: 0x001208A5 File Offset: 0x0011EAA5
	// (set) Token: 0x060037BF RID: 14271 RVA: 0x001208AD File Offset: 0x0011EAAD
	public DateTime DateOfBirth { get; set; }

	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x060037C0 RID: 14272 RVA: 0x001208B6 File Offset: 0x0011EAB6
	// (set) Token: 0x060037C1 RID: 14273 RVA: 0x001208BE File Offset: 0x0011EABE
	public string Jurisdiction { get; set; }
}
