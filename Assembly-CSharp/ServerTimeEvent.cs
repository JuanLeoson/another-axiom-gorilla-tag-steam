using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000B1A RID: 2842
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x0600447D RID: 17533 RVA: 0x0015662D File Offset: 0x0015482D
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x0600447E RID: 17534 RVA: 0x00156640 File Offset: 0x00154840
	private void Update()
	{
		if (GorillaComputer.instance == null || Time.time - this.lastQueryTime < this.queryTime)
		{
			return;
		}
		ServerTimeEvent.EventTime item = new ServerTimeEvent.EventTime(GorillaComputer.instance.GetServerTime().Hour, GorillaComputer.instance.GetServerTime().Minute);
		bool flag = this.eventTimes.Contains(item);
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
		this.lastQueryTime = Time.time;
	}

	// Token: 0x04004EA4 RID: 20132
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x04004EA5 RID: 20133
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x04004EA6 RID: 20134
	private float lastQueryTime;

	// Token: 0x04004EA7 RID: 20135
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x02000B1B RID: 2843
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06004480 RID: 17536 RVA: 0x001566EF File Offset: 0x001548EF
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x04004EA8 RID: 20136
		public int hour;

		// Token: 0x04004EA9 RID: 20137
		public int minute;
	}
}
