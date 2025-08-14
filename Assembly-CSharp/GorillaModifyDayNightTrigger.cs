using System;

// Token: 0x020006E7 RID: 1767
public class GorillaModifyDayNightTrigger : GorillaTriggerBox
{
	// Token: 0x06002BED RID: 11245 RVA: 0x000E8DE8 File Offset: 0x000E6FE8
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (this.clearModifiedTime)
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}
		else
		{
			int num = this.timeOfDayIndex % BetterDayNightManager.instance.timeOfDayRange.Length;
			BetterDayNightManager.instance.SetTimeOfDay(this.timeOfDayIndex);
			BetterDayNightManager.instance.SetOverrideIndex(this.timeOfDayIndex);
		}
		if (this.setFixedWeather)
		{
			BetterDayNightManager.instance.SetFixedWeather(this.fixedWeather);
			return;
		}
		BetterDayNightManager.instance.ClearFixedWeather();
	}

	// Token: 0x04003754 RID: 14164
	public bool clearModifiedTime;

	// Token: 0x04003755 RID: 14165
	public int timeOfDayIndex;

	// Token: 0x04003756 RID: 14166
	public bool setFixedWeather;

	// Token: 0x04003757 RID: 14167
	public BetterDayNightManager.WeatherType fixedWeather;
}
