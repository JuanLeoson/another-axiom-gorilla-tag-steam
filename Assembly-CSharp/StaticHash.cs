using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000AEE RID: 2798
public static class StaticHash
{
	// Token: 0x0600435B RID: 17243 RVA: 0x001532EC File Offset: 0x001514EC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i)
	{
		uint num = (uint)(i + 2127912214 + (i << 12));
		num = (num ^ 3345072700U ^ num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U ^ num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ num >> 16);
	}

	// Token: 0x0600435C RID: 17244 RVA: 0x00153348 File Offset: 0x00151548
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u)
	{
		uint num = u + 2127912214U + (u << 12);
		num = (num ^ 3345072700U ^ num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U ^ num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ num >> 16);
	}

	// Token: 0x0600435D RID: 17245 RVA: 0x001533A4 File Offset: 0x001515A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Compute(float f)
	{
		return StaticHash.Compute(*Unsafe.As<float, int>(ref f));
	}

	// Token: 0x0600435E RID: 17246 RVA: 0x001533B4 File Offset: 0x001515B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x0600435F RID: 17247 RVA: 0x001533D4 File Offset: 0x001515D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2, float f3)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		int i3 = StaticHash.Compute(f3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004360 RID: 17248 RVA: 0x001533FC File Offset: 0x001515FC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(float f1, float f2, float f3, float f4)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		int i3 = StaticHash.Compute(f3);
		int i4 = StaticHash.Compute(f4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004361 RID: 17249 RVA: 0x0015342C File Offset: 0x0015162C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l)
	{
		ulong num = (ulong)(~(ulong)l + (l << 18));
		num ^= num >> 31;
		num *= 21UL;
		num ^= num >> 11;
		num += num << 6;
		num ^= num >> 22;
		return (int)num;
	}

	// Token: 0x06004362 RID: 17250 RVA: 0x00153468 File Offset: 0x00151668
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004363 RID: 17251 RVA: 0x00153488 File Offset: 0x00151688
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2, long l3)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		int i3 = StaticHash.Compute(l3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004364 RID: 17252 RVA: 0x001534B0 File Offset: 0x001516B0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(long l1, long l2, long l3, long l4)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		int i3 = StaticHash.Compute(l3);
		int i4 = StaticHash.Compute(l4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004365 RID: 17253 RVA: 0x001534E0 File Offset: 0x001516E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Compute(double d)
	{
		return StaticHash.Compute(*Unsafe.As<double, long>(ref d));
	}

	// Token: 0x06004366 RID: 17254 RVA: 0x001534F0 File Offset: 0x001516F0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004367 RID: 17255 RVA: 0x00153510 File Offset: 0x00151710
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2, double d3)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		int i3 = StaticHash.Compute(d3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004368 RID: 17256 RVA: 0x00153538 File Offset: 0x00151738
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(double d1, double d2, double d3, double d4)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		int i3 = StaticHash.Compute(d3);
		int i4 = StaticHash.Compute(d4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004369 RID: 17257 RVA: 0x00153568 File Offset: 0x00151768
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b)
	{
		if (!b)
		{
			return 1800329511;
		}
		return -1266253386;
	}

	// Token: 0x0600436A RID: 17258 RVA: 0x00153578 File Offset: 0x00151778
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x0600436B RID: 17259 RVA: 0x00153598 File Offset: 0x00151798
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2, bool b3)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		int i3 = StaticHash.Compute(b3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x0600436C RID: 17260 RVA: 0x001535C0 File Offset: 0x001517C0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(bool b1, bool b2, bool b3, bool b4)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		int i3 = StaticHash.Compute(b3);
		int i4 = StaticHash.Compute(b4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x0600436D RID: 17261 RVA: 0x001535F0 File Offset: 0x001517F0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(DateTime dt)
	{
		return StaticHash.Compute(dt.ToBinary());
	}

	// Token: 0x0600436E RID: 17262 RVA: 0x00153600 File Offset: 0x00151800
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s)
	{
		if (s == null || s.Length == 0)
		{
			return 0;
		}
		int i = s.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)s[num3];
			uint num4 = (uint)((uint)s[num3 + 1] << 11) ^ num;
			num = (num << 16 ^ num4);
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)s[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x0600436F RID: 17263 RVA: 0x001536A8 File Offset: 0x001518A8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004370 RID: 17264 RVA: 0x001536C8 File Offset: 0x001518C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2, string s3)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		int i3 = StaticHash.Compute(s3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004371 RID: 17265 RVA: 0x001536F0 File Offset: 0x001518F0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s1, string s2, string s3, string s4)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		int i3 = StaticHash.Compute(s3);
		int i4 = StaticHash.Compute(s4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x00153720 File Offset: 0x00151920
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return 0;
		}
		int i = bytes.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)bytes[num3];
			uint num4 = (uint)((int)bytes[num3 + 1] << 11 ^ (int)num);
			num = (num << 16 ^ num4);
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)bytes[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x06004373 RID: 17267 RVA: 0x001537B4 File Offset: 0x001519B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint result = num;
		num += (uint)i1;
		num2 += (uint)i2;
		StaticHash.Finalize(ref num, ref num2, ref result);
		return (int)result;
	}

	// Token: 0x06004374 RID: 17268 RVA: 0x001537E0 File Offset: 0x001519E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2, int i3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x00153810 File Offset: 0x00151A10
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int i1, int i2, int i3, int i4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += (uint)i4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x00153850 File Offset: 0x00151A50
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += (uint)values[num5];
			num3 += (uint)values[num5 + 1];
			num4 += (uint)values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += (uint)values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += (uint)values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += (uint)values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x001538EC File Offset: 0x00151AEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += values[num5];
			num3 += values[num5 + 1];
			num4 += values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06004378 RID: 17272 RVA: 0x00153988 File Offset: 0x00151B88
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint result = num;
		num += u1;
		num2 += u2;
		StaticHash.Finalize(ref num, ref num2, ref result);
		return (int)result;
	}

	// Token: 0x06004379 RID: 17273 RVA: 0x001539B4 File Offset: 0x00151BB4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2, uint u3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x0600437A RID: 17274 RVA: 0x001539E4 File Offset: 0x00151BE4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(uint u1, uint u2, uint u3, uint u4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += u4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x0600437B RID: 17275 RVA: 0x00153A24 File Offset: 0x00151C24
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeOrderAgnostic(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 0;
		}
		uint num = (uint)StaticHash.Compute(values[0]);
		if (values.Length == 1)
		{
			return (int)num;
		}
		for (int i = 1; i < values.Length; i++)
		{
			num += (uint)StaticHash.Compute(values[i]);
		}
		return (int)num;
	}

	// Token: 0x0600437C RID: 17276 RVA: 0x00153A68 File Offset: 0x00151C68
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Compute128To64(long a, long b)
	{
		ulong num = (ulong)((b ^ a) * -7070675565921424023L);
		num ^= num >> 47;
		long num2 = (a ^ (long)num) * -7070675565921424023L;
		return (num2 ^ (long)((ulong)num2 >> 47)) * -7070675565921424023L;
	}

	// Token: 0x0600437D RID: 17277 RVA: 0x00153AA8 File Offset: 0x00151CA8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Compute128To64(ulong a, ulong b)
	{
		ulong num = (b ^ a) * 11376068507788127593UL;
		num ^= num >> 47;
		ulong num2 = (a ^ num) * 11376068507788127593UL;
		return (long)((num2 ^ num2 >> 47) * 11376068507788127593UL);
	}

	// Token: 0x0600437E RID: 17278 RVA: 0x00153AE6 File Offset: 0x00151CE6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeTriple32(int i)
	{
		int num = i + 1;
		int num2 = (num ^ (int)((uint)num >> 17)) * -312814405;
		int num3 = (num2 ^ (int)((uint)num2 >> 11)) * -1404298415;
		int num4 = (num3 ^ (int)((uint)num3 >> 15)) * 830770091;
		return num4 ^ (int)((uint)num4 >> 14);
	}

	// Token: 0x0600437F RID: 17279 RVA: 0x00153B14 File Offset: 0x00151D14
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReverseTriple32(int i)
	{
		uint num = (uint)(i ^ (int)((uint)i >> 14 ^ (uint)i >> 28));
		num *= 850532099U;
		num ^= (num >> 15 ^ num >> 30);
		num *= 1184763313U;
		num ^= (num >> 11 ^ num >> 22);
		num *= 2041073779U;
		num ^= num >> 17;
		return (int)(num - 1U);
	}

	// Token: 0x06004380 RID: 17280 RVA: 0x00153B6C File Offset: 0x00151D6C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Mix(ref uint a, ref uint b, ref uint c)
	{
		a -= c;
		a ^= StaticHash.Rotate(c, 4);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 6);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 8);
		b += a;
		a -= c;
		a ^= StaticHash.Rotate(c, 16);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 19);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 4);
		b += a;
	}

	// Token: 0x06004381 RID: 17281 RVA: 0x00153C20 File Offset: 0x00151E20
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Finalize(ref uint a, ref uint b, ref uint c)
	{
		c ^= b;
		c -= StaticHash.Rotate(b, 14);
		a ^= c;
		a -= StaticHash.Rotate(c, 11);
		b ^= a;
		b -= StaticHash.Rotate(a, 25);
		c ^= b;
		c -= StaticHash.Rotate(b, 16);
		a ^= c;
		a -= StaticHash.Rotate(c, 4);
		b ^= a;
		b -= StaticHash.Rotate(a, 14);
		c ^= b;
		c -= StaticHash.Rotate(b, 24);
	}

	// Token: 0x06004382 RID: 17282 RVA: 0x00153CBF File Offset: 0x00151EBF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Rotate(uint x, int k)
	{
		return x << k | x >> 32 - k;
	}

	// Token: 0x02000AEF RID: 2799
	[StructLayout(LayoutKind.Explicit)]
	private struct SingleInt32
	{
		// Token: 0x04004E1E RID: 19998
		[FieldOffset(0)]
		public float single;

		// Token: 0x04004E1F RID: 19999
		[FieldOffset(0)]
		public int int32;
	}

	// Token: 0x02000AF0 RID: 2800
	[StructLayout(LayoutKind.Explicit)]
	private struct DoubleInt64
	{
		// Token: 0x04004E20 RID: 20000
		[FieldOffset(0)]
		public double @double;

		// Token: 0x04004E21 RID: 20001
		[FieldOffset(0)]
		public long int64;
	}
}
