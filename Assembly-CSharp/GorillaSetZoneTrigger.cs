using System;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x06000E2F RID: 3631 RVA: 0x00057067 File Offset: 0x00055267
	public override void OnBoxTriggered()
	{
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x040016ED RID: 5869
	[SerializeField]
	private GTZone[] zones;
}
