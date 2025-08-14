using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001041 RID: 4161
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32 : UnityDictionarySurrogate<int, ReaderWriter@System_Int32, int, ReaderWriter@System_Int32>
	{
		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06006702 RID: 26370 RVA: 0x0020A349 File Offset: 0x00208549
		// (set) Token: 0x06006703 RID: 26371 RVA: 0x0020A351 File Offset: 0x00208551
		[WeaverGenerated]
		public override SerializableDictionary<int, int> DataProperty
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

		// Token: 0x06006704 RID: 26372 RVA: 0x0020A35A File Offset: 0x0020855A
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x0400749B RID: 29851
		[WeaverGenerated]
		public SerializableDictionary<int, int> Data = SerializableDictionary.Create<int, int>();
	}
}
