using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001037 RID: 4151
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3 : UnityLinkedListSurrogate<Vector3, ReaderWriter@UnityEngine_Vector3>
	{
		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x060066F3 RID: 26355 RVA: 0x0020A26C File Offset: 0x0020846C
		// (set) Token: 0x060066F4 RID: 26356 RVA: 0x0020A274 File Offset: 0x00208474
		[WeaverGenerated]
		public override Vector3[] DataProperty
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

		// Token: 0x060066F5 RID: 26357 RVA: 0x0020A27D File Offset: 0x0020847D
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x04007315 RID: 29461
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}
