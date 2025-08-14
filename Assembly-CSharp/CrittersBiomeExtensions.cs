using System;
using System.Collections.Generic;

// Token: 0x0200004C RID: 76
public static class CrittersBiomeExtensions
{
	// Token: 0x0600016C RID: 364 RVA: 0x00009768 File Offset: 0x00007968
	static CrittersBiomeExtensions()
	{
		CrittersBiomeExtensions._allScannableBiomes = new List<CrittersBiome>();
		foreach (object obj in Enum.GetValues(typeof(CrittersBiome)))
		{
			CrittersBiome crittersBiome = (CrittersBiome)obj;
			if (crittersBiome != CrittersBiome.Any && crittersBiome != CrittersBiome.IntroArea)
			{
				CrittersBiomeExtensions._allScannableBiomes.Add(crittersBiome);
			}
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x000097F0 File Offset: 0x000079F0
	public static string GetHabitatDescription(this CrittersBiome biome)
	{
		string text;
		if (!CrittersBiomeExtensions._habitatLookup.TryGetValue(biome, out text))
		{
			if (biome == CrittersBiome.Any)
			{
				text = "Any";
			}
			else
			{
				if (CrittersBiomeExtensions._habitatBiomes == null)
				{
					CrittersBiomeExtensions._habitatBiomes = new List<CrittersBiome>();
				}
				CrittersBiomeExtensions._habitatBiomes.Clear();
				for (int i = 0; i < CrittersBiomeExtensions._allScannableBiomes.Count; i++)
				{
					if (biome.HasFlag(CrittersBiomeExtensions._allScannableBiomes[i]))
					{
						CrittersBiomeExtensions._habitatBiomes.Add(CrittersBiomeExtensions._allScannableBiomes[i]);
					}
				}
			}
			text = ((CrittersBiomeExtensions._habitatBiomes.Count > 3) ? "Various" : string.Join<CrittersBiome>(", ", CrittersBiomeExtensions._habitatBiomes));
			CrittersBiomeExtensions._habitatLookup[biome] = text;
		}
		return text;
	}

	// Token: 0x040001AB RID: 427
	private static List<CrittersBiome> _allScannableBiomes;

	// Token: 0x040001AC RID: 428
	private static Dictionary<CrittersBiome, string> _habitatLookup = new Dictionary<CrittersBiome, string>();

	// Token: 0x040001AD RID: 429
	private static List<CrittersBiome> _habitatBiomes;
}
