using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020001F5 RID: 501
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public class GTContactPoint
{
	// Token: 0x04000EB6 RID: 3766
	[NonSerialized]
	[FieldOffset(0)]
	public Matrix4x4 data;

	// Token: 0x04000EB7 RID: 3767
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 data0;

	// Token: 0x04000EB8 RID: 3768
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 data1;

	// Token: 0x04000EB9 RID: 3769
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 data2;

	// Token: 0x04000EBA RID: 3770
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 data3;

	// Token: 0x04000EBB RID: 3771
	[FieldOffset(0)]
	public Vector3 contactPoint;

	// Token: 0x04000EBC RID: 3772
	[FieldOffset(12)]
	public float radius;

	// Token: 0x04000EBD RID: 3773
	[FieldOffset(16)]
	public Vector3 counterVelocity;

	// Token: 0x04000EBE RID: 3774
	[FieldOffset(28)]
	public float timestamp;

	// Token: 0x04000EBF RID: 3775
	[FieldOffset(32)]
	public Color color;

	// Token: 0x04000EC0 RID: 3776
	[FieldOffset(48)]
	public GTContactType contactType;

	// Token: 0x04000EC1 RID: 3777
	[FieldOffset(52)]
	public float lifetime = 1f;

	// Token: 0x04000EC2 RID: 3778
	[FieldOffset(56)]
	public uint free = 1U;
}
