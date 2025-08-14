using System;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class MonkeyeProjectileTarget : MonoBehaviour
{
	// Token: 0x06000FCA RID: 4042 RVA: 0x0005C131 File Offset: 0x0005A331
	private void Awake()
	{
		this.monkeyeAI = base.GetComponent<MonkeyeAI>();
		this.notifier = base.GetComponentInChildren<SlingshotProjectileHitNotifier>();
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0005C14B File Offset: 0x0005A34B
	private void OnEnable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit += this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit += this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0005C189 File Offset: 0x0005A389
	private void OnDisable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit -= this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit -= this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0005C1C7 File Offset: 0x0005A3C7
	private void Notifier_OnProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0005C1C7 File Offset: 0x0005A3C7
	private void Notifier_OnPaperPlaneHit(PaperPlaneProjectile projectile, Collider collider)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x04001856 RID: 6230
	private MonkeyeAI monkeyeAI;

	// Token: 0x04001857 RID: 6231
	private SlingshotProjectileHitNotifier notifier;
}
