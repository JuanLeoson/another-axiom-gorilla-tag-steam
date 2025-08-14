using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C09 RID: 3081
	public struct BuilderPotentialPlacement
	{
		// Token: 0x06004B0B RID: 19211 RVA: 0x0016CFD0 File Offset: 0x0016B1D0
		public void Reset()
		{
			this.attachPiece = null;
			this.parentPiece = null;
			this.attachIndex = -1;
			this.parentAttachIndex = -1;
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.attachDistance = float.MaxValue;
			this.attachPlaneNormal = Vector3.zero;
			this.score = float.MinValue;
			this.twist = 0;
			this.bumpOffsetX = 0;
			this.bumpOffsetZ = 0;
		}

		// Token: 0x04005402 RID: 21506
		public BuilderPiece attachPiece;

		// Token: 0x04005403 RID: 21507
		public BuilderPiece parentPiece;

		// Token: 0x04005404 RID: 21508
		public int attachIndex;

		// Token: 0x04005405 RID: 21509
		public int parentAttachIndex;

		// Token: 0x04005406 RID: 21510
		public Vector3 localPosition;

		// Token: 0x04005407 RID: 21511
		public Quaternion localRotation;

		// Token: 0x04005408 RID: 21512
		public Vector3 attachPlaneNormal;

		// Token: 0x04005409 RID: 21513
		public float attachDistance;

		// Token: 0x0400540A RID: 21514
		public float score;

		// Token: 0x0400540B RID: 21515
		public SnapBounds attachBounds;

		// Token: 0x0400540C RID: 21516
		public SnapBounds parentAttachBounds;

		// Token: 0x0400540D RID: 21517
		public byte twist;

		// Token: 0x0400540E RID: 21518
		public sbyte bumpOffsetX;

		// Token: 0x0400540F RID: 21519
		public sbyte bumpOffsetZ;
	}
}
