using System;
using System.Runtime.CompilerServices;

// Token: 0x020001FA RID: 506
public static class GTBitOps
{
	// Token: 0x06000BE8 RID: 3048 RVA: 0x00040D62 File Offset: 0x0003EF62
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00040D6C File Offset: 0x0003EF6C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00040D75 File Offset: 0x0003EF75
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00040D85 File Offset: 0x0003EF85
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return bits >> index & valueMask;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00040D8F File Offset: 0x0003EF8F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return bits >> info.index & info.valueMask;
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00040DA3 File Offset: 0x0003EFA3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return bits >> index & (1 << count) - 1;
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00040DB4 File Offset: 0x0003EFB4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return (bits >> index & 1) == 1;
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x00040DC1 File Offset: 0x0003EFC1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = ((bits & info.clearMask) | (value & info.valueMask) << info.index);
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00040DE1 File Offset: 0x0003EFE1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00040DED File Offset: 0x0003EFED
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = ((bits & clearMask) | (value & valueMask) << index);
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00040DFF File Offset: 0x0003EFFF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00040E0E File Offset: 0x0003F00E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = ((bits & ~((1 << count) - 1 << index)) | (value & (1 << count) - 1) << index);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00040E33 File Offset: 0x0003F033
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00040E40 File Offset: 0x0003F040
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = ((bits & ~(1 << index)) | (value ? 1 : 0) << index);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00040E5B File Offset: 0x0003F05B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00040E67 File Offset: 0x0003F067
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x020001FB RID: 507
	public readonly struct BitWriteInfo
	{
		// Token: 0x06000BF8 RID: 3064 RVA: 0x00040E79 File Offset: 0x0003F079
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04000EDA RID: 3802
		public readonly int index;

		// Token: 0x04000EDB RID: 3803
		public readonly int valueMask;

		// Token: 0x04000EDC RID: 3804
		public readonly int clearMask;
	}
}
