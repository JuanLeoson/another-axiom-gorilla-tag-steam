using System;
using UnityEngine;

// Token: 0x0200088F RID: 2191
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x1700053F RID: 1343
	// (get) Token: 0x06003720 RID: 14112 RVA: 0x0011EEE4 File Offset: 0x0011D0E4
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x0011EEEC File Offset: 0x0011D0EC
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0011EEE4 File Offset: 0x0011D0E4
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x0011EF0A File Offset: 0x0011D10A
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x0011EF29 File Offset: 0x0011D129
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x0011EF38 File Offset: 0x0011D138
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x0011EF72 File Offset: 0x0011D172
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x040043C9 RID: 17353
	[SerializeField]
	private TEnum m_EnumValue;
}
