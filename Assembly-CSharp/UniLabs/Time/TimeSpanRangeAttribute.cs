using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000BCE RID: 3022
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanRangeAttribute : Attribute
	{
		// Token: 0x06004935 RID: 18741 RVA: 0x00164BB9 File Offset: 0x00162DB9
		public TimeSpanRangeAttribute(string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x06004936 RID: 18742 RVA: 0x00164BD6 File Offset: 0x00162DD6
		public TimeSpanRangeAttribute(string minGetter, string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x04005269 RID: 21097
		public string MinGetter;

		// Token: 0x0400526A RID: 21098
		public string MaxGetter;

		// Token: 0x0400526B RID: 21099
		public TimeUnit SnappingUnit;

		// Token: 0x0400526C RID: 21100
		public bool Inline;

		// Token: 0x0400526D RID: 21101
		public string DisableMinMaxIf;
	}
}
