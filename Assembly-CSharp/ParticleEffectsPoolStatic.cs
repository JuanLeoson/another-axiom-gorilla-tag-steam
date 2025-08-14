using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class ParticleEffectsPoolStatic<T> : ParticleEffectsPool where T : ParticleEffectsPool
{
	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000AEE RID: 2798 RVA: 0x0003A79B File Offset: 0x0003899B
	public static T Instance
	{
		get
		{
			return ParticleEffectsPoolStatic<T>.gInstance;
		}
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003A7A2 File Offset: 0x000389A2
	protected override void OnPoolAwake()
	{
		if (ParticleEffectsPoolStatic<T>.gInstance && ParticleEffectsPoolStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
			return;
		}
		ParticleEffectsPoolStatic<T>.gInstance = (this as T);
	}

	// Token: 0x04000D59 RID: 3417
	protected static T gInstance;
}
