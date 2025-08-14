using System;
using Unity.Burst;

// Token: 0x020009C7 RID: 2503
public static class LuaHashing
{
	// Token: 0x06003CC6 RID: 15558 RVA: 0x00137138 File Offset: 0x00135338
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes, int len)
	{
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < len; i += 2)
		{
			num = ((num << 5) + num ^ (int)bytes[i]);
			if (i == len - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2 ^ (int)bytes[i + 1]);
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06003CC7 RID: 15559 RVA: 0x00137180 File Offset: 0x00135380
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes)
	{
		int num = 352654597;
		int num2 = num;
		int num3 = 0;
		while (bytes[num3] != 0)
		{
			num = ((num << 5) + num ^ (int)bytes[num3]);
			num3++;
			if (bytes[num3] == 0)
			{
				break;
			}
			num2 = ((num2 << 5) + num2 ^ (int)bytes[num3]);
			num3++;
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06003CC8 RID: 15560 RVA: 0x001371CC File Offset: 0x001353CC
	public static int ByteHash(string bytes)
	{
		int length = bytes.Length;
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < length; i += 2)
		{
			num = ((num << 5) + num ^ (int)bytes[i]);
			if (i == length - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2 ^ (int)bytes[i + 1]);
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06003CC9 RID: 15561 RVA: 0x00137224 File Offset: 0x00135424
	[BurstCompile]
	public static int ByteHash(byte[] bytes)
	{
		int num = bytes.Length;
		int num2 = 352654597;
		int num3 = num2;
		for (int i = 0; i < num; i += 2)
		{
			num2 = ((num2 << 5) + num2 ^ (int)bytes[i]);
			if (i == num - 1)
			{
				break;
			}
			num3 = ((num3 << 5) + num3 ^ (int)bytes[i + 1]);
		}
		return num2 + num3 * 1648465312;
	}

	// Token: 0x0400493B RID: 18747
	private const int k_enhancer = 1648465312;

	// Token: 0x0400493C RID: 18748
	private const int k_Seed = 352654597;
}
