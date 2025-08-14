using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000D62 RID: 3426
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06005508 RID: 21768 RVA: 0x001A5DD7 File Offset: 0x001A3FD7
		// (set) Token: 0x06005509 RID: 21769 RVA: 0x001A5DDF File Offset: 0x001A3FDF
		public string Title { get; set; }

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x0600550A RID: 21770 RVA: 0x001A5DE8 File Offset: 0x001A3FE8
		// (set) Token: 0x0600550B RID: 21771 RVA: 0x001A5DF0 File Offset: 0x001A3FF0
		public List<string> Entries { get; set; }
	}
}
