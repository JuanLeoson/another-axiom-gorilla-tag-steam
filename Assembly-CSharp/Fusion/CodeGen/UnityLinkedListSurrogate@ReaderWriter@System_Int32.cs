using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001046 RID: 4166
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Int32 : UnityLinkedListSurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x0600670E RID: 26382 RVA: 0x0020A3EC File Offset: 0x002085EC
		// (set) Token: 0x0600670F RID: 26383 RVA: 0x0020A3F4 File Offset: 0x002085F4
		[WeaverGenerated]
		public override int[] DataProperty
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

		// Token: 0x06006710 RID: 26384 RVA: 0x0020A3FD File Offset: 0x002085FD
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040074A5 RID: 29861
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
