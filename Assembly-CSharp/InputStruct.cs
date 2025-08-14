using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x020002D5 RID: 725
[NetworkStructWeaved(37)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 148)]
public struct InputStruct : INetworkStruct
{
	// Token: 0x0400192C RID: 6444
	[FieldOffset(0)]
	public int headRotation;

	// Token: 0x0400192D RID: 6445
	[FieldOffset(4)]
	public long rightHandLong;

	// Token: 0x0400192E RID: 6446
	[FieldOffset(12)]
	public long leftHandLong;

	// Token: 0x0400192F RID: 6447
	[FieldOffset(20)]
	public long position;

	// Token: 0x04001930 RID: 6448
	[FieldOffset(28)]
	public int handPosition;

	// Token: 0x04001931 RID: 6449
	[FieldOffset(32)]
	public int packedFields;

	// Token: 0x04001932 RID: 6450
	[FieldOffset(36)]
	public short packedCompetitiveData;

	// Token: 0x04001933 RID: 6451
	[FieldOffset(40)]
	public Vector3 velocity;

	// Token: 0x04001934 RID: 6452
	[FieldOffset(52)]
	public int grabbedRopeIndex;

	// Token: 0x04001935 RID: 6453
	[FieldOffset(56)]
	public int ropeBoneIndex;

	// Token: 0x04001936 RID: 6454
	[FieldOffset(60)]
	public bool ropeGrabIsLeft;

	// Token: 0x04001937 RID: 6455
	[FieldOffset(64)]
	public bool ropeGrabIsBody;

	// Token: 0x04001938 RID: 6456
	[FieldOffset(68)]
	public Vector3 ropeGrabOffset;

	// Token: 0x04001939 RID: 6457
	[FieldOffset(80)]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x0400193A RID: 6458
	[FieldOffset(84)]
	public long hoverboardPosRot;

	// Token: 0x0400193B RID: 6459
	[FieldOffset(92)]
	public short hoverboardColor;

	// Token: 0x0400193C RID: 6460
	[FieldOffset(96)]
	public long propHuntPosRot;

	// Token: 0x0400193D RID: 6461
	[FieldOffset(104)]
	public double serverTimeStamp;

	// Token: 0x0400193E RID: 6462
	[FieldOffset(112)]
	public short taggedById;

	// Token: 0x0400193F RID: 6463
	[FieldOffset(116)]
	public bool isGroundedHand;

	// Token: 0x04001940 RID: 6464
	[FieldOffset(120)]
	public bool isGroundedButt;

	// Token: 0x04001941 RID: 6465
	[FieldOffset(124)]
	public int leftHandGrabbedActorNumber;

	// Token: 0x04001942 RID: 6466
	[FieldOffset(128)]
	public bool leftGrabbedHandIsLeft;

	// Token: 0x04001943 RID: 6467
	[FieldOffset(132)]
	public int rightHandGrabbedActorNumber;

	// Token: 0x04001944 RID: 6468
	[FieldOffset(136)]
	public bool rightGrabbedHandIsLeft;

	// Token: 0x04001945 RID: 6469
	[FieldOffset(140)]
	public float lastTouchedGroundAtTime;

	// Token: 0x04001946 RID: 6470
	[FieldOffset(144)]
	public float lastHandTouchedGroundAtTime;
}
