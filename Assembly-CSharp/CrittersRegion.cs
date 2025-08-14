using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class CrittersRegion : MonoBehaviour
{
	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000294 RID: 660 RVA: 0x00010527 File Offset: 0x0000E727
	public static List<CrittersRegion> Regions
	{
		get
		{
			return CrittersRegion._regions;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000295 RID: 661 RVA: 0x0001052E File Offset: 0x0000E72E
	public int CritterCount
	{
		get
		{
			return this._critters.Count;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000296 RID: 662 RVA: 0x0001053B File Offset: 0x0000E73B
	// (set) Token: 0x06000297 RID: 663 RVA: 0x00010543 File Offset: 0x0000E743
	public int ID { get; private set; }

	// Token: 0x06000298 RID: 664 RVA: 0x0001054C File Offset: 0x0000E74C
	private void OnEnable()
	{
		CrittersRegion.RegisterRegion(this);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00010554 File Offset: 0x0000E754
	private void OnDisable()
	{
		CrittersRegion.UnregisterRegion(this);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0001055C File Offset: 0x0000E75C
	private static void RegisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup[region.ID] = region;
		CrittersRegion._regions.Add(region);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0001057A File Offset: 0x0000E77A
	private static void UnregisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup.Remove(region.ID);
		CrittersRegion._regions.Remove(region);
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0001059C File Offset: 0x0000E79C
	public static void AddCritterToRegion(CrittersPawn critter, int regionId)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(regionId, out crittersRegion))
		{
			crittersRegion.AddCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Attempted to add critter to non-existing region {0}.", regionId), null);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000105D8 File Offset: 0x0000E7D8
	public static void RemoveCritterFromRegion(CrittersPawn critter)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(critter.regionId, out crittersRegion))
		{
			crittersRegion.RemoveCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Couldn't find region with id {0}", critter.regionId), null);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0001061C File Offset: 0x0000E81C
	public void AddCritter(CrittersPawn pawn)
	{
		this._critters.Add(pawn);
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0001062A File Offset: 0x0000E82A
	public void RemoveCritter(CrittersPawn pawn)
	{
		this._critters.Remove(pawn);
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001063C File Offset: 0x0000E83C
	public Vector3 GetSpawnPoint()
	{
		float num = this.scale / 2f;
		float num2 = base.transform.lossyScale.y * this.scale;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, -base.transform.up, out raycastHit, num2, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
			return raycastHit.point;
		}
		Debug.DrawLine(vector, vector - base.transform.up * num2, Color.red, 5f);
		return vector;
	}

	// Token: 0x04000313 RID: 787
	private static List<CrittersRegion> _regions = new List<CrittersRegion>();

	// Token: 0x04000314 RID: 788
	private static Dictionary<int, CrittersRegion> _regionLookup = new Dictionary<int, CrittersRegion>();

	// Token: 0x04000315 RID: 789
	public CrittersBiome Biome = CrittersBiome.Any;

	// Token: 0x04000316 RID: 790
	public int maxCritters = 10;

	// Token: 0x04000317 RID: 791
	public float scale = 10f;

	// Token: 0x04000318 RID: 792
	public List<CrittersPawn> _critters = new List<CrittersPawn>();
}
