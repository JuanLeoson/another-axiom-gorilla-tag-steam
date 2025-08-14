using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class GRToolUnlock : ScriptableObject
{
	// Token: 0x040034D3 RID: 13523
	public string toolName;

	// Token: 0x040034D4 RID: 13524
	public string toolId;

	// Token: 0x040034D5 RID: 13525
	public int unlockLevel;

	// Token: 0x040034D6 RID: 13526
	public int unlockCost;

	// Token: 0x040034D7 RID: 13527
	public GRToolUpgrade[] toolUpgrades;
}
