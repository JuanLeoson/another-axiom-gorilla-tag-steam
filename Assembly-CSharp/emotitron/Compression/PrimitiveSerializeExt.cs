using System;
using emotitron.Compression.HalfFloat;
using emotitron.Compression.Utilities;

namespace emotitron.Compression
{
	// Token: 0x02000F79 RID: 3961
	public static class PrimitiveSerializeExt
	{
		// Token: 0x06006297 RID: 25239 RVA: 0x001F3856 File Offset: 0x001F1A56
		public static void Inject(this ByteConverter value, ref ulong buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x001F3866 File Offset: 0x001F1A66
		public static void Inject(this ByteConverter value, ref uint buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x001F3876 File Offset: 0x001F1A76
		public static void Inject(this ByteConverter value, ref ushort buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x001F3886 File Offset: 0x001F1A86
		public static void Inject(this ByteConverter value, ref byte buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x001F3898 File Offset: 0x001F1A98
		public static ulong WriteSigned(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x001F38B8 File Offset: 0x001F1AB8
		public static void InjectSigned(this long value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x001F38CB File Offset: 0x001F1ACB
		public static void InjectSigned(this int value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x001F38CB File Offset: 0x001F1ACB
		public static void InjectSigned(this short value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x001F38CB File Offset: 0x001F1ACB
		public static void InjectSigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x001F38E0 File Offset: 0x001F1AE0
		public static int ReadSigned(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060062A1 RID: 25249 RVA: 0x001F3904 File Offset: 0x001F1B04
		public static uint WriteSigned(this uint buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060062A2 RID: 25250 RVA: 0x001F3924 File Offset: 0x001F1B24
		public static void InjectSigned(this long value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A3 RID: 25251 RVA: 0x001F3937 File Offset: 0x001F1B37
		public static void InjectSigned(this int value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A4 RID: 25252 RVA: 0x001F3937 File Offset: 0x001F1B37
		public static void InjectSigned(this short value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A5 RID: 25253 RVA: 0x001F3937 File Offset: 0x001F1B37
		public static void InjectSigned(this sbyte value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A6 RID: 25254 RVA: 0x001F394C File Offset: 0x001F1B4C
		public static int ReadSigned(this uint buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x001F3970 File Offset: 0x001F1B70
		public static ushort WriteSigned(this ushort buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x001F3990 File Offset: 0x001F1B90
		public static void InjectSigned(this long value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062A9 RID: 25257 RVA: 0x001F39A3 File Offset: 0x001F1BA3
		public static void InjectSigned(this int value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x001F39A3 File Offset: 0x001F1BA3
		public static void InjectSigned(this short value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062AB RID: 25259 RVA: 0x001F39A3 File Offset: 0x001F1BA3
		public static void InjectSigned(this sbyte value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062AC RID: 25260 RVA: 0x001F39B8 File Offset: 0x001F1BB8
		public static int ReadSigned(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060062AD RID: 25261 RVA: 0x001F39DC File Offset: 0x001F1BDC
		public static byte WriteSigned(this byte buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060062AE RID: 25262 RVA: 0x001F39FC File Offset: 0x001F1BFC
		public static void InjectSigned(this long value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062AF RID: 25263 RVA: 0x001F3A0F File Offset: 0x001F1C0F
		public static void InjectSigned(this int value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062B0 RID: 25264 RVA: 0x001F3A0F File Offset: 0x001F1C0F
		public static void InjectSigned(this short value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062B1 RID: 25265 RVA: 0x001F3A0F File Offset: 0x001F1C0F
		public static void InjectSigned(this sbyte value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x060062B2 RID: 25266 RVA: 0x001F3A24 File Offset: 0x001F1C24
		public static int ReadSigned(this byte buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060062B3 RID: 25267 RVA: 0x001F3A45 File Offset: 0x001F1C45
		public static ulong WritetBool(this ulong buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060062B4 RID: 25268 RVA: 0x001F3A57 File Offset: 0x001F1C57
		public static uint WritetBool(this uint buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x001F3A69 File Offset: 0x001F1C69
		public static ushort WritetBool(this ushort buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x001F3A7B File Offset: 0x001F1C7B
		public static byte WritetBool(this byte buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x001F3A8D File Offset: 0x001F1C8D
		public static void Inject(this bool value, ref ulong buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x001F3A9F File Offset: 0x001F1C9F
		public static void Inject(this bool value, ref uint buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x001F3AB1 File Offset: 0x001F1CB1
		public static void Inject(this bool value, ref ushort buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060062BA RID: 25274 RVA: 0x001F3AC3 File Offset: 0x001F1CC3
		public static void Inject(this bool value, ref byte buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x001F3AD5 File Offset: 0x001F1CD5
		public static bool ReadBool(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0UL;
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x001F3AE4 File Offset: 0x001F1CE4
		public static bool ReadtBool(this uint buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x001F3AF3 File Offset: 0x001F1CF3
		public static bool ReadBool(this ushort buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x001F3B02 File Offset: 0x001F1D02
		public static bool ReadBool(this byte buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x060062BF RID: 25279 RVA: 0x001F3B14 File Offset: 0x001F1D14
		public static ulong Write(this ulong buffer, ulong value, ref int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060062C0 RID: 25280 RVA: 0x001F3B50 File Offset: 0x001F1D50
		public static uint Write(this uint buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = uint.MaxValue >> 32 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060062C1 RID: 25281 RVA: 0x001F3B8C File Offset: 0x001F1D8C
		public static ushort Write(this ushort buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 65535U >> 16 - bits << bitposition;
			buffer = (ushort)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060062C2 RID: 25282 RVA: 0x001F3BC8 File Offset: 0x001F1DC8
		public static byte Write(this byte buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 255U >> 8 - bits << bitposition;
			buffer = (byte)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x060062C3 RID: 25283 RVA: 0x001F3C03 File Offset: 0x001F1E03
		public static void Inject(this ulong value, ref ulong buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062C4 RID: 25284 RVA: 0x001F3C14 File Offset: 0x001F1E14
		public static void Inject(this ulong value, ref ulong buffer, int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
		}

		// Token: 0x060062C5 RID: 25285 RVA: 0x001F3C47 File Offset: 0x001F1E47
		public static void Inject(this uint value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062C6 RID: 25286 RVA: 0x001F3C58 File Offset: 0x001F1E58
		public static void Inject(this uint value, ref ulong buffer, int bitposition, int bits = 32)
		{
			ulong num = (ulong)value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
		}

		// Token: 0x060062C7 RID: 25287 RVA: 0x001F3C47 File Offset: 0x001F1E47
		public static void Inject(this ushort value, ref ulong buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062C8 RID: 25288 RVA: 0x001F3C8C File Offset: 0x001F1E8C
		public static void Inject(this ushort value, ref ulong buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x001F3C47 File Offset: 0x001F1E47
		public static void Inject(this byte value, ref ulong buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062CA RID: 25290 RVA: 0x001F3C8C File Offset: 0x001F1E8C
		public static void Inject(this byte value, ref ulong buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x001F3C03 File Offset: 0x001F1E03
		public static void InjectUnsigned(this long value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x001F3C9C File Offset: 0x001F1E9C
		public static void InjectUnsigned(this int value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062CD RID: 25293 RVA: 0x001F3C9C File Offset: 0x001F1E9C
		public static void InjectUnsigned(this short value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x001F3C9C File Offset: 0x001F1E9C
		public static void InjectUnsigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x001F3CAB File Offset: 0x001F1EAB
		public static void Inject(this ulong value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x001F3CB9 File Offset: 0x001F1EB9
		public static void Inject(this ulong value, ref uint buffer, int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x001F3CC8 File Offset: 0x001F1EC8
		public static void Inject(this uint value, ref uint buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x001F3CD7 File Offset: 0x001F1ED7
		public static void Inject(this uint value, ref uint buffer, int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D3 RID: 25299 RVA: 0x001F3CC8 File Offset: 0x001F1EC8
		public static void Inject(this ushort value, ref uint buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D4 RID: 25300 RVA: 0x001F3CD7 File Offset: 0x001F1ED7
		public static void Inject(this ushort value, ref uint buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D5 RID: 25301 RVA: 0x001F3CC8 File Offset: 0x001F1EC8
		public static void Inject(this byte value, ref uint buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x001F3CD7 File Offset: 0x001F1ED7
		public static void Inject(this byte value, ref uint buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D7 RID: 25303 RVA: 0x001F3CAB File Offset: 0x001F1EAB
		public static void InjectUnsigned(this long value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062D8 RID: 25304 RVA: 0x001F3CE7 File Offset: 0x001F1EE7
		public static void InjectUnsigned(this int value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x001F3CE7 File Offset: 0x001F1EE7
		public static void InjectUnsigned(this short value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x001F3CE7 File Offset: 0x001F1EE7
		public static void InjectUnsigned(this sbyte value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x001F3CF6 File Offset: 0x001F1EF6
		public static void Inject(this ulong value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x001F3D04 File Offset: 0x001F1F04
		public static void Inject(this ulong value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x001F3D13 File Offset: 0x001F1F13
		public static void Inject(this uint value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062DE RID: 25310 RVA: 0x001F3D22 File Offset: 0x001F1F22
		public static void Inject(this uint value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062DF RID: 25311 RVA: 0x001F3D13 File Offset: 0x001F1F13
		public static void Inject(this ushort value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x001F3D22 File Offset: 0x001F1F22
		public static void Inject(this ushort value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x001F3D13 File Offset: 0x001F1F13
		public static void Inject(this byte value, ref ushort buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x001F3D22 File Offset: 0x001F1F22
		public static void Inject(this byte value, ref ushort buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E3 RID: 25315 RVA: 0x001F3D32 File Offset: 0x001F1F32
		public static void Inject(this ulong value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062E4 RID: 25316 RVA: 0x001F3D40 File Offset: 0x001F1F40
		public static void Inject(this ulong value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x001F3D4F File Offset: 0x001F1F4F
		public static void Inject(this uint value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x001F3D5E File Offset: 0x001F1F5E
		public static void Inject(this uint value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x001F3D4F File Offset: 0x001F1F4F
		public static void Inject(this ushort value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E8 RID: 25320 RVA: 0x001F3D5E File Offset: 0x001F1F5E
		public static void Inject(this ushort value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062E9 RID: 25321 RVA: 0x001F3D4F File Offset: 0x001F1F4F
		public static void Inject(this byte value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062EA RID: 25322 RVA: 0x001F3D5E File Offset: 0x001F1F5E
		public static void Inject(this byte value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x001F3D6E File Offset: 0x001F1F6E
		[Obsolete("Argument order changed")]
		public static ulong Extract(this ulong value, int bits, ref int bitposition)
		{
			return value.Extract(bits, ref bitposition);
		}

		// Token: 0x060062EC RID: 25324 RVA: 0x001F3D78 File Offset: 0x001F1F78
		public static ulong Read(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062ED RID: 25325 RVA: 0x001F3DA0 File Offset: 0x001F1FA0
		[Obsolete("Use Read instead.")]
		public static ulong Extract(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062EE RID: 25326 RVA: 0x001F3DC8 File Offset: 0x001F1FC8
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static ulong Extract(this ulong value, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			return value & num;
		}

		// Token: 0x060062EF RID: 25327 RVA: 0x001F3DE4 File Offset: 0x001F1FE4
		public static uint Read(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F0 RID: 25328 RVA: 0x001F3E0C File Offset: 0x001F200C
		[Obsolete("Use Read instead.")]
		public static uint Extract(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F1 RID: 25329 RVA: 0x001F3E34 File Offset: 0x001F2034
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static uint Extract(this uint value, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			return value & num;
		}

		// Token: 0x060062F2 RID: 25330 RVA: 0x001F3E50 File Offset: 0x001F2050
		public static uint Read(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F3 RID: 25331 RVA: 0x001F3E7C File Offset: 0x001F207C
		[Obsolete("Use Read instead.")]
		public static uint Extract(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F4 RID: 25332 RVA: 0x001F3EA8 File Offset: 0x001F20A8
		public static uint Read(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x001F3ED4 File Offset: 0x001F20D4
		[Obsolete("Use Read instead.")]
		public static uint Extract(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x001F3F00 File Offset: 0x001F2100
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static byte Extract(this byte value, int bits)
		{
			uint num = 255U >> 8 - bits;
			return (byte)((uint)value & num);
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x001F3F1E File Offset: 0x001F211E
		public static void Inject(this float f, ref ulong buffer, ref int bitposition)
		{
			buffer = buffer.Write(f, ref bitposition, 32);
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x001F3F37 File Offset: 0x001F2137
		public static float ReadFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x060062F9 RID: 25337 RVA: 0x001F3F4C File Offset: 0x001F214C
		[Obsolete("Use Read instead.")]
		public static float ExtractFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Extract(ref bitposition, 32);
		}

		// Token: 0x060062FA RID: 25338 RVA: 0x001F3F64 File Offset: 0x001F2164
		public static ushort InjectAsHalfFloat(this float f, ref ulong buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001F3F88 File Offset: 0x001F2188
		public static ushort InjectAsHalfFloat(this float f, ref uint buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x001F3FAB File Offset: 0x001F21AB
		public static float ReadHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x060062FD RID: 25341 RVA: 0x001F3FBC File Offset: 0x001F21BC
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x060062FE RID: 25342 RVA: 0x001F3FCD File Offset: 0x001F21CD
		public static float ReadHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x060062FF RID: 25343 RVA: 0x001F3FDE File Offset: 0x001F21DE
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x06006300 RID: 25344 RVA: 0x001F3FEF File Offset: 0x001F21EF
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref uint buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006301 RID: 25345 RVA: 0x001F3FFA File Offset: 0x001F21FA
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref ulong buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x04006E28 RID: 28200
		private const string overrunerror = "Write buffer overrun. writepos + bits exceeds target length. Data loss will occur.";
	}
}
