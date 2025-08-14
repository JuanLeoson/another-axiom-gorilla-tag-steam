using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
[NetworkStructWeaved(31)]
[StructLayout(LayoutKind.Explicit, Size = 124)]
public struct PaintbrawlData : INetworkStruct
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001DFE RID: 7678 RVA: 0x000A0734 File Offset: 0x0009E934
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerLivesArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001DFF RID: 7679 RVA: 0x000A075C File Offset: 0x0009E95C
	[Networked]
	[Capacity(10)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerActorNumberArray), 10, ReaderWriter@System_Int32.GetInstance());
		}
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001E00 RID: 7680 RVA: 0x000A0784 File Offset: 0x0009E984
	[Networked]
	[Capacity(10)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerStatusArray), 10, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	// Token: 0x040026A7 RID: 9895
	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	// Token: 0x040026A8 RID: 9896
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _playerLivesArray;

	// Token: 0x040026A9 RID: 9897
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _playerActorNumberArray;

	// Token: 0x040026AA RID: 9898
	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@10 _playerStatusArray;
}
