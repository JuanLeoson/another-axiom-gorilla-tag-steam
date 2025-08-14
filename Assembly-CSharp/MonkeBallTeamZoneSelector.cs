using System;
using UnityEngine;

// Token: 0x0200050C RID: 1292
public class MonkeBallTeamZoneSelector : MonoBehaviour
{
	// Token: 0x06001F85 RID: 8069 RVA: 0x000A695C File Offset: 0x000A4B5C
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.IsLocalPlayer() && gamePlayer.teamId != this.teamId)
		{
			MonkeBallGame.Instance.RequestSetTeam(this.teamId);
		}
	}

	// Token: 0x0400281A RID: 10266
	public int teamId;
}
