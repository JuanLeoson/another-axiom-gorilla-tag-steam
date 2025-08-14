using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x020002BE RID: 702
[NetworkInputWeaved(35)]
[StructLayout(LayoutKind.Explicit, Size = 140)]
public struct NetworkedInput : INetworkInput
{
	// Token: 0x04001891 RID: 6289
	[FieldOffset(0)]
	public Quaternion headRot_LS;

	// Token: 0x04001892 RID: 6290
	[FieldOffset(16)]
	public Vector3 rightHandPos_LS;

	// Token: 0x04001893 RID: 6291
	[FieldOffset(28)]
	public Quaternion rightHandRot_LS;

	// Token: 0x04001894 RID: 6292
	[FieldOffset(44)]
	public Vector3 leftHandPos_LS;

	// Token: 0x04001895 RID: 6293
	[FieldOffset(56)]
	public Quaternion leftHandRot_LS;

	// Token: 0x04001896 RID: 6294
	[FieldOffset(72)]
	public Vector3 rootPosition;

	// Token: 0x04001897 RID: 6295
	[FieldOffset(84)]
	public Quaternion rootRotation;

	// Token: 0x04001898 RID: 6296
	[FieldOffset(100)]
	public bool leftThumbTouch;

	// Token: 0x04001899 RID: 6297
	[FieldOffset(104)]
	public bool leftThumbPress;

	// Token: 0x0400189A RID: 6298
	[FieldOffset(108)]
	public float leftIndexValue;

	// Token: 0x0400189B RID: 6299
	[FieldOffset(112)]
	public float leftMiddleValue;

	// Token: 0x0400189C RID: 6300
	[FieldOffset(116)]
	public bool rightThumbTouch;

	// Token: 0x0400189D RID: 6301
	[FieldOffset(120)]
	public bool rightThumbPress;

	// Token: 0x0400189E RID: 6302
	[FieldOffset(124)]
	public float rightIndexValue;

	// Token: 0x0400189F RID: 6303
	[FieldOffset(128)]
	public float rightMiddleValue;

	// Token: 0x040018A0 RID: 6304
	[FieldOffset(132)]
	public float scale;

	// Token: 0x040018A1 RID: 6305
	[FieldOffset(136)]
	public int handPoseData;
}
