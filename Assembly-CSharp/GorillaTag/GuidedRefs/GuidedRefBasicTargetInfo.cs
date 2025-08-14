using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EB4 RID: 3764
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x040067F7 RID: 26615
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040067F8 RID: 26616
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO[] hubIds;

		// Token: 0x040067F9 RID: 26617
		[DebugOption]
		[SerializeField]
		public bool hackIgnoreDuplicateRegistration;
	}
}
