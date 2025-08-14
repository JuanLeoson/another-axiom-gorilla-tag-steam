using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C3B RID: 3131
	public class GorillaPlayerTimerCountDisplay : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06004D5A RID: 19802 RVA: 0x00180BF4 File Offset: 0x0017EDF4
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x00180BF4 File Offset: 0x0017EDF4
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x00180BFC File Offset: 0x0017EDFC
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			this.displayText.text = "TIME: --.--.-";
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.isInitialized = true;
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x00180C88 File Offset: 0x0017EE88
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x00180CED File Offset: 0x0017EEED
		private void OnLocalTimerStarted()
		{
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06004D5F RID: 19807 RVA: 0x00180D00 File Offset: 0x0017EF00
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				double value = timeDelta / 1000.0;
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds(value).ToString("mm\\:ss\\:f");
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x00180D64 File Offset: 0x0017EF64
		private void UpdateLatestTime()
		{
			float timeForPlayer = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)timeForPlayer).ToString("mm\\:ss\\:f");
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06004D61 RID: 19809 RVA: 0x00180DB4 File Offset: 0x0017EFB4
		// (set) Token: 0x06004D62 RID: 19810 RVA: 0x00180DBC File Offset: 0x0017EFBC
		public bool TickRunning { get; set; }

		// Token: 0x06004D63 RID: 19811 RVA: 0x00180DC5 File Offset: 0x0017EFC5
		public void Tick()
		{
			this.UpdateLatestTime();
		}

		// Token: 0x04005659 RID: 22105
		[SerializeField]
		private TMP_Text displayText;

		// Token: 0x0400565A RID: 22106
		private bool isInitialized;
	}
}
