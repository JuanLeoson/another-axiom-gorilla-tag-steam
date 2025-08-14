using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000B65 RID: 2917
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x060045DF RID: 17887 RVA: 0x0015C8BD File Offset: 0x0015AABD
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x060045E0 RID: 17888 RVA: 0x0015C8E0 File Offset: 0x0015AAE0
		public void Update()
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue()();
				}
			}
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x0015C938 File Offset: 0x0015AB38
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0015C957 File Offset: 0x0015AB57
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0015C960 File Offset: 0x0015AB60
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> obj = MainThreadDispatcher.actions;
			lock (obj)
			{
				MainThreadDispatcher.actions.Enqueue(delegate
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x0015C9C4 File Offset: 0x0015ABC4
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x0015C9D3 File Offset: 0x0015ABD3
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x060045E6 RID: 17894 RVA: 0x0015C9E3 File Offset: 0x0015ABE3
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x060045E7 RID: 17895 RVA: 0x0015C9F4 File Offset: 0x0015ABF4
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x0015CA07 File Offset: 0x0015AC07
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x0015CA1C File Offset: 0x0015AC1C
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x060045EA RID: 17898 RVA: 0x0015CA2B File Offset: 0x0015AC2B
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x060045EB RID: 17899 RVA: 0x0015CA41 File Offset: 0x0015AC41
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x060045EC RID: 17900 RVA: 0x0015CA5E File Offset: 0x0015AC5E
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x060045ED RID: 17901 RVA: 0x0015CA83 File Offset: 0x0015AC83
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x040050D8 RID: 20696
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x040050D9 RID: 20697
		private static MainThreadDispatcher instance = null;
	}
}
