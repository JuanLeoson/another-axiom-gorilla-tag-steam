using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C2F RID: 3119
	[NetworkStructWeaved(13)]
	[StructLayout(LayoutKind.Explicit, Size = 52)]
	public struct FlowersDataStruct : INetworkStruct
	{
		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06004CC7 RID: 19655 RVA: 0x0017CBFB File Offset: 0x0017ADFB
		// (set) Token: 0x06004CC8 RID: 19656 RVA: 0x0017CC03 File Offset: 0x0017AE03
		public int FlowerCount { readonly get; set; }

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06004CC9 RID: 19657 RVA: 0x0017CC0C File Offset: 0x0017AE0C
		[Networked]
		public NetworkLinkedList<byte> FlowerWateredData
		{
			get
			{
				return new NetworkLinkedList<byte>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerWateredData), 1, ReaderWriter@System_Byte.GetInstance());
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06004CCA RID: 19658 RVA: 0x0017CC30 File Offset: 0x0017AE30
		[Networked]
		public NetworkLinkedList<int> FlowerStateData
		{
			get
			{
				return new NetworkLinkedList<int>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerStateData), 1, ReaderWriter@System_Int32.GetInstance());
			}
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x0017CC54 File Offset: 0x0017AE54
		public FlowersDataStruct(List<Flower> allFlowers)
		{
			this.FlowerCount = allFlowers.Count;
			foreach (Flower flower in allFlowers)
			{
				this.FlowerWateredData.Add(flower.IsWatered ? 1 : 0);
				this.FlowerStateData.Add((int)flower.GetCurrentState());
			}
		}

		// Token: 0x040055DC RID: 21980
		[FixedBufferProperty(typeof(NetworkLinkedList<byte>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Byte), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@6 _FlowerWateredData;

		// Token: 0x040055DD RID: 21981
		[FixedBufferProperty(typeof(NetworkLinkedList<int>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Int32), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(28)]
		private FixedStorage@6 _FlowerStateData;
	}
}
