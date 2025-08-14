using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000707 RID: 1799
public class GorillaTagCompetitiveScoreboard : MonoBehaviour
{
	// Token: 0x06002D23 RID: 11555 RVA: 0x000EDD5C File Offset: 0x000EBF5C
	private void Awake()
	{
		GorillaTagCompetitiveManager.RegisterScoreboard(this);
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x000EDD95 File Offset: 0x000EBF95
	private void OnDestroy()
	{
		GorillaTagCompetitiveManager.DeregisterScoreboard(this);
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x000EDDA0 File Offset: 0x000EBFA0
	public void UpdateScores(GorillaTagCompetitiveManager.GameState gameState, float activeRoundTime, List<RankedMultiplayerScore.PlayerScoreInRound> scores, Dictionary<int, int> PlayerRankedTiers, Dictionary<int, float> PlayerPredictedEloDeltas, List<NetPlayer> infectedPlayers, RankedProgressionManager progressionManager)
	{
		this.waitingForPlayers.SetActive(gameState == GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		for (int i = 0; i < this.lines.Length; i++)
		{
			if (gameState != GorillaTagCompetitiveManager.GameState.WaitingForPlayers && scores != null && scores.Count > i)
			{
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = scores[i];
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerScoreInRound.PlayerId);
				if (netPlayerByID != null)
				{
					this.lines[i].gameObject.SetActive(true);
					if (PlayerRankedTiers == null || !PlayerRankedTiers.ContainsKey(playerScoreInRound.PlayerId))
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, null);
					}
					else
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, progressionManager.GetProgressionRankIcon(PlayerRankedTiers[playerScoreInRound.PlayerId]));
					}
					if (playerScoreInRound.TaggedTime.Approx(0f, 1E-06f))
					{
						this.lines[i].SetScore(Mathf.Max(activeRoundTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					else
					{
						this.lines[i].SetScore(Mathf.Max(playerScoreInRound.TaggedTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					if (PlayerPredictedEloDeltas.ContainsKey(playerScoreInRound.PlayerId))
					{
						float num = PlayerPredictedEloDeltas[playerScoreInRound.PlayerId];
						GorillaTagCompetitiveScoreboard.PredictedResult predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Even;
						if (num > this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Great;
						}
						else if (num > this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Good;
						}
						else if (num < -this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Poor;
						}
						else if (num < -this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Bad;
						}
						this.lines[i].SetPredictedResult(predictedResult);
					}
					this.lines[i].SetInfected(gameState == GorillaTagCompetitiveManager.GameState.Playing && infectedPlayers.Contains(netPlayerByID));
				}
			}
			else
			{
				this.lines[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x000EDF70 File Offset: 0x000EC170
	public void DisplayPredictedResults(bool bShow)
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x0400386D RID: 14445
	public GorillaTagCompetitiveScoreboardLine[] lines;

	// Token: 0x0400386E RID: 14446
	public GameObject waitingForPlayers;

	// Token: 0x0400386F RID: 14447
	public float smallEloDelta = 10f;

	// Token: 0x04003870 RID: 14448
	public float largeEloDelta = 25f;

	// Token: 0x02000708 RID: 1800
	public enum PredictedResult
	{
		// Token: 0x04003872 RID: 14450
		Great,
		// Token: 0x04003873 RID: 14451
		Good,
		// Token: 0x04003874 RID: 14452
		Even,
		// Token: 0x04003875 RID: 14453
		Bad,
		// Token: 0x04003876 RID: 14454
		Poor
	}
}
