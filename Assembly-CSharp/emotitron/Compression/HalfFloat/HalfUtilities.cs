using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000F83 RID: 3971
	public static class HalfUtilities
	{
		// Token: 0x06006368 RID: 25448 RVA: 0x001F4EC8 File Offset: 0x001F30C8
		static HalfUtilities()
		{
			HalfUtilities.HalfToFloatMantissaTable[0] = 0U;
			for (int i = 1; i < 1024; i++)
			{
				uint num = (uint)((uint)i << 13);
				uint num2 = 0U;
				while ((num & 8388608U) == 0U)
				{
					num2 -= 8388608U;
					num <<= 1;
				}
				num &= 4286578687U;
				num2 += 947912704U;
				HalfUtilities.HalfToFloatMantissaTable[i] = (num | num2);
			}
			for (int i = 1024; i < 2048; i++)
			{
				HalfUtilities.HalfToFloatMantissaTable[i] = (uint)(939524096 + (i - 1024 << 13));
			}
			HalfUtilities.HalfToFloatExponentTable[0] = 0U;
			for (int i = 1; i < 63; i++)
			{
				if (i < 31)
				{
					HalfUtilities.HalfToFloatExponentTable[i] = (uint)((uint)i << 23);
				}
				else
				{
					HalfUtilities.HalfToFloatExponentTable[i] = (uint)(int.MinValue + (i - 32 << 23));
				}
			}
			HalfUtilities.HalfToFloatExponentTable[31] = 1199570944U;
			HalfUtilities.HalfToFloatExponentTable[32] = 2147483648U;
			HalfUtilities.HalfToFloatExponentTable[63] = 3347054592U;
			HalfUtilities.HalfToFloatOffsetTable[0] = 0U;
			for (int i = 1; i < 64; i++)
			{
				HalfUtilities.HalfToFloatOffsetTable[i] = 1024U;
			}
			HalfUtilities.HalfToFloatOffsetTable[32] = 0U;
			for (int i = 0; i < 256; i++)
			{
				int num3 = i - 127;
				if (num3 < -24)
				{
					HalfUtilities.FloatToHalfBaseTable[i | 0] = 0;
					HalfUtilities.FloatToHalfBaseTable[i | 256] = 32768;
					HalfUtilities.FloatToHalfShiftTable[i | 0] = 24;
					HalfUtilities.FloatToHalfShiftTable[i | 256] = 24;
				}
				else if (num3 < -14)
				{
					HalfUtilities.FloatToHalfBaseTable[i | 0] = (ushort)(1024 >> -num3 - 14);
					HalfUtilities.FloatToHalfBaseTable[i | 256] = (ushort)(1024 >> -num3 - 14 | 32768);
					HalfUtilities.FloatToHalfShiftTable[i | 0] = (byte)(-num3 - 1);
					HalfUtilities.FloatToHalfShiftTable[i | 256] = (byte)(-num3 - 1);
				}
				else if (num3 <= 15)
				{
					HalfUtilities.FloatToHalfBaseTable[i | 0] = (ushort)(num3 + 15 << 10);
					HalfUtilities.FloatToHalfBaseTable[i | 256] = (ushort)(num3 + 15 << 10 | 32768);
					HalfUtilities.FloatToHalfShiftTable[i | 0] = 13;
					HalfUtilities.FloatToHalfShiftTable[i | 256] = 13;
				}
				else if (num3 < 128)
				{
					HalfUtilities.FloatToHalfBaseTable[i | 0] = 31744;
					HalfUtilities.FloatToHalfBaseTable[i | 256] = 64512;
					HalfUtilities.FloatToHalfShiftTable[i | 0] = 24;
					HalfUtilities.FloatToHalfShiftTable[i | 256] = 24;
				}
				else
				{
					HalfUtilities.FloatToHalfBaseTable[i | 0] = 31744;
					HalfUtilities.FloatToHalfBaseTable[i | 256] = 64512;
					HalfUtilities.FloatToHalfShiftTable[i | 0] = 13;
					HalfUtilities.FloatToHalfShiftTable[i | 256] = 13;
				}
			}
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x001F51AC File Offset: 0x001F33AC
		public static float Unpack(ushort value)
		{
			return new HalfUtilities.FloatToUint
			{
				uintValue = HalfUtilities.HalfToFloatMantissaTable[(int)(HalfUtilities.HalfToFloatOffsetTable[value >> 10] + (uint)(value & 1023))] + HalfUtilities.HalfToFloatExponentTable[value >> 10]
			}.floatValue;
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x001F51F4 File Offset: 0x001F33F4
		public static ushort Pack(float value)
		{
			HalfUtilities.FloatToUint floatToUint = default(HalfUtilities.FloatToUint);
			floatToUint.floatValue = value;
			return (ushort)((uint)HalfUtilities.FloatToHalfBaseTable[(int)(floatToUint.uintValue >> 23 & 511U)] + ((floatToUint.uintValue & 8388607U) >> (int)HalfUtilities.FloatToHalfShiftTable[(int)(floatToUint.uintValue >> 23 & 511U)]));
		}

		// Token: 0x04006E6A RID: 28266
		private static readonly uint[] HalfToFloatMantissaTable = new uint[2048];

		// Token: 0x04006E6B RID: 28267
		private static readonly uint[] HalfToFloatExponentTable = new uint[64];

		// Token: 0x04006E6C RID: 28268
		private static readonly uint[] HalfToFloatOffsetTable = new uint[64];

		// Token: 0x04006E6D RID: 28269
		private static readonly ushort[] FloatToHalfBaseTable = new ushort[512];

		// Token: 0x04006E6E RID: 28270
		private static readonly byte[] FloatToHalfShiftTable = new byte[512];

		// Token: 0x02000F84 RID: 3972
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatToUint
		{
			// Token: 0x04006E6F RID: 28271
			[FieldOffset(0)]
			public uint uintValue;

			// Token: 0x04006E70 RID: 28272
			[FieldOffset(0)]
			public float floatValue;
		}
	}
}
