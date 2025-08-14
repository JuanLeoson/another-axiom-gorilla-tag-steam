using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200087E RID: 2174
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct m4x4
{
	// Token: 0x06003682 RID: 13954 RVA: 0x0011D0EC File Offset: 0x0011B2EC
	public m4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
	{
		this = default(m4x4);
		this.m00 = m00;
		this.m01 = m01;
		this.m02 = m02;
		this.m03 = m03;
		this.m10 = m10;
		this.m11 = m11;
		this.m12 = m12;
		this.m13 = m13;
		this.m20 = m20;
		this.m21 = m21;
		this.m22 = m22;
		this.m23 = m23;
		this.m30 = m30;
		this.m31 = m31;
		this.m32 = m32;
		this.m33 = m33;
	}

	// Token: 0x06003683 RID: 13955 RVA: 0x0011D17D File Offset: 0x0011B37D
	public m4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
	{
		this = default(m4x4);
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x0011D1A4 File Offset: 0x0011B3A4
	public void Clear()
	{
		this.m00 = 0f;
		this.m01 = 0f;
		this.m02 = 0f;
		this.m03 = 0f;
		this.m10 = 0f;
		this.m11 = 0f;
		this.m12 = 0f;
		this.m13 = 0f;
		this.m20 = 0f;
		this.m21 = 0f;
		this.m22 = 0f;
		this.m23 = 0f;
		this.m30 = 0f;
		this.m31 = 0f;
		this.m32 = 0f;
		this.m33 = 0f;
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x0011D261 File Offset: 0x0011B461
	public void SetRow0(ref Vector4 v)
	{
		this.m00 = v.x;
		this.m01 = v.y;
		this.m02 = v.z;
		this.m03 = v.w;
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x0011D293 File Offset: 0x0011B493
	public void SetRow1(ref Vector4 v)
	{
		this.m10 = v.x;
		this.m11 = v.y;
		this.m12 = v.z;
		this.m13 = v.w;
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x0011D2C5 File Offset: 0x0011B4C5
	public void SetRow2(ref Vector4 v)
	{
		this.m20 = v.x;
		this.m21 = v.y;
		this.m22 = v.z;
		this.m23 = v.w;
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x0011D2F7 File Offset: 0x0011B4F7
	public void SetRow3(ref Vector4 v)
	{
		this.m30 = v.x;
		this.m31 = v.y;
		this.m32 = v.z;
		this.m33 = v.w;
	}

	// Token: 0x06003689 RID: 13961 RVA: 0x0011D32C File Offset: 0x0011B52C
	public void Transpose()
	{
		float num = this.m01;
		float num2 = this.m02;
		float num3 = this.m03;
		float num4 = this.m10;
		float num5 = this.m12;
		float num6 = this.m13;
		float num7 = this.m20;
		float num8 = this.m21;
		float num9 = this.m23;
		float num10 = this.m30;
		float num11 = this.m31;
		float num12 = this.m32;
		this.m01 = num4;
		this.m02 = num7;
		this.m03 = num10;
		this.m10 = num;
		this.m12 = num8;
		this.m13 = num11;
		this.m20 = num2;
		this.m21 = num5;
		this.m23 = num12;
		this.m30 = num3;
		this.m31 = num6;
		this.m32 = num9;
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x0011D3F1 File Offset: 0x0011B5F1
	public void Set(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x0011D424 File Offset: 0x0011B624
	public void SetTransposed(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.m00 = row0.x;
		this.m01 = row1.x;
		this.m02 = row2.x;
		this.m03 = row3.x;
		this.m10 = row0.y;
		this.m11 = row1.y;
		this.m12 = row2.y;
		this.m13 = row3.y;
		this.m20 = row0.z;
		this.m21 = row1.z;
		this.m22 = row2.z;
		this.m23 = row3.z;
		this.m30 = row0.w;
		this.m31 = row1.w;
		this.m32 = row2.w;
		this.m33 = row3.w;
	}

	// Token: 0x0600368C RID: 13964 RVA: 0x0011D4F8 File Offset: 0x0011B6F8
	public void Set(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m01;
		this.m02 = x.m02;
		this.m03 = x.m03;
		this.m10 = x.m10;
		this.m11 = x.m11;
		this.m12 = x.m12;
		this.m13 = x.m13;
		this.m20 = x.m20;
		this.m21 = x.m21;
		this.m22 = x.m22;
		this.m23 = x.m23;
		this.m30 = x.m30;
		this.m31 = x.m31;
		this.m32 = x.m32;
		this.m33 = x.m33;
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x0011D5C8 File Offset: 0x0011B7C8
	public void SetTransposed(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m10;
		this.m02 = x.m20;
		this.m03 = x.m30;
		this.m10 = x.m01;
		this.m11 = x.m11;
		this.m12 = x.m21;
		this.m13 = x.m31;
		this.m20 = x.m02;
		this.m21 = x.m12;
		this.m22 = x.m22;
		this.m23 = x.m32;
		this.m30 = x.m03;
		this.m31 = x.m13;
		this.m32 = x.m23;
		this.m33 = x.m33;
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x0011D698 File Offset: 0x0011B898
	public void Push(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m01;
		x.m02 = this.m02;
		x.m03 = this.m03;
		x.m10 = this.m10;
		x.m11 = this.m11;
		x.m12 = this.m12;
		x.m13 = this.m13;
		x.m20 = this.m20;
		x.m21 = this.m21;
		x.m22 = this.m22;
		x.m23 = this.m23;
		x.m30 = this.m30;
		x.m31 = this.m31;
		x.m32 = this.m32;
		x.m33 = this.m33;
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x0011D768 File Offset: 0x0011B968
	public void PushTransposed(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m10;
		x.m02 = this.m20;
		x.m03 = this.m30;
		x.m10 = this.m01;
		x.m11 = this.m11;
		x.m12 = this.m21;
		x.m13 = this.m31;
		x.m20 = this.m02;
		x.m21 = this.m12;
		x.m22 = this.m22;
		x.m23 = this.m32;
		x.m30 = this.m03;
		x.m31 = this.m13;
		x.m32 = this.m23;
		x.m33 = this.m33;
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x0011D835 File Offset: 0x0011BA35
	public static ref m4x4 From(ref Matrix4x4 src)
	{
		return Unsafe.As<Matrix4x4, m4x4>(ref src);
	}

	// Token: 0x0400434C RID: 17228
	[FixedBuffer(typeof(float), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_f>e__FixedBuffer data_f;

	// Token: 0x0400434D RID: 17229
	[FixedBuffer(typeof(int), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_i>e__FixedBuffer data_i;

	// Token: 0x0400434E RID: 17230
	[FixedBuffer(typeof(ushort), 32)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public m4x4.<data_h>e__FixedBuffer data_h;

	// Token: 0x0400434F RID: 17231
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 r0;

	// Token: 0x04004350 RID: 17232
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 r1;

	// Token: 0x04004351 RID: 17233
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 r2;

	// Token: 0x04004352 RID: 17234
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 r3;

	// Token: 0x04004353 RID: 17235
	[NonSerialized]
	[FieldOffset(0)]
	public float m00;

	// Token: 0x04004354 RID: 17236
	[NonSerialized]
	[FieldOffset(4)]
	public float m01;

	// Token: 0x04004355 RID: 17237
	[NonSerialized]
	[FieldOffset(8)]
	public float m02;

	// Token: 0x04004356 RID: 17238
	[NonSerialized]
	[FieldOffset(12)]
	public float m03;

	// Token: 0x04004357 RID: 17239
	[NonSerialized]
	[FieldOffset(16)]
	public float m10;

	// Token: 0x04004358 RID: 17240
	[NonSerialized]
	[FieldOffset(20)]
	public float m11;

	// Token: 0x04004359 RID: 17241
	[NonSerialized]
	[FieldOffset(24)]
	public float m12;

	// Token: 0x0400435A RID: 17242
	[NonSerialized]
	[FieldOffset(28)]
	public float m13;

	// Token: 0x0400435B RID: 17243
	[NonSerialized]
	[FieldOffset(32)]
	public float m20;

	// Token: 0x0400435C RID: 17244
	[NonSerialized]
	[FieldOffset(36)]
	public float m21;

	// Token: 0x0400435D RID: 17245
	[NonSerialized]
	[FieldOffset(40)]
	public float m22;

	// Token: 0x0400435E RID: 17246
	[NonSerialized]
	[FieldOffset(44)]
	public float m23;

	// Token: 0x0400435F RID: 17247
	[NonSerialized]
	[FieldOffset(48)]
	public float m30;

	// Token: 0x04004360 RID: 17248
	[NonSerialized]
	[FieldOffset(52)]
	public float m31;

	// Token: 0x04004361 RID: 17249
	[NonSerialized]
	[FieldOffset(56)]
	public float m32;

	// Token: 0x04004362 RID: 17250
	[NonSerialized]
	[FieldOffset(60)]
	public float m33;

	// Token: 0x04004363 RID: 17251
	[HideInInspector]
	[FieldOffset(0)]
	public int i00;

	// Token: 0x04004364 RID: 17252
	[HideInInspector]
	[FieldOffset(4)]
	public int i01;

	// Token: 0x04004365 RID: 17253
	[HideInInspector]
	[FieldOffset(8)]
	public int i02;

	// Token: 0x04004366 RID: 17254
	[HideInInspector]
	[FieldOffset(12)]
	public int i03;

	// Token: 0x04004367 RID: 17255
	[HideInInspector]
	[FieldOffset(16)]
	public int i10;

	// Token: 0x04004368 RID: 17256
	[HideInInspector]
	[FieldOffset(20)]
	public int i11;

	// Token: 0x04004369 RID: 17257
	[HideInInspector]
	[FieldOffset(24)]
	public int i12;

	// Token: 0x0400436A RID: 17258
	[HideInInspector]
	[FieldOffset(28)]
	public int i13;

	// Token: 0x0400436B RID: 17259
	[HideInInspector]
	[FieldOffset(32)]
	public int i20;

	// Token: 0x0400436C RID: 17260
	[HideInInspector]
	[FieldOffset(36)]
	public int i21;

	// Token: 0x0400436D RID: 17261
	[HideInInspector]
	[FieldOffset(40)]
	public int i22;

	// Token: 0x0400436E RID: 17262
	[HideInInspector]
	[FieldOffset(44)]
	public int i23;

	// Token: 0x0400436F RID: 17263
	[HideInInspector]
	[FieldOffset(48)]
	public int i30;

	// Token: 0x04004370 RID: 17264
	[HideInInspector]
	[FieldOffset(52)]
	public int i31;

	// Token: 0x04004371 RID: 17265
	[HideInInspector]
	[FieldOffset(56)]
	public int i32;

	// Token: 0x04004372 RID: 17266
	[HideInInspector]
	[FieldOffset(60)]
	public int i33;

	// Token: 0x04004373 RID: 17267
	[NonSerialized]
	[FieldOffset(0)]
	public ushort h00_a;

	// Token: 0x04004374 RID: 17268
	[NonSerialized]
	[FieldOffset(2)]
	public ushort h00_b;

	// Token: 0x04004375 RID: 17269
	[NonSerialized]
	[FieldOffset(4)]
	public ushort h01_a;

	// Token: 0x04004376 RID: 17270
	[NonSerialized]
	[FieldOffset(6)]
	public ushort h01_b;

	// Token: 0x04004377 RID: 17271
	[NonSerialized]
	[FieldOffset(8)]
	public ushort h02_a;

	// Token: 0x04004378 RID: 17272
	[NonSerialized]
	[FieldOffset(10)]
	public ushort h02_b;

	// Token: 0x04004379 RID: 17273
	[NonSerialized]
	[FieldOffset(12)]
	public ushort h03_a;

	// Token: 0x0400437A RID: 17274
	[NonSerialized]
	[FieldOffset(14)]
	public ushort h03_b;

	// Token: 0x0400437B RID: 17275
	[NonSerialized]
	[FieldOffset(16)]
	public ushort h10_a;

	// Token: 0x0400437C RID: 17276
	[NonSerialized]
	[FieldOffset(18)]
	public ushort h10_b;

	// Token: 0x0400437D RID: 17277
	[NonSerialized]
	[FieldOffset(20)]
	public ushort h11_a;

	// Token: 0x0400437E RID: 17278
	[NonSerialized]
	[FieldOffset(22)]
	public ushort h11_b;

	// Token: 0x0400437F RID: 17279
	[NonSerialized]
	[FieldOffset(24)]
	public ushort h12_a;

	// Token: 0x04004380 RID: 17280
	[NonSerialized]
	[FieldOffset(26)]
	public ushort h12_b;

	// Token: 0x04004381 RID: 17281
	[NonSerialized]
	[FieldOffset(28)]
	public ushort h13_a;

	// Token: 0x04004382 RID: 17282
	[NonSerialized]
	[FieldOffset(30)]
	public ushort h13_b;

	// Token: 0x04004383 RID: 17283
	[NonSerialized]
	[FieldOffset(32)]
	public ushort h20_a;

	// Token: 0x04004384 RID: 17284
	[NonSerialized]
	[FieldOffset(34)]
	public ushort h20_b;

	// Token: 0x04004385 RID: 17285
	[NonSerialized]
	[FieldOffset(36)]
	public ushort h21_a;

	// Token: 0x04004386 RID: 17286
	[NonSerialized]
	[FieldOffset(38)]
	public ushort h21_b;

	// Token: 0x04004387 RID: 17287
	[NonSerialized]
	[FieldOffset(40)]
	public ushort h22_a;

	// Token: 0x04004388 RID: 17288
	[NonSerialized]
	[FieldOffset(42)]
	public ushort h22_b;

	// Token: 0x04004389 RID: 17289
	[NonSerialized]
	[FieldOffset(44)]
	public ushort h23_a;

	// Token: 0x0400438A RID: 17290
	[NonSerialized]
	[FieldOffset(46)]
	public ushort h23_b;

	// Token: 0x0400438B RID: 17291
	[NonSerialized]
	[FieldOffset(48)]
	public ushort h30_a;

	// Token: 0x0400438C RID: 17292
	[NonSerialized]
	[FieldOffset(50)]
	public ushort h30_b;

	// Token: 0x0400438D RID: 17293
	[NonSerialized]
	[FieldOffset(52)]
	public ushort h31_a;

	// Token: 0x0400438E RID: 17294
	[NonSerialized]
	[FieldOffset(54)]
	public ushort h31_b;

	// Token: 0x0400438F RID: 17295
	[NonSerialized]
	[FieldOffset(56)]
	public ushort h32_a;

	// Token: 0x04004390 RID: 17296
	[NonSerialized]
	[FieldOffset(58)]
	public ushort h32_b;

	// Token: 0x04004391 RID: 17297
	[NonSerialized]
	[FieldOffset(60)]
	public ushort h33_a;

	// Token: 0x04004392 RID: 17298
	[NonSerialized]
	[FieldOffset(62)]
	public ushort h33_b;

	// Token: 0x0200087F RID: 2175
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_f>e__FixedBuffer
	{
		// Token: 0x04004393 RID: 17299
		public float FixedElementField;
	}

	// Token: 0x02000880 RID: 2176
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_h>e__FixedBuffer
	{
		// Token: 0x04004394 RID: 17300
		public ushort FixedElementField;
	}

	// Token: 0x02000881 RID: 2177
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_i>e__FixedBuffer
	{
		// Token: 0x04004395 RID: 17301
		public int FixedElementField;
	}
}
