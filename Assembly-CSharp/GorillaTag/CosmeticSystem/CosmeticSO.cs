using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED8 RID: 3800
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x06005E7B RID: 24187 RVA: 0x001DC503 File Offset: 0x001DA703
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x04006882 RID: 26754
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");

		// Token: 0x04006883 RID: 26755
		public int propHuntWeight = 1;
	}
}
