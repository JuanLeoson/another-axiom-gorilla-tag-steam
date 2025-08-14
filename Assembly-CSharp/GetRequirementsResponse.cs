using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Token: 0x020008BA RID: 2234
[Serializable]
public class GetRequirementsResponse
{
	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x060037D2 RID: 14290 RVA: 0x0012091F File Offset: 0x0011EB1F
	// (set) Token: 0x060037D3 RID: 14291 RVA: 0x00120927 File Offset: 0x0011EB27
	[JsonProperty("age")]
	public int? Age { get; set; }

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x060037D4 RID: 14292 RVA: 0x00120930 File Offset: 0x0011EB30
	// (set) Token: 0x060037D5 RID: 14293 RVA: 0x00120938 File Offset: 0x0011EB38
	public int? PlatformMinimumAge { get; set; }

	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x060037D6 RID: 14294 RVA: 0x00120941 File Offset: 0x0011EB41
	// (set) Token: 0x060037D7 RID: 14295 RVA: 0x00120949 File Offset: 0x0011EB49
	[JsonProperty("ageStatus")]
	public SessionStatus AgeStatus { get; set; }

	// Token: 0x17000566 RID: 1382
	// (get) Token: 0x060037D8 RID: 14296 RVA: 0x00120952 File Offset: 0x0011EB52
	// (set) Token: 0x060037D9 RID: 14297 RVA: 0x0012095A File Offset: 0x0011EB5A
	[JsonProperty("digitalContentAge")]
	public int DigitalConsentAge { get; set; }

	// Token: 0x17000567 RID: 1383
	// (get) Token: 0x060037DA RID: 14298 RVA: 0x00120963 File Offset: 0x0011EB63
	// (set) Token: 0x060037DB RID: 14299 RVA: 0x0012096B File Offset: 0x0011EB6B
	[JsonProperty("minimumAge")]
	public int MinimumAge { get; set; }

	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x060037DC RID: 14300 RVA: 0x00120974 File Offset: 0x0011EB74
	// (set) Token: 0x060037DD RID: 14301 RVA: 0x0012097C File Offset: 0x0011EB7C
	[JsonProperty("civilAge")]
	public int CivilAge { get; set; }

	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x060037DE RID: 14302 RVA: 0x00120985 File Offset: 0x0011EB85
	// (set) Token: 0x060037DF RID: 14303 RVA: 0x0012098D File Offset: 0x0011EB8D
	[JsonProperty("approvedAgeCollectionMethods")]
	public List<ApprovedAgeCollectionMethods> ApprovedAgeCollectionMethods { get; set; }
}
