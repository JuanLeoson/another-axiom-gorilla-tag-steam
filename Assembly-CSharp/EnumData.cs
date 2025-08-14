using System;
using System.Collections.Generic;

// Token: 0x02000867 RID: 2151
public class EnumData<TEnum> where TEnum : struct, Enum
{
	// Token: 0x17000528 RID: 1320
	// (get) Token: 0x06003601 RID: 13825 RVA: 0x0011BD85 File Offset: 0x00119F85
	public static EnumData<TEnum> Shared { get; } = new EnumData<TEnum>();

	// Token: 0x06003602 RID: 13826 RVA: 0x0011BD8C File Offset: 0x00119F8C
	private EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		int num = this.Names.Length;
		this.Values = new TEnum[num];
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = Enum.Parse<TEnum>(text);
			long num2 = Convert.ToInt64(tenum);
			this.Values[i] = tenum;
			this.LongValues[i] = num2;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.EnumToIndex[tenum] = i;
			this.IndexToEnum[i] = tenum;
			this.EnumToLong[tenum] = num2;
			this.LongToEnum[num2] = tenum;
		}
		long num3 = 0L;
		bool isBitMaskCompatible = true;
		foreach (long num4 in this.LongValues)
		{
			if (num4 != 0L && (num4 & num4 - 1L) != 0L && (num3 & num4) != num4)
			{
				isBitMaskCompatible = false;
				break;
			}
			num3 |= num4;
		}
		this.IsBitMaskCompatible = isBitMaskCompatible;
	}

	// Token: 0x040042E8 RID: 17128
	public readonly string[] Names;

	// Token: 0x040042E9 RID: 17129
	public readonly TEnum[] Values;

	// Token: 0x040042EA RID: 17130
	public readonly long[] LongValues;

	// Token: 0x040042EB RID: 17131
	public readonly bool IsBitMaskCompatible;

	// Token: 0x040042EC RID: 17132
	public readonly Dictionary<TEnum, string> EnumToName;

	// Token: 0x040042ED RID: 17133
	public readonly Dictionary<string, TEnum> NameToEnum;

	// Token: 0x040042EE RID: 17134
	public readonly Dictionary<TEnum, int> EnumToIndex;

	// Token: 0x040042EF RID: 17135
	public readonly Dictionary<int, TEnum> IndexToEnum;

	// Token: 0x040042F0 RID: 17136
	public readonly Dictionary<TEnum, long> EnumToLong;

	// Token: 0x040042F1 RID: 17137
	public readonly Dictionary<long, TEnum> LongToEnum;
}
