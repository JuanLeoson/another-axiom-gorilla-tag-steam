using System;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x02000E88 RID: 3720
	internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
	{
		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06005D32 RID: 23858 RVA: 0x001D7909 File Offset: 0x001D5B09
		// (set) Token: 0x06005D33 RID: 23859 RVA: 0x001D7911 File Offset: 0x001D5B11
		public int Muted { get; set; }

		// Token: 0x06005D34 RID: 23860 RVA: 0x001D791C File Offset: 0x001D5B1C
		public override void OnTimedEvent()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				this.Stop();
				return;
			}
			ReportMuteTimer.content[0] = this.m_playerID;
			ReportMuteTimer.content[1] = this.Muted;
			ReportMuteTimer.content[2] = ((this.m_nickName.Length > 12) ? this.m_nickName.Remove(12) : this.m_nickName);
			ReportMuteTimer.content[3] = NetworkSystem.Instance.LocalPlayer.NickName;
			ReportMuteTimer.content[4] = !NetworkSystem.Instance.SessionIsPrivate;
			ReportMuteTimer.content[5] = NetworkSystem.Instance.RoomStringStripped();
			NetworkSystemRaiseEvent.RaiseEvent(51, ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
			this.Stop();
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001D79DE File Offset: 0x001D5BDE
		public void SetReportData(string id, string name, int muted)
		{
			this.Muted = muted;
			this.m_playerID = id;
			this.m_nickName = name;
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x000023F5 File Offset: 0x000005F5
		void ObjectPoolEvents.OnTaken()
		{
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001D79F5 File Offset: 0x001D5BF5
		void ObjectPoolEvents.OnReturned()
		{
			if (base.Running)
			{
				this.OnTimedEvent();
			}
			this.m_playerID = string.Empty;
			this.m_nickName = string.Empty;
			this.Muted = 0;
		}

		// Token: 0x0400674D RID: 26445
		private static readonly NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = new WebFlags(1),
			TargetActors = new int[]
			{
				-1
			}
		};

		// Token: 0x0400674E RID: 26446
		private static readonly object[] content = new object[6];

		// Token: 0x0400674F RID: 26447
		private const byte evCode = 51;

		// Token: 0x04006751 RID: 26449
		private string m_playerID;

		// Token: 0x04006752 RID: 26450
		private string m_nickName;
	}
}
