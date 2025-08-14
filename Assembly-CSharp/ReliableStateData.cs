using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000415 RID: 1045
[NetworkStructWeaved(21)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct ReliableStateData : INetworkStruct
{
	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001955 RID: 6485 RVA: 0x00088898 File Offset: 0x00086A98
	// (set) Token: 0x06001956 RID: 6486 RVA: 0x000888A0 File Offset: 0x00086AA0
	public long Header { readonly get; set; }

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001957 RID: 6487 RVA: 0x000888AC File Offset: 0x00086AAC
	[Networked]
	[Capacity(5)]
	public NetworkArray<long> TransferrableStates
	{
		get
		{
			return new NetworkArray<long>(Native.ReferenceToPointer<FixedStorage@10>(ref this._TransferrableStates), 5, ReaderWriter@System_Int64.GetInstance());
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001958 RID: 6488 RVA: 0x000888CF File Offset: 0x00086ACF
	// (set) Token: 0x06001959 RID: 6489 RVA: 0x000888D7 File Offset: 0x00086AD7
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x0600195A RID: 6490 RVA: 0x000888E0 File Offset: 0x00086AE0
	// (set) Token: 0x0600195B RID: 6491 RVA: 0x000888E8 File Offset: 0x00086AE8
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x0600195C RID: 6492 RVA: 0x000888F1 File Offset: 0x00086AF1
	// (set) Token: 0x0600195D RID: 6493 RVA: 0x000888F9 File Offset: 0x00086AF9
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x0600195E RID: 6494 RVA: 0x00088902 File Offset: 0x00086B02
	// (set) Token: 0x0600195F RID: 6495 RVA: 0x0008890A File Offset: 0x00086B0A
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001960 RID: 6496 RVA: 0x00088913 File Offset: 0x00086B13
	// (set) Token: 0x06001961 RID: 6497 RVA: 0x0008891B File Offset: 0x00086B1B
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001962 RID: 6498 RVA: 0x00088924 File Offset: 0x00086B24
	// (set) Token: 0x06001963 RID: 6499 RVA: 0x0008892C File Offset: 0x00086B2C
	public long PackedBeads { readonly get; set; }

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001964 RID: 6500 RVA: 0x00088935 File Offset: 0x00086B35
	// (set) Token: 0x06001965 RID: 6501 RVA: 0x0008893D File Offset: 0x00086B3D
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x040021B4 RID: 8628
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ReaderWriter@System_Int64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
