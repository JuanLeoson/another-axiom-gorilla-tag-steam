using System;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public abstract class CosmeticCritterCatcher : CosmeticCritterHoldable
{
	// Token: 0x0600220B RID: 8715 RVA: 0x000B89B3 File Offset: 0x000B6BB3
	public CosmeticCritterSpawner GetLinkedSpawner()
	{
		return this.optionalLinkedSpawner;
	}

	// Token: 0x0600220C RID: 8716
	public abstract CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter);

	// Token: 0x0600220D RID: 8717 RVA: 0x000B89BB File Offset: 0x000B6BBB
	public virtual bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600220E RID: 8718
	public abstract void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime);

	// Token: 0x0600220F RID: 8719 RVA: 0x000B89C9 File Offset: 0x000B6BC9
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterCatcher(this);
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000B89DC File Offset: 0x000B6BDC
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterCatcher(this);
	}

	// Token: 0x04002B93 RID: 11155
	[SerializeField]
	[Tooltip("If this catcher is capable of spawning immediately after catching, the linked spawner must be assigned here.")]
	protected CosmeticCritterSpawner optionalLinkedSpawner;
}
