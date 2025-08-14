using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F96 RID: 3990
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		// Token: 0x04006EA2 RID: 28322
		public string CountdownTo = "1/1/0001 00:00:00";

		// Token: 0x04006EA3 RID: 28323
		public string FormatString = "{0} {1}";

		// Token: 0x04006EA4 RID: 28324
		public string DefaultString = "";

		// Token: 0x04006EA5 RID: 28325
		public int DaysThreshold = 365;
	}
}
