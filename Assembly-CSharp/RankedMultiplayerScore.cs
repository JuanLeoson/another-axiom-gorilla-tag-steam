using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000781 RID: 1921
public class RankedMultiplayerScore : MonoBehaviour
{
	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x06003032 RID: 12338 RVA: 0x000FD44E File Offset: 0x000FB64E
	// (set) Token: 0x06003033 RID: 12339 RVA: 0x000FD456 File Offset: 0x000FB656
	public RankedProgressionManager Progression { get; private set; }

	// Token: 0x06003034 RID: 12340 RVA: 0x000FD460 File Offset: 0x000FB660
	public void Initialize()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart += this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd += this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined += this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft += this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred += this.OnTagReported;
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null)
		{
			this.CompetitiveManager = (instance as GorillaTagCompetitiveManager);
		}
		this.Progression = RankedProgressionManager.Instance;
		RankedProgressionManager progression = this.Progression;
		progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Combine(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x000FD520 File Offset: 0x000FB720
	private void HandlePlayerEloAcquired(int playerId, float elo, int tier)
	{
		this.CachePlayerRankedProgressionData(playerId, tier, elo);
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x000FD52B File Offset: 0x000FB72B
	private void OnDestroy()
	{
		this.Unsubscribe();
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x000FD534 File Offset: 0x000FB734
	public void Unsubscribe()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart -= this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd -= this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined -= this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft -= this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred -= this.OnTagReported;
		if (this.Progression != null)
		{
			RankedProgressionManager progression = this.Progression;
			progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Remove(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
		}
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x000FD5DC File Offset: 0x000FB7DC
	private void Update()
	{
		if (this.PerSecondTimer > 0f && Time.time >= this.PerSecondTimer + 1f)
		{
			if (this.CompetitiveManager == null)
			{
				return;
			}
			this.OnPerSecondTimerElapsed(NetworkSystem.Instance.AllNetPlayers.Length, this.CompetitiveManager.currentInfected.Count);
			this.PerSecondTimer = Time.time;
		}
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x000FD648 File Offset: 0x000FB848
	private void OnPerSecondTimerElapsed(int playersInGame, int infectedPlayers)
	{
		foreach (int num in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			playerScoreInRound.Infected = this.CompetitiveManager.IsInfected(NetworkSystem.Instance.GetPlayer(num));
			if (!playerScoreInRound.Infected)
			{
				float t = (float)infectedPlayers / (float)playersInGame;
				playerScoreInRound.PointsOnDefense += Mathf.Lerp(this.PointsPerUninfectedSecMin, this.PointsPerUninfectedSecMax, t);
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x000FD700 File Offset: 0x000FB900
	public void ResetMatch()
	{
		this.AllFinalPlayerScores.Clear();
		this.AllPlayerInRoundScores.Clear();
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x000FD718 File Offset: 0x000FB918
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState state)
	{
		if (state == GorillaTagCompetitiveManager.GameState.StartingCountdown)
		{
			this.OnGameStarted();
			this.Progression.AcquireRoomRankInformation(true);
		}
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x000FD730 File Offset: 0x000FB930
	public void OnGameStarted()
	{
		this.PerSecondTimer = Time.time;
		if (!this.IsLateJoiner)
		{
			this.ResetMatch();
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				this.StartTrackingPlayer(NetworkSystem.Instance.AllNetPlayers[i], false);
			}
		}
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x000FD780 File Offset: 0x000FB980
	public void OnGameEnded()
	{
		foreach (int key in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[key];
			if (!playerScoreInRound.Infected)
			{
				playerScoreInRound.TaggedTime = Time.time;
			}
			this.AllPlayerInRoundScores[key] = playerScoreInRound;
		}
		this.PerSecondTimer = -1f;
		this.ReportScore();
		this.WasInfectedInitially = false;
		this.IsLateJoiner = false;
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x000FD824 File Offset: 0x000FBA24
	private void OnPlayerJoined(NetPlayer player)
	{
		if (NetworkSystem.Instance.IsMasterClient && this.CompetitiveManager.IsMatchActive())
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			List<bool> list5 = new List<bool>();
			List<float> list6 = new List<float>();
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				list.Add(keyValuePair.Value.PlayerId);
				list2.Add(keyValuePair.Value.NumTags);
				list3.Add(keyValuePair.Value.PointsOnDefense);
				list4.Add(Time.time - keyValuePair.Value.JoinTime);
				list5.Add(keyValuePair.Value.Infected);
				if (!keyValuePair.Value.Infected)
				{
					list6.Add(0f);
				}
				else
				{
					list6.Add(Time.time - keyValuePair.Value.TaggedTime);
				}
			}
			GameMode.ActiveNetworkHandler.SendRPC("SendScoresToLateJoinerRPC", player, new object[]
			{
				list.ToArray(),
				list2.ToArray(),
				list3.ToArray(),
				list4.ToArray(),
				list5.ToArray(),
				list6.ToArray()
			});
		}
		this.StartTrackingPlayer(player, true);
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x000FD9AC File Offset: 0x000FBBAC
	public void ReceivedScoresForLateJoiner(int[] playerIds, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			this.IsLateJoiner = true;
			for (int i = 0; i < playerIds.Length; i++)
			{
				int num = playerIds[i];
				RankedMultiplayerScore.PlayerScoreInRound value = new RankedMultiplayerScore.PlayerScoreInRound(num, infected[i]);
				value.NumTags = numTags[i];
				value.PointsOnDefense = pointsOnDefense[i];
				value.JoinTime = Time.time - joinTime[i];
				if (!infected[i])
				{
					value.TaggedTime = 0f;
				}
				else
				{
					value.TaggedTime = Time.time - taggedTime[i];
				}
				this.AllPlayerInRoundScores.TryAdd(num, value);
			}
		}
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x000FDA42 File Offset: 0x000FBC42
	private void OnPlayerLeft(NetPlayer player)
	{
		this.AllPlayerInRoundScores.Remove(player.ActorNumber);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x000FDA58 File Offset: 0x000FBC58
	private void StartTrackingPlayer(NetPlayer player, bool lateJoin)
	{
		bool initInfected = lateJoin;
		if (!lateJoin && this.CompetitiveManager != null)
		{
			initInfected = this.CompetitiveManager.IsInfected(player);
			if (player.ActorNumber == NetworkSystem.Instance.LocalPlayerID)
			{
				this.WasInfectedInitially = true;
			}
		}
		if (player == NetworkSystem.Instance.LocalPlayer)
		{
			this.CachePlayerRankedProgressionData(player.ActorNumber, this.Progression.GetProgressionRankIndex(), this.Progression.GetEloScore());
		}
		this.AllPlayerInRoundScores.TryAdd(player.ActorNumber, new RankedMultiplayerScore.PlayerScoreInRound(player.ActorNumber, initInfected));
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x000FDAEC File Offset: 0x000FBCEC
	public RankedMultiplayerScore.PlayerScoreInRound GetInGameScoreForSelf()
	{
		RankedMultiplayerScore.PlayerScoreInRound result;
		if (this.AllPlayerInRoundScores.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out result))
		{
			return result;
		}
		return default(RankedMultiplayerScore.PlayerScoreInRound);
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x000FDB20 File Offset: 0x000FBD20
	public void OnTagReported(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		RankedMultiplayerScore.PlayerScoreInRound value;
		if (this.AllPlayerInRoundScores.TryGetValue(taggingPlayer.ActorNumber, out value))
		{
			value.NumTags++;
			this.AllPlayerInRoundScores[taggingPlayer.ActorNumber] = value;
		}
		RankedMultiplayerScore.PlayerScoreInRound value2;
		if (this.AllPlayerInRoundScores.TryGetValue(taggedPlayer.ActorNumber, out value2))
		{
			value2.Infected = true;
			value2.TaggedTime = Time.time;
			this.AllPlayerInRoundScores[taggedPlayer.ActorNumber] = value2;
		}
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x000FDB9C File Offset: 0x000FBD9C
	private void ReportScore()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				this.AllFinalPlayerScores.Add(new RankedMultiplayerScore.PlayerScore
				{
					PlayerId = keyValuePair.Key,
					GameScore = this.ComputeGameScore(keyValuePair.Value.NumTags, keyValuePair.Value.PointsOnDefense),
					EloScore = (this.PlayerRankedElos.ContainsKey(keyValuePair.Key) ? this.PlayerRankedElos[keyValuePair.Key] : 0f),
					NumTags = keyValuePair.Value.NumTags,
					TimeUntagged = keyValuePair.Value.TaggedTime - keyValuePair.Value.JoinTime,
					PointsOnDefense = keyValuePair.Value.PointsOnDefense
				});
			}
			GorillaTagCompetitiveServerApi.Instance.RequestSubmitMatchScores((string)obj, this.AllFinalPlayerScores);
		}
		this.PredictPlayerEloChanges();
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x000FDCE8 File Offset: 0x000FBEE8
	public float ComputeGameScore(int tags, float pointsOnDefense)
	{
		return (float)(tags * this.PointsPerTag) + pointsOnDefense;
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x000FDCF8 File Offset: 0x000FBEF8
	private void PredictPlayerEloChanges()
	{
		this.VisitedScoreCombintations.Clear();
		this.AllFinalPlayerScores = (from s in this.AllFinalPlayerScores
		orderby s.GameScore descending
		select s).ToList<RankedMultiplayerScore.PlayerScore>();
		float k = this.Progression.MaxEloConstant / (float)(this.AllFinalPlayerScores.Count - 1);
		this.InProgressEloDeltaPerPlayer.Clear();
		for (int i = 0; i < this.AllFinalPlayerScores.Count; i++)
		{
			this.InProgressEloDeltaPerPlayer.Add(this.AllFinalPlayerScores[i].PlayerId, 0f);
		}
		for (int j = 0; j < this.AllFinalPlayerScores.Count; j++)
		{
			for (int l = 0; l < this.AllFinalPlayerScores.Count; l++)
			{
				if (j != l)
				{
					bool flag = this.AllFinalPlayerScores[j].GameScore.Approx(this.AllFinalPlayerScores[l].GameScore, 1E-06f);
					float eloWinProbability = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[l].EloScore, this.AllFinalPlayerScores[j].EloScore);
					float eloWinProbability2 = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[j].EloScore, this.AllFinalPlayerScores[l].EloScore);
					int key = j * this.AllFinalPlayerScores.Count + l;
					if (!this.VisitedScoreCombintations.ContainsKey(key))
					{
						RankedMultiplayerScore.PlayerScore playerScore = this.AllFinalPlayerScores[j];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((j < l) ? 1 : 0);
						}
						float eloScore = playerScore.EloScore;
						float num = RankedProgressionManager.UpdateEloScore(eloScore, eloWinProbability, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num - eloScore;
						this.VisitedScoreCombintations.Add(key, true);
					}
					int key2 = l * this.AllFinalPlayerScores.Count + j;
					if (!this.VisitedScoreCombintations.ContainsKey(key2))
					{
						RankedMultiplayerScore.PlayerScore playerScore2 = this.AllFinalPlayerScores[l];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((l < j) ? 1 : 0);
						}
						float eloScore2 = playerScore2.EloScore;
						float num2 = RankedProgressionManager.UpdateEloScore(eloScore2, eloWinProbability2, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore2.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num2 - eloScore2;
						this.VisitedScoreCombintations.Add(key2, true);
					}
				}
			}
		}
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x000FDF88 File Offset: 0x000FC188
	public void CachePlayerRankedProgressionData(int playerId, int tierIdx, float elo)
	{
		if (this.PlayerRankedTierIndices.ContainsKey(playerId))
		{
			this.PlayerRankedTierIndices[playerId] = tierIdx;
		}
		else
		{
			this.PlayerRankedTierIndices.Add(playerId, tierIdx);
		}
		if (this.PlayerRankedElos.ContainsKey(playerId))
		{
			this.PlayerRankedElos[playerId] = elo;
			return;
		}
		this.PlayerRankedElos.Add(playerId, elo);
	}

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x06003048 RID: 12360 RVA: 0x000FDFE8 File Offset: 0x000FC1E8
	// (set) Token: 0x06003049 RID: 12361 RVA: 0x000FDFF0 File Offset: 0x000FC1F0
	public Dictionary<int, int> PlayerRankedTiers
	{
		get
		{
			return this.PlayerRankedTierIndices;
		}
		set
		{
			this.PlayerRankedTierIndices = value;
		}
	}

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x0600304A RID: 12362 RVA: 0x000FDFF9 File Offset: 0x000FC1F9
	// (set) Token: 0x0600304B RID: 12363 RVA: 0x000FE001 File Offset: 0x000FC201
	public Dictionary<int, float> PlayerRankedEloScores
	{
		get
		{
			return this.PlayerRankedElos;
		}
		set
		{
			this.PlayerRankedElos = value;
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x0600304C RID: 12364 RVA: 0x000FE00A File Offset: 0x000FC20A
	// (set) Token: 0x0600304D RID: 12365 RVA: 0x000FE012 File Offset: 0x000FC212
	public Dictionary<int, float> ProjectedEloDeltas
	{
		get
		{
			return this.InProgressEloDeltaPerPlayer;
		}
		set
		{
			this.InProgressEloDeltaPerPlayer = value;
		}
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x000FE01C File Offset: 0x000FC21C
	public List<RankedMultiplayerScore.PlayerScoreInRound> GetSortedScores()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> list = new List<RankedMultiplayerScore.PlayerScoreInRound>();
		foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
		{
			list.Add(keyValuePair.Value);
		}
		list.Sort((RankedMultiplayerScore.PlayerScoreInRound s1, RankedMultiplayerScore.PlayerScoreInRound s2) => this.ComputeGameScore(s2.NumTags, s2.PointsOnDefense).CompareTo(this.ComputeGameScore(s1.NumTags, s1.PointsOnDefense)));
		return list;
	}

	// Token: 0x04003C2C RID: 15404
	public static float LongestUntaggedTieEpsilon = 0.2f;

	// Token: 0x04003C2D RID: 15405
	public static int RESULT_TIE = -1;

	// Token: 0x04003C2E RID: 15406
	[SerializeField]
	private int PointsPerTag = 30;

	// Token: 0x04003C2F RID: 15407
	[SerializeField]
	private float PointsPerUninfectedSecMin = 0.5f;

	// Token: 0x04003C30 RID: 15408
	[SerializeField]
	private float PointsPerUninfectedSecMax = 2f;

	// Token: 0x04003C31 RID: 15409
	private float PerSecondTimer = -1f;

	// Token: 0x04003C32 RID: 15410
	private bool WasInfectedInitially;

	// Token: 0x04003C33 RID: 15411
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04003C34 RID: 15412
	protected Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound> AllPlayerInRoundScores = new Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound>();

	// Token: 0x04003C35 RID: 15413
	protected List<RankedMultiplayerScore.PlayerScore> AllFinalPlayerScores = new List<RankedMultiplayerScore.PlayerScore>();

	// Token: 0x04003C36 RID: 15414
	protected Dictionary<int, bool> VisitedScoreCombintations = new Dictionary<int, bool>();

	// Token: 0x04003C37 RID: 15415
	protected Dictionary<int, float> InProgressEloDeltaPerPlayer = new Dictionary<int, float>();

	// Token: 0x04003C38 RID: 15416
	protected Dictionary<int, int> PlayerRankedTierIndices = new Dictionary<int, int>();

	// Token: 0x04003C39 RID: 15417
	protected Dictionary<int, float> PlayerRankedElos = new Dictionary<int, float>();

	// Token: 0x04003C3A RID: 15418
	private RankedMultiplayerScore.ResultData PendingResults;

	// Token: 0x04003C3B RID: 15419
	private RankedMultiplayerScore.RecordHolder<int> ResultsMostTags;

	// Token: 0x04003C3C RID: 15420
	private RankedMultiplayerScore.RecordHolder<float> ResultsLongestUntagged;

	// Token: 0x04003C3D RID: 15421
	private bool IsLateJoiner;

	// Token: 0x02000782 RID: 1922
	public struct PlayerScore
	{
		// Token: 0x04003C3F RID: 15423
		public int PlayerId;

		// Token: 0x04003C40 RID: 15424
		public float GameScore;

		// Token: 0x04003C41 RID: 15425
		public float EloScore;

		// Token: 0x04003C42 RID: 15426
		public int NumTags;

		// Token: 0x04003C43 RID: 15427
		public float TimeUntagged;

		// Token: 0x04003C44 RID: 15428
		public float PointsOnDefense;
	}

	// Token: 0x02000783 RID: 1923
	public struct PlayerScoreInRound
	{
		// Token: 0x06003052 RID: 12370 RVA: 0x000FE15C File Offset: 0x000FC35C
		public PlayerScoreInRound(int id, bool initInfected = false)
		{
			this.PlayerId = id;
			this.NumTags = 0;
			this.PointsOnDefense = 0f;
			this.JoinTime = Time.time;
			this.Infected = initInfected;
			this.TaggedTime = (initInfected ? Time.time : 0f);
		}

		// Token: 0x04003C45 RID: 15429
		public int PlayerId;

		// Token: 0x04003C46 RID: 15430
		public int NumTags;

		// Token: 0x04003C47 RID: 15431
		public float PointsOnDefense;

		// Token: 0x04003C48 RID: 15432
		public float JoinTime;

		// Token: 0x04003C49 RID: 15433
		public float TaggedTime;

		// Token: 0x04003C4A RID: 15434
		public bool Infected;
	}

	// Token: 0x02000784 RID: 1924
	public struct ResultData
	{
		// Token: 0x06003053 RID: 12371 RVA: 0x000FE1A9 File Offset: 0x000FC3A9
		public bool IsMostTagsTied()
		{
			return this.MostTagsPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x000FE1B8 File Offset: 0x000FC3B8
		public bool IsLongestUntaggedTied()
		{
			return this.LongestUntaggedPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x04003C4B RID: 15435
		public float Elo;

		// Token: 0x04003C4C RID: 15436
		public int Rank;

		// Token: 0x04003C4D RID: 15437
		public int MostTags;

		// Token: 0x04003C4E RID: 15438
		public float LongestUntagged;

		// Token: 0x04003C4F RID: 15439
		public int MostTagsPlayerId;

		// Token: 0x04003C50 RID: 15440
		public int LongestUntaggedPlayerId;
	}

	// Token: 0x02000785 RID: 1925
	public struct RecordHolder<T>
	{
		// Token: 0x04003C51 RID: 15441
		public int PlayerId;

		// Token: 0x04003C52 RID: 15442
		public T Value;
	}
}
