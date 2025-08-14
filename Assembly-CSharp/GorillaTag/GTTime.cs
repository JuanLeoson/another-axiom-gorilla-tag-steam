using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E55 RID: 3669
	public class GTTime
	{
		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06005C19 RID: 23577 RVA: 0x001D0190 File Offset: 0x001CE390
		// (set) Token: 0x06005C1A RID: 23578 RVA: 0x001D0197 File Offset: 0x001CE397
		public static bool usingServerTime { get; private set; }

		// Token: 0x06005C1B RID: 23579 RVA: 0x001D019F File Offset: 0x001CE39F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x06005C1C RID: 23580 RVA: 0x001D01B0 File Offset: 0x001CE3B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x06005C1D RID: 23581 RVA: 0x001D01E8 File Offset: 0x001CE3E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetStartupTimeAsMilliseconds()
		{
			GTTime.usingServerTime = true;
			long num = 0L;
			if (GorillaComputer.hasInstance)
			{
				num = GTTime.GetServerStartupTimeAsMilliseconds();
			}
			if (num == 0L)
			{
				GTTime.usingServerTime = false;
				num = GTTime.GetDeviceStartupTimeAsMilliseconds();
			}
			return num;
		}

		// Token: 0x06005C1E RID: 23582 RVA: 0x001D021B File Offset: 0x001CE41B
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x06005C1F RID: 23583 RVA: 0x001D0233 File Offset: 0x001CE433
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x06005C20 RID: 23584 RVA: 0x001D024C File Offset: 0x001CE44C
		public static DateTime GetAAxiomDateTime()
		{
			DateTime result;
			try
			{
				TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, destinationTimeZone);
			}
			catch
			{
				try
				{
					TimeZoneInfo destinationTimeZone2 = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
					result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, destinationTimeZone2);
				}
				catch
				{
					result = DateTime.UtcNow;
				}
			}
			return result;
		}

		// Token: 0x06005C21 RID: 23585 RVA: 0x001D02B4 File Offset: 0x001CE4B4
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x06005C22 RID: 23586 RVA: 0x001D02D4 File Offset: 0x001CE4D4
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x06005C23 RID: 23587 RVA: 0x001D02F4 File Offset: 0x001CE4F4
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x06005C24 RID: 23588 RVA: 0x001D0318 File Offset: 0x001CE518
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}
	}
}
