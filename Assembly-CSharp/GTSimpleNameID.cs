using System;
using Unity.Mathematics;

// Token: 0x02000230 RID: 560
[Serializable]
public struct GTSimpleNameID
{
	// Token: 0x06000D2C RID: 3372 RVA: 0x000463B0 File Offset: 0x000445B0
	static GTSimpleNameID()
	{
		if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".Length != 64 || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[0] != '0' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[9] != '9' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[10] != 'A' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[36] != 'a' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[62] != '_' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[63] != '-')
		{
			throw new Exception("GTSimpleNameID: The constant string `_k_possibleChars` does not match the expected format. Did you change something without updating the logic?");
		}
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00046438 File Offset: 0x00044638
	public unsafe static GTSimpleNameID FromString(string input)
	{
		if (input == null)
		{
			input = string.Empty;
		}
		GTSimpleNameID result = default(GTSimpleNameID);
		int num = math.min(input.Length, 41);
		result.U0 = (ulong)((long)num & 63L);
		int num2 = 6;
		int i = 0;
		while (i < num)
		{
			char c = input[i];
			byte b;
			if (c >= 'A')
			{
				if (c >= 'a')
				{
					if (c > 'z')
					{
						goto IL_A7;
					}
					b = (byte)(c - 'a' + '$');
				}
				else if (c > 'Z')
				{
					if (c != '_')
					{
						goto IL_A7;
					}
					b = 62;
				}
				else
				{
					b = (byte)(c - 'A' + '\n');
				}
			}
			else if (c >= '0')
			{
				if (c > '9')
				{
					goto IL_A7;
				}
				b = (byte)(c - '0');
			}
			else
			{
				if (c != '-')
				{
					goto IL_A7;
				}
				b = 63;
			}
			ulong num3 = (ulong)b;
			int num4 = num2 + i * 6;
			ulong* ptr = &result.U0;
			int num5 = num4 / 64;
			int num6 = num4 % 64;
			ulong num7 = 63UL;
			ulong num8 = num3 & num7;
			ulong num9 = ~(num7 << num6);
			ptr[num5] &= num9;
			ptr[num5] |= num8 << num6;
			int num10 = 64 - num6;
			if (num10 < 6 && num5 < 3)
			{
				int num11 = 6 - num10;
				ulong num12 = (1UL << num11) - 1UL;
				ulong num13 = num8 >> num10;
				ptr[num5 + 1] &= ~num12;
				ptr[num5 + 1] |= num13;
			}
			i++;
			continue;
			IL_A7:
			throw new ArgumentException(string.Format("Invalid character '{0}' in input string.", c), "input");
		}
		return result;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x000465B8 File Offset: 0x000447B8
	public override string ToString()
	{
		int num = math.min((int)(this.U0 & 63UL), 41);
		char[] array = new char[num];
		int num2 = 6;
		for (int i = 0; i < num; i++)
		{
			int bitOffset = num2 + i * 6;
			ulong num3 = GTSimpleNameID._Read6Bits(this, bitOffset);
			array[i] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[(int)num3];
		}
		return new string(array);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00046614 File Offset: 0x00044814
	private unsafe static ulong _Read6Bits(in GTSimpleNameID cv, int bitOffset)
	{
		fixed (ulong* ptr = &cv.U0)
		{
			ulong* ptr2 = ptr;
			int num = bitOffset / 64;
			int num2 = bitOffset % 64;
			ulong num3 = ptr2[num] >> num2;
			int num4 = 64 - num2;
			if (num4 < 6 && num < 3)
			{
				int num5 = 6 - num4;
				ulong num6 = (1UL << num5) - 1UL;
				ulong num7 = ptr2[num + 1] & num6;
				num7 <<= num4;
				num3 |= num7;
			}
			return num3 & 63UL;
		}
	}

	// Token: 0x04001010 RID: 4112
	public ulong U0;

	// Token: 0x04001011 RID: 4113
	public ulong U1;

	// Token: 0x04001012 RID: 4114
	public ulong U2;

	// Token: 0x04001013 RID: 4115
	public ulong U3;

	// Token: 0x04001014 RID: 4116
	private const string _k_possibleChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";

	// Token: 0x04001015 RID: 4117
	private const int _k_maxLength = 41;

	// Token: 0x04001016 RID: 4118
	private const ulong _k_bitmask6Bits = 63UL;

	// Token: 0x04001017 RID: 4119
	private const ushort _k_indexOf_A = 10;

	// Token: 0x04001018 RID: 4120
	private const ushort _k_indexOf_a = 36;

	// Token: 0x04001019 RID: 4121
	private const ushort _k_indexOf_underscore = 62;

	// Token: 0x0400101A RID: 4122
	private const ushort _k_indexOf_hyphen = 63;
}
