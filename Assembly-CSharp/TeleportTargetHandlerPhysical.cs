using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class TeleportTargetHandlerPhysical : TeleportTargetHandler
{
	// Token: 0x060013E4 RID: 5092 RVA: 0x0006A9F8 File Offset: 0x00068BF8
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, this.AimCollisionLayerMask, out this.AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * this.AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}
}
