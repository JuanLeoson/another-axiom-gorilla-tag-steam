using System;

// Token: 0x02000342 RID: 834
public abstract class TeleportTransition : TeleportSupport
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x0006AA6C File Offset: 0x00068C6C
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += this.LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0006AA8C File Offset: 0x00068C8C
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= this.LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	// Token: 0x060013E8 RID: 5096
	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
