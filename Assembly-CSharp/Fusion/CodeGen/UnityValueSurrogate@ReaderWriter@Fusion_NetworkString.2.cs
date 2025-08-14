using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200103E RID: 4158
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_128>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x060066FF RID: 26367 RVA: 0x0020A330 File Offset: 0x00208530
		// (set) Token: 0x06006700 RID: 26368 RVA: 0x0020A338 File Offset: 0x00208538
		[WeaverGenerated]
		public override NetworkString<_128> DataProperty
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

		// Token: 0x06006701 RID: 26369 RVA: 0x0020A341 File Offset: 0x00208541
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04007452 RID: 29778
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}
