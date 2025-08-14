using System;

namespace GorillaTagScripts
{
	// Token: 0x02000C18 RID: 3096
	public struct BuilderPrivatePlotData
	{
		// Token: 0x06004C06 RID: 19462 RVA: 0x0017781F File Offset: 0x00175A1F
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x04005517 RID: 21783
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x04005518 RID: 21784
		public int ownerActorNumber;

		// Token: 0x04005519 RID: 21785
		public bool isUnderCapacityLeft;

		// Token: 0x0400551A RID: 21786
		public bool isUnderCapacityRight;
	}
}
