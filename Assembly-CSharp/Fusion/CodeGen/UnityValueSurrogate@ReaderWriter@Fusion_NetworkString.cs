using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200101D RID: 4125
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x060066B1 RID: 26289 RVA: 0x00209EC4 File Offset: 0x002080C4
		// (set) Token: 0x060066B2 RID: 26290 RVA: 0x00209ECC File Offset: 0x002080CC
		[WeaverGenerated]
		public override NetworkString<_32> DataProperty
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

		// Token: 0x060066B3 RID: 26291 RVA: 0x00209ED5 File Offset: 0x002080D5
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04007187 RID: 29063
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}
