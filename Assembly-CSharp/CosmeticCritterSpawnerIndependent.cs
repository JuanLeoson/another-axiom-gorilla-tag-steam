using System;

// Token: 0x02000576 RID: 1398
public class CosmeticCritterSpawnerIndependent : CosmeticCritterSpawner
{
	// Token: 0x06002239 RID: 8761 RVA: 0x000B9402 File Offset: 0x000B7602
	public virtual bool CanSpawnLocal()
	{
		return this.numCritters < this.maxCritters;
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000B9412 File Offset: 0x000B7612
	public virtual bool CanSpawnRemote(double serverTime)
	{
		return this.numCritters < this.maxCritters && this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000B9430 File Offset: 0x000B7630
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterIndependentSpawner(this);
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000B9443 File Offset: 0x000B7643
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterIndependentSpawner(this);
	}
}
