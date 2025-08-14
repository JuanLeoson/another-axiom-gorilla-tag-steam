using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EC4 RID: 3780
	public struct GuidedRefTryResolveInfo
	{
		// Token: 0x04006818 RID: 26648
		public int fieldId;

		// Token: 0x04006819 RID: 26649
		public int index;

		// Token: 0x0400681A RID: 26650
		[FormerlySerializedAs("target")]
		public IGuidedRefTargetMono targetMono;
	}
}
