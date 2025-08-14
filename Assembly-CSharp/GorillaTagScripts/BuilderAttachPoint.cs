using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BFF RID: 3071
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06004AB9 RID: 19129 RVA: 0x0016AF49 File Offset: 0x00169149
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x040053A3 RID: 21411
		public Transform center;
	}
}
