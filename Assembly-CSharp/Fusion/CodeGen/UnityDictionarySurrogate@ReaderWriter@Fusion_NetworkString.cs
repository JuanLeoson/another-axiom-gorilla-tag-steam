using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200102F RID: 4143
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString : UnityDictionarySurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString, NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x060066DB RID: 26331 RVA: 0x0020A109 File Offset: 0x00208309
		// (set) Token: 0x060066DC RID: 26332 RVA: 0x0020A111 File Offset: 0x00208311
		[WeaverGenerated]
		public override SerializableDictionary<NetworkString<_32>, NetworkString<_32>> DataProperty
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

		// Token: 0x060066DD RID: 26333 RVA: 0x0020A11A File Offset: 0x0020831A
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04007275 RID: 29301
		[WeaverGenerated]
		public SerializableDictionary<NetworkString<_32>, NetworkString<_32>> Data = SerializableDictionary.Create<NetworkString<_32>, NetworkString<_32>>();
	}
}
