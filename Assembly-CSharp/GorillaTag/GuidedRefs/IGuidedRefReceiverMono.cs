using System;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EC0 RID: 3776
	public interface IGuidedRefReceiverMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06005E43 RID: 24131
		bool GuidedRefTryResolveReference(GuidedRefTryResolveInfo target);

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06005E44 RID: 24132
		// (set) Token: 0x06005E45 RID: 24133
		int GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06005E46 RID: 24134
		void OnAllGuidedRefsResolved();

		// Token: 0x06005E47 RID: 24135
		void OnGuidedRefTargetDestroyed(int fieldId);
	}
}
