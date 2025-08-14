using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000E9C RID: 3740
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06005D92 RID: 23954 RVA: 0x001D816C File Offset: 0x001D636C
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x04006764 RID: 26468
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
