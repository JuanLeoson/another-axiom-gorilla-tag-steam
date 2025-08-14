using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C15 RID: 3093
	public struct BuilderGridPlaneData
	{
		// Token: 0x06004C04 RID: 19460 RVA: 0x001776DC File Offset: 0x001758DC
		public BuilderGridPlaneData(BuilderAttachGridPlane gridPlane, int pieceIndex)
		{
			gridPlane.center.transform.GetPositionAndRotation(out this.position, out this.rotation);
			this.localPosition = gridPlane.pieceToGridPosition;
			this.localRotation = gridPlane.pieceToGridRotation;
			this.width = gridPlane.width;
			this.length = gridPlane.length;
			this.male = gridPlane.male;
			this.pieceId = gridPlane.piece.pieceId;
			this.pieceIndex = pieceIndex;
			this.boundingRadius = gridPlane.boundingRadius;
			this.attachIndex = gridPlane.attachIndex;
		}

		// Token: 0x04005500 RID: 21760
		public int width;

		// Token: 0x04005501 RID: 21761
		public int length;

		// Token: 0x04005502 RID: 21762
		public bool male;

		// Token: 0x04005503 RID: 21763
		public int pieceId;

		// Token: 0x04005504 RID: 21764
		public int pieceIndex;

		// Token: 0x04005505 RID: 21765
		public float boundingRadius;

		// Token: 0x04005506 RID: 21766
		public int attachIndex;

		// Token: 0x04005507 RID: 21767
		public Vector3 position;

		// Token: 0x04005508 RID: 21768
		public Quaternion rotation;

		// Token: 0x04005509 RID: 21769
		public Vector3 localPosition;

		// Token: 0x0400550A RID: 21770
		public Quaternion localRotation;
	}
}
