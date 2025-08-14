using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
[Serializable]
public class CritterConfiguration
{
	// Token: 0x060000D3 RID: 211 RVA: 0x00005B34 File Offset: 0x00003D34
	public CritterConfiguration()
	{
		this.animalType = CritterConfiguration.AnimalType.UNKNOWN;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00005B60 File Offset: 0x00003D60
	public int GetIndex()
	{
		return CrittersManager.instance.creatureIndex.critterTypes.IndexOf(this);
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00005B79 File Offset: 0x00003D79
	private bool RegionMatches(CrittersRegion region)
	{
		return !region || (region.Biome & this.biome) > (CrittersBiome)0;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00005B95 File Offset: 0x00003D95
	private bool SpawnCriteriaMatches()
	{
		return !this.spawnCriteria || this.spawnCriteria.CanSpawn();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00005BB1 File Offset: 0x00003DB1
	public bool CanSpawn()
	{
		return this.SpawnCriteriaMatches();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00005BB9 File Offset: 0x00003DB9
	public bool CanSpawn(CrittersRegion region)
	{
		return this.RegionMatches(region) && this.SpawnCriteriaMatches();
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00005BCC File Offset: 0x00003DCC
	public bool DateConditionsMet(DateTime utcDate)
	{
		return !this.dateLimit || this.dateLimit.MatchesDate(utcDate);
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00005BE9 File Offset: 0x00003DE9
	public bool ShouldDespawn()
	{
		return !this.SpawnCriteriaMatches();
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00005BF4 File Offset: 0x00003DF4
	public void ApplyToCreature(CrittersPawn crittersPawn)
	{
		this.behaviour.ApplyToCritter(crittersPawn);
		if (CrittersManager.instance.LocalAuthority())
		{
			this.ApplyVisualsTo(crittersPawn, true);
			return;
		}
		this.ApplyVisualsTo(crittersPawn, false);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00005C21 File Offset: 0x00003E21
	private void ApplyVisualsTo(CrittersPawn critter, bool generateAppearance = true)
	{
		this.ApplyVisualsTo(critter.visuals, generateAppearance);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00005C30 File Offset: 0x00003E30
	public void ApplyVisualsTo(CritterVisuals visuals, bool generateAppearance = true)
	{
		visuals.critterType = this.GetIndex();
		visuals.ApplyMesh(CritterIndex.GetMesh(this.animalType));
		visuals.ApplyMaterial(this.critterMat);
		if (generateAppearance)
		{
			visuals.SetAppearance(this.GenerateAppearance());
		}
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00005C6C File Offset: 0x00003E6C
	public CritterAppearance GenerateAppearance()
	{
		string hatName = "";
		if (Random.value <= this.behaviour.GetTemplateValue<float>("hatChance"))
		{
			GameObject[] templateValue = this.behaviour.GetTemplateValue<GameObject[]>("hats");
			if (!templateValue.IsNullOrEmpty<GameObject>())
			{
				hatName = templateValue[Random.Range(0, templateValue.Length)].name;
			}
		}
		float templateValue2 = this.behaviour.GetTemplateValue<float>("minSize");
		float templateValue3 = this.behaviour.GetTemplateValue<float>("maxSize");
		float size = Random.Range(templateValue2, templateValue3);
		return new CritterAppearance(hatName, size);
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00005CF4 File Offset: 0x00003EF4
	public override string ToString()
	{
		return string.Format("{0} B:{1} C:{2}", this.critterName, this.behaviour, this.spawnCriteria);
	}

	// Token: 0x040000E2 RID: 226
	[Tooltip("Basic internal description of critter.  Could be role, purpose, player experience, etc.")]
	public string internalDescription;

	// Token: 0x040000E3 RID: 227
	public string critterName = "UNNAMED CRITTER";

	// Token: 0x040000E4 RID: 228
	public CritterConfiguration.AnimalType animalType;

	// Token: 0x040000E5 RID: 229
	public CritterTemplate behaviour;

	// Token: 0x040000E6 RID: 230
	public CritterSpawnCriteria spawnCriteria;

	// Token: 0x040000E7 RID: 231
	public RealWorldDateTimeWindow dateLimit;

	// Token: 0x040000E8 RID: 232
	public CrittersBiome biome = CrittersBiome.Any;

	// Token: 0x040000E9 RID: 233
	public float spawnWeight = 1f;

	// Token: 0x040000EA RID: 234
	public Material critterMat;

	// Token: 0x02000039 RID: 57
	public enum AnimalType
	{
		// Token: 0x040000EC RID: 236
		Raccoon,
		// Token: 0x040000ED RID: 237
		Cat,
		// Token: 0x040000EE RID: 238
		Bird,
		// Token: 0x040000EF RID: 239
		Goblin,
		// Token: 0x040000F0 RID: 240
		Egg,
		// Token: 0x040000F1 RID: 241
		UNKNOWN = -1
	}
}
