using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class CritterIndex : ScriptableObject
{
	// Token: 0x17000015 RID: 21
	public CritterConfiguration this[int index]
	{
		get
		{
			if (index < 0 || index >= this.critterTypes.Count)
			{
				return null;
			}
			return this.critterTypes[index];
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00005D34 File Offset: 0x00003F34
	private void OnEnable()
	{
		CritterIndex._instance = this;
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00005D3C File Offset: 0x00003F3C
	public static Mesh GetMesh(CritterConfiguration.AnimalType animalType)
	{
		if (animalType < CritterConfiguration.AnimalType.Raccoon || animalType >= (CritterConfiguration.AnimalType)CritterIndex._instance.animalMeshes.Count)
		{
			return null;
		}
		return CritterIndex._instance.animalMeshes[(int)animalType].mesh;
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00005D6B File Offset: 0x00003F6B
	public int GetRandomCritterType(CrittersRegion region = null)
	{
		return this.critterTypes.IndexOf(this.GetRandomConfiguration(region));
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00005D80 File Offset: 0x00003F80
	public CritterConfiguration GetRandomConfiguration(CrittersRegion region = null)
	{
		WeightedList<CritterConfiguration> validCritterTypes = this.GetValidCritterTypes(region);
		if (validCritterTypes.Count == 0)
		{
			return null;
		}
		return validCritterTypes.GetRandomItem();
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00005DA5 File Offset: 0x00003FA5
	public static DateTime GetCritterDateTime()
	{
		if (!GorillaComputer.instance)
		{
			return DateTime.UtcNow;
		}
		return GorillaComputer.instance.GetServerTime();
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00005DC8 File Offset: 0x00003FC8
	private WeightedList<CritterConfiguration> GetValidCritterTypes(CrittersRegion region = null)
	{
		this._currentConfigs.Clear();
		DateTime critterDateTime = CritterIndex.GetCritterDateTime();
		foreach (CritterConfiguration critterConfiguration in this.critterTypes)
		{
			if (critterConfiguration.DateConditionsMet(critterDateTime) && critterConfiguration.CanSpawn(region))
			{
				this._currentConfigs.Add(critterConfiguration, critterConfiguration.spawnWeight);
			}
		}
		return this._currentConfigs;
	}

	// Token: 0x040000F2 RID: 242
	public List<CritterIndex.AnimalTypeMeshEntry> animalMeshes = new List<CritterIndex.AnimalTypeMeshEntry>();

	// Token: 0x040000F3 RID: 243
	public List<CritterConfiguration> critterTypes;

	// Token: 0x040000F4 RID: 244
	private WeightedList<CritterConfiguration> _currentConfigs = new WeightedList<CritterConfiguration>();

	// Token: 0x040000F5 RID: 245
	private static CritterIndex _instance;

	// Token: 0x0200003B RID: 59
	[Serializable]
	public class AnimalTypeMeshEntry
	{
		// Token: 0x040000F6 RID: 246
		public CritterConfiguration.AnimalType animalType;

		// Token: 0x040000F7 RID: 247
		public Mesh mesh;
	}
}
