using System;
using UnityEngine;

// Token: 0x02000689 RID: 1673
public class GRToolUpgrade : ScriptableObject
{
	// Token: 0x040034D8 RID: 13528
	public string upgradeName;

	// Token: 0x040034D9 RID: 13529
	public string description;

	// Token: 0x040034DA RID: 13530
	public string upgradeId;

	// Token: 0x040034DB RID: 13531
	[SerializeField]
	public GRToolUpgrade.ToolUpgradeLevel[] upgradeLevels;

	// Token: 0x0200068A RID: 1674
	[Serializable]
	public struct ToolUpgradeLevel
	{
		// Token: 0x040034DC RID: 13532
		[SerializeField]
		public int Cost;

		// Token: 0x040034DD RID: 13533
		[SerializeField]
		public float upgradeAmount;
	}
}
