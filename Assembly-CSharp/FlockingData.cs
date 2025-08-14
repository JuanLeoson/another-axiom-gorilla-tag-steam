using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000590 RID: 1424
[NetworkStructWeaved(337)]
[StructLayout(LayoutKind.Explicit, Size = 1348)]
public struct FlockingData : INetworkStruct
{
	// Token: 0x17000377 RID: 887
	// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000BBED0 File Offset: 0x000BA0D0
	// (set) Token: 0x060022C4 RID: 8900 RVA: 0x000BBED8 File Offset: 0x000BA0D8
	public int count { readonly get; set; }

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000BBEE4 File Offset: 0x000BA0E4
	[Networked]
	[Capacity(30)]
	public NetworkLinkedList<Vector3> Positions
	{
		get
		{
			return new NetworkLinkedList<Vector3>(Native.ReferenceToPointer<FixedStorage@153>(ref this._Positions), 30, ReaderWriter@UnityEngine_Vector3.GetInstance());
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x060022C6 RID: 8902 RVA: 0x000BBF0C File Offset: 0x000BA10C
	[Networked]
	[Capacity(30)]
	public NetworkLinkedList<Quaternion> Rotations
	{
		get
		{
			return new NetworkLinkedList<Quaternion>(Native.ReferenceToPointer<FixedStorage@183>(ref this._Rotations), 30, ReaderWriter@UnityEngine_Quaternion.GetInstance());
		}
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000BBF34 File Offset: 0x000BA134
	public FlockingData(List<Flocking> items)
	{
		this.count = items.Count;
		foreach (Flocking flocking in items)
		{
			this.Positions.Add(flocking.pos);
			this.Rotations.Add(flocking.rot);
		}
	}

	// Token: 0x04002C69 RID: 11369
	[FixedBufferProperty(typeof(NetworkLinkedList<Vector3>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@153 _Positions;

	// Token: 0x04002C6A RID: 11370
	[FixedBufferProperty(typeof(NetworkLinkedList<Quaternion>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(616)]
	private FixedStorage@183 _Rotations;
}
