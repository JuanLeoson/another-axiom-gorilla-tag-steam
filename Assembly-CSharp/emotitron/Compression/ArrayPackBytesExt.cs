using System;

namespace emotitron.Compression
{
	// Token: 0x02000F70 RID: 3952
	public static class ArrayPackBytesExt
	{
		// Token: 0x060061E1 RID: 25057 RVA: 0x001F194C File Offset: 0x001EFB4C
		public unsafe static void WritePackedBytes(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num << 3);
		}

		// Token: 0x060061E2 RID: 25058 RVA: 0x001F1984 File Offset: 0x001EFB84
		public static void WritePackedBytes(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060061E3 RID: 25059 RVA: 0x001F19BC File Offset: 0x001EFBBC
		public static void WritePackedBytes(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060061E4 RID: 25060 RVA: 0x001F19F4 File Offset: 0x001EFBF4
		public static void WritePackedBytes(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060061E5 RID: 25061 RVA: 0x001F1A2C File Offset: 0x001EFC2C
		public unsafe static ulong ReadPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits2) << 3;
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits3);
		}

		// Token: 0x060061E6 RID: 25062 RVA: 0x001F1A60 File Offset: 0x001EFC60
		public static ulong ReadPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061E7 RID: 25063 RVA: 0x001F1A94 File Offset: 0x001EFC94
		public static ulong ReadPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x001F1AC8 File Offset: 0x001EFCC8
		public static ulong ReadPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060061E9 RID: 25065 RVA: 0x001F1AFC File Offset: 0x001EFCFC
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061EA RID: 25066 RVA: 0x001F1B1C File Offset: 0x001EFD1C
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x001F1B40 File Offset: 0x001EFD40
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061EC RID: 25068 RVA: 0x001F1B60 File Offset: 0x001EFD60
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x001F1B84 File Offset: 0x001EFD84
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061EE RID: 25070 RVA: 0x001F1BA4 File Offset: 0x001EFDA4
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061EF RID: 25071 RVA: 0x001F1BC8 File Offset: 0x001EFDC8
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x001F1BE8 File Offset: 0x001EFDE8
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060061F1 RID: 25073 RVA: 0x001F1C0C File Offset: 0x001EFE0C
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.WritePackedBytes(value2, ref bitposition, bits);
		}

		// Token: 0x060061F2 RID: 25074 RVA: 0x001F1C2C File Offset: 0x001EFE2C
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}
	}
}
