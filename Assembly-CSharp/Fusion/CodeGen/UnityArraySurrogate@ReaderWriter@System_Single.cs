using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001049 RID: 4169
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Single : UnityArraySurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x0600671A RID: 26394 RVA: 0x0020A4A4 File Offset: 0x002086A4
		// (set) Token: 0x0600671B RID: 26395 RVA: 0x0020A4AC File Offset: 0x002086AC
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x0600671C RID: 26396 RVA: 0x0020A4B5 File Offset: 0x002086B5
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040074A8 RID: 29864
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
