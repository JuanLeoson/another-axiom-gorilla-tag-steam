using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C7E RID: 3198
	public class TappableBell : Tappable
	{
		// Token: 0x14000088 RID: 136
		// (add) Token: 0x06004F20 RID: 20256 RVA: 0x00189684 File Offset: 0x00187884
		// (remove) Token: 0x06004F21 RID: 20257 RVA: 0x001896BC File Offset: 0x001878BC
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x06004F22 RID: 20258 RVA: 0x001896F4 File Offset: 0x001878F4
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		// Token: 0x0400580E RID: 22542
		private VRRig winnerRig;

		// Token: 0x04005810 RID: 22544
		public CallLimiter rpcCooldown;

		// Token: 0x02000C7F RID: 3199
		// (Invoke) Token: 0x06004F25 RID: 20261
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
