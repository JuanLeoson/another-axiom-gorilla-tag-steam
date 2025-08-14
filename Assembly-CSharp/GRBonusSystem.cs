using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200060C RID: 1548
public class GRBonusSystem
{
	// Token: 0x060025FB RID: 9723 RVA: 0x000CB2AC File Offset: 0x000C94AC
	public void Init(GRAttributes attributes)
	{
		this.defaultAttributes = attributes;
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000CB2B8 File Offset: 0x000C94B8
	public void AddBonus(GRBonusEntry entry)
	{
		if (entry.bonusType == GRBonusEntry.GRBonusType.None)
		{
			return;
		}
		if (!this.currentAdditiveBonuses.ContainsKey(entry.attributeType))
		{
			this.currentAdditiveBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (!this.currentMultiplicativeBonuses.ContainsKey(entry.attributeType))
		{
			this.currentMultiplicativeBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Additive)
		{
			this.currentAdditiveBonuses[entry.attributeType].Add(entry);
			return;
		}
		this.currentMultiplicativeBonuses[entry.attributeType].Add(entry);
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000CB358 File Offset: 0x000C9558
	public void RemoveBonus(GRBonusEntry entry)
	{
		foreach (List<GRBonusEntry> list in this.currentAdditiveBonuses.Values)
		{
			list.Remove(entry);
		}
		foreach (List<GRBonusEntry> list2 in this.currentMultiplicativeBonuses.Values)
		{
			list2.Remove(entry);
		}
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000CB3F8 File Offset: 0x000C95F8
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		if (this.defaultAttributes == null)
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes null.  Please fix configuration.", Array.Empty<object>());
			return 0;
		}
		if (!this.defaultAttributes.defaultAttributes.ContainsKey(attributeType))
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes Does not have entry for {0}.  Please fix configuration.", new object[]
			{
				attributeType
			});
			return 0;
		}
		int num = this.defaultAttributes.defaultAttributes[attributeType];
		if (this.currentAdditiveBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry in this.currentAdditiveBonuses[attributeType])
			{
				if (grbonusEntry.customBonus != null)
				{
					num = grbonusEntry.customBonus(num, grbonusEntry);
				}
				else
				{
					num += grbonusEntry.bonusValue;
				}
			}
		}
		if (this.currentMultiplicativeBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry2 in this.currentMultiplicativeBonuses[attributeType])
			{
				if (grbonusEntry2.customBonus != null)
				{
					num = grbonusEntry2.customBonus(num, grbonusEntry2);
				}
				else
				{
					num *= grbonusEntry2.bonusValue;
				}
			}
		}
		return num;
	}

	// Token: 0x0400302C RID: 12332
	private GRAttributes defaultAttributes;

	// Token: 0x0400302D RID: 12333
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentAdditiveBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();

	// Token: 0x0400302E RID: 12334
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentMultiplicativeBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();
}
