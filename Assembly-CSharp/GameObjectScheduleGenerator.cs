using System;
using System.Globalization;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x02000095 RID: 149
[CreateAssetMenu(fileName = "New Game Object Schedule Generator", menuName = "Game Object Scheduling/Game Object Schedule Generator")]
public class GameObjectScheduleGenerator : ScriptableObject
{
	// Token: 0x060003B5 RID: 949 RVA: 0x00016CB4 File Offset: 0x00014EB4
	private void GenerateSchedule()
	{
		DateTime startDate;
		try
		{
			startDate = DateTime.Parse(this.scheduleStart, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand Start Date " + this.scheduleStart);
			return;
		}
		DateTime endDate;
		try
		{
			endDate = DateTime.Parse(this.scheduleEnd, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand End Date " + this.scheduleEnd);
			return;
		}
		if (this.scheduleType == GameObjectScheduleGenerator.ScheduleType.DailyShuffle)
		{
			GameObjectSchedule.GenerateDailyShuffle(startDate, endDate, this.schedules);
		}
	}

	// Token: 0x0400042F RID: 1071
	[SerializeField]
	private GameObjectSchedule[] schedules;

	// Token: 0x04000430 RID: 1072
	[SerializeField]
	private string scheduleStart = "1/1/0001 00:00:00";

	// Token: 0x04000431 RID: 1073
	[SerializeField]
	private string scheduleEnd = "1/1/0001 00:00:00";

	// Token: 0x04000432 RID: 1074
	[SerializeField]
	private GameObjectScheduleGenerator.ScheduleType scheduleType;

	// Token: 0x02000096 RID: 150
	private enum ScheduleType
	{
		// Token: 0x04000434 RID: 1076
		DailyShuffle
	}
}
