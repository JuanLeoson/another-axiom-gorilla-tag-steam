using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020000D3 RID: 211
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct GhostLabData : INetworkStruct
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000521 RID: 1313 RVA: 0x0001D945 File Offset: 0x0001BB45
	// (set) Token: 0x06000522 RID: 1314 RVA: 0x0001D94D File Offset: 0x0001BB4D
	public int DoorState { readonly get; set; }

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000523 RID: 1315 RVA: 0x0001D958 File Offset: 0x0001BB58
	[Networked]
	[Capacity(10)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._OpenDoors), 10, ReaderWriter@Fusion_NetworkBool.GetInstance());
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001D980 File Offset: 0x0001BB80
	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool val = openDoors[i];
			this.OpenDoors.Set(i, val);
		}
	}

	// Token: 0x04000621 RID: 1569
	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _OpenDoors;
}
