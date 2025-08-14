using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class CosmeticCritterSpawnerShadeFleeing : CosmeticCritterSpawner
{
	// Token: 0x06000444 RID: 1092 RVA: 0x00019097 File Offset: 0x00017297
	public void SetSpawnPosition(Vector3 pos)
	{
		this.spawnPosition = pos;
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x000190A0 File Offset: 0x000172A0
	public override void OnSpawn(CosmeticCritter critter)
	{
		base.OnSpawn(critter);
		(critter as CosmeticCritterShadeFleeing).SetFleePosition(this.spawnPosition, base.transform.position);
	}

	// Token: 0x040004D6 RID: 1238
	private Vector3 spawnPosition;
}
