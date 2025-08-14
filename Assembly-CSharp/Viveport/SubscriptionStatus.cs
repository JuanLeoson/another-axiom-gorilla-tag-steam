using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000B71 RID: 2929
	public class SubscriptionStatus
	{
		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06004623 RID: 17955 RVA: 0x0015CD22 File Offset: 0x0015AF22
		// (set) Token: 0x06004624 RID: 17956 RVA: 0x0015CD2A File Offset: 0x0015AF2A
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06004625 RID: 17957 RVA: 0x0015CD33 File Offset: 0x0015AF33
		// (set) Token: 0x06004626 RID: 17958 RVA: 0x0015CD3B File Offset: 0x0015AF3B
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06004627 RID: 17959 RVA: 0x0015CD44 File Offset: 0x0015AF44
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000B72 RID: 2930
		public enum Platform
		{
			// Token: 0x04005104 RID: 20740
			Windows,
			// Token: 0x04005105 RID: 20741
			Android
		}

		// Token: 0x02000B73 RID: 2931
		public enum TransactionType
		{
			// Token: 0x04005107 RID: 20743
			Unknown,
			// Token: 0x04005108 RID: 20744
			Paid,
			// Token: 0x04005109 RID: 20745
			Redeem,
			// Token: 0x0400510A RID: 20746
			FreeTrial
		}
	}
}
