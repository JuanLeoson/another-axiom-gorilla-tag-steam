using System;
using UnityEngine.Events;

// Token: 0x020004DE RID: 1246
public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	// Token: 0x06001E6D RID: 7789 RVA: 0x000A14E3 File Offset: 0x0009F6E3
	public override void OnBoxTriggered()
	{
		if (this.onBoxTriggered != null)
		{
			this.onBoxTriggered.Invoke();
		}
	}

	// Token: 0x0400271D RID: 10013
	public UnityEvent onBoxTriggered;
}
