using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020000DA RID: 218
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct SkeletonNetData : INetworkStruct
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600055F RID: 1375 RVA: 0x0001F78D File Offset: 0x0001D98D
	// (set) Token: 0x06000560 RID: 1376 RVA: 0x0001F795 File Offset: 0x0001D995
	public int CurrentState { readonly get; set; }

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000561 RID: 1377 RVA: 0x0001F79E File Offset: 0x0001D99E
	// (set) Token: 0x06000562 RID: 1378 RVA: 0x0001F7B0 File Offset: 0x0001D9B0
	[Networked]
	public unsafe Vector3 Position
	{
		readonly get
		{
			return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position);
		}
		set
		{
			*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position) = value;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000563 RID: 1379 RVA: 0x0001F7C3 File Offset: 0x0001D9C3
	// (set) Token: 0x06000564 RID: 1380 RVA: 0x0001F7D5 File Offset: 0x0001D9D5
	[Networked]
	public unsafe Quaternion Rotation
	{
		readonly get
		{
			return *(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation);
		}
		set
		{
			*(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation) = value;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001F7E8 File Offset: 0x0001D9E8
	// (set) Token: 0x06000566 RID: 1382 RVA: 0x0001F7F0 File Offset: 0x0001D9F0
	public int CurrentNode { readonly get; set; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000567 RID: 1383 RVA: 0x0001F7F9 File Offset: 0x0001D9F9
	// (set) Token: 0x06000568 RID: 1384 RVA: 0x0001F801 File Offset: 0x0001DA01
	public int NextNode { readonly get; set; }

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001F80A File Offset: 0x0001DA0A
	// (set) Token: 0x0600056A RID: 1386 RVA: 0x0001F812 File Offset: 0x0001DA12
	public int AngerPoint { readonly get; set; }

	// Token: 0x0600056B RID: 1387 RVA: 0x0001F81B File Offset: 0x0001DA1B
	public SkeletonNetData(int state, Vector3 pos, Quaternion rot, int cNode, int nNode, int angerPoint)
	{
		this.CurrentState = state;
		this.Position = pos;
		this.Rotation = rot;
		this.CurrentNode = cNode;
		this.NextNode = nNode;
		this.AngerPoint = angerPoint;
	}

	// Token: 0x04000670 RID: 1648
	[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@3 _Position;

	// Token: 0x04000671 RID: 1649
	[FixedBufferProperty(typeof(Quaternion), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(16)]
	private FixedStorage@4 _Rotation;
}
