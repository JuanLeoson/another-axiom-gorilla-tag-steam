using System;

// Token: 0x02000345 RID: 837
public class TeleportTransitionInstant : TeleportTransition
{
	// Token: 0x060013F3 RID: 5107 RVA: 0x0006AC2A File Offset: 0x00068E2A
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.LocomotionTeleport.DoTeleport();
	}
}
