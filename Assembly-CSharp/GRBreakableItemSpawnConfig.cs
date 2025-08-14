using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200060F RID: 1551
[CreateAssetMenu(fileName = "GhostReactorBreakableItemSpawnConfig", menuName = "ScriptableObjects/GhostReactorBreakableItemSpawnConfig")]
public class GRBreakableItemSpawnConfig : ScriptableObject
{
	// Token: 0x06002609 RID: 9737 RVA: 0x000CB818 File Offset: 0x000C9A18
	public bool TryForRandomItem(out GameEntity entity)
	{
		if (Random.Range(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = Random.Range(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000CB8AC File Offset: 0x000C9AAC
	public bool TryForRandomItem(ref SRand srand, out GameEntity entity)
	{
		if (srand.NextFloat(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = srand.NextFloat(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000CB940 File Offset: 0x000C9B40
	private void OnValidate()
	{
		this.precomputedItemTotalWeight = 0f;
		for (int i = 0; i < this.perItemProbabilities.Count; i++)
		{
			this.precomputedItemTotalWeight += this.perItemProbabilities[i].probability;
		}
	}

	// Token: 0x0400303D RID: 12349
	[SerializeField]
	[Range(0f, 1f)]
	public float spawnAnythingProbability = 0.2f;

	// Token: 0x0400303E RID: 12350
	public List<GRBreakableItemSpawnConfig.ItemProbability> perItemProbabilities = new List<GRBreakableItemSpawnConfig.ItemProbability>();

	// Token: 0x0400303F RID: 12351
	[SerializeField]
	[ReadOnly]
	private float precomputedItemTotalWeight;

	// Token: 0x02000610 RID: 1552
	[Serializable]
	public struct ItemProbability
	{
		// Token: 0x04003040 RID: 12352
		public GameEntity entity;

		// Token: 0x04003041 RID: 12353
		public float probability;
	}
}
