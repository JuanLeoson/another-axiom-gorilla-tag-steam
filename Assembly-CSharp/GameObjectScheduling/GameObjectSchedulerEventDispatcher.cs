using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x02000F9C RID: 3996
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x060063DF RID: 25567 RVA: 0x001F6C11 File Offset: 0x001F4E11
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x060063E0 RID: 25568 RVA: 0x001F6C19 File Offset: 0x001F4E19
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x04006EB8 RID: 28344
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x04006EB9 RID: 28345
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}
