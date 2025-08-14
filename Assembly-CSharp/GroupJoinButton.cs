using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x02000739 RID: 1849
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06002E37 RID: 11831 RVA: 0x000F4F92 File Offset: 0x000F3192
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x000F4FBA File Offset: 0x000F31BA
	public void Update()
	{
		this.inPrivate = (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible);
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04003A14 RID: 14868
	public int gameModeIndex;

	// Token: 0x04003A15 RID: 14869
	public GorillaFriendCollider friendCollider;

	// Token: 0x04003A16 RID: 14870
	public bool inPrivate;
}
