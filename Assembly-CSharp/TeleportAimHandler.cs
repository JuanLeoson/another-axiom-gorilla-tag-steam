using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000329 RID: 809
public abstract class TeleportAimHandler : TeleportSupport
{
	// Token: 0x06001372 RID: 4978 RVA: 0x000694AC File Offset: 0x000676AC
	protected override void OnEnable()
	{
		base.OnEnable();
		base.LocomotionTeleport.AimHandler = this;
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x000694C0 File Offset: 0x000676C0
	protected override void OnDisable()
	{
		if (base.LocomotionTeleport.AimHandler == this)
		{
			base.LocomotionTeleport.AimHandler = null;
		}
		base.OnDisable();
	}

	// Token: 0x06001374 RID: 4980
	public abstract void GetPoints(List<Vector3> points);
}
