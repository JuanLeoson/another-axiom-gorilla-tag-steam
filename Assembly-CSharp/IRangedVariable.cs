using System;
using UnityEngine;

// Token: 0x0200075C RID: 1884
public interface IRangedVariable<T> : IVariable<T>, IVariable
{
	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x06002F38 RID: 12088
	// (set) Token: 0x06002F39 RID: 12089
	T Min { get; set; }

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x06002F3A RID: 12090
	// (set) Token: 0x06002F3B RID: 12091
	T Max { get; set; }

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x06002F3C RID: 12092
	T Range { get; }

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x06002F3D RID: 12093
	AnimationCurve Curve { get; }
}
