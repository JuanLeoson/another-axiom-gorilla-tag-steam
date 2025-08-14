using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EC3 RID: 3779
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x04006815 RID: 26645
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x04006816 RID: 26646
		public int fieldId;

		// Token: 0x04006817 RID: 26647
		public int index;
	}
}
