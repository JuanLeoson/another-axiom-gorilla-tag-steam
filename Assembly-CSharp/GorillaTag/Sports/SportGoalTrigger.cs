using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000E9D RID: 3741
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x06005D94 RID: 23956 RVA: 0x001D81A3 File Offset: 0x001D63A3
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001D81C0 File Offset: 0x001D63C0
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x001D824C File Offset: 0x001D644C
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x04006765 RID: 26469
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04006766 RID: 26470
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x04006767 RID: 26471
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x04006768 RID: 26472
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
