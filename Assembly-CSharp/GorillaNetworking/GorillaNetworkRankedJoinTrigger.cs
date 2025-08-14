using System;

namespace GorillaNetworking
{
	// Token: 0x02000D87 RID: 3463
	public class GorillaNetworkRankedJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06005633 RID: 22067 RVA: 0x001AC53A File Offset: 0x001AA73A
		public override string GetFullDesiredGameModeString()
		{
			return this.networkZone + base.GetDesiredGameType();
		}

		// Token: 0x06005634 RID: 22068 RVA: 0x001AC54D File Offset: 0x001AA74D
		public override void OnBoxTriggered()
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoom(this, JoinType.Solo);
		}
	}
}
