using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000BCD RID: 3021
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x06004931 RID: 18737 RVA: 0x00164B2B File Offset: 0x00162D2B
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x06004932 RID: 18738 RVA: 0x00164B41 File Offset: 0x00162D41
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x06004933 RID: 18739 RVA: 0x00164B65 File Offset: 0x00162D65
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x06004934 RID: 18740 RVA: 0x00164B8F File Offset: 0x00162D8F
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x04005267 RID: 21095
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x04005268 RID: 21096
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
