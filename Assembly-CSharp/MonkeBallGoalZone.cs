using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000505 RID: 1285
public class MonkeBallGoalZone : MonoBehaviour
{
	// Token: 0x06001F63 RID: 8035 RVA: 0x000A6358 File Offset: 0x000A4558
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (MonkeBallGame.Instance.GetGameState() == MonkeBallGame.GameState.Playing)
		{
			for (int i = 0; i < this.playersInGoalZone.Count; i++)
			{
				MonkeBallPlayer monkeBallPlayer = this.playersInGoalZone[i];
				if (monkeBallPlayer.gamePlayer.teamId != this.teamId)
				{
					GameBallId gameBallId = monkeBallPlayer.gamePlayer.GetGameBallId();
					if (gameBallId.IsValid())
					{
						MonkeBallGame.Instance.RequestScore(monkeBallPlayer.gamePlayer.teamId);
						GameBallId gameBallId2 = monkeBallPlayer.gamePlayer.GetGameBallId();
						int otherTeam = MonkeBallGame.Instance.GetOtherTeam(monkeBallPlayer.gamePlayer.teamId);
						if (MonkeBallGame.Instance.resetBallPositionOnScore)
						{
							MonkeBallGame.Instance.RequestResetBall(gameBallId2, otherTeam);
						}
						MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(gameBallId2, otherTeam);
						monkeBallPlayer.gamePlayer.ClearGrabbedIfHeld(gameBallId);
					}
				}
			}
		}
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000A6438 File Offset: 0x000A4638
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = this;
				this.playersInGoalZone.Add(component);
			}
		}
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000A6488 File Offset: 0x000A4688
	private void OnTriggerExit(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = null;
				this.playersInGoalZone.Remove(component);
			}
		}
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000A64D8 File Offset: 0x000A46D8
	public void CleanupPlayer(MonkeBallPlayer player)
	{
		this.playersInGoalZone.Remove(player);
	}

	// Token: 0x040027F4 RID: 10228
	public int teamId;

	// Token: 0x040027F5 RID: 10229
	public List<MonkeBallPlayer> playersInGoalZone;
}
