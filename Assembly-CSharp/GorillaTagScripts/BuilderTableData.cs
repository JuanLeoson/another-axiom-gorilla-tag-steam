using System;
using System.Collections.Generic;

namespace GorillaTagScripts
{
	// Token: 0x02000C21 RID: 3105
	[Serializable]
	public class BuilderTableData
	{
		// Token: 0x06004C60 RID: 19552 RVA: 0x0017ADE0 File Offset: 0x00178FE0
		public BuilderTableData()
		{
			this.version = 4;
			this.numEdits = 0;
			this.numPieces = 0;
			this.pieceType = new List<int>(1024);
			this.pieceId = new List<int>(1024);
			this.parentId = new List<int>(1024);
			this.attachIndex = new List<int>(1024);
			this.parentAttachIndex = new List<int>(1024);
			this.placement = new List<int>(1024);
			this.materialType = new List<int>(1024);
			this.overlapingPieces = new List<int>(1024);
			this.overlappedPieces = new List<int>(1024);
			this.overlapInfo = new List<long>(1024);
			this.timeOffset = new List<int>(1024);
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x0017AEB8 File Offset: 0x001790B8
		public void Clear()
		{
			this.numPieces = 0;
			this.pieceType.Clear();
			this.pieceId.Clear();
			this.parentId.Clear();
			this.attachIndex.Clear();
			this.parentAttachIndex.Clear();
			this.placement.Clear();
			this.materialType.Clear();
			this.overlapingPieces.Clear();
			this.overlappedPieces.Clear();
			this.overlapInfo.Clear();
			this.timeOffset.Clear();
		}

		// Token: 0x04005575 RID: 21877
		public const int BUILDER_TABLE_DATA_VERSION = 4;

		// Token: 0x04005576 RID: 21878
		public int version;

		// Token: 0x04005577 RID: 21879
		public int numEdits;

		// Token: 0x04005578 RID: 21880
		public int numPieces;

		// Token: 0x04005579 RID: 21881
		public List<int> pieceType;

		// Token: 0x0400557A RID: 21882
		public List<int> pieceId;

		// Token: 0x0400557B RID: 21883
		public List<int> parentId;

		// Token: 0x0400557C RID: 21884
		public List<int> attachIndex;

		// Token: 0x0400557D RID: 21885
		public List<int> parentAttachIndex;

		// Token: 0x0400557E RID: 21886
		public List<int> placement;

		// Token: 0x0400557F RID: 21887
		public List<int> materialType;

		// Token: 0x04005580 RID: 21888
		public List<int> overlapingPieces;

		// Token: 0x04005581 RID: 21889
		public List<int> overlappedPieces;

		// Token: 0x04005582 RID: 21890
		public List<long> overlapInfo;

		// Token: 0x04005583 RID: 21891
		public List<int> timeOffset;
	}
}
