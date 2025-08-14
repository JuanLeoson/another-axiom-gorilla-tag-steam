using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000602 RID: 1538
public class GRAttributes : MonoBehaviour
{
	// Token: 0x060025D1 RID: 9681 RVA: 0x000CAA88 File Offset: 0x000C8C88
	private void Awake()
	{
		foreach (GRAttributes.GRAttributePair grattributePair in this.startingAttributes)
		{
			this.defaultAttributes[grattributePair.type] = grattributePair.value;
		}
		this.bonusSystem.Init(this);
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000CAAF8 File Offset: 0x000C8CF8
	public void AddBonus(GRBonusEntry entry)
	{
		this.bonusSystem.AddBonus(entry);
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000CAB06 File Offset: 0x000C8D06
	public void RemoveBonus(GRBonusEntry entry)
	{
		this.bonusSystem.RemoveBonus(entry);
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000CAB14 File Offset: 0x000C8D14
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		return this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
	}

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private List<GRAttributes.GRAttributePair> startingAttributes;

	// Token: 0x04002FF5 RID: 12277
	[NonSerialized]
	private GRBonusSystem bonusSystem = new GRBonusSystem();

	// Token: 0x04002FF6 RID: 12278
	public Dictionary<GRAttributeType, int> defaultAttributes = new Dictionary<GRAttributeType, int>();

	// Token: 0x02000603 RID: 1539
	[Serializable]
	public struct GRAttributePair
	{
		// Token: 0x04002FF7 RID: 12279
		public GRAttributeType type;

		// Token: 0x04002FF8 RID: 12280
		public int value;
	}
}
