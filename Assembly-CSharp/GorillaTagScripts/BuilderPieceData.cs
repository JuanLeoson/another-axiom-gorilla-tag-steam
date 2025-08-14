using System;

namespace GorillaTagScripts
{
	// Token: 0x02000C16 RID: 3094
	public struct BuilderPieceData
	{
		// Token: 0x06004C05 RID: 19461 RVA: 0x00177774 File Offset: 0x00175974
		public BuilderPieceData(BuilderPiece piece)
		{
			this.pieceId = piece.pieceId;
			this.pieceIndex = piece.pieceDataIndex;
			BuilderPiece parentPiece = piece.parentPiece;
			this.parentPieceIndex = ((parentPiece == null) ? -1 : parentPiece.pieceDataIndex);
			BuilderPiece requestedParentPiece = piece.requestedParentPiece;
			this.requestedParentPieceIndex = ((requestedParentPiece == null) ? -1 : requestedParentPiece.pieceDataIndex);
			this.preventSnapUntilMoved = piece.preventSnapUntilMoved;
			this.isBuiltIntoTable = piece.isBuiltIntoTable;
			this.state = piece.state;
			this.privatePlotIndex = piece.privatePlotIndex;
			this.isArmPiece = piece.isArmShelf;
			this.heldByActorNumber = piece.heldByPlayerActorNumber;
		}

		// Token: 0x0400550B RID: 21771
		public int pieceId;

		// Token: 0x0400550C RID: 21772
		public int pieceIndex;

		// Token: 0x0400550D RID: 21773
		public int parentPieceIndex;

		// Token: 0x0400550E RID: 21774
		public int requestedParentPieceIndex;

		// Token: 0x0400550F RID: 21775
		public int heldByActorNumber;

		// Token: 0x04005510 RID: 21776
		public int preventSnapUntilMoved;

		// Token: 0x04005511 RID: 21777
		public bool isBuiltIntoTable;

		// Token: 0x04005512 RID: 21778
		public BuilderPiece.State state;

		// Token: 0x04005513 RID: 21779
		public int privatePlotIndex;

		// Token: 0x04005514 RID: 21780
		public bool isArmPiece;
	}
}
