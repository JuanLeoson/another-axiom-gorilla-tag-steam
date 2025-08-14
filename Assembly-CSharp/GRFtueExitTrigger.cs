using System;
using UnityEngine;

// Token: 0x0200063E RID: 1598
public class GRFtueExitTrigger : GorillaTriggerBox
{
	// Token: 0x0600275C RID: 10076 RVA: 0x000D454D File Offset: 0x000D274D
	public override void OnBoxTriggered()
	{
		this.startTime = Time.time;
		this.ftueObject.InterruptWaitingTimer();
		this.ftueObject.playerLight.GetComponentInChildren<Light>().intensity = 0.25f;
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000D457F File Offset: 0x000D277F
	private void Update()
	{
		if (this.startTime > 0f && Time.time - this.startTime > this.delayTime)
		{
			this.ftueObject.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
			this.startTime = -1f;
		}
	}

	// Token: 0x04003286 RID: 12934
	public GRFirstTimeUserExperience ftueObject;

	// Token: 0x04003287 RID: 12935
	public float delayTime = 5f;

	// Token: 0x04003288 RID: 12936
	private float startTime = -1f;
}
