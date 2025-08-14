using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED5 RID: 3797
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x0400687B RID: 26747
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x0400687C RID: 26748
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x0400687D RID: 26749
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}
