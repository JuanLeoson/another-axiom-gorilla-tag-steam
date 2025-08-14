using System;

namespace emotitron.Compression
{
	// Token: 0x02000F6F RID: 3951
	public static class ArrayPackBitsExt
	{
		// Token: 0x060061CF RID: 25039 RVA: 0x001F1680 File Offset: 0x001EF880
		public unsafe static void WritePackedBits(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num);
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x001F16B4 File Offset: 0x001EF8B4
		public static void WritePackedBits(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060061D1 RID: 25041 RVA: 0x001F16E8 File Offset: 0x001EF8E8
		public static void WritePackedBits(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)((long)num), ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060061D2 RID: 25042 RVA: 0x001F171C File Offset: 0x001EF91C
		public static void WritePackedBits(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x060061D3 RID: 25043 RVA: 0x001F174C File Offset: 0x001EF94C
		public unsafe static ulong ReadPackedBits(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits2);
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits3);
		}

		// Token: 0x060061D4 RID: 25044 RVA: 0x001F1778 File Offset: 0x001EF978
		public static ulong ReadPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061D5 RID: 25045 RVA: 0x001F17A4 File Offset: 0x001EF9A4
		public static ulong ReadPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061D6 RID: 25046 RVA: 0x001F17D0 File Offset: 0x001EF9D0
		public static ulong ReadPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x001F17FC File Offset: 0x001EF9FC
		public unsafe static void WriteSignedPackedBits(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArrayPackBitsExt.WritePackedBits(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061D8 RID: 25048 RVA: 0x001F181C File Offset: 0x001EFA1C
		public unsafe static int ReadSignedPackedBits(ulong* buffer, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x001F1840 File Offset: 0x001EFA40
		public static void WriteSignedPackedBits(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x001F1860 File Offset: 0x001EFA60
		public static int ReadSignedPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x001F1884 File Offset: 0x001EFA84
		public static void WriteSignedPackedBits(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061DC RID: 25052 RVA: 0x001F18A4 File Offset: 0x001EFAA4
		public static int ReadSignedPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061DD RID: 25053 RVA: 0x001F18C8 File Offset: 0x001EFAC8
		public static void WriteSignedPackedBits(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061DE RID: 25054 RVA: 0x001F18E8 File Offset: 0x001EFAE8
		public static int ReadSignedPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061DF RID: 25055 RVA: 0x001F190C File Offset: 0x001EFB0C
		public static void WriteSignedPackedBits64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.WritePackedBits(value2, ref bitposition, bits);
		}

		// Token: 0x060061E0 RID: 25056 RVA: 0x001F192C File Offset: 0x001EFB2C
		public static long ReadSignedPackedBits64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBits(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}
	}
}
