using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED7 RID: 3799
	[Serializable]
	public struct CosmeticPlacementInfo
	{
		// Token: 0x04006880 RID: 26752
		[Tooltip("The bone to attach the cosmetic to.")]
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x04006881 RID: 26753
		public XformOffset offset;
	}
}
