using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001045 RID: 4165
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Byte : UnityLinkedListSurrogate<byte, ReaderWriter@System_Byte>
	{
		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x0600670B RID: 26379 RVA: 0x0020A3C8 File Offset: 0x002085C8
		// (set) Token: 0x0600670C RID: 26380 RVA: 0x0020A3D0 File Offset: 0x002085D0
		[WeaverGenerated]
		public override byte[] DataProperty
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

		// Token: 0x0600670D RID: 26381 RVA: 0x0020A3D9 File Offset: 0x002085D9
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Byte()
		{
		}

		// Token: 0x040074A4 RID: 29860
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}
