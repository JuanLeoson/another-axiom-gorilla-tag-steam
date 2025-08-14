using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020004CB RID: 1227
[NetworkStructWeaved(12)]
[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct TagData : INetworkStruct
{
	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001E2D RID: 7725 RVA: 0x000A0C90 File Offset: 0x0009EE90
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._infectedPlayerList), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001E2E RID: 7726 RVA: 0x000A0CB7 File Offset: 0x0009EEB7
	// (set) Token: 0x06001E2F RID: 7727 RVA: 0x000A0CBF File Offset: 0x0009EEBF
	public int currentItID { readonly get; set; }

	// Token: 0x040026B8 RID: 9912
	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	// Token: 0x040026B9 RID: 9913
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@10 _infectedPlayerList;
}
