using System;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x020003E5 RID: 997
public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06001761 RID: 5985 RVA: 0x0007EB84 File Offset: 0x0007CD84
	// (remove) Token: 0x06001762 RID: 5986 RVA: 0x0007EBBC File Offset: 0x0007CDBC
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06001763 RID: 5987 RVA: 0x0007EBF4 File Offset: 0x0007CDF4
	// (remove) Token: 0x06001764 RID: 5988 RVA: 0x0007EC2C File Offset: 0x0007CE2C
	public event SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent OnPaperPlaneHit;

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06001765 RID: 5989 RVA: 0x0007EC64 File Offset: 0x0007CE64
	// (remove) Token: 0x06001766 RID: 5990 RVA: 0x0007EC9C File Offset: 0x0007CE9C
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x06001767 RID: 5991 RVA: 0x0007ECD4 File Offset: 0x0007CED4
	// (remove) Token: 0x06001768 RID: 5992 RVA: 0x0007ED0C File Offset: 0x0007CF0C
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x06001769 RID: 5993 RVA: 0x0007ED44 File Offset: 0x0007CF44
	// (remove) Token: 0x0600176A RID: 5994 RVA: 0x0007ED7C File Offset: 0x0007CF7C
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	// Token: 0x0600176B RID: 5995 RVA: 0x0007EDB1 File Offset: 0x0007CFB1
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0007EDC5 File Offset: 0x0007CFC5
	public void InvokeHit(PaperPlaneProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent onPaperPlaneHit = this.OnPaperPlaneHit;
		if (onPaperPlaneHit == null)
		{
			return;
		}
		onPaperPlaneHit(projectile, collider);
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x0007EDD9 File Offset: 0x0007CFD9
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x0007EDED File Offset: 0x0007CFED
	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x0007EE01 File Offset: 0x0007D001
	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x0007EE15 File Offset: 0x0007D015
	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerEnter = null;
		this.OnProjectileTriggerExit = null;
	}

	// Token: 0x020003E6 RID: 998
	// (Invoke) Token: 0x06001773 RID: 6003
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	// Token: 0x020003E7 RID: 999
	// (Invoke) Token: 0x06001777 RID: 6007
	public delegate void PaperPlaneProjectileHitEvent(PaperPlaneProjectile projectile, Collider collider);

	// Token: 0x020003E8 RID: 1000
	// (Invoke) Token: 0x0600177B RID: 6011
	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}
