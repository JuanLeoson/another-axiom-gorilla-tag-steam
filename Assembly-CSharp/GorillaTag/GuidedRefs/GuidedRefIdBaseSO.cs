using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EB8 RID: 3768
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x06005E20 RID: 24096 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x06005E22 RID: 24098 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
