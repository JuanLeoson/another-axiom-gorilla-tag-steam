using System;

namespace UniLabs.Time
{
	// Token: 0x02000BD0 RID: 3024
	public static class TimeUnitExtensions
	{
		// Token: 0x06004937 RID: 18743 RVA: 0x00164BFC File Offset: 0x00162DFC
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "ms";
				break;
			case TimeUnit.Seconds:
				result = "s";
				break;
			case TimeUnit.Minutes:
				result = "m";
				break;
			case TimeUnit.Hours:
				result = "h";
				break;
			case TimeUnit.Days:
				result = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06004938 RID: 18744 RVA: 0x00164C6C File Offset: 0x00162E6C
		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "";
				break;
			case TimeUnit.Seconds:
				result = ".";
				break;
			case TimeUnit.Minutes:
				result = ":";
				break;
			case TimeUnit.Hours:
				result = ":";
				break;
			case TimeUnit.Days:
				result = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06004939 RID: 18745 RVA: 0x00164CDC File Offset: 0x00162EDC
		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		// Token: 0x0600493A RID: 18746 RVA: 0x00164D54 File Offset: 0x00162F54
		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				result = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				result = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				result = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				result = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600493B RID: 18747 RVA: 0x00164E18 File Offset: 0x00163018
		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600493C RID: 18748 RVA: 0x00164EF4 File Offset: 0x001630F4
		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x00164FE0 File Offset: 0x001631E0
		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				result = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600493E RID: 18750 RVA: 0x001650BC File Offset: 0x001632BC
		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x001651BC File Offset: 0x001633BC
		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x0016522C File Offset: 0x0016342C
		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				result = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				result = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x001652A4 File Offset: 0x001634A4
		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x02000BD1 RID: 3025
		// (Invoke) Token: 0x06004943 RID: 18755
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000BD2 RID: 3026
		// (Invoke) Token: 0x06004947 RID: 18759
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
