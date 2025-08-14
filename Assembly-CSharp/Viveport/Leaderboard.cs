using System;

namespace Viveport
{
	// Token: 0x02000B70 RID: 2928
	public class Leaderboard
	{
		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x0600461C RID: 17948 RVA: 0x0015CCEF File Offset: 0x0015AEEF
		// (set) Token: 0x0600461D RID: 17949 RVA: 0x0015CCF7 File Offset: 0x0015AEF7
		public int Rank { get; set; }

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x0600461E RID: 17950 RVA: 0x0015CD00 File Offset: 0x0015AF00
		// (set) Token: 0x0600461F RID: 17951 RVA: 0x0015CD08 File Offset: 0x0015AF08
		public int Score { get; set; }

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06004620 RID: 17952 RVA: 0x0015CD11 File Offset: 0x0015AF11
		// (set) Token: 0x06004621 RID: 17953 RVA: 0x0015CD19 File Offset: 0x0015AF19
		public string UserName { get; set; }
	}
}
