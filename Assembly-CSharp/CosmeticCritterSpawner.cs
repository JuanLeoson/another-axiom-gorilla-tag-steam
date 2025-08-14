using System;
using UnityEngine;

// Token: 0x02000575 RID: 1397
public abstract class CosmeticCritterSpawner : CosmeticCritterHoldable
{
	// Token: 0x06002230 RID: 8752 RVA: 0x000B9384 File Offset: 0x000B7584
	public GameObject GetCritterPrefab()
	{
		return this.critterPrefab;
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x000B938C File Offset: 0x000B758C
	public CosmeticCritter GetCritter()
	{
		return this.cachedCritter;
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000B9394 File Offset: 0x000B7594
	public Type GetCritterType()
	{
		return this.cachedType;
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void SetRandomVariables(CosmeticCritter critter)
	{
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000B939C File Offset: 0x000B759C
	public virtual void OnSpawn(CosmeticCritter critter)
	{
		this.numCritters++;
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000B93AC File Offset: 0x000B75AC
	public virtual void OnDespawn(CosmeticCritter critter)
	{
		this.numCritters = Math.Max(this.numCritters - 1, 0);
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000B93C2 File Offset: 0x000B75C2
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.cachedCritter == null)
		{
			this.cachedCritter = this.critterPrefab.GetComponent<CosmeticCritter>();
			this.cachedType = this.cachedCritter.GetType();
		}
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000B93FA File Offset: 0x000B75FA
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04002BA9 RID: 11177
	[Tooltip("The critter prefab to spawn.")]
	[SerializeField]
	protected GameObject critterPrefab;

	// Token: 0x04002BAA RID: 11178
	[Tooltip("The maximum number of critters that this spawner can have active at once.")]
	[SerializeField]
	protected int maxCritters;

	// Token: 0x04002BAB RID: 11179
	protected CosmeticCritter cachedCritter;

	// Token: 0x04002BAC RID: 11180
	protected Type cachedType;

	// Token: 0x04002BAD RID: 11181
	protected int numCritters;

	// Token: 0x04002BAE RID: 11182
	protected float nextLocalSpawnTime;
}
