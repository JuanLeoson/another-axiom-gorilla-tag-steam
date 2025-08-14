using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C45 RID: 3141
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x06004DAD RID: 19885 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Awake()
		{
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x00182250 File Offset: 0x00180450
		private void Start()
		{
			this.EnableCandle(false);
			this.EnableFlame(false);
			this.litDate = new DateTime(this.year, this.month, this.day);
			this.currentDate = DateTime.Now;
			this.EnableCandle(this.CandleShouldBeVisible());
			this.EnableFlame(false);
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x001822CE File Offset: 0x001804CE
		private void UpdateMenorah()
		{
			this.EnableCandle(this.CandleShouldBeVisible());
			if (this.ShouldLightCandle())
			{
				this.EnableFlame(true);
				return;
			}
			if (this.ShouldSnuffCandle())
			{
				this.EnableFlame(false);
			}
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x001822FB File Offset: 0x001804FB
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x06004DB1 RID: 19889 RVA: 0x00182315 File Offset: 0x00180515
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x06004DB2 RID: 19890 RVA: 0x00182324 File Offset: 0x00180524
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x06004DB3 RID: 19891 RVA: 0x00182333 File Offset: 0x00180533
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x06004DB4 RID: 19892 RVA: 0x0018234E File Offset: 0x0018054E
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x06004DB5 RID: 19893 RVA: 0x00182361 File Offset: 0x00180561
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x06004DB6 RID: 19894 RVA: 0x0018237C File Offset: 0x0018057C
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x0018239E File Offset: 0x0018059E
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x06004DB8 RID: 19896 RVA: 0x001823B5 File Offset: 0x001805B5
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x040056B4 RID: 22196
		public int day;

		// Token: 0x040056B5 RID: 22197
		public int month;

		// Token: 0x040056B6 RID: 22198
		public int year;

		// Token: 0x040056B7 RID: 22199
		public GameObject flame;

		// Token: 0x040056B8 RID: 22200
		public GameObject candle;

		// Token: 0x040056B9 RID: 22201
		private DateTime litDate;

		// Token: 0x040056BA RID: 22202
		private bool activeTimeEventDay;

		// Token: 0x040056BB RID: 22203
		private DateTime currentDate;
	}
}
