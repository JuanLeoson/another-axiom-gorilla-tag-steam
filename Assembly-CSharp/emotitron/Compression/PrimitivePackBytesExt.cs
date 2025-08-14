using System;

namespace emotitron.Compression
{
	// Token: 0x02000F78 RID: 3960
	public static class PrimitivePackBytesExt
	{
		// Token: 0x0600628F RID: 25231 RVA: 0x001F36CC File Offset: 0x001F18CC
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06006290 RID: 25232 RVA: 0x001F3708 File Offset: 0x001F1908
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x001F3744 File Offset: 0x001F1944
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06006292 RID: 25234 RVA: 0x001F3780 File Offset: 0x001F1980
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x001F37BC File Offset: 0x001F19BC
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x001F37E8 File Offset: 0x001F19E8
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06006295 RID: 25237 RVA: 0x001F3814 File Offset: 0x001F1A14
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x001F3834 File Offset: 0x001F1A34
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
