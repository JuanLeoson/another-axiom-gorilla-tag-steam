using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001031 RID: 4145
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Int64 : UnityArraySurrogate<long, ReaderWriter@System_Int64>
	{
		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x060066E4 RID: 26340 RVA: 0x0020A194 File Offset: 0x00208394
		// (set) Token: 0x060066E5 RID: 26341 RVA: 0x0020A19C File Offset: 0x0020839C
		[WeaverGenerated]
		public override long[] DataProperty
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

		// Token: 0x060066E6 RID: 26342 RVA: 0x0020A1A5 File Offset: 0x002083A5
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Int64()
		{
		}

		// Token: 0x04007277 RID: 29303
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}
