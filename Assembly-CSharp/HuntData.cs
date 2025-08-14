using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
[NetworkStructWeaved(23)]
[StructLayout(LayoutKind.Explicit, Size = 92)]
public struct HuntData : INetworkStruct
{
	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001E24 RID: 7716 RVA: 0x000A0BA4 File Offset: 0x0009EDA4
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentHuntedArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001E25 RID: 7717 RVA: 0x000A0BCC File Offset: 0x0009EDCC
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentTargetArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x040026B1 RID: 9905
	[FieldOffset(0)]
	public NetworkBool huntStarted;

	// Token: 0x040026B2 RID: 9906
	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	// Token: 0x040026B3 RID: 9907
	[FieldOffset(8)]
	public int countDownTime;

	// Token: 0x040026B4 RID: 9908
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@10 _currentHuntedArray;

	// Token: 0x040026B5 RID: 9909
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(52)]
	private FixedStorage@10 _currentTargetArray;
}
