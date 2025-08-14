using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000518 RID: 1304
[CreateAssetMenu(fileName = "BuilderMaterialResourceColors", menuName = "Gorilla Tag/Builder/ResourceColors", order = 0)]
public class BuilderResourceColors : ScriptableObject
{
	// Token: 0x06001FBA RID: 8122 RVA: 0x000A78DC File Offset: 0x000A5ADC
	public Color GetColorForType(BuilderResourceType type)
	{
		foreach (BuilderResourceColor builderResourceColor in this.colors)
		{
			if (builderResourceColor.type == type)
			{
				return builderResourceColor.color;
			}
		}
		return Color.black;
	}

	// Token: 0x0400285F RID: 10335
	public List<BuilderResourceColor> colors;
}
