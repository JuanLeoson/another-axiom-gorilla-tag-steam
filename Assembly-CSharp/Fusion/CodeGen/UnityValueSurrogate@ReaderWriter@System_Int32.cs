using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200102C RID: 4140
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@System_Int32 : UnityValueSurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x060066D8 RID: 26328 RVA: 0x0020A0F0 File Offset: 0x002082F0
		// (set) Token: 0x060066D9 RID: 26329 RVA: 0x0020A0F8 File Offset: 0x002082F8
		[WeaverGenerated]
		public override int DataProperty
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

		// Token: 0x060066DA RID: 26330 RVA: 0x0020A101 File Offset: 0x00208301
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040071A4 RID: 29092
		[WeaverGenerated]
		public int Data;
	}
}
