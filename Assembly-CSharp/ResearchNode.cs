using System;
using System.Collections.Generic;

// Token: 0x02000663 RID: 1635
[Serializable]
public class ResearchNode
{
	// Token: 0x06002819 RID: 10265 RVA: 0x000D86A0 File Offset: 0x000D68A0
	public ResearchNode(bool unlocked, int requiredEmployeeLevel, int researchCost, int cost, string name, string id, string description, string[] requiredNodeIDs, string rootUpgrade = "", bool defaultUnlocked = false)
	{
		this.unlocked = unlocked;
		this.requiredEmployeeLevel = requiredEmployeeLevel;
		this.cost = cost;
		this.researchCost = researchCost;
		this.name = name;
		this.description = description;
		this.id = id;
		this.requiredNodeIDs = new List<string>(requiredNodeIDs);
		this.rootUpgrade = rootUpgrade;
		this.defaultUnlocked = defaultUnlocked;
	}

	// Token: 0x04003382 RID: 13186
	[NonSerialized]
	public bool unlocked;

	// Token: 0x04003383 RID: 13187
	[NonSerialized]
	public ResearchNode parentNode;

	// Token: 0x04003384 RID: 13188
	[NonSerialized]
	public List<ResearchNode> modNodes = new List<ResearchNode>();

	// Token: 0x04003385 RID: 13189
	[NonSerialized]
	public List<ResearchNode> requiredNodes = new List<ResearchNode>();

	// Token: 0x04003386 RID: 13190
	public int requiredEmployeeLevel;

	// Token: 0x04003387 RID: 13191
	public int researchCost;

	// Token: 0x04003388 RID: 13192
	public int cost;

	// Token: 0x04003389 RID: 13193
	public string name;

	// Token: 0x0400338A RID: 13194
	public string description;

	// Token: 0x0400338B RID: 13195
	public string id;

	// Token: 0x0400338C RID: 13196
	public string rootUpgrade = "";

	// Token: 0x0400338D RID: 13197
	public bool defaultUnlocked;

	// Token: 0x0400338E RID: 13198
	public List<string> requiredNodeIDs;
}
