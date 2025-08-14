using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class TeleportAimHandlerLaser : TeleportAimHandler
{
	// Token: 0x06001376 RID: 4982 RVA: 0x000694F0 File Offset: 0x000676F0
	public override void GetPoints(List<Vector3> points)
	{
		Ray ray;
		base.LocomotionTeleport.InputHandler.GetAimData(out ray);
		points.Add(ray.origin);
		points.Add(ray.origin + ray.direction * this.Range);
	}

	// Token: 0x04001AF9 RID: 6905
	[Tooltip("Maximum range for aiming.")]
	public float Range = 100f;
}
