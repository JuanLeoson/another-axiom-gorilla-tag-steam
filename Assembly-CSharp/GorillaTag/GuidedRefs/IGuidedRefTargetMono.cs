using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EC1 RID: 3777
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06005E48 RID: 24136
		// (set) Token: 0x06005E49 RID: 24137
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06005E4A RID: 24138
		Object GuidedRefTargetObject { get; }
	}
}
