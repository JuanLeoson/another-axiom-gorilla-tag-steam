using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000EE1 RID: 3809
	[CreateAssetMenu(fileName = "UntitledSeason_SeasonSO", menuName = "- Gorilla Tag/SeasonSO", order = 0)]
	public class SeasonSO : ScriptableObject
	{
		// Token: 0x040068F0 RID: 26864
		[Delayed]
		public GTDateTimeSerializable releaseDate = new GTDateTimeSerializable(1);

		// Token: 0x040068F1 RID: 26865
		[Delayed]
		public string seasonName;
	}
}
