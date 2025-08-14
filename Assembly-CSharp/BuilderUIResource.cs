using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000561 RID: 1377
public class BuilderUIResource : MonoBehaviour
{
	// Token: 0x06002194 RID: 8596 RVA: 0x000B6C04 File Offset: 0x000B4E04
	public void SetResourceCost(BuilderResourceQuantity resourceCost, BuilderTable table)
	{
		BuilderResourceType type = resourceCost.type;
		int count = resourceCost.count;
		int availableResources = table.GetAvailableResources(type);
		if (this.resourceNameLabel != null)
		{
			this.resourceNameLabel.text = this.GetResourceName(type);
		}
		if (this.costLabel != null)
		{
			this.costLabel.text = count.ToString();
		}
		if (this.availableLabel != null)
		{
			this.availableLabel.text = availableResources.ToString();
		}
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000B6C87 File Offset: 0x000B4E87
	private string GetResourceName(BuilderResourceType type)
	{
		switch (type)
		{
		case BuilderResourceType.Basic:
			return "Basic";
		case BuilderResourceType.Decorative:
			return "Decorative";
		case BuilderResourceType.Functional:
			return "Functional";
		default:
			return "Resource Needs Name";
		}
	}

	// Token: 0x04002AF3 RID: 10995
	public TextMeshPro resourceNameLabel;

	// Token: 0x04002AF4 RID: 10996
	public TextMeshPro costLabel;

	// Token: 0x04002AF5 RID: 10997
	public TextMeshPro availableLabel;
}
