using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02000F86 RID: 3974
	public class BasicWriter
	{
		// Token: 0x06006376 RID: 25462 RVA: 0x001F5982 File Offset: 0x001F3B82
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x001F598A File Offset: 0x001F3B8A
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x001F59A1 File Offset: 0x001F3BA1
		public static byte BasicRead(byte[] buffer)
		{
			byte result = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return result;
		}

		// Token: 0x04006E77 RID: 28279
		public static int pos;
	}
}
