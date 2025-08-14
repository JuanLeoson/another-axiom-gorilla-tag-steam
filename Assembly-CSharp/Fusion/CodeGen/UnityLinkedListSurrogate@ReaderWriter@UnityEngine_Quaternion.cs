using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x0200103A RID: 4154
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityLinkedListSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x060066F6 RID: 26358 RVA: 0x0020A290 File Offset: 0x00208490
		// (set) Token: 0x060066F7 RID: 26359 RVA: 0x0020A298 File Offset: 0x00208498
		[WeaverGenerated]
		public override Quaternion[] DataProperty
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

		// Token: 0x060066F8 RID: 26360 RVA: 0x0020A2A1 File Offset: 0x002084A1
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040073CE RID: 29646
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}
