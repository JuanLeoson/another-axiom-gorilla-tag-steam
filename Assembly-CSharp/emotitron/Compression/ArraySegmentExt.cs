using System;

namespace emotitron.Compression
{
	// Token: 0x02000F71 RID: 3953
	public static class ArraySegmentExt
	{
		// Token: 0x060061F3 RID: 25075 RVA: 0x001F1C4B File Offset: 0x001EFE4B
		public static ArraySegment<byte> ExtractArraySegment(byte[] buffer, ref int bitposition)
		{
			return new ArraySegment<byte>(buffer, 0, bitposition + 7 >> 3);
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x001F1C5A File Offset: 0x001EFE5A
		public static ArraySegment<ushort> ExtractArraySegment(ushort[] buffer, ref int bitposition)
		{
			return new ArraySegment<ushort>(buffer, 0, bitposition + 15 >> 4);
		}

		// Token: 0x060061F5 RID: 25077 RVA: 0x001F1C6A File Offset: 0x001EFE6A
		public static ArraySegment<uint> ExtractArraySegment(uint[] buffer, ref int bitposition)
		{
			return new ArraySegment<uint>(buffer, 0, bitposition + 31 >> 5);
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x001F1C7A File Offset: 0x001EFE7A
		public static ArraySegment<ulong> ExtractArraySegment(ulong[] buffer, ref int bitposition)
		{
			return new ArraySegment<ulong>(buffer, 0, bitposition + 63 >> 6);
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x001F1C8C File Offset: 0x001EFE8C
		public static void Append(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x001F1CC0 File Offset: 0x001EFEC0
		public static void Append(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061F9 RID: 25081 RVA: 0x001F1CF4 File Offset: 0x001EFEF4
		public static void Append(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x001F1D28 File Offset: 0x001EFF28
		public static void Write(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061FB RID: 25083 RVA: 0x001F1D5C File Offset: 0x001EFF5C
		public static void Write(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061FC RID: 25084 RVA: 0x001F1D90 File Offset: 0x001EFF90
		public static void Write(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x060061FD RID: 25085 RVA: 0x001F1DC4 File Offset: 0x001EFFC4
		public static ulong Read(this ArraySegment<byte> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x060061FE RID: 25086 RVA: 0x001F1DF8 File Offset: 0x001EFFF8
		public static ulong Read(this ArraySegment<uint> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x060061FF RID: 25087 RVA: 0x001F1E2C File Offset: 0x001F002C
		public static ulong Read(this ArraySegment<ulong> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x001F1E60 File Offset: 0x001F0060
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006201 RID: 25089 RVA: 0x001F1E90 File Offset: 0x001F0090
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006202 RID: 25090 RVA: 0x001F1EC0 File Offset: 0x001F00C0
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006203 RID: 25091 RVA: 0x001F1EF0 File Offset: 0x001F00F0
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}
	}
}
