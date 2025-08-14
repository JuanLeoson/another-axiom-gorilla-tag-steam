using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E66 RID: 3686
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x06005C49 RID: 23625 RVA: 0x001D0795 File Offset: 0x001CE995
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x06005C4A RID: 23626 RVA: 0x001D07A3 File Offset: 0x001CE9A3
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x06005C4B RID: 23627 RVA: 0x001D07B1 File Offset: 0x001CE9B1
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x06005C4C RID: 23628 RVA: 0x001D07BF File Offset: 0x001CE9BF
		private void OnDestroy()
		{
			StaticLodManager.Unregister(this.index);
		}

		// Token: 0x040065E7 RID: 26087
		private int index;

		// Token: 0x040065E8 RID: 26088
		public float collisionEnableDistance = 3f;

		// Token: 0x040065E9 RID: 26089
		public float uiFadeDistanceMin = 1f;

		// Token: 0x040065EA RID: 26090
		public float uiFadeDistanceMax = 10f;
	}
}
