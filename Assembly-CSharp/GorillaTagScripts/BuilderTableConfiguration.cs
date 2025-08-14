using System;

namespace GorillaTagScripts
{
	// Token: 0x02000C08 RID: 3080
	[Serializable]
	public class BuilderTableConfiguration
	{
		// Token: 0x06004B0A RID: 19210 RVA: 0x0016CF9C File Offset: 0x0016B19C
		public BuilderTableConfiguration()
		{
			this.version = 0;
			this.TableResourceLimits = new int[3];
			this.PlotResourceLimits = new int[3];
			this.updateCountdownDate = string.Empty;
		}

		// Token: 0x040053FC RID: 21500
		public const int CONFIGURATION_VERSION = 0;

		// Token: 0x040053FD RID: 21501
		public int version;

		// Token: 0x040053FE RID: 21502
		public int[] TableResourceLimits;

		// Token: 0x040053FF RID: 21503
		public int[] PlotResourceLimits;

		// Token: 0x04005400 RID: 21504
		public int DroppedPieceLimit;

		// Token: 0x04005401 RID: 21505
		public string updateCountdownDate;
	}
}
