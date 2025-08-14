using System;

namespace emotitron.Compression
{
	// Token: 0x02000F7B RID: 3963
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x06006304 RID: 25348
		public abstract ulong Encode(T val);

		// Token: 0x06006305 RID: 25349
		public abstract T Decode(uint val);

		// Token: 0x06006306 RID: 25350
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x06006307 RID: 25351
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x06006308 RID: 25352
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
