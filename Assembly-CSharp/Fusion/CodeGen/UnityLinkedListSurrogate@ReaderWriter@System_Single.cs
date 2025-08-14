using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200104C RID: 4172
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Single : UnityLinkedListSurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x0600671D RID: 26397 RVA: 0x0020A4C8 File Offset: 0x002086C8
		// (set) Token: 0x0600671E RID: 26398 RVA: 0x0020A4D0 File Offset: 0x002086D0
		[WeaverGenerated]
		public override float[] DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x0600671F RID: 26399 RVA: 0x0020A4D9 File Offset: 0x002086D9
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040074BC RID: 29884
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
