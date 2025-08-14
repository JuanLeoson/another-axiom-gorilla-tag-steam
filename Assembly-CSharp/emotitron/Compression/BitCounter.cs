using System;

namespace emotitron.Compression
{
	// Token: 0x02000F76 RID: 3958
	public static class BitCounter
	{
		// Token: 0x0600627A RID: 25210 RVA: 0x001F32D4 File Offset: 0x001F14D4
		public static int UsedBitCount(this ulong val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			val |= val >> 32;
			return BitCounter.bitPatternToLog2[(int)(checked((IntPtr)(unchecked(val * 7783611145303519083UL) >> 57)))];
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x001F3322 File Offset: 0x001F1522
		public static int UsedBitCount(this uint val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			return BitCounter.bitPatternToLog2[(int)(checked((IntPtr)(unchecked((ulong)val * 7783611145303519083UL) >> 57)))];
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x001F335E File Offset: 0x001F155E
		public static int UsedBitCount(this int val)
		{
			val |= val >> 1;
			val |= val >> 2;
			val |= val >> 4;
			val |= val >> 8;
			val |= val >> 16;
			return BitCounter.bitPatternToLog2[(int)(checked((IntPtr)((ulong)(unchecked((long)val * 7783611145303519083L)) >> 57)))];
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x001F339C File Offset: 0x001F159C
		public static int UsedBitCount(this ushort val)
		{
			uint num = (uint)val | (uint)val >> 1;
			num |= num >> 2;
			num |= num >> 4;
			num |= num >> 8;
			return BitCounter.bitPatternToLog2[(int)(checked((IntPtr)(unchecked((ulong)num * 7783611145303519083UL) >> 57)))];
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x001F33DC File Offset: 0x001F15DC
		public static int UsedBitCount(this byte val)
		{
			uint num = (uint)val | (uint)val >> 1;
			num |= num >> 2;
			num |= num >> 4;
			return BitCounter.bitPatternToLog2[(int)(checked((IntPtr)(unchecked((ulong)num * 7783611145303519083UL) >> 57)))];
		}

		// Token: 0x0600627F RID: 25215 RVA: 0x001F3414 File Offset: 0x001F1614
		public static int UsedByteCount(this ulong val)
		{
			if (val == 0UL)
			{
				return 0;
			}
			if ((val & 1095216660480UL) != 0UL)
			{
				if ((val & 71776119061217280UL) != 0UL)
				{
					if ((val & 18374686479671623680UL) != 0UL)
					{
						return 8;
					}
					return 7;
				}
				else
				{
					if ((val & 280375465082880UL) != 0UL)
					{
						return 6;
					}
					return 5;
				}
			}
			else if ((val & 16711680UL) != 0UL)
			{
				if ((val & (ulong)-16777216) != 0UL)
				{
					return 4;
				}
				return 3;
			}
			else
			{
				if ((val & 65280UL) != 0UL)
				{
					return 2;
				}
				return 1;
			}
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x001F3487 File Offset: 0x001F1687
		public static int UsedByteCount(this uint val)
		{
			if (val == 0U)
			{
				return 0;
			}
			if ((val & 16711680U) != 0U)
			{
				if ((val & 4278190080U) != 0U)
				{
					return 4;
				}
				return 3;
			}
			else
			{
				if ((val & 65280U) != 0U)
				{
					return 2;
				}
				return 1;
			}
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x001F34B0 File Offset: 0x001F16B0
		public static int UsedByteCount(this ushort val)
		{
			if (val == 0)
			{
				return 0;
			}
			if ((val & 65280) != 0)
			{
				return 2;
			}
			return 1;
		}

		// Token: 0x04006E26 RID: 28198
		public static readonly int[] bitPatternToLog2 = new int[]
		{
			0,
			48,
			-1,
			-1,
			31,
			-1,
			15,
			51,
			-1,
			63,
			5,
			-1,
			-1,
			-1,
			19,
			-1,
			23,
			28,
			-1,
			-1,
			-1,
			40,
			36,
			46,
			-1,
			13,
			-1,
			-1,
			-1,
			34,
			-1,
			58,
			-1,
			60,
			2,
			43,
			55,
			-1,
			-1,
			-1,
			50,
			62,
			4,
			-1,
			18,
			27,
			-1,
			39,
			45,
			-1,
			-1,
			33,
			57,
			-1,
			1,
			54,
			-1,
			49,
			-1,
			17,
			-1,
			-1,
			32,
			-1,
			53,
			-1,
			16,
			-1,
			-1,
			52,
			-1,
			-1,
			-1,
			64,
			6,
			7,
			8,
			-1,
			9,
			-1,
			-1,
			-1,
			20,
			10,
			-1,
			-1,
			24,
			-1,
			29,
			-1,
			-1,
			21,
			-1,
			11,
			-1,
			-1,
			41,
			-1,
			25,
			37,
			-1,
			47,
			-1,
			30,
			14,
			-1,
			-1,
			-1,
			-1,
			22,
			-1,
			-1,
			35,
			12,
			-1,
			-1,
			-1,
			59,
			42,
			-1,
			-1,
			61,
			3,
			26,
			38,
			44,
			-1,
			56
		};

		// Token: 0x04006E27 RID: 28199
		public const ulong MULTIPLICATOR = 7783611145303519083UL;
	}
}
