using System;

// Token: 0x02000A00 RID: 2560
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x06003E93 RID: 16019 RVA: 0x0013ECA5 File Offset: 0x0013CEA5
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
