using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CrittersCageSettings : CrittersActorSettings
{
	// Token: 0x06000190 RID: 400 RVA: 0x0000A084 File Offset: 0x00008284
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersCage crittersCage = (CrittersCage)this.parentActor;
		crittersCage.cagePosition = this.cagePoint;
		crittersCage.grabPosition = this.grabPoint;
	}

	// Token: 0x040001DE RID: 478
	public Transform cagePoint;

	// Token: 0x040001DF RID: 479
	public Transform grabPoint;
}
