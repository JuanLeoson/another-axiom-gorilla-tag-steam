using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000DE1 RID: 3553
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		// Token: 0x140000A0 RID: 160
		// (add) Token: 0x06005840 RID: 22592 RVA: 0x001B6158 File Offset: 0x001B4358
		// (remove) Token: 0x06005841 RID: 22593 RVA: 0x001B6190 File Offset: 0x001B4390
		public event RandomDispatcher.RandomDispatcherEvent Dispatch;

		// Token: 0x06005842 RID: 22594 RVA: 0x001B61C8 File Offset: 0x001B43C8
		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Compute(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x001B626C File Offset: 0x001B446C
		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x001B62C0 File Offset: 0x001B44C0
		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		// Token: 0x040061E6 RID: 25062
		[SerializeField]
		private float minWaitTime = 1f;

		// Token: 0x040061E7 RID: 25063
		[SerializeField]
		private float maxWaitTime = 10f;

		// Token: 0x040061E8 RID: 25064
		[SerializeField]
		private float totalMinutes = 60f;

		// Token: 0x040061E9 RID: 25065
		private List<float> dispatchTimes;

		// Token: 0x040061EA RID: 25066
		private int index = -1;

		// Token: 0x02000DE2 RID: 3554
		// (Invoke) Token: 0x06005847 RID: 22599
		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
