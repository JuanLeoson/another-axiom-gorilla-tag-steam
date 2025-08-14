using System;

namespace emotitron.Compression
{
	// Token: 0x02000F80 RID: 3968
	public static class ZigZagExt
	{
		// Token: 0x0600631C RID: 25372 RVA: 0x001F468D File Offset: 0x001F288D
		public static ulong ZigZag(this long s)
		{
			return (ulong)(s << 1 ^ s >> 63);
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x001F4697 File Offset: 0x001F2897
		public static long UnZigZag(this ulong u)
		{
			return (long)(u >> 1 ^ -(long)(u & 1UL));
		}

		// Token: 0x0600631E RID: 25374 RVA: 0x001F46A2 File Offset: 0x001F28A2
		public static uint ZigZag(this int s)
		{
			return (uint)(s << 1 ^ s >> 31);
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x001F46AC File Offset: 0x001F28AC
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x001F46B9 File Offset: 0x001F28B9
		public static ushort ZigZag(this short s)
		{
			return (ushort)((int)s << 1 ^ s >> 15);
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x001F46C4 File Offset: 0x001F28C4
		public static short UnZigZag(this ushort u)
		{
			return (short)(u >> 1 ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x001F46D0 File Offset: 0x001F28D0
		public static byte ZigZag(this sbyte s)
		{
			return (byte)((int)s << 1 ^ s >> 7);
		}

		// Token: 0x06006323 RID: 25379 RVA: 0x001F46DA File Offset: 0x001F28DA
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)(u >> 1 ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
