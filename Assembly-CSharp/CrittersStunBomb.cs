using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class CrittersStunBomb : CrittersToolThrowable
{
	// Token: 0x060002B9 RID: 697 RVA: 0x00010C50 File Offset: 0x0000EE50
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			Vector3 position = base.transform.position;
			List<CrittersPawn> crittersPawns = CrittersManager.instance.crittersPawns;
			for (int i = 0; i < crittersPawns.Count; i++)
			{
				CrittersPawn crittersPawn = crittersPawns[i];
				if (crittersPawn.isActiveAndEnabled && Vector3.Distance(crittersPawn.transform.position, position) < this.radius)
				{
					crittersPawn.Stunned(this.stunDuration);
				}
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, this.actorId, position, Quaternion.LookRotation(hitNormal));
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00010CE4 File Offset: 0x0000EEE4
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			impactedCritter.Stunned(this.stunDuration);
		}
	}

	// Token: 0x04000329 RID: 809
	[Header("Stun Bomb")]
	public float radius = 1f;

	// Token: 0x0400032A RID: 810
	public float stunDuration = 5f;
}
