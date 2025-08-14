using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200078C RID: 1932
public class RankedProgressionManager : MonoBehaviour
{
	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x06003080 RID: 12416 RVA: 0x000FE64A File Offset: 0x000FC84A
	// (set) Token: 0x06003081 RID: 12417 RVA: 0x000FE652 File Offset: 0x000FC852
	public int MaxRank { get; private set; }

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06003082 RID: 12418 RVA: 0x000FE65B File Offset: 0x000FC85B
	// (set) Token: 0x06003083 RID: 12419 RVA: 0x000FE663 File Offset: 0x000FC863
	public float LowTierThreshold { get; set; }

	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x06003084 RID: 12420 RVA: 0x000FE66C File Offset: 0x000FC86C
	// (set) Token: 0x06003085 RID: 12421 RVA: 0x000FE674 File Offset: 0x000FC874
	public float HighTierThreshold { get; set; }

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06003086 RID: 12422 RVA: 0x000FE67D File Offset: 0x000FC87D
	// (set) Token: 0x06003087 RID: 12423 RVA: 0x000023F5 File Offset: 0x000005F5
	public List<RankedProgressionManager.RankedProgressionTier> MajorTiers
	{
		get
		{
			return this.majorTiers;
		}
		private set
		{
		}
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x000023F5 File Offset: 0x000005F5
	private void DebugSetELO()
	{
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x000023F5 File Offset: 0x000005F5
	[ContextMenu("Reset ELO")]
	private void DebugResetELO()
	{
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x000FE685 File Offset: 0x000FC885
	private void Awake()
	{
		if (RankedProgressionManager.Instance)
		{
			GTDev.LogError<string>("Duplicate RankedProgressionManager detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		RankedProgressionManager.Instance = this;
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x000FE6B4 File Offset: 0x000FC8B4
	private void Start()
	{
		if (this.majorTiers.Count < 3)
		{
			GTDev.LogWarning<string>("At least 3 MMR tiers must be defined.", null);
			return;
		}
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
		float minThreshold = 100f;
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			this.majorTiers[i].SetMinThreshold((i == 0) ? 100f : this.majorTiers[i - 1].thresholdMax);
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				num++;
				this.majorTiers[i].subTiers[j].SetMinThreshold(minThreshold);
				minThreshold = this.majorTiers[i].subTiers[j].thresholdMax;
			}
		}
		this.MaxRank = num - 1;
		this.LowTierThreshold = this.majorTiers[0].thresholdMax;
		List<RankedProgressionManager.RankedProgressionTier> list = this.majorTiers;
		this.HighTierThreshold = list[list.Count - 1].GetMinThreshold();
		this.EloScorePC = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_PC_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.EloScoreQuest = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxPC = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxQuest = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x000FE866 File Offset: 0x000FCA66
	private void OnDestroy()
	{
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x000FE894 File Offset: 0x000FCA94
	public void RequestUnlockCompetitiveQueue(bool unlock)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestUnlockCompetitiveQueue(unlock, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x000FE8AD File Offset: 0x000FCAAD
	public IEnumerator LoadStatsWhenReady()
	{
		yield return new WaitUntil(() => NetworkSystem.Instance.LocalPlayer.UserId != null);
		if (this.HasUnlockedCompetitiveQueue())
		{
			this.RequestUnlockCompetitiveQueue(true);
		}
		else
		{
			this.AcquireLocalPlayerRankInformation();
		}
		yield break;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x000FE8BC File Offset: 0x000FCABC
	private void OnJoinedRoom(GameModeType newGameModeType)
	{
		if (newGameModeType == GameModeType.InfectionCompetitive)
		{
			this.AcquireRoomRankInformation(false);
		}
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x000FE8CA File Offset: 0x000FCACA
	private void OnPlayerJoined(NetPlayer player)
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.InfectionCompetitive)
		{
			this.AcquireSinglePlayerRankInformation(player);
		}
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x000FE8F0 File Offset: 0x000FCAF0
	private void AcquireLocalPlayerRankInformation()
	{
		List<string> list = new List<string>();
		list.Add(NetworkSystem.Instance.LocalPlayer.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnLocalPlayerRankedInformationAcquired));
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x000FE930 File Offset: 0x000FCB30
	private void AcquireSinglePlayerRankInformation(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(player.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x000FE96C File Offset: 0x000FCB6C
	public void AcquireRoomRankInformation(bool includeLocalPlayer = true)
	{
		List<string> list = new List<string>();
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (includeLocalPlayer || !netPlayer.IsLocal)
			{
				list.Add(netPlayer.UserId);
			}
		}
		if (list.Count > 0)
		{
			GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
		}
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x000FE9F4 File Offset: 0x000FCBF4
	private void OnPlayersRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		foreach (GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData rankedModePlayerProgressionData in rankedModeProgressionData.playerData)
		{
			if (rankedModePlayerProgressionData != null && rankedModePlayerProgressionData.platformData != null && rankedModePlayerProgressionData.platformData.Length >= 2)
			{
				int num = -1;
				foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
				{
					if (netPlayer.UserId == rankedModePlayerProgressionData.playfabID)
					{
						num = netPlayer.ActorNumber;
						break;
					}
				}
				if (num >= 0)
				{
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = rankedModePlayerProgressionData.platformData[1];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData2 = rankedModePlayerProgressionData.platformData[0];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData3 = rankedModeProgressionPlatformData2;
					int rankFromTiers = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData3.majorTier, rankedModeProgressionPlatformData3.minorTier);
					Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
					if (onPlayerEloAcquired != null)
					{
						onPlayerEloAcquired(num, rankedModeProgressionPlatformData3.elo, rankFromTiers);
					}
					if (num == NetworkSystem.Instance.LocalPlayerID)
					{
						this.SetLocalProgressionData(rankedModePlayerProgressionData);
					}
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
					{
						VRRig rig = rigContainer.Rig;
						if (rig != null)
						{
							int rankFromTiers2 = this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
							int rankFromTiers3 = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData2.majorTier, rankedModeProgressionPlatformData2.minorTier);
							rig.SetRankedInfo(rankedModeProgressionPlatformData3.elo, rankFromTiers2, rankFromTiers3, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x000FEB84 File Offset: 0x000FCD84
	private void OnLocalPlayerRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		if (rankedModeProgressionData.playerData.Count > 0)
		{
			this.SetLocalProgressionData(rankedModeProgressionData.playerData[0]);
			float eloScore = this.GetEloScore();
			int progressionRankIndexQuest = this.GetProgressionRankIndexQuest();
			int progressionRankIndexPC = this.GetProgressionRankIndexPC();
			int tier = progressionRankIndexPC;
			this.HandlePlayerRankedInfoReceived(NetworkSystem.Instance.LocalPlayer.ActorNumber, eloScore, tier);
			VRRig.LocalRig.SetRankedInfo(eloScore, progressionRankIndexQuest, progressionRankIndexPC, true);
		}
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x000FEBEF File Offset: 0x000FCDEF
	public bool AreValuesValid(float elo, int questTier, int pcTier)
	{
		return elo >= 100f && elo <= 4000f && questTier >= 0 && questTier <= this.MaxRank && pcTier >= 0 && pcTier <= this.MaxRank;
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x000FEC1E File Offset: 0x000FCE1E
	public void HandlePlayerRankedInfoReceived(int actorNum, float elo, int tier)
	{
		Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
		if (onPlayerEloAcquired == null)
		{
			return;
		}
		onPlayerEloAcquired(actorNum, elo, tier);
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x000FEC33 File Offset: 0x000FCE33
	public void SetLocalProgressionData(GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
	{
		this.ProgressionData = data;
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x000FEC3C File Offset: 0x000FCE3C
	public void LoadStats()
	{
		base.StartCoroutine(this.LoadStatsWhenReady());
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x000FEC4B File Offset: 0x000FCE4B
	public float GetEloScore()
	{
		return this.GetEloScorePC();
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x000FEC53 File Offset: 0x000FCE53
	public void SetEloScore(float val)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestSetEloValue(val, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x000FEC6C File Offset: 0x000FCE6C
	public float GetEloScorePC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[0].elo;
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x000FECAB File Offset: 0x000FCEAB
	public float GetEloScoreQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[1].elo;
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x000FECEA File Offset: 0x000FCEEA
	private int GetNewTierGracePeriodIdx()
	{
		return this.NewTierGracePeriodIdxPC;
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x000FECF7 File Offset: 0x000FCEF7
	private void SetNewTierGracePeriodIdx(int val)
	{
		this.NewTierGracePeriodIdxPC.Set(val);
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x000FED05 File Offset: 0x000FCF05
	private void IncrementNewTierGracePeriodIdx()
	{
		this.NewTierGracePeriodIdxPC.Increment();
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x000FED12 File Offset: 0x000FCF12
	public bool TryGetProgressionSubTier(out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		subTier = null;
		index = -1;
		return this.TryGetProgressionSubTier(this.GetEloScore(), out subTier, out index);
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x000FED28 File Offset: 0x000FCF28
	public bool TryGetProgressionSubTier(float elo, out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		int num = 0;
		subTier = null;
		index = -1;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			float num2 = (i < this.majorTiers.Count - 1) ? this.majorTiers[i].thresholdMax : 4000.1f;
			if (elo < num2)
			{
				int j = 0;
				while (j < this.majorTiers[i].subTiers.Count)
				{
					float num3 = (j < this.majorTiers[i].subTiers.Count - 1) ? this.majorTiers[i].subTiers[j].thresholdMax : num2;
					if (elo < num3)
					{
						subTier = this.majorTiers[i].subTiers[j];
						index = num;
						return true;
					}
					j++;
					num++;
				}
			}
			else
			{
				num += this.majorTiers[i].subTiers.Count;
			}
		}
		return false;
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x000FEE2C File Offset: 0x000FD02C
	private RankedProgressionManager.RankedProgressionTier GetProgressionMajorTierBySubTierIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x000FEE88 File Offset: 0x000FD088
	private RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTierByIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i].subTiers[j];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x000FEEF0 File Offset: 0x000FD0F0
	private RankedProgressionManager.RankedProgressionSubTier GetNextProgressionSubTierByIndex(int idx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx + 1);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex;
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x000FEF14 File Offset: 0x000FD114
	private RankedProgressionManager.RankedProgressionSubTier GetPrevProgressionSubTierByIndex(int idx)
	{
		if (idx > 0)
		{
			RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx - 1);
			if (progressionSubTierByIndex != null)
			{
				return progressionSubTierByIndex;
			}
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x000FEF3B File Offset: 0x000FD13B
	public string GetProgressionRankName()
	{
		return this.GetProgressionRankName(this.GetEloScore());
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x000FEF4C File Offset: 0x000FD14C
	public string GetProgressionRankName(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.name;
		}
		return string.Empty;
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x000FEF74 File Offset: 0x000FD174
	public string GetNextProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x000FEF94 File Offset: 0x000FD194
	public string GetPrevProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x000FEFB4 File Offset: 0x000FD1B4
	public int GetProgressionRankIndex()
	{
		return this.GetProgressionRankIndexPC();
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x000FEFBC File Offset: 0x000FD1BC
	public RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTier()
	{
		return this.GetProgressionSubTierByIndex(this.GetProgressionRankIndex());
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x000FEFCC File Offset: 0x000FD1CC
	public int GetProgressionRankIndexQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[1];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x000FF020 File Offset: 0x000FD220
	public int GetProgressionRankIndexPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[0];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x000FF074 File Offset: 0x000FD274
	public int GetRankFromTiers(int majorTier, int minorTier)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				if (i == majorTier && j == minorTier)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x000FF0CC File Offset: 0x000FD2CC
	public int GetProgressionRankIndex(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int result;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x000FF0E9 File Offset: 0x000FD2E9
	public float GetProgressionRankProgress()
	{
		return this.GetProgressionRankProgressPC();
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x000FF0F1 File Offset: 0x000FD2F1
	public float GetProgressionRankProgressQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[1].rankProgress;
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x000FF130 File Offset: 0x000FD330
	public float GetProgressionRankProgressPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[0].rankProgress;
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x000FF170 File Offset: 0x000FD370
	public int ClampProgressionRankIndex(int subTierIdx)
	{
		if (subTierIdx < 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == subTierIdx)
				{
					return subTierIdx;
				}
				j++;
				num++;
			}
		}
		return num - 1;
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x000FF1CC File Offset: 0x000FD3CC
	public Sprite GetProgressionRankIcon()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return null;
		}
		int index = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].minorTier;
		int index2 = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[index2].subTiers[index];
		if (rankedProgressionSubTier == null)
		{
			return null;
		}
		return rankedProgressionSubTier.icon;
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x000FF264 File Offset: 0x000FD464
	public string GetRankedProgressionTierName()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return "None";
		}
		int minorTier = this.ProgressionData.platformData[0].minorTier;
		int majorTier = this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[majorTier].subTiers[minorTier];
		if (rankedProgressionSubTier != null)
		{
			return rankedProgressionSubTier.name;
		}
		return "None";
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x000FF2F0 File Offset: 0x000FD4F0
	public Sprite GetProgressionRankIcon(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.icon;
		}
		return null;
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x000FF314 File Offset: 0x000FD514
	public Sprite GetProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x000FF334 File Offset: 0x000FD534
	public Sprite GetNextProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x000FF354 File Offset: 0x000FD554
	public Sprite GetPrevProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x000FF374 File Offset: 0x000FD574
	public float GetCurrentELO()
	{
		return this.GetEloScore();
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x000FF37C File Offset: 0x000FD57C
	public void GetSubtierRankThresholds(int subTierIdx, out float minThreshold, out float maxThreshold)
	{
		minThreshold = 0f;
		maxThreshold = 1f;
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			maxThreshold = progressionSubTierByIndex.thresholdMax;
			if (maxThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex != null)
				{
					maxThreshold = progressionMajorTierBySubTierIndex.thresholdMax;
					if (maxThreshold <= 0f)
					{
						maxThreshold = 4000f;
					}
				}
			}
			minThreshold = progressionSubTierByIndex.GetMinThreshold();
			if (minThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex2 = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex2 != null)
				{
					minThreshold = progressionMajorTierBySubTierIndex2.GetMinThreshold();
					if (minThreshold <= 0f)
					{
						minThreshold = 100f;
					}
				}
			}
		}
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x000FF40A File Offset: 0x000FD60A
	public static float GetEloWinProbability(float ratingPlayer1, float ratingPlayer2)
	{
		return 1f / (1f + Mathf.Pow(10f, (ratingPlayer1 - ratingPlayer2) / 400f));
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x000FF42B File Offset: 0x000FD62B
	public static float UpdateEloScore(float eloScore, float expectedResult, float actualResult, float k)
	{
		return Mathf.Clamp(eloScore + k * (actualResult - expectedResult), 100f, 4000f);
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x000FF443 File Offset: 0x000FD643
	public RankedProgressionManager.ERankedMatchmakingTier GetRankedMatchmakingTier()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return RankedProgressionManager.ERankedMatchmakingTier.Low;
		}
		return (RankedProgressionManager.ERankedMatchmakingTier)this.ProgressionData.platformData[0].majorTier;
	}

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x060030C0 RID: 12480 RVA: 0x000FF47E File Offset: 0x000FD67E
	public float CompetitiveQueueEloFloor
	{
		get
		{
			return this.LowTierThreshold;
		}
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x000FF486 File Offset: 0x000FD686
	private bool HasUnlockedCompetitiveQueue()
	{
		return GorillaComputer.instance.allowedInCompetitive;
	}

	// Token: 0x04003C63 RID: 15459
	public static RankedProgressionManager Instance;

	// Token: 0x04003C64 RID: 15460
	public const float DEFAULT_ELO = 100f;

	// Token: 0x04003C65 RID: 15461
	public const float MIN_ELO = 100f;

	// Token: 0x04003C66 RID: 15462
	public const float MAX_ELO = 4000f;

	// Token: 0x04003C67 RID: 15463
	public const float MAJOR_TIER_MIN_RANGE = 200f;

	// Token: 0x04003C68 RID: 15464
	public const float SUB_TIER_MIN_RANGE = 20f;

	// Token: 0x04003C69 RID: 15465
	public static string RANKED_ELO_KEY = "RankedElo";

	// Token: 0x04003C6A RID: 15466
	public static string RANKED_PROGRESSION_GRACE_PERIOD_KEY = "RankedProgGracePeriod";

	// Token: 0x04003C6B RID: 15467
	public static string RANKED_ELO_PC_KEY = "RankedEloPC";

	// Token: 0x04003C6C RID: 15468
	public static string RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY = "RankedProgGracePeriodPC";

	// Token: 0x04003C6D RID: 15469
	private RankedMultiplayerStatisticFloat EloScorePC;

	// Token: 0x04003C6E RID: 15470
	private RankedMultiplayerStatisticFloat EloScoreQuest;

	// Token: 0x04003C6F RID: 15471
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxPC;

	// Token: 0x04003C70 RID: 15472
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxQuest;

	// Token: 0x04003C71 RID: 15473
	private GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData ProgressionData;

	// Token: 0x04003C72 RID: 15474
	[SerializeField]
	private List<RankedProgressionManager.RankedProgressionTier> majorTiers = new List<RankedProgressionManager.RankedProgressionTier>();

	// Token: 0x04003C73 RID: 15475
	[SerializeField]
	private int newTierGracePeriod = 3;

	// Token: 0x04003C74 RID: 15476
	public float MaxEloConstant = 90f;

	// Token: 0x04003C76 RID: 15478
	private RankedProgressionManager.RankedProgressionEvent ProgressionEvent;

	// Token: 0x04003C77 RID: 15479
	public Action<int, float, int> OnPlayerEloAcquired;

	// Token: 0x04003C7A RID: 15482
	[Space]
	[ContextMenuItem("Set ELO", "DebugSetELO")]
	public int debugEloPoints = 100;

	// Token: 0x0200078D RID: 1933
	public enum ERankedMatchmakingTier
	{
		// Token: 0x04003C7C RID: 15484
		Low,
		// Token: 0x04003C7D RID: 15485
		Medium,
		// Token: 0x04003C7E RID: 15486
		High
	}

	// Token: 0x0200078E RID: 1934
	public enum ERankedProgressionEventType
	{
		// Token: 0x04003C80 RID: 15488
		None,
		// Token: 0x04003C81 RID: 15489
		Progress,
		// Token: 0x04003C82 RID: 15490
		Promotion,
		// Token: 0x04003C83 RID: 15491
		Relegation
	}

	// Token: 0x0200078F RID: 1935
	public class RankedProgressionEvent
	{
		// Token: 0x060030C6 RID: 12486 RVA: 0x000FF4F4 File Offset: 0x000FD6F4
		public override string ToString()
		{
			string text = "Progression Info\n";
			text += string.Format("Event Type: {0}\n", this.evtType.ToString());
			text += string.Format("Left Tier: {0}\n", this.leftName);
			text += string.Format("Right Tier: {0}\n", this.rightName);
			text += string.Format("Left Value: {0}\n", this.minVal.ToString("N0"));
			text += string.Format("Right Value: {0}\n", this.maxVal.ToString("N0"));
			text += string.Format("Elo Delta: {0}\n", this.delta.ToString("N0"));
			if (this.evtType == RankedProgressionManager.ERankedProgressionEventType.Promotion || this.evtType == RankedProgressionManager.ERankedProgressionEventType.Relegation)
			{
				text += string.Format("Fanfare Tier: {0}\n", this.newTierName);
			}
			return text;
		}

		// Token: 0x04003C84 RID: 15492
		public RankedProgressionManager.ERankedProgressionEventType evtType;

		// Token: 0x04003C85 RID: 15493
		public Sprite progressIconLeft;

		// Token: 0x04003C86 RID: 15494
		public Sprite progressIconRight;

		// Token: 0x04003C87 RID: 15495
		public Sprite newTierIcon;

		// Token: 0x04003C88 RID: 15496
		public string leftName;

		// Token: 0x04003C89 RID: 15497
		public string rightName;

		// Token: 0x04003C8A RID: 15498
		public string newTierName;

		// Token: 0x04003C8B RID: 15499
		public float minVal;

		// Token: 0x04003C8C RID: 15500
		public float maxVal;

		// Token: 0x04003C8D RID: 15501
		public float delta;
	}

	// Token: 0x02000790 RID: 1936
	public abstract class RankedProgressionTierBase
	{
		// Token: 0x060030C8 RID: 12488 RVA: 0x000FF5E4 File Offset: 0x000FD7E4
		public void SetMinThreshold(float val)
		{
			this.thresholdMin = val;
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x000FF5ED File Offset: 0x000FD7ED
		public float GetMinThreshold()
		{
			if (this.thresholdMin < 0f)
			{
				GTDev.LogError<string>("Tier min threshold not initialized. Can only be used at runtime.", null);
			}
			return this.thresholdMin;
		}

		// Token: 0x04003C8E RID: 15502
		public string name;

		// Token: 0x04003C8F RID: 15503
		public Color color = Color.white;

		// Token: 0x04003C90 RID: 15504
		public float thresholdMax;

		// Token: 0x04003C91 RID: 15505
		private float thresholdMin = -1f;
	}

	// Token: 0x02000791 RID: 1937
	[Serializable]
	public class RankedProgressionSubTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x04003C92 RID: 15506
		public Sprite icon;
	}

	// Token: 0x02000792 RID: 1938
	[Serializable]
	public class RankedProgressionTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x060030CC RID: 12492 RVA: 0x000FF634 File Offset: 0x000FD834
		public void InsertSubTierAt(int idx, float tierMin)
		{
			RankedProgressionManager.RankedProgressionSubTier item = new RankedProgressionManager.RankedProgressionSubTier
			{
				name = "NewTier"
			};
			this.subTiers.Insert(idx, item);
			this.EnforceSubTierValidity(tierMin);
		}

		// Token: 0x060030CD RID: 12493 RVA: 0x000FF668 File Offset: 0x000FD868
		public void EnforceSubTierValidity(float thresholdMin)
		{
			float num = (((this.thresholdMax == 0f) ? 4000f : this.thresholdMax) - thresholdMin) / (float)this.subTiers.Count;
			for (int i = 0; i < this.subTiers.Count - 1; i++)
			{
				float num2 = thresholdMin + (float)(i + 1) * num;
				num2 = Mathf.Round(num2 / 10f);
				this.subTiers[i].thresholdMax = num2 * 10f;
			}
		}

		// Token: 0x04003C93 RID: 15507
		public List<RankedProgressionManager.RankedProgressionSubTier> subTiers = new List<RankedProgressionManager.RankedProgressionSubTier>();
	}
}
