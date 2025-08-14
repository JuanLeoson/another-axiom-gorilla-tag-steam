using System;
using System.Collections;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F91 RID: 3985
	public class CountdownText : MonoBehaviour
	{
		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x0600639D RID: 25501 RVA: 0x001F5F4D File Offset: 0x001F414D
		// (set) Token: 0x0600639E RID: 25502 RVA: 0x001F5F58 File Offset: 0x001F4158
		public CountdownTextDate Countdown
		{
			get
			{
				return this.CountdownTo;
			}
			set
			{
				this.CountdownTo = value;
				if (this.CountdownTo.FormatString.Length > 0)
				{
					this.displayTextFormat = this.CountdownTo.FormatString;
				}
				this.displayText.text = this.CountdownTo.DefaultString;
				if (base.gameObject.activeInHierarchy && !this.useExternalTime && this.monitor == null && this.CountdownTo != null)
				{
					this.monitor = base.StartCoroutine(this.MonitorTime());
				}
			}
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x001F5FE4 File Offset: 0x001F41E4
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = this.displayText.text.Trim();
			this.displayText.text = string.Empty;
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.displayTextFormat.Length == 0 && this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = this.CountdownTo.DefaultString;
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x001F6079 File Offset: 0x001F4279
		private void OnEnable()
		{
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.monitor == null && !this.useExternalTime)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x060063A1 RID: 25505 RVA: 0x001F60AC File Offset: 0x001F42AC
		private void OnDisable()
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x001F60BA File Offset: 0x001F42BA
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = this.TryParseDateTime();
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x001F60C9 File Offset: 0x001F42C9
		private IEnumerator MonitorExternalTime(DateTime countdown)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = countdown;
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x060063A4 RID: 25508 RVA: 0x001F60DF File Offset: 0x001F42DF
		private void StopMonitorTime()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x060063A5 RID: 25509 RVA: 0x001F60FC File Offset: 0x001F42FC
		public void SetCountdownTime(DateTime countdown)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.monitor = base.StartCoroutine(this.MonitorExternalTime(countdown));
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x001F611D File Offset: 0x001F431D
		public void SetFixedText(string text)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.displayText.text = text;
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x001F6137 File Offset: 0x001F4337
		private void StartDisplayRefresh()
		{
			this.StopDisplayRefresh();
			this.displayRefresh = base.StartCoroutine(this.WaitForDisplayRefresh());
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x001F6151 File Offset: 0x001F4351
		private void StopDisplayRefresh()
		{
			if (this.displayRefresh != null)
			{
				base.StopCoroutine(this.displayRefresh);
			}
			this.displayRefresh = null;
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x001F616E File Offset: 0x001F436E
		private IEnumerator WaitForDisplayRefresh()
		{
			for (;;)
			{
				this.RefreshDisplay();
				TimeSpan timeSpan;
				if (this.countdownTime.Days > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromDays((double)this.countdownTime.Days);
				}
				else if (this.countdownTime.Hours > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromHours((double)this.countdownTime.Hours);
				}
				else if (this.countdownTime.Minutes > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromMinutes((double)this.countdownTime.Minutes);
				}
				else
				{
					if (this.countdownTime.Seconds <= 0)
					{
						break;
					}
					timeSpan = this.countdownTime - TimeSpan.FromSeconds((double)this.countdownTime.Seconds);
				}
				yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
			}
			yield break;
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x001F6180 File Offset: 0x001F4380
		private void RefreshDisplay()
		{
			this.countdownTime = this.targetTime.Subtract(GorillaComputer.instance.GetServerTime());
			this.displayText.text = CountdownText.GetTimeDisplay(this.countdownTime, this.displayTextFormat, this.CountdownTo.DaysThreshold, string.Empty, this.CountdownTo.DefaultString);
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x001F61E1 File Offset: 0x001F43E1
		public static string GetTimeDisplay(TimeSpan ts, string format)
		{
			return CountdownText.GetTimeDisplay(ts, format, int.MaxValue, string.Empty, string.Empty);
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x001F61FC File Offset: 0x001F43FC
		public static string GetTimeDisplay(TimeSpan ts, string format, int maxDaysToDisplay, string elapsedString, string overMaxString)
		{
			if (ts.TotalSeconds < 0.0)
			{
				return elapsedString;
			}
			if (ts.TotalDays < (double)maxDaysToDisplay)
			{
				if (ts.Days > 0)
				{
					return string.Format(format, ts.Days, CountdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, ts.Days));
				}
				if (ts.Hours > 0)
				{
					return string.Format(format, ts.Hours, CountdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, ts.Hours));
				}
				if (ts.Minutes > 0)
				{
					return string.Format(format, ts.Minutes, CountdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, ts.Minutes));
				}
				if (ts.Seconds > 0)
				{
					return string.Format(format, ts.Seconds, CountdownText.getTimeChunkString(CountdownText.TimeChunk.SECOND, ts.Seconds));
				}
			}
			return overMaxString;
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x001F62D8 File Offset: 0x001F44D8
		private static string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "DAY";
				}
				return "DAYS";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "HOUR";
				}
				return "HOURS";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "MINUTE";
				}
				return "MINUTES";
			case CountdownText.TimeChunk.SECOND:
				if (n == 1)
				{
					return "SECOND";
				}
				return "SECONDS";
			default:
				return string.Empty;
			}
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x001F6344 File Offset: 0x001F4544
		private DateTime TryParseDateTime()
		{
			DateTime result;
			try
			{
				result = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x04006E8A RID: 28298
		[SerializeField]
		private CountdownTextDate CountdownTo;

		// Token: 0x04006E8B RID: 28299
		[SerializeField]
		private bool updateDisplay;

		// Token: 0x04006E8C RID: 28300
		[SerializeField]
		private bool useExternalTime;

		// Token: 0x04006E8D RID: 28301
		private TMP_Text displayText;

		// Token: 0x04006E8E RID: 28302
		private string displayTextFormat;

		// Token: 0x04006E8F RID: 28303
		private DateTime targetTime;

		// Token: 0x04006E90 RID: 28304
		private TimeSpan countdownTime;

		// Token: 0x04006E91 RID: 28305
		private Coroutine monitor;

		// Token: 0x04006E92 RID: 28306
		private Coroutine displayRefresh;

		// Token: 0x02000F92 RID: 3986
		private enum TimeChunk
		{
			// Token: 0x04006E94 RID: 28308
			DAY,
			// Token: 0x04006E95 RID: 28309
			HOUR,
			// Token: 0x04006E96 RID: 28310
			MINUTE,
			// Token: 0x04006E97 RID: 28311
			SECOND
		}
	}
}
