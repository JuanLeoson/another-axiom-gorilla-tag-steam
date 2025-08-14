using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001023 RID: 4131
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@System_Single : UnityValueSurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x060066C3 RID: 26307 RVA: 0x00209FC8 File Offset: 0x002081C8
		// (set) Token: 0x060066C4 RID: 26308 RVA: 0x00209FD0 File Offset: 0x002081D0
		[WeaverGenerated]
		public override float DataProperty
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

		// Token: 0x060066C5 RID: 26309 RVA: 0x00209FD9 File Offset: 0x002081D9
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x0400718F RID: 29071
		[WeaverGenerated]
		public float Data;
	}
}
