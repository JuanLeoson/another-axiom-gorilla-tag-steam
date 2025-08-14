using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ECE RID: 3790
	public class AllCosmeticsArraySO : ScriptableObject
	{
		// Token: 0x06005E6E RID: 24174 RVA: 0x001DC248 File Offset: 0x001DA448
		public CosmeticSO SearchForCosmeticSO(string playfabId)
		{
			GTDirectAssetRef<CosmeticSO>[] array = this.sturdyAssetRefs;
			for (int i = 0; i < array.Length; i++)
			{
				CosmeticSO cosmeticSO = array[i];
				if (cosmeticSO.info.playFabID == playfabId)
				{
					return cosmeticSO;
				}
			}
			Debug.LogWarning("AllCosmeticsArraySO - SearchForCosmeticSO - No Cosmetic found with playfabId: " + playfabId, this);
			return null;
		}

		// Token: 0x0400684A RID: 26698
		[SerializeField]
		public GTDirectAssetRef<CosmeticSO>[] sturdyAssetRefs;
	}
}
