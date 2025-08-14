using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x020002D6 RID: 726
[NetworkStructWeaved(1)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct HitTargetStruct : INetworkStruct
{
	// Token: 0x060010F3 RID: 4339 RVA: 0x00061E34 File Offset: 0x00060034
	public HitTargetStruct(int v)
	{
		this.Score = v;
	}

	// Token: 0x04001947 RID: 6471
	[FieldOffset(0)]
	public int Score;
}
