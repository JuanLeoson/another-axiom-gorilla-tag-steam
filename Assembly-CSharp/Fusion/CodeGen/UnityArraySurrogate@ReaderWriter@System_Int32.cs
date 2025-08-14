using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001032 RID: 4146
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Int32 : UnityArraySurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x060066E7 RID: 26343 RVA: 0x0020A1B8 File Offset: 0x002083B8
		// (set) Token: 0x060066E8 RID: 26344 RVA: 0x0020A1C0 File Offset: 0x002083C0
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

		// Token: 0x060066E9 RID: 26345 RVA: 0x0020A1C9 File Offset: 0x002083C9
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x04007278 RID: 29304
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
