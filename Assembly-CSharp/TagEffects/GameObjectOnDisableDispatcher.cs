using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000E04 RID: 3588
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x060058D1 RID: 22737 RVA: 0x001B99C8 File Offset: 0x001B7BC8
		// (remove) Token: 0x060058D2 RID: 22738 RVA: 0x001B9A00 File Offset: 0x001B7C00
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x060058D3 RID: 22739 RVA: 0x001B9A35 File Offset: 0x001B7C35
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x02000E05 RID: 3589
		// (Invoke) Token: 0x060058D6 RID: 22742
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}
