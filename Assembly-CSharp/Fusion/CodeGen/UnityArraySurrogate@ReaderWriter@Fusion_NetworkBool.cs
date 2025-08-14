using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001026 RID: 4134
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool : UnityArraySurrogate<NetworkBool, ReaderWriter@Fusion_NetworkBool>
	{
		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x060066C6 RID: 26310 RVA: 0x00209FE1 File Offset: 0x002081E1
		// (set) Token: 0x060066C7 RID: 26311 RVA: 0x00209FE9 File Offset: 0x002081E9
		[WeaverGenerated]
		public override NetworkBool[] DataProperty
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

		// Token: 0x060066C8 RID: 26312 RVA: 0x00209FF2 File Offset: 0x002081F2
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x0400719B RID: 29083
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}
