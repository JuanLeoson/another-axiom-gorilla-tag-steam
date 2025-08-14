using System;

// Token: 0x02000338 RID: 824
public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
	// Token: 0x060013B9 RID: 5049 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void InitializeTeleportDestination()
	{
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0006A0E4 File Offset: 0x000682E4
	protected override void UpdateTeleportDestination()
	{
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, null, null);
	}
}
