using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D84 RID: 3460
	public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
	{
		// Token: 0x0600562D RID: 22061 RVA: 0x001AC374 File Offset: 0x001AA574
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (NetworkSystem.Instance.InRoom && (!this.excludePrivateRooms || !NetworkSystem.Instance.SessionIsPrivate))
			{
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x0600562E RID: 22062 RVA: 0x001AC3D4 File Offset: 0x001AA5D4
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x04005FEC RID: 24556
		[SerializeField]
		private bool excludePrivateRooms;
	}
}
