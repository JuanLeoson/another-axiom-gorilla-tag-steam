using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C19 RID: 3097
	public struct BuilderPotentialPlacementData
	{
		// Token: 0x06004C07 RID: 19463 RVA: 0x00177848 File Offset: 0x00175A48
		public BuilderPotentialPlacement ToPotentialPlacement(BuilderTable table)
		{
			BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
			{
				attachPiece = table.GetPiece(this.pieceId),
				parentPiece = table.GetPiece(this.parentPieceId),
				score = this.score,
				localPosition = this.localPosition,
				localRotation = this.localRotation,
				attachIndex = this.attachIndex,
				parentAttachIndex = this.parentAttachIndex,
				attachDistance = this.attachDistance,
				attachPlaneNormal = this.attachPlaneNormal,
				attachBounds = this.attachBounds,
				parentAttachBounds = this.parentAttachBounds,
				twist = this.twist,
				bumpOffsetX = this.bumpOffsetX,
				bumpOffsetZ = this.bumpOffsetZ
			};
			if (builderPotentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				builderPotentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(builderPotentialPlacement.localPosition);
				builderPotentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * builderPotentialPlacement.localRotation;
			}
			return builderPotentialPlacement;
		}

		// Token: 0x0400551B RID: 21787
		public int pieceId;

		// Token: 0x0400551C RID: 21788
		public int parentPieceId;

		// Token: 0x0400551D RID: 21789
		public float score;

		// Token: 0x0400551E RID: 21790
		public Vector3 localPosition;

		// Token: 0x0400551F RID: 21791
		public Quaternion localRotation;

		// Token: 0x04005520 RID: 21792
		public int attachIndex;

		// Token: 0x04005521 RID: 21793
		public int parentAttachIndex;

		// Token: 0x04005522 RID: 21794
		public float attachDistance;

		// Token: 0x04005523 RID: 21795
		public Vector3 attachPlaneNormal;

		// Token: 0x04005524 RID: 21796
		public SnapBounds attachBounds;

		// Token: 0x04005525 RID: 21797
		public SnapBounds parentAttachBounds;

		// Token: 0x04005526 RID: 21798
		public byte twist;

		// Token: 0x04005527 RID: 21799
		public sbyte bumpOffsetX;

		// Token: 0x04005528 RID: 21800
		public sbyte bumpOffsetZ;
	}
}
