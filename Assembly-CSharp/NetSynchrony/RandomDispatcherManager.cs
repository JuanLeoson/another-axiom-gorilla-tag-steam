using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000DE3 RID: 3555
	public class RandomDispatcherManager : MonoBehaviour
	{
		// Token: 0x0600584A RID: 22602 RVA: 0x001B6354 File Offset: 0x001B4554
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x0600584B RID: 22603 RVA: 0x001B63A0 File Offset: 0x001B45A0
		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync(this.serverTime);
			}
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x001B63DC File Offset: 0x001B45DC
		private void AdjustedServerTime()
		{
			DateTime dateTime = new DateTime(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x001B6424 File Offset: 0x001B4624
		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init(this.serverTime);
			}
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x001B6480 File Offset: 0x001B4680
		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick(this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		// Token: 0x040061EB RID: 25067
		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		// Token: 0x040061EC RID: 25068
		private static RandomDispatcherManager __instance;

		// Token: 0x040061ED RID: 25069
		private double serverTime;
	}
}
