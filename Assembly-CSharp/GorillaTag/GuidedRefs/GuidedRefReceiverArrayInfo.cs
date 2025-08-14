using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EC2 RID: 3778
	[Serializable]
	public struct GuidedRefReceiverArrayInfo
	{
		// Token: 0x06005E4B RID: 24139 RVA: 0x001DB752 File Offset: 0x001D9952
		public GuidedRefReceiverArrayInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targets = Array.Empty<GuidedRefTargetIdSO>();
			this.hubId = null;
			this.fieldId = 0;
			this.resolveCount = 0;
		}

		// Token: 0x04006810 RID: 26640
		[Tooltip("Controls whether the array should be overridden by the guided refs.")]
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x04006811 RID: 26641
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x04006812 RID: 26642
		[SerializeField]
		public GuidedRefTargetIdSO[] targets;

		// Token: 0x04006813 RID: 26643
		[NonSerialized]
		public int fieldId;

		// Token: 0x04006814 RID: 26644
		[NonSerialized]
		public int resolveCount;
	}
}
