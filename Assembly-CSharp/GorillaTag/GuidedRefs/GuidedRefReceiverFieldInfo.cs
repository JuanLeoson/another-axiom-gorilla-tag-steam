using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EBB RID: 3771
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x06005E26 RID: 24102 RVA: 0x001DB69C File Offset: 0x001D989C
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x04006808 RID: 26632
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x04006809 RID: 26633
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x0400680A RID: 26634
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x0400680B RID: 26635
		[NonSerialized]
		public int fieldId;
	}
}
