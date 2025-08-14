using System;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public struct BuilderAction
{
	// Token: 0x04002888 RID: 10376
	public BuilderActionType type;

	// Token: 0x04002889 RID: 10377
	public int pieceId;

	// Token: 0x0400288A RID: 10378
	public int parentPieceId;

	// Token: 0x0400288B RID: 10379
	public Vector3 localPosition;

	// Token: 0x0400288C RID: 10380
	public Quaternion localRotation;

	// Token: 0x0400288D RID: 10381
	public byte twist;

	// Token: 0x0400288E RID: 10382
	public sbyte bumpOffsetx;

	// Token: 0x0400288F RID: 10383
	public sbyte bumpOffsetz;

	// Token: 0x04002890 RID: 10384
	public bool isLeftHand;

	// Token: 0x04002891 RID: 10385
	public int playerActorNumber;

	// Token: 0x04002892 RID: 10386
	public int parentAttachIndex;

	// Token: 0x04002893 RID: 10387
	public int attachIndex;

	// Token: 0x04002894 RID: 10388
	public SnapBounds attachBounds;

	// Token: 0x04002895 RID: 10389
	public SnapBounds parentAttachBounds;

	// Token: 0x04002896 RID: 10390
	public Vector3 velocity;

	// Token: 0x04002897 RID: 10391
	public Vector3 angVelocity;

	// Token: 0x04002898 RID: 10392
	public int localCommandId;

	// Token: 0x04002899 RID: 10393
	public int timeStamp;
}
