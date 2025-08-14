using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E6E RID: 3694
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06005C68 RID: 23656 RVA: 0x001D12E2 File Offset: 0x001CF4E2
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
