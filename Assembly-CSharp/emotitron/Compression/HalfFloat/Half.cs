using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000F82 RID: 3970
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x0600633D RID: 25405 RVA: 0x001F49F6 File Offset: 0x001F2BF6
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x0600633E RID: 25406 RVA: 0x001F4A04 File Offset: 0x001F2C04
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x001F4A0C File Offset: 0x001F2C0C
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x001F4A48 File Offset: 0x001F2C48
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x001F4A7C File Offset: 0x001F2C7C
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06006342 RID: 25410 RVA: 0x001F4A98 File Offset: 0x001F2C98
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06006343 RID: 25411 RVA: 0x001F4AA5 File Offset: 0x001F2CA5
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06006344 RID: 25412 RVA: 0x001F4AB2 File Offset: 0x001F2CB2
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06006345 RID: 25413 RVA: 0x001F4ABF File Offset: 0x001F2CBF
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06006346 RID: 25414 RVA: 0x001F4AD1 File Offset: 0x001F2CD1
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x001F4AE3 File Offset: 0x001F2CE3
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06006348 RID: 25416 RVA: 0x001F4AF8 File Offset: 0x001F2CF8
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06006349 RID: 25417 RVA: 0x001F4B0D File Offset: 0x001F2D0D
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600634A RID: 25418 RVA: 0x001F4B17 File Offset: 0x001F2D17
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x001F4B24 File Offset: 0x001F2D24
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x0600634C RID: 25420 RVA: 0x001F4B2C File Offset: 0x001F2D2C
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x0600634D RID: 25421 RVA: 0x001F4B3C File Offset: 0x001F2D3C
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x0600634E RID: 25422 RVA: 0x001F4B6C File Offset: 0x001F2D6C
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x0600634F RID: 25423 RVA: 0x001F4BB4 File Offset: 0x001F2DB4
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06006350 RID: 25424 RVA: 0x001F4BE0 File Offset: 0x001F2DE0
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x001F4C19 File Offset: 0x001F2E19
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x001F4C2C File Offset: 0x001F2E2C
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06006353 RID: 25427 RVA: 0x001F4C84 File Offset: 0x001F2E84
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x001F4CF8 File Offset: 0x001F2EF8
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x001F4D08 File Offset: 0x001F2F08
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x001F4D18 File Offset: 0x001F2F18
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x001F4D4A File Offset: 0x001F2F4A
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x001F4D5B File Offset: 0x001F2F5B
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06006359 RID: 25433 RVA: 0x001F4D6D File Offset: 0x001F2F6D
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x0600635A RID: 25434 RVA: 0x001F4D7F File Offset: 0x001F2F7F
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x001F4D8B File Offset: 0x001F2F8B
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x001F4D97 File Offset: 0x001F2F97
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x001F4DA9 File Offset: 0x001F2FA9
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x001F4DBB File Offset: 0x001F2FBB
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x001F4DCD File Offset: 0x001F2FCD
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x001F4DDF File Offset: 0x001F2FDF
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x001F4DF1 File Offset: 0x001F2FF1
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x001F4E03 File Offset: 0x001F3003
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x001F4E10 File Offset: 0x001F3010
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06006364 RID: 25444 RVA: 0x001F4E2A File Offset: 0x001F302A
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x001F4E3C File Offset: 0x001F303C
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x001F4E4E File Offset: 0x001F304E
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04006E5B RID: 28251
		private ushort value;

		// Token: 0x04006E5C RID: 28252
		public const int PrecisionDigits = 3;

		// Token: 0x04006E5D RID: 28253
		public const int MantissaBits = 11;

		// Token: 0x04006E5E RID: 28254
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04006E5F RID: 28255
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04006E60 RID: 28256
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04006E61 RID: 28257
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04006E62 RID: 28258
		public const int ExponentRadix = 2;

		// Token: 0x04006E63 RID: 28259
		public const int AdditionRounding = 1;

		// Token: 0x04006E64 RID: 28260
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04006E65 RID: 28261
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04006E66 RID: 28262
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x04006E67 RID: 28263
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x04006E68 RID: 28264
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x04006E69 RID: 28265
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
