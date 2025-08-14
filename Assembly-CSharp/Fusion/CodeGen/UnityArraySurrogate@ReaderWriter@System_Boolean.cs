using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001048 RID: 4168
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Boolean : UnityArraySurrogate<bool, ReaderWriter@System_Boolean>
	{
		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06006717 RID: 26391 RVA: 0x0020A480 File Offset: 0x00208680
		// (set) Token: 0x06006718 RID: 26392 RVA: 0x0020A488 File Offset: 0x00208688
		[WeaverGenerated]
		public override bool[] DataProperty
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

		// Token: 0x06006719 RID: 26393 RVA: 0x0020A491 File Offset: 0x00208691
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Boolean()
		{
		}

		// Token: 0x040074A7 RID: 29863
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}
