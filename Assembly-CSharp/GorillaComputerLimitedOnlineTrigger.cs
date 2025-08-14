using System;
using GorillaNetworking;

// Token: 0x020007D7 RID: 2007
public class GorillaComputerLimitedOnlineTrigger : GorillaTriggerBox
{
	// Token: 0x06003243 RID: 12867 RVA: 0x001061BB File Offset: 0x001043BB
	public override void OnBoxTriggered()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(true);
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x001061CA File Offset: 0x001043CA
	public override void OnBoxExited()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(false);
	}
}
