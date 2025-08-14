using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020000F5 RID: 245
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000629 RID: 1577 RVA: 0x00023B97 File Offset: 0x00021D97
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00023BB6 File Offset: 0x00021DB6
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00023BC3 File Offset: 0x00021DC3
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x04000750 RID: 1872
	public PlayableDirector timeline;

	// Token: 0x04000751 RID: 1873
	public int eventHour = 7;

	// Token: 0x04000752 RID: 1874
	private int scheduledEventID;
}
