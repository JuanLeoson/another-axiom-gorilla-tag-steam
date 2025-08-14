using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C80 RID: 3200
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x06004F28 RID: 20264 RVA: 0x00189754 File Offset: 0x00187954
		public void UpdateBoard(string winner, ObstacleCourse.RaceState _currentState)
		{
			if (this.output == null)
			{
				return;
			}
			switch (_currentState)
			{
			case ObstacleCourse.RaceState.Started:
				Debug.Log(this.raceStarted);
				this.output.text = this.raceStarted;
				return;
			case ObstacleCourse.RaceState.Waiting:
				Debug.Log(this.raceLoading);
				this.output.text = this.raceLoading;
				return;
			case ObstacleCourse.RaceState.Finished:
				Debug.Log(winner + " WON!!");
				this.output.text = winner + " WON!!";
				return;
			default:
				return;
			}
		}

		// Token: 0x04005811 RID: 22545
		public string raceStarted = "RACE STARTED!";

		// Token: 0x04005812 RID: 22546
		public string raceLoading = "RACE LOADING...";

		// Token: 0x04005813 RID: 22547
		[SerializeField]
		private TextMeshPro output;
	}
}
