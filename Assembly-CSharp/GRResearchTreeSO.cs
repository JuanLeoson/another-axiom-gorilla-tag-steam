using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000662 RID: 1634
[CreateAssetMenu(fileName = "ResearchTree", menuName = "GhostReactor", order = 0)]
public class GRResearchTreeSO : ScriptableObject
{
	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x0600280B RID: 10251 RVA: 0x000D7904 File Offset: 0x000D5B04
	public List<ResearchNode> RootNodes
	{
		get
		{
			if (this.rootNodes.Count == 0)
			{
				Debug.Log("RootNodes - Finding Root Nodes");
				this.FindRootNodes();
				Debug.Log(string.Format("RootNodes - Found {0}", this.rootNodes.Count));
			}
			return this.rootNodes;
		}
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000D7954 File Offset: 0x000D5B54
	public void CreatePlaceHoldData()
	{
		this.tree = new List<ResearchNode>
		{
			new ResearchNode(true, 0, 0, 0, "Flash Tool", "RESEARCH_TOOL_FLASH", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Flash Regulator", "RESEARCH_TOOL_FLASH_FLASH_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 1, 30, "Flash Optimizer", "RESEARCH_TOOL_FLASH_FLASH_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH_FLASH_REGULATOR"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 1, 30, "Flash Recycler", "RESEARCH_TOOL_FLASH_FLASH_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH_FLASH_OPTIMIZER"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 1, 20, "Spectral Lens", "RESEARCH_TOOL_FLASH_SPECTRAL_LENS", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 1, 20, "Parabolic Focuser", "RESEARCH_TOOL_FLASH_PARABOLIC_FOCUSER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH_SPECTRAL_LENS"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 1, 20, "Beta Wave Amplifier", "RESEARCH_TOOL_FLASH_BETA_WAVE_AMPLIFIER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_FLASH_PARABOLIC_FOCUSER"
			}, "RESEARCH_TOOL_FLASH", false),
			new ResearchNode(true, 0, 0, 0, "Charge Baton", "RESEARCH_TOOL_CHARGEBATON", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Baton Regulator", "RESEARCH_TOOL_CHARGEBATON_BATON_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 1, 30, "Baton Optimizer", "RESEARCH_TOOL_CHARGEBATON_BATON_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON_BATON_REGULATOR"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 1, 30, "Baton Recycler", "RESEARCH_TOOL_CHARGEBATON_BATON_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON_BATON_OPTIMIZER"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 1, 30, "Lead Core", "RESEARCH_TOOL_CHARGEBATON_LEAD_CORE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 1, 50, "Osmium Core", "RESEARCH_TOOL_CHARGEBATON_OSMIUM_CORE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON_LEAD_CORE"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 1, 40, "Electrified Spikes", "RESEARCH_TOOL_CHARGEBATON_ELECTRIFIED_SPIKES", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_CHARGEBATON_OSMIUM_CORE"
			}, "RESEARCH_TOOL_CHARGEBATON", false),
			new ResearchNode(true, 0, 0, 0, "Lantern", "RESEARCH_TOOL_LANTERN", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Lantern Regulator", "RESEARCH_TOOL_LANTERN_LANTERN_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 1, 30, "Lantern Optimizer", "RESEARCH_TOOL_LANTERN_LANTERN_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN_LANTERN_REGULATOR"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 1, 30, "Lantern Recycler", "RESEARCH_TOOL_LANTERN_LANTERN_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN_LANTERN_OPTIMIZER"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 1, 30, "Carbon Arc", "RESEARCH_TOOL_LANTERN_CARBON_ARC", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 1, 40, "Tungsten Filament", "RESEARCH_TOOL_LANTERN_TUNGSTEN_FILAMENT", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN_CARBON_ARC"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 1, 50, "Halogen Quartz", "RESEARCH_TOOL_LANTERN_HALOGEN_QUARTZ", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_LANTERN_TUNGSTEN_FILAMENT"
			}, "RESEARCH_TOOL_LANTERN", false),
			new ResearchNode(true, 0, 0, 0, "Collector", "RESEARCH_TOOL_COLLECTOR", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Collector Regulator", "RESEARCH_TOOL_COLLECTOR_COLLECTOR_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 1, 30, "Collector Optimizer", "RESEARCH_TOOL_COLLECTOR_COLLECTOR_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR_COLLECTOR_REGULATOR"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 1, 30, "Collector Recycler", "RESEARCH_TOOL_COLLECTOR_COLLECTOR_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR_COLLECTOR_OPTIMIZER"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 1, 40, "Vortex Intake", "RESEARCH_TOOL_COLLECTOR_VORTEX_INTAKE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 1, 50, "Cyclone Intake", "RESEARCH_TOOL_COLLECTOR_CYCLONE_INTAKE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR_VORTEX_INTAKE"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 1, 70, "Hurricane Intake", "RESEARCH_TOOL_COLLECTOR_HURRICANE_INTAKE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_COLLECTOR_CYCLONE_INTAKE"
			}, "RESEARCH_TOOL_COLLECTOR", false),
			new ResearchNode(true, 0, 0, 0, "Shield Gun", "RESEARCH_TOOL_SHIELDGUN", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Shield Gun Regulator", "RESEARCH_TOOL_SHIELDGUN_SHIELDGUN_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 1, 30, "Shield Gun Optimizer", "RESEARCH_TOOL_SHIELDGUN_SHIELDGUN_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN_SHIELDGUN_REGULATOR"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 1, 30, "Shield Gun Recycler", "RESEARCH_TOOL_SHIELDGUN_FLASH_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN_SHIELDGUN_OPTIMIZER"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 1, 60, "Megaflow Nozzle", "RESEARCH_TOOL_SHIELDGUN_MEGAFLOW_NOZZLE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 1, 70, "Ultraflow Nozzle", "RESEARCH_TOOL_SHIELDGUN_ULTRAFLOW_NOZZLE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN_ULTRAFLOW_NOZZLE"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 1, 80, "TitanJet Nozzle", "RESEARCH_TOOL_SHIELDGUN_TITANJET_NOZZLE", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_SHIELDGUN_ULTRAFLOW_NOZZLE"
			}, "RESEARCH_TOOL_SHIELDGUN", false),
			new ResearchNode(true, 0, 0, 0, "Revive", "RESEARCH_TOOL_REVIVE", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(true, 0, 1, 50, "Revive Regulator", "RESEARCH_TOOL_REVIVE_REVIVE_REGULATOR", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 1, 30, "Revive Optimizer", "RESEARCH_TOOL_REVIVE_REVIVE_OPTIMIZER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE_REVIVE_REGULATOR"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 1, 30, "Revive Recycler", "RESEARCH_TOOL_REVIVE_FLASH_RECYCLER", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE_REVIVE_OPTIMIZER"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 1, 20, "Upgrade A", "RESEARCH_TOOL_REVIVE_UPGRADE_A", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 1, 40, "Upgrade B", "RESEARCH_TOOL_REVIVE_UPGRADE_B", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE_UPGRADE_A"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 1, 60, "Upgrade C", "RESEARCH_TOOL_REVIVE_UPGRADE_C", this.placeholdDescription, new string[]
			{
				"RESEARCH_TOOL_REVIVE_UPGRADE_B"
			}, "RESEARCH_TOOL_REVIVE", false),
			new ResearchNode(true, 0, 0, 0, "Player", "RESEARCH_PLAYER", this.placeholdDescription, Array.Empty<string>(), "", true),
			new ResearchNode(false, 0, 1, 20, "Upgrade A", "RESEARCH_PLAYER_UPGRADE_A", this.placeholdDescription, new string[]
			{
				"RESEARCH_PLAYER"
			}, "RESEARCH_PLAYER", false),
			new ResearchNode(false, 0, 1, 40, "Upgrade B", "RESEARCH_PLAYER_UPGRADE_B", this.placeholdDescription, new string[]
			{
				"RESEARCH_PLAYER"
			}, "RESEARCH_PLAYER", false),
			new ResearchNode(false, 0, 1, 60, "Upgrade C", "RESEARCH_PLAYER_UPGRADE_C", this.placeholdDescription, new string[]
			{
				"RESEARCH_PLAYER"
			}, "RESEARCH_PLAYER", false),
			new ResearchNode(false, 0, 1, 70, "Upgrade D", "RESEARCH_PLAYER_UPGRADE_D", this.placeholdDescription, new string[]
			{
				"RESEARCH_PLAYER_UPGRADE_B, RESEARCH_PLAYER_UPGRADE_C"
			}, "RESEARCH_PLAYER", false)
		}.ToArray();
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000D82BC File Offset: 0x000D64BC
	public void Assemble()
	{
		this.dictionary = new Dictionary<string, ResearchNode>();
		for (int i = 0; i < this.tree.Length; i++)
		{
			this.dictionary.TryAdd(this.tree[i].id, this.tree[i]);
			this.tree[i].modNodes = new List<ResearchNode>();
		}
		for (int j = 0; j < this.tree.Length; j++)
		{
			ResearchNode parentNode;
			if (!this.tree[j].rootUpgrade.IsNullOrEmpty() && this.dictionary.TryGetValue(this.tree[j].rootUpgrade, out parentNode))
			{
				this.dictionary[this.tree[j].id].parentNode = parentNode;
				this.dictionary[this.tree[j].rootUpgrade].modNodes.Add(this.dictionary[this.tree[j].id]);
			}
			if (!this.tree[j].requiredNodeIDs.IsNullOrEmpty<string>())
			{
				int num = 0;
				while (j < this.tree[num].requiredNodeIDs.Count)
				{
					this.dictionary[this.tree[j].id].requiredNodes = new List<ResearchNode>();
					ResearchNode item;
					if (this.dictionary.TryGetValue(this.tree[j].requiredNodeIDs[num], out item))
					{
						this.dictionary[this.tree[j].id].requiredNodes.Add(item);
					}
					num++;
				}
			}
		}
		this.FindRootNodes();
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000D8460 File Offset: 0x000D6660
	public bool IsNodeUnlocked(string nodeID)
	{
		ResearchNode researchNode;
		return this.dictionary.TryGetValue(nodeID, out researchNode) && researchNode.unlocked;
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000D8488 File Offset: 0x000D6688
	public int ResearchCost(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.researchCost;
		}
		return -1;
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000D84B0 File Offset: 0x000D66B0
	public int GetStoreCost(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.cost;
		}
		return -1;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000D84D8 File Offset: 0x000D66D8
	public string GetDisplayName(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.name;
		}
		return "";
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000D8504 File Offset: 0x000D6704
	public string GetDescription(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.description;
		}
		return "";
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000D8530 File Offset: 0x000D6730
	public ResearchNode GetParentNode(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.parentNode;
		}
		return null;
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000D8558 File Offset: 0x000D6758
	public List<ResearchNode> GetRequiredNodes(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.requiredNodes;
		}
		return null;
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000D8580 File Offset: 0x000D6780
	public List<ResearchNode> GetChildrenNodes(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			return researchNode.modNodes;
		}
		return null;
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000D85A8 File Offset: 0x000D67A8
	public bool IsNodeUnlockable(string nodeID)
	{
		ResearchNode researchNode;
		if (this.dictionary.TryGetValue(nodeID, out researchNode))
		{
			using (List<ResearchNode>.Enumerator enumerator = researchNode.requiredNodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.unlocked)
					{
						return false;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000D8614 File Offset: 0x000D6814
	private void FindRootNodes()
	{
		this.rootNodes = new List<ResearchNode>();
		for (int i = 0; i < this.tree.Length; i++)
		{
			if (this.tree[i].rootUpgrade.IsNullOrEmpty())
			{
				Debug.Log("RootNodes - Adding Root Node " + this.tree[i].id);
				this.rootNodes.Add(this.tree[i]);
			}
		}
	}

	// Token: 0x0400337E RID: 13182
	[FormerlySerializedAs("ResearchTreeNodesE")]
	public ResearchNode[] tree;

	// Token: 0x0400337F RID: 13183
	public string placeholdDescription = "Insert a really cool description here";

	// Token: 0x04003380 RID: 13184
	private List<ResearchNode> rootNodes = new List<ResearchNode>();

	// Token: 0x04003381 RID: 13185
	[NonSerialized]
	public Dictionary<string, ResearchNode> dictionary;
}
