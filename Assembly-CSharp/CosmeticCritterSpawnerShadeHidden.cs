using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class CosmeticCritterSpawnerShadeHidden : CosmeticCritterSpawnerTimed
{
	// Token: 0x06000447 RID: 1095 RVA: 0x000190D0 File Offset: 0x000172D0
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		float y = Random.Range(this.orbitHeightOffsetMinMax.x, this.orbitHeightOffsetMinMax.y);
		float radius = Random.Range(this.orbitRadiusMinMax.x, this.orbitRadiusMinMax.y);
		(critter as CosmeticCritterShadeHidden).SetCenterAndRadius(base.transform.position + new Vector3(0f, y, 0f), radius);
	}

	// Token: 0x040004D7 RID: 1239
	[Tooltip("Add between X and Y extra height to the base orbit height.")]
	[SerializeField]
	private Vector2 orbitHeightOffsetMinMax = new Vector2(0f, 2f);

	// Token: 0x040004D8 RID: 1240
	[Tooltip("Orbit between X (green sphere) and Y (red sphere) units away from this spawner's position when first spawned.")]
	[SerializeField]
	private Vector2 orbitRadiusMinMax = new Vector2(5f, 10f);
}
