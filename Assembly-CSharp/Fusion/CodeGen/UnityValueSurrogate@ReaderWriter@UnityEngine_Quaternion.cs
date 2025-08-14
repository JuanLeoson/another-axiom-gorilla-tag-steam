using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x0200102A RID: 4138
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityValueSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x060066CF RID: 26319 RVA: 0x0020A07C File Offset: 0x0020827C
		// (set) Token: 0x060066D0 RID: 26320 RVA: 0x0020A084 File Offset: 0x00208284
		[WeaverGenerated]
		public override Quaternion DataProperty
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

		// Token: 0x060066D1 RID: 26321 RVA: 0x0020A08D File Offset: 0x0020828D
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040071A2 RID: 29090
		[WeaverGenerated]
		public Quaternion Data;
	}
}
