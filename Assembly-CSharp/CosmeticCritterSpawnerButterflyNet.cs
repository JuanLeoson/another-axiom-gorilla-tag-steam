using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	// Token: 0x0600042C RID: 1068 RVA: 0x0001892C File Offset: 0x00016B2C
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 startPos = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(startPos);
	}

	// Token: 0x040004B8 RID: 1208
	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
