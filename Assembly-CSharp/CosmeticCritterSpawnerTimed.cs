using System;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public abstract class CosmeticCritterSpawnerTimed : CosmeticCritterSpawnerIndependent
{
	// Token: 0x0600223E RID: 8766 RVA: 0x000B9456 File Offset: 0x000B7656
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(5, this.spawnIntervalMinMax.x, 0.5f);
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x000B946E File Offset: 0x000B766E
	public override bool CanSpawnLocal()
	{
		if (Time.time >= this.nextLocalSpawnTime)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
			return base.CanSpawnLocal();
		}
		return false;
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x000B94AC File Offset: 0x000B76AC
	public override bool CanSpawnRemote(double serverTime)
	{
		return base.CanSpawnRemote(serverTime);
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000B94B5 File Offset: 0x000B76B5
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.IsLocal)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
		}
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000B94EC File Offset: 0x000B76EC
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04002BAF RID: 11183
	[Tooltip("The minimum and maximum time to wait between spawn attempts.")]
	[SerializeField]
	private Vector2 spawnIntervalMinMax = new Vector2(2f, 5f);

	// Token: 0x04002BB0 RID: 11184
	[Tooltip("Currently does nothing.")]
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance = 1f;
}
