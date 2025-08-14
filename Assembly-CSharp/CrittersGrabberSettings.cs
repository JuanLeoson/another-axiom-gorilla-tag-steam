using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class CrittersGrabberSettings : CrittersActorSettings
{
	// Token: 0x060001A7 RID: 423 RVA: 0x0000A5EA File Offset: 0x000087EA
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersGrabber crittersGrabber = (CrittersGrabber)this.parentActor;
		crittersGrabber.grabPosition = this._grabPosition;
		crittersGrabber.grabDistance = this._grabDistance;
	}

	// Token: 0x040001F7 RID: 503
	public Transform _grabPosition;

	// Token: 0x040001F8 RID: 504
	public float _grabDistance;
}
