using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001021 RID: 4129
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3 : UnityValueSurrogate<Vector3, ReaderWriter@UnityEngine_Vector3>
	{
		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x060066BA RID: 26298 RVA: 0x00209F54 File Offset: 0x00208154
		// (set) Token: 0x060066BB RID: 26299 RVA: 0x00209F5C File Offset: 0x0020815C
		[WeaverGenerated]
		public override Vector3 DataProperty
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

		// Token: 0x060066BC RID: 26300 RVA: 0x00209F65 File Offset: 0x00208165
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x0400718D RID: 29069
		[WeaverGenerated]
		public Vector3 Data;
	}
}
