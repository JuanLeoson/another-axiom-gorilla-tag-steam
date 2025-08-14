using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001019 RID: 4121
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool : UnityValueSurrogate<NetworkBool, ReaderWriter@Fusion_NetworkBool>
	{
		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x060066A8 RID: 26280 RVA: 0x00209E30 File Offset: 0x00208030
		// (set) Token: 0x060066A9 RID: 26281 RVA: 0x00209E38 File Offset: 0x00208038
		[WeaverGenerated]
		public override NetworkBool DataProperty
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

		// Token: 0x060066AA RID: 26282 RVA: 0x00209E41 File Offset: 0x00208041
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x04007163 RID: 29027
		[WeaverGenerated]
		public NetworkBool Data;
	}
}
