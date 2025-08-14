using System;
using System.Runtime.InteropServices;

// Token: 0x020002D3 RID: 723
public struct RPCArgBuffer<T> where T : struct
{
	// Token: 0x060010EB RID: 4331 RVA: 0x00061AAE File Offset: 0x0005FCAE
	public RPCArgBuffer(T argStruct)
	{
		this.DataLength = Marshal.SizeOf(typeof(T));
		this.Data = new byte[this.DataLength];
		this.Args = argStruct;
	}

	// Token: 0x04001927 RID: 6439
	public T Args;

	// Token: 0x04001928 RID: 6440
	public byte[] Data;

	// Token: 0x04001929 RID: 6441
	public int DataLength;
}
