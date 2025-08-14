using System;

namespace emotitron.Compression
{
	// Token: 0x02000F73 RID: 3955
	public static class ArraySerializeUnsafe
	{
		// Token: 0x06006247 RID: 25159 RVA: 0x001F29D0 File Offset: 0x001F0BD0
		public unsafe static void WriteSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006248 RID: 25160 RVA: 0x001F29F0 File Offset: 0x001F0BF0
		public unsafe static void AppendSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x001F2A10 File Offset: 0x001F0C10
		public unsafe static void AddSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x001F2A30 File Offset: 0x001F0C30
		public unsafe static void AddSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624B RID: 25163 RVA: 0x001F2A50 File Offset: 0x001F0C50
		public unsafe static void AddSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624C RID: 25164 RVA: 0x001F2A70 File Offset: 0x001F0C70
		public unsafe static void InjectSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x001F2A90 File Offset: 0x001F0C90
		public unsafe static void InjectSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x001F2AB0 File Offset: 0x001F0CB0
		public unsafe static void InjectSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x001F2AD0 File Offset: 0x001F0CD0
		public unsafe static void PokeSigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006250 RID: 25168 RVA: 0x001F2AF4 File Offset: 0x001F0CF4
		public unsafe static void PokeSigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006251 RID: 25169 RVA: 0x001F2B18 File Offset: 0x001F0D18
		public unsafe static void PokeSigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x001F2B3C File Offset: 0x001F0D3C
		public unsafe static int ReadSigned(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x001F2B60 File Offset: 0x001F0D60
		public unsafe static int PeekSigned(ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006254 RID: 25172 RVA: 0x001F2B84 File Offset: 0x001F0D84
		public unsafe static void Append(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (uPtr[num2] & num3) | value << num;
			uPtr[num2] = num4;
			uPtr[num2 + 1] = num4 >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06006255 RID: 25173 RVA: 0x001F2BDC File Offset: 0x001F0DDC
		public unsafe static void Write(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			num = 64 - num;
			if (num < bits)
			{
				num4 = num3 >> num;
				num5 = value >> num;
				num2++;
				uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x001F2C60 File Offset: 0x001F0E60
		public unsafe static ulong Read(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x001F2CC4 File Offset: 0x001F0EC4
		public unsafe static ulong Read(ulong* uPtr, int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x001F2D23 File Offset: 0x001F0F23
		public unsafe static void Add(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x001F2D2F File Offset: 0x001F0F2F
		public unsafe static void Add(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x001F2D2F File Offset: 0x001F0F2F
		public unsafe static void Add(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x001F2D2F File Offset: 0x001F0F2F
		public unsafe static void Add(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x001F2D23 File Offset: 0x001F0F23
		public unsafe static void AddUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600625D RID: 25181 RVA: 0x001F2D3C File Offset: 0x001F0F3C
		public unsafe static void AddUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600625E RID: 25182 RVA: 0x001F2D3C File Offset: 0x001F0F3C
		public unsafe static void AddUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x001F2D3C File Offset: 0x001F0F3C
		public unsafe static void AddUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006260 RID: 25184 RVA: 0x001F2D49 File Offset: 0x001F0F49
		public unsafe static void Inject(this ulong value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006261 RID: 25185 RVA: 0x001F2D54 File Offset: 0x001F0F54
		public unsafe static void Inject(this uint value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006262 RID: 25186 RVA: 0x001F2D54 File Offset: 0x001F0F54
		public unsafe static void Inject(this ushort value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x001F2D54 File Offset: 0x001F0F54
		public unsafe static void Inject(this byte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006264 RID: 25188 RVA: 0x001F2D49 File Offset: 0x001F0F49
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x001F2D60 File Offset: 0x001F0F60
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006266 RID: 25190 RVA: 0x001F2D6C File Offset: 0x001F0F6C
		public unsafe static void InjectUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006267 RID: 25191 RVA: 0x001F2D60 File Offset: 0x001F0F60
		public unsafe static void InjectUnsigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x001F2D79 File Offset: 0x001F0F79
		public unsafe static void Poke(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x001F2D85 File Offset: 0x001F0F85
		public unsafe static void Poke(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x001F2D85 File Offset: 0x001F0F85
		public unsafe static void Poke(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600626B RID: 25195 RVA: 0x001F2D85 File Offset: 0x001F0F85
		public unsafe static void Poke(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600626C RID: 25196 RVA: 0x001F2D79 File Offset: 0x001F0F79
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600626D RID: 25197 RVA: 0x001F2D6C File Offset: 0x001F0F6C
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001F2D6C File Offset: 0x001F0F6C
		public unsafe static void PokeUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x001F2D6C File Offset: 0x001F0F6C
		public unsafe static void PokeUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x001F2D94 File Offset: 0x001F0F94
		public unsafe static void ReadOutUnsafe(ulong* sourcePtr, int sourcePos, ulong* targetPtr, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = ArraySerializeUnsafe.Read(sourcePtr, ref num, num2);
				ArraySerializeUnsafe.Write(targetPtr, value, ref targetPos, num2);
			}
			targetPos += bits;
		}

		// Token: 0x06006271 RID: 25201 RVA: 0x001F2DDC File Offset: 0x001F0FDC
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x001F2E68 File Offset: 0x001F1068
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x001F2EF4 File Offset: 0x001F10F4
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr2;
					if (target == null || array2.Length == 0)
					{
						uPtr2 = null;
					}
					else
					{
						uPtr2 = &array2[0];
					}
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006274 RID: 25204 RVA: 0x001F2F7C File Offset: 0x001F117C
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006275 RID: 25205 RVA: 0x001F300C File Offset: 0x001F120C
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006276 RID: 25206 RVA: 0x001F309C File Offset: 0x001F129C
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x001F3128 File Offset: 0x001F1328
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x001F31B4 File Offset: 0x001F13B4
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x001F3244 File Offset: 0x001F1444
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x04006E1B RID: 28187
		private const string bufferOverrunMsg = "Byte buffer overrun. Dataloss will occur.";
	}
}
