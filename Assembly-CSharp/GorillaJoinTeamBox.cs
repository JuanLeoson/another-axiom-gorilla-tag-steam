using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006E3 RID: 1763
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x06002BE3 RID: 11235 RVA: 0x000E8C8D File Offset: 0x000E6E8D
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x0400374A RID: 14154
	public bool joinRedTeam;
}
