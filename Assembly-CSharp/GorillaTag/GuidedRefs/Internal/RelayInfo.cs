using System;
using System.Collections.Generic;

namespace GorillaTag.GuidedRefs.Internal
{
	// Token: 0x02000EC5 RID: 3781
	public class RelayInfo
	{
		// Token: 0x0400681B RID: 26651
		[NonSerialized]
		public IGuidedRefTargetMono targetMono;

		// Token: 0x0400681C RID: 26652
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> registeredFields;

		// Token: 0x0400681D RID: 26653
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> resolvedFields;
	}
}
