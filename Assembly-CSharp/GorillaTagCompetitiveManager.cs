using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class GorillaTagCompetitiveManager : GorillaTagManager
{
	// Token: 0x06002CBF RID: 11455 RVA: 0x000EC988 File Offset: 0x000EAB88
	public float GetRoundDuration()
	{
		return this.roundDuration;
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x000EC990 File Offset: 0x000EAB90
	public GorillaTagCompetitiveManager.GameState GetCurrentGameState()
	{
		return this.gameState;
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x000EC998 File Offset: 0x000EAB98
	public bool IsMatchActive()
	{
		return this.gameState == GorillaTagCompetitiveManager.GameState.Playing;
	}

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06002CC2 RID: 11458 RVA: 0x000EC9A4 File Offset: 0x000EABA4
	// (remove) Token: 0x06002CC3 RID: 11459 RVA: 0x000EC9D8 File Offset: 0x000EABD8
	public static event Action<GorillaTagCompetitiveManager.GameState> onStateChanged;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x06002CC4 RID: 11460 RVA: 0x000ECA0C File Offset: 0x000EAC0C
	// (remove) Token: 0x06002CC5 RID: 11461 RVA: 0x000ECA40 File Offset: 0x000EAC40
	public static event Action<float> onUpdateRemainingTime;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x06002CC6 RID: 11462 RVA: 0x000ECA74 File Offset: 0x000EAC74
	// (remove) Token: 0x06002CC7 RID: 11463 RVA: 0x000ECAA8 File Offset: 0x000EACA8
	public static event Action<NetPlayer> onPlayerJoined;

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x06002CC8 RID: 11464 RVA: 0x000ECADC File Offset: 0x000EACDC
	// (remove) Token: 0x06002CC9 RID: 11465 RVA: 0x000ECB10 File Offset: 0x000EAD10
	public static event Action<NetPlayer> onPlayerLeft;

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06002CCA RID: 11466 RVA: 0x000ECB44 File Offset: 0x000EAD44
	// (remove) Token: 0x06002CCB RID: 11467 RVA: 0x000ECB78 File Offset: 0x000EAD78
	public static event Action onRoundStart;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06002CCC RID: 11468 RVA: 0x000ECBAC File Offset: 0x000EADAC
	// (remove) Token: 0x06002CCD RID: 11469 RVA: 0x000ECBE0 File Offset: 0x000EADE0
	public static event Action onRoundEnd;

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06002CCE RID: 11470 RVA: 0x000ECC14 File Offset: 0x000EAE14
	// (remove) Token: 0x06002CCF RID: 11471 RVA: 0x000ECC48 File Offset: 0x000EAE48
	public static event Action<NetPlayer, NetPlayer> onTagOccurred;

	// Token: 0x06002CD0 RID: 11472 RVA: 0x000ECC7B File Offset: 0x000EAE7B
	public static void RegisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Add(scoreboard);
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000ECC88 File Offset: 0x000EAE88
	public static void DeregisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Remove(scoreboard);
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000ECC98 File Offset: 0x000EAE98
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.scoring = base.GetComponentInChildren<RankedMultiplayerScore>();
		if (this.scoring != null)
		{
			this.scoring.Initialize();
		}
		VRRig.LocalRig.EnableRankedTimerWatch(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableRankedTimerWatch(true);
			}
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000ECD10 File Offset: 0x000EAF10
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableRankedTimerWatch(false);
		if (this.scoring != null)
		{
			this.scoring.ResetMatch();
			this.scoring.Unsubscribe();
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, null, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000ECDAB File Offset: 0x000EAFAB
	public override void Reset()
	{
		base.Reset();
		this.gameState = GorillaTagCompetitiveManager.GameState.None;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000ECDBA File Offset: 0x000EAFBA
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GorillaTagCompetitiveRPCs>();
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000ECDCC File Offset: 0x000EAFCC
	public override void Tick()
	{
		if (this.stateRemainingTime > 0f)
		{
			this.stateRemainingTime -= Time.deltaTime;
			if (this.stateRemainingTime <= 0f)
			{
				this.UpdateState();
			}
			Action<float> action = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action != null)
			{
				action(this.stateRemainingTime);
			}
		}
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > this.waitingForPlayerPingRoomDuration)
			{
				this.PingRoom();
				this.lastWaitingForPlayerPingRoomTime = Time.time;
			}
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > 3f)
			{
				this.ShowDebugPing = false;
			}
		}
		this.UpdateScoreboards();
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000ECE78 File Offset: 0x000EB078
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.PingRoom();
			this.lastWaitingForPlayerPingRoomTime = Time.time;
		}
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x000ECEA0 File Offset: 0x000EB0A0
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			using (List<GorillaTagCompetitiveForcedLeaveRoomVolume>.Enumerator enumerator = this.forceLeaveRoomVolumes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ContainsPoint(VRRig.LocalRig.transform.position))
					{
						NetworkSystem.Instance.ReturnToSinglePlayer();
						return;
					}
				}
			}
			object obj;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaTagCompetitiveServerApi.Instance.RequestCreateMatchId(delegate(string id)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("matchId", id);
					PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				});
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
			{
				GorillaTagCompetitiveServerApi.Instance.RequestValidateMatchJoin((string)obj, delegate(bool valid)
				{
					if (!valid)
					{
						Debug.LogError("ValidateMatchJoin failed. Leaving room!");
						NetworkSystem.Instance.ReturnToSinglePlayer();
					}
				});
			}
		}
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerJoined;
		if (action != null)
		{
			action(newPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(true);
		}
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x000ECFD4 File Offset: 0x000EB1D4
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerLeft;
		if (action != null)
		{
			action(otherPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(otherPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(false);
		}
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000ED014 File Offset: 0x000EB214
	public RankedMultiplayerScore GetScoring()
	{
		return this.scoring;
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000ED01C File Offset: 0x000EB21C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer) && this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound;
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x000ED03F File Offset: 0x000EB23F
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		base.ReportTag(taggedPlayer, taggingPlayer);
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x000ED049 File Offset: 0x000EB249
	public override GameModeType GameType()
	{
		return GameModeType.InfectionCompetitive;
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000ED04D File Offset: 0x000EB24D
	public override string GameModeName()
	{
		return "COMP-INFECT";
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x00002076 File Offset: 0x00000276
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return false;
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x000ED054 File Offset: 0x000EB254
	public override void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing && this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
		}
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000ED07C File Offset: 0x000EB27C
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (!this.currentInfected.Contains(taggingPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			VRRig rig = rigContainer2.Rig;
			VRRig rig2 = rigContainer.Rig;
			if (!rig.IsPositionInRange(rig2.transform.position, 6f) && !rig.CheckTagDistanceRollback(rig2, 6f, 0.2f))
			{
				return;
			}
			if (!NetworkSystem.Instance.IsMasterClient && this.gameState == GorillaTagCompetitiveManager.GameState.Playing && !this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.currentInfected.Add(taggedPlayer);
			}
			Action<NetPlayer, NetPlayer> action = GorillaTagCompetitiveManager.onTagOccurred;
			if (action == null)
			{
				return;
			}
			action(taggedPlayer, taggingPlayer);
		}
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x000ED140 File Offset: 0x000EB340
	private void SetState(GorillaTagCompetitiveManager.GameState newState)
	{
		if (newState != this.gameState)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.gameState;
			this.gameState = newState;
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.EnterStateWaitingForPlayers();
				break;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.EnterStateStartingCountdown();
				break;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.EnterStatePlaying();
				break;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.EnterStatePostRound();
				break;
			}
			Action<GorillaTagCompetitiveManager.GameState> action = GorillaTagCompetitiveManager.onStateChanged;
			if (action != null)
			{
				action(this.gameState);
			}
			Action<float> action2 = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action2 != null)
			{
				action2(this.stateRemainingTime);
			}
			if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action3 = GorillaTagCompetitiveManager.onRoundStart;
				if (action3 != null)
				{
					action3();
				}
			}
			else if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action4 = GorillaTagCompetitiveManager.onRoundEnd;
				if (action4 != null)
				{
					action4();
				}
			}
			GTDev.Log<string>(string.Format("!! Competitive SetState: {0} at: {1}", this.gameState, Time.time), null);
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000ED226 File Offset: 0x000EB426
	private void EnterStateWaitingForPlayers()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			base.SetisCurrentlyTag(true);
			base.ClearInfectionState();
		}
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x000ED244 File Offset: 0x000EB444
	private void EnterStateStartingCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			base.ClearInfectionState();
			GameMode.RefreshPlayers();
			this.CheckForInfected();
			this.stateRemainingTime = this.startCountdownDuration;
		}
	}

	// Token: 0x06002CE5 RID: 11493 RVA: 0x000ED290 File Offset: 0x000EB490
	private void EnterStatePlaying()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.roundDuration;
			this.PingRoom();
		}
		this.DisplayScoreboardPredictedResults(false);
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x000ED2CD File Offset: 0x000EB4CD
	private void EnterStatePostRound()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.postRoundDuration;
		}
		this.DisplayScoreboardPredictedResults(true);
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x000ED304 File Offset: 0x000EB504
	public override void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.None:
				this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
				return;
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.UpdateStateWaitingForPlayers();
				return;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.UpdateStateStartingCountdown();
				return;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.UpdateStatePlaying();
				return;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.UpdateStatePostRound();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x000ED364 File Offset: 0x000EB564
	private void UpdateStateWaitingForPlayers()
	{
		if (this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
			return;
		}
		if (this.isCurrentlyTag && this.currentIt == null)
		{
			int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
		}
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x000ED3B4 File Offset: 0x000EB5B4
	private void UpdateStateStartingCountdown()
	{
		if (!this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.Playing);
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x000ED3E1 File Offset: 0x000EB5E1
	private void UpdateStatePlaying()
	{
		if (this.IsGameInvalid())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		if (this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x000ED41C File Offset: 0x000EB61C
	private void HandleInfectionRoundComplete()
	{
		foreach (NetPlayer player in GameMode.ParticipatingPlayers)
		{
			RoomSystem.SendSoundEffectToPlayer(2, 0.25f, player, true);
		}
		PlayerGameEvents.GameModeCompleteRound();
		GameMode.BroadcastRoundComplete();
		this.lastTaggedActorNr.Clear();
		this.waitingToStartNextInfectionGame = true;
		this.timeInfectedGameEnded = (double)Time.time;
		this.SetState(GorillaTagCompetitiveManager.GameState.PostRound);
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x000ED4A4 File Offset: 0x000EB6A4
	private void UpdateStatePostRound()
	{
		if (this.stateRemainingTime < 0f)
		{
			if (this.IsInfectionPossible())
			{
				this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
				return;
			}
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		}
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000ED4CC File Offset: 0x000EB6CC
	private void PingRoom()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			GorillaTagCompetitiveServerApi.Instance.RequestPingRoom((string)obj, delegate
			{
				this.ShowDebugPing = true;
			});
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002CEE RID: 11502 RVA: 0x000ED50D File Offset: 0x000EB70D
	// (set) Token: 0x06002CEF RID: 11503 RVA: 0x000ED515 File Offset: 0x000EB715
	public bool ShowDebugPing { get; set; }

	// Token: 0x06002CF0 RID: 11504 RVA: 0x000ED51E File Offset: 0x000EB71E
	private bool IsGameInvalid()
	{
		return GameMode.ParticipatingPlayers.Count <= 1;
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000ED530 File Offset: 0x000EB730
	private bool IsInfectionPossible()
	{
		return GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold;
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x000ED548 File Offset: 0x000EB748
	private bool IsEveryoneTagged()
	{
		bool result = true;
		foreach (NetPlayer item in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(item))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x000ED5A8 File Offset: 0x000EB7A8
	private void CheckForInfected()
	{
		if (this.currentInfected.Count == 0)
		{
			int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index], true);
		}
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x000ED5E5 File Offset: 0x000EB7E5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
		stream.SendNext(this.stateRemainingTime);
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x000ED614 File Offset: 0x000EB814
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		base.OnSerializeRead(stream, info);
		GorillaTagCompetitiveManager.GameState state = (GorillaTagCompetitiveManager.GameState)stream.ReceiveNext();
		this.stateRemainingTime = (float)stream.ReceiveNext();
		this.SetState(state);
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x000ED660 File Offset: 0x000EB860
	public void UpdateScoreboards()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = this.scoring.GetSortedScores();
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
		{
			this.lastActiveTime = Time.time;
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, sortedScores, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x000ED6E8 File Offset: 0x000EB8E8
	public void DisplayScoreboardPredictedResults(bool bShow)
	{
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x06002CF8 RID: 11512 RVA: 0x000ED71B File Offset: 0x000EB91B
	public void RegisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		if (!this.forceLeaveRoomVolumes.Contains(volume))
		{
			this.forceLeaveRoomVolumes.Add(volume);
		}
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x000ED737 File Offset: 0x000EB937
	public void UnregisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		this.forceLeaveRoomVolumes.Remove(volume);
	}

	// Token: 0x04003833 RID: 14387
	[SerializeField]
	private float startCountdownDuration = 3f;

	// Token: 0x04003834 RID: 14388
	[SerializeField]
	private float roundDuration = 300f;

	// Token: 0x04003835 RID: 14389
	[SerializeField]
	private float postRoundDuration = 15f;

	// Token: 0x04003836 RID: 14390
	[SerializeField]
	private float waitingForPlayerPingRoomDuration = 60f;

	// Token: 0x04003837 RID: 14391
	private GorillaTagCompetitiveManager.GameState gameState;

	// Token: 0x04003838 RID: 14392
	private float stateRemainingTime;

	// Token: 0x04003839 RID: 14393
	private float lastActiveTime;

	// Token: 0x0400383A RID: 14394
	private float lastWaitingForPlayerPingRoomTime;

	// Token: 0x04003842 RID: 14402
	private RankedMultiplayerScore scoring;

	// Token: 0x04003843 RID: 14403
	private List<GorillaTagCompetitiveForcedLeaveRoomVolume> forceLeaveRoomVolumes = new List<GorillaTagCompetitiveForcedLeaveRoomVolume>();

	// Token: 0x04003844 RID: 14404
	private static List<GorillaTagCompetitiveScoreboard> scoreboards = new List<GorillaTagCompetitiveScoreboard>();

	// Token: 0x02000700 RID: 1792
	public enum GameState
	{
		// Token: 0x04003847 RID: 14407
		None,
		// Token: 0x04003848 RID: 14408
		WaitingForPlayers,
		// Token: 0x04003849 RID: 14409
		StartingCountdown,
		// Token: 0x0400384A RID: 14410
		Playing,
		// Token: 0x0400384B RID: 14411
		PostRound
	}
}
