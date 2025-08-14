using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000502 RID: 1282
[NetworkBehaviourWeaved(0)]
public class MonkeBallGame : NetworkComponent
{
	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001F26 RID: 7974 RVA: 0x000A4A2A File Offset: 0x000A2C2A
	public Transform BallLauncher
	{
		get
		{
			return this._ballLauncher;
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000A4A34 File Offset: 0x000A2C34
	protected override void Awake()
	{
		base.Awake();
		MonkeBallGame.Instance = this;
		this.gameState = MonkeBallGame.GameState.None;
		this._callLimiters = new CallLimiter[8];
		this._callLimiters[0] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[1] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[2] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[3] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[4] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[5] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[6] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[7] = new CallLimiter(5, 10f, 0.5f);
		this.AssignNetworkListeners();
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000A4B30 File Offset: 0x000A2D30
	private bool ValidateCallLimits(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info)
	{
		if (rpcCall < MonkeBallGame.RPC.SetGameState || rpcCall >= MonkeBallGame.RPC.Count)
		{
			return false;
		}
		bool flag = this._callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		if (!flag)
		{
			this.ReportRPCCall(rpcCall, info, "Too many RPC Calls!");
		}
		return flag;
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000A4B6B File Offset: 0x000A2D6B
	private void ReportRPCCall(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info, string susReason)
	{
		GorillaNot.instance.SendReport(string.Format("Reason: {0}   RPC: {1}", susReason, rpcCall), info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000A4BA0 File Offset: 0x000A2DA0
	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			GameBallManager.Instance.AddGameBall(this.startingBalls[i].gameBall);
		}
		for (int j = 0; j < this.scoreboards.Count; j++)
		{
			this.scoreboards[j].Setup(this);
		}
		this.gameEndTime = -1.0;
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000A4C1E File Offset: 0x000A2E1E
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		this.UnassignNetworkListeners();
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000A4C30 File Offset: 0x000A2E30
	public void OnPlayerDestroy()
	{
		if (this._setStoredLocalPlayerColor)
		{
			PlayerPrefs.SetFloat("redValue", this._storedLocalPlayerColor.r);
			PlayerPrefs.SetFloat("greenValue", this._storedLocalPlayerColor.g);
			PlayerPrefs.SetFloat("blueValue", this._storedLocalPlayerColor.b);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000A4C8C File Offset: 0x000A2E8C
	private void AssignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerJoined;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeft;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000A4CFC File Offset: 0x000A2EFC
	private void UnassignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerJoined;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeft;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000A4D6C File Offset: 0x000A2F6C
	private void Update()
	{
		if (this.IsMasterClient() && this.gameState != MonkeBallGame.GameState.None && this.gameEndTime >= 0.0 && PhotonNetwork.Time > this.gameEndTime)
		{
			this.gameEndTime = -1.0;
			this.RequestGameState(MonkeBallGame.GameState.PostGame);
		}
		this.RefreshTime();
		if (this._forceSync)
		{
			this._forceSyncDelay -= Time.deltaTime;
			if (this._forceSyncDelay <= 0f)
			{
				this._forceSync = false;
				this.ForceSyncPlayersVisuals();
				this.RefreshTeamPlayers(false);
			}
		}
		if (this._forceOrigColorFix)
		{
			this._forceOrigColorDelay -= Time.deltaTime;
			if (this._forceOrigColorDelay <= 0f)
			{
				this._forceOrigColorFix = false;
				this.ForceOriginalColorSync();
			}
		}
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x000A4E34 File Offset: 0x000A3034
	private void OnPlayerJoined(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		if (!this.IsMasterClient())
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", player.GetPlayerRef(), new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000A4EBC File Offset: 0x000A30BC
	private void OnPlayerLeft(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.CleanupPlayer();
		}
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetTeamRPC", RpcTarget.All, new object[]
		{
			-1,
			player.GetPlayerRef()
		});
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000A4F28 File Offset: 0x000A3128
	private void OnMasterClientSwitched(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", RpcTarget.Others, new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000A4F9C File Offset: 0x000A319C
	private void GetCurrentGameState(out int[] playerIds, out int[] playerTeams, out int[] scores, out long[] packedBallPosRot, out long[] packedBallVel)
	{
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		playerIds = new int[allNetPlayers.Length];
		playerTeams = new int[allNetPlayers.Length];
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			playerIds[i] = allNetPlayers[i].ActorNumber;
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[i].ActorNumber);
			if (gamePlayer != null)
			{
				playerTeams[i] = gamePlayer.teamId;
			}
			else
			{
				playerTeams[i] = -1;
			}
		}
		scores = new int[this.team.Count];
		for (int j = 0; j < this.team.Count; j++)
		{
			scores[j] = this.team[j].score;
		}
		packedBallPosRot = new long[this.startingBalls.Count];
		packedBallVel = new long[this.startingBalls.Count];
		for (int k = 0; k < this.startingBalls.Count; k++)
		{
			packedBallPosRot[k] = BitPackUtils.PackHandPosRotForNetwork(this.startingBalls[k].transform.position, this.startingBalls[k].transform.rotation);
			packedBallVel[k] = BitPackUtils.PackWorldPosForNetwork(this.startingBalls[k].gameBall.GetVelocity());
		}
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000638F3 File Offset: 0x00061AF3
	private bool IsMasterClient()
	{
		return PhotonNetwork.IsMasterClient;
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000A50E2 File Offset: 0x000A32E2
	public MonkeBallGame.GameState GetGameState()
	{
		return this.gameState;
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000A50EA File Offset: 0x000A32EA
	public void RequestGameState(MonkeBallGame.GameState newGameState)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetGameStateRPC", RpcTarget.All, new object[]
		{
			(int)newGameState
		});
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000A5118 File Offset: 0x000A3318
	[PunRPC]
	private void SetGameStateRPC(int newGameState, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetGameState, info))
		{
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetGameState, info, "newGameState outside of enum range.");
			return;
		}
		this.SetGameState((MonkeBallGame.GameState)newGameState);
		if (newGameState == 1)
		{
			this.gameEndTime = info.SentServerTime + (double)this.gameDuration;
		}
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000A5180 File Offset: 0x000A3380
	private void SetGameState(MonkeBallGame.GameState newGameState)
	{
		this.gameState = newGameState;
		switch (this.gameState)
		{
		case MonkeBallGame.GameState.PreGame:
			this.OnEnterStatePreGame();
			return;
		case MonkeBallGame.GameState.Playing:
			this.OnEnterStatePlaying();
			return;
		case MonkeBallGame.GameState.PostScore:
			this.OnEnterStatePostScore();
			return;
		case MonkeBallGame.GameState.PostGame:
			this.OnEnterStatePostGame();
			return;
		default:
			return;
		}
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000A51D0 File Offset: 0x000A33D0
	private void OnEnterStatePreGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameStartFx();
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000A5204 File Offset: 0x000A3404
	private void OnEnterStatePlaying()
	{
		this._forceSync = true;
		this._forceSyncDelay = 0.1f;
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnEnterStatePostScore()
	{
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000A5218 File Offset: 0x000A3418
	private void OnEnterStatePostGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameEndFx();
		}
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000A524C File Offset: 0x000A344C
	[PunRPC]
	private void RequestSetGameStateRPC(int newGameState, double newGameEndTime, int[] playerIds, int[] playerTeams, int[] scores, long[] packedBallPosRot, long[] packedBallVel, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "RequestSetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetGameState, info))
		{
			return;
		}
		if (playerIds.IsNullOrEmpty<int>() || playerTeams.IsNullOrEmpty<int>() || scores.IsNullOrEmpty<int>() || packedBallPosRot.IsNullOrEmpty<long>() || packedBallVel.IsNullOrEmpty<long>())
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "Array params are null or empty.");
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameState outside of enum range.");
			return;
		}
		if (playerIds.Length != playerTeams.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "playerIDs and playerTeams are not the same length.");
			return;
		}
		if (scores.Length > this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "scores and team are not the same length.");
			return;
		}
		if (packedBallPosRot.Length != this.startingBalls.Count || packedBallPosRot.Length != packedBallVel.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "packedBall arrays are not the same length.");
			return;
		}
		if (double.IsNaN(newGameEndTime) || double.IsInfinity(newGameEndTime))
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime is not valid.");
			return;
		}
		if (newGameEndTime < -1.0 || newGameEndTime > PhotonNetwork.Time + (double)this.gameDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime exceeds possible time limits.");
			return;
		}
		this.gameState = (MonkeBallGame.GameState)newGameState;
		this.gameEndTime = newGameEndTime;
		for (int i = 0; i < playerIds.Length; i++)
		{
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != playerIds[i])
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(playerIds[i]);
				if (!(gamePlayer == null))
				{
					gamePlayer.teamId = playerTeams[i];
					RigContainer rigContainer;
					if (playerTeams[i] >= 0 && playerTeams[i] < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerIds[i]), out rigContainer))
					{
						Color color = this.team[playerTeams[i]].color;
						rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
						rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet);
					}
				}
			}
		}
		this.RefreshTeamPlayers(false);
		for (int j = 0; j < scores.Length; j++)
		{
			this.SetScore(j, scores[j], false);
		}
		for (int k = 0; k < packedBallPosRot.Length; k++)
		{
			Vector3 position;
			Quaternion rotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedBallPosRot[k], out position, out rotation);
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				this.startingBalls[k].transform.position = position;
				this.startingBalls[k].transform.rotation = rotation;
				if ((this.startingBalls[k].transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					this.startingBalls[k].transform.position = this._neutralBallStartLocation.transform.position;
				}
				Vector3 velocity = BitPackUtils.UnpackWorldPosFromNetwork(packedBallVel[k]);
				num = 10000f;
				if (velocity.IsValid(num))
				{
					this.startingBalls[k].gameBall.SetVelocity(velocity);
					this.startingBalls[k].TriggerDelayedResync();
				}
			}
		}
		this._forceSync = true;
		this._forceSyncDelay = 5f;
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000A55B5 File Offset: 0x000A37B5
	public void RequestResetGame()
	{
		this.photonView.RPC("RequestResetGameRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x000A55D0 File Offset: 0x000A37D0
	[PunRPC]
	private void RequestResetGameRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestResetGameRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestResetGame, info))
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(info.Sender.ActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		if (gamePlayer.teamId != this.resetButton.allowedTeamId)
		{
			return;
		}
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			this.RequestResetBall(this.startingBalls[i].gameBall.id, -1);
		}
		for (int j = 0; j < this.team.Count; j++)
		{
			this.RequestSetScore(j, 0);
		}
		this.RequestGameState(MonkeBallGame.GameState.PreGame);
		this.resetButton.ToggleReset(false, -1, true);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(false, -1, true);
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000A56B0 File Offset: 0x000A38B0
	public void ToggleResetButton(bool toggle, int teamId)
	{
		int otherTeam = this.GetOtherTeam(teamId);
		this.photonView.RPC("SetResetButtonRPC", RpcTarget.All, new object[]
		{
			toggle,
			otherTeam
		});
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x000A56F0 File Offset: 0x000A38F0
	[PunRPC]
	private void SetResetButtonRPC(bool toggleReset, int teamId, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetResetButtonRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetResetButton, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetResetButton, info, "teamID exceeds possible range.");
			return;
		}
		this.resetButton.ToggleReset(toggleReset, teamId, false);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(toggleReset, teamId, false);
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000A576B File Offset: 0x000A396B
	public void OnBallGrabbed(GameBallId gameBallId)
	{
		if (this.gameState == MonkeBallGame.GameState.PreGame)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
		if (this.gameState == MonkeBallGame.GameState.PostScore)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000A5790 File Offset: 0x000A3990
	private void RefreshTime()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshTime();
		}
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000A57C4 File Offset: 0x000A39C4
	public void RequestResetBall(GameBallId gameBallId, int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId >= 0)
		{
			this.LaunchBallWithTeam(gameBallId, teamId, this.team[teamId].ballLaunchPosition, this.team[teamId].ballLaunchVelocityRange, this.team[teamId].ballLaunchAngleXRange, this.team[teamId].ballLaunchAngleXRange);
			return;
		}
		this.LaunchBallNeutral(gameBallId);
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000A5834 File Offset: 0x000A3A34
	public void RequestScore(int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", RpcTarget.All, new object[]
		{
			teamId,
			this.team[teamId].score + 1
		});
		this.RequestGameState(MonkeBallGame.GameState.PostScore);
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000A589F File Offset: 0x000A3A9F
	public void RequestSetScore(int teamId, int score)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", RpcTarget.All, new object[]
		{
			teamId,
			score
		});
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000A58D4 File Offset: 0x000A3AD4
	[PunRPC]
	private void SetScoreRPC(int teamId, int score, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetScoreRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetScore, info))
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "teamID exceeds possible range.");
			return;
		}
		if (score != 0 && score != this.team[teamId].score + 1)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "Score is being set to a non-achievable value.");
			return;
		}
		this.SetScore(teamId, Mathf.Clamp(score, 0, 999), true);
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000A5960 File Offset: 0x000A3B60
	private void SetScore(int teamId, int score, bool playFX = true)
	{
		if (teamId < 0 || teamId > this.team.Count)
		{
			return;
		}
		int score2 = this.team[teamId].score;
		this.team[teamId].score = score;
		if (playFX && score > score2)
		{
			this.PlayScoreFx();
			Color color = this.team[teamId].color;
			for (int i = 0; i < this.endZoneEffects.Length; i++)
			{
				this.endZoneEffects[i].startColor = color;
				this.endZoneEffects[i].Play();
			}
		}
		this.RefreshScore();
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000A59F8 File Offset: 0x000A3BF8
	private void RefreshScore()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshScore();
		}
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000A5A2C File Offset: 0x000A3C2C
	private void PlayScoreFx()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayScoreFx();
		}
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000A5A60 File Offset: 0x000A3C60
	public MonkeBallTeam GetTeam(int teamId)
	{
		return this.team[teamId];
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000A5A6E File Offset: 0x000A3C6E
	public int GetOtherTeam(int teamId)
	{
		return (teamId + 1) % this.team.Count;
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000A5A80 File Offset: 0x000A3C80
	public void RequestSetTeam(int teamId)
	{
		this.photonView.RPC("RequestSetTeamRPC", RpcTarget.MasterClient, new object[]
		{
			teamId
		});
		bool flag = false;
		Color color = Color.white;
		if (teamId >= 0 && teamId < this.team.Count)
		{
			flag = true;
			if (!this._setStoredLocalPlayerColor)
			{
				this._storedLocalPlayerColor = new Color(PlayerPrefs.GetFloat("redValue", 1f), PlayerPrefs.GetFloat("greenValue", 1f), PlayerPrefs.GetFloat("blueValue", 1f));
				this._setStoredLocalPlayerColor = true;
			}
			this._forceOrigColorFix = false;
			color = this.team[teamId].color;
		}
		else
		{
			color = this._storedLocalPlayerColor;
			this._setStoredLocalPlayerColor = false;
		}
		PlayerPrefs.SetFloat("redValue", color.r);
		PlayerPrefs.SetFloat("greenValue", color.g);
		PlayerPrefs.SetFloat("blueValue", color.b);
		PlayerPrefs.Save();
		GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
		GorillaComputer.instance.UpdateColor(color.r, color.g, color.b);
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				color.r,
				color.g,
				color.b
			});
			if (flag)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this._forceOrigColorFix = true;
				this._forceOrigColorDelay = 3f;
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			this.ForceSyncPlayersVisuals();
		}
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000A5C40 File Offset: 0x000A3E40
	private MonkeBall GetMonkeBall(GameBallId gameBallId)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (!(gameBall == null))
		{
			return gameBall.GetComponent<MonkeBall>();
		}
		return null;
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000A5C6C File Offset: 0x000A3E6C
	[PunRPC]
	private void RequestSetTeamRPC(int teamId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestSetTeamRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.photonView.RPC("SetTeamRPC", RpcTarget.All, new object[]
		{
			teamId,
			info.Sender
		});
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000A5CE4 File Offset: 0x000A3EE4
	[PunRPC]
	private void SetTeamRPC(int teamId, Player player, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetTeamRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.SetTeamPlayer(teamId, player);
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000A5D40 File Offset: 0x000A3F40
	private void SetTeamPlayer(int teamId, Player player)
	{
		if (player == null)
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.teamId = teamId;
		}
		this.RefreshTeamPlayers(true);
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000A5D74 File Offset: 0x000A3F74
	private void RefreshTeamPlayers(bool playSounds)
	{
		int[] array = new int[this.team.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
		int num = 0;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int j = 0; j < allNetPlayers.Length; j++)
		{
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[j].ActorNumber);
			if (!(gamePlayer == null))
			{
				int teamId = gamePlayer.teamId;
				if (teamId >= 0)
				{
					array[teamId]++;
					num++;
				}
			}
		}
		for (int k = 0; k < this.scoreboards.Count; k++)
		{
			for (int l = 0; l < array.Length; l++)
			{
				this.scoreboards[k].RefreshTeamPlayers(l, array[l]);
			}
			if (playSounds)
			{
				if (this._currentPlayerTotal < num)
				{
					this.scoreboards[k].PlayPlayerJoinFx();
				}
				else if (this._currentPlayerTotal > num)
				{
					this.scoreboards[k].PlayPlayerLeaveFx();
				}
			}
		}
		this._currentPlayerTotal = num;
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000A5E84 File Offset: 0x000A4084
	private void ForceSyncPlayersVisuals()
	{
		for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
		{
			int actorNumber = NetworkSystem.Instance.AllNetPlayers[i].ActorNumber;
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != actorNumber)
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(actorNumber);
				RigContainer rigContainer;
				if (!(gamePlayer == null) && gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
				{
					Color color = this.team[gamePlayer.teamId].color;
					rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
					rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet);
				}
			}
		}
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000A5F70 File Offset: 0x000A4170
	private void ForceOriginalColorSync()
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(VRRigCache.Instance.localRig.Creator.ActorNumber);
		if (gamePlayer == null || (gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count))
		{
			return;
		}
		Color storedLocalPlayerColor = this._storedLocalPlayerColor;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				storedLocalPlayerColor.r,
				storedLocalPlayerColor.g,
				storedLocalPlayerColor.b
			});
		}
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000A6015 File Offset: 0x000A4215
	public void RequestRestrictBallToTeam(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDuration);
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000A6025 File Offset: 0x000A4225
	public void RequestRestrictBallToTeamOnScore(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDurationAfterScore);
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000A6038 File Offset: 0x000A4238
	private void RestrictBallToTeam(GameBallId gameBallId, int teamId, float restrictDuration)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetRestrictBallToTeam", RpcTarget.All, new object[]
		{
			gameBallId.index,
			teamId,
			restrictDuration
		});
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000A6088 File Offset: 0x000A4288
	[PunRPC]
	private void SetRestrictBallToTeam(int gameBallIndex, int teamId, float restrictDuration, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetRestrictBallToTeam");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetRestrictBallToTeam, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.startingBalls.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "gameBallIndex exceeds possible range.");
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "teamID exceeds possible range.");
			return;
		}
		if (float.IsNaN(restrictDuration) || float.IsInfinity(restrictDuration) || restrictDuration < 0f || restrictDuration > this.restrictBallDurationAfterScore + this.restrictBallDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "restrictDuration is not a feasible value.");
			return;
		}
		GameBallId gameBallId = new GameBallId(gameBallIndex);
		MonkeBall monkeBall = this.GetMonkeBall(gameBallId);
		bool flag = false;
		if (monkeBall != null)
		{
			flag = monkeBall.RestrictBallToTeam(teamId, restrictDuration);
		}
		if (flag)
		{
			for (int i = 0; i < this.shotclocks.Count; i++)
			{
				this.shotclocks[i].SetTime(teamId, restrictDuration);
			}
		}
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000A6188 File Offset: 0x000A4388
	public void LaunchBallNeutral(GameBallId gameBallId)
	{
		this.LaunchBall(gameBallId, this._ballLauncher, this.ballLauncherVelocityRange.x, this.ballLauncherVelocityRange.y, this.ballLaunchAngleXRange.x, this.ballLaunchAngleXRange.y, this.ballLaunchAngleYRange.x, this.ballLaunchAngleYRange.y);
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000A61E4 File Offset: 0x000A43E4
	public void LaunchBallWithTeam(GameBallId gameBallId, int teamId, Transform launcher, Vector2 velocityRange, Vector2 angleXRange, Vector2 angleYRange)
	{
		this.LaunchBall(gameBallId, launcher, velocityRange.x, velocityRange.y, angleXRange.x, angleXRange.y, angleYRange.x, angleYRange.y);
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000A6224 File Offset: 0x000A4424
	private void LaunchBall(GameBallId gameBallId, Transform launcher, float minVelocity, float maxVelocity, float minXAngle, float maxXAngle, float minYAngle, float maxYAngle)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (gameBall == null)
		{
			return;
		}
		gameBall.transform.position = launcher.transform.position;
		Quaternion rotation = launcher.transform.rotation;
		launcher.transform.Rotate(Vector3.up, Random.Range(minXAngle, maxXAngle));
		launcher.transform.Rotate(Vector3.right, Random.Range(minYAngle, maxYAngle));
		gameBall.transform.rotation = launcher.transform.rotation;
		Vector3 velocity = launcher.transform.forward * Random.Range(minVelocity, maxVelocity);
		launcher.transform.rotation = rotation;
		GameBallManager.Instance.RequestLaunchBall(gameBallId, velocity);
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040027C7 RID: 10183
	public static MonkeBallGame Instance;

	// Token: 0x040027C8 RID: 10184
	public List<MonkeBall> startingBalls;

	// Token: 0x040027C9 RID: 10185
	public List<MonkeBallScoreboard> scoreboards;

	// Token: 0x040027CA RID: 10186
	public List<MonkeBallShotclock> shotclocks;

	// Token: 0x040027CB RID: 10187
	public List<MonkeBallGoalZone> goalZones;

	// Token: 0x040027CC RID: 10188
	[Space]
	public MonkeBallResetGame resetButton;

	// Token: 0x040027CD RID: 10189
	public MonkeBallResetGame centerResetButton;

	// Token: 0x040027CE RID: 10190
	[Space]
	public PhotonView photonView;

	// Token: 0x040027CF RID: 10191
	public List<MonkeBallTeam> team;

	// Token: 0x040027D0 RID: 10192
	private int _currentPlayerTotal;

	// Token: 0x040027D1 RID: 10193
	[Space]
	[Tooltip("The length of the game in seconds.")]
	public float gameDuration;

	// Token: 0x040027D2 RID: 10194
	[Space]
	[Tooltip("If the ball should be reset to a team starting position after a score. If not set to true then the will reset back to a neutral starting position.")]
	public bool resetBallPositionOnScore = true;

	// Token: 0x040027D3 RID: 10195
	[Tooltip("The duration in which a team is restricted from grabbing the ball after toss.")]
	public float restrictBallDuration = 5f;

	// Token: 0x040027D4 RID: 10196
	[Tooltip("The duration in which a team is restricted from grabbing the ball after a score.")]
	public float restrictBallDurationAfterScore = 10f;

	// Token: 0x040027D5 RID: 10197
	[Header("Neutral Launcher")]
	[SerializeField]
	private Transform _ballLauncher;

	// Token: 0x040027D6 RID: 10198
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLauncherVelocityRange = new Vector2(8f, 15f);

	// Token: 0x040027D7 RID: 10199
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x040027D8 RID: 10200
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);

	// Token: 0x040027D9 RID: 10201
	[Space]
	[SerializeField]
	private Transform _neutralBallStartLocation;

	// Token: 0x040027DA RID: 10202
	[SerializeField]
	private ParticleSystem[] endZoneEffects;

	// Token: 0x040027DB RID: 10203
	private MonkeBallGame.GameState gameState;

	// Token: 0x040027DC RID: 10204
	public double gameEndTime;

	// Token: 0x040027DD RID: 10205
	private bool _forceSync;

	// Token: 0x040027DE RID: 10206
	private float _forceSyncDelay;

	// Token: 0x040027DF RID: 10207
	private bool _forceOrigColorFix;

	// Token: 0x040027E0 RID: 10208
	private float _forceOrigColorDelay;

	// Token: 0x040027E1 RID: 10209
	private Color _storedLocalPlayerColor;

	// Token: 0x040027E2 RID: 10210
	private bool _setStoredLocalPlayerColor;

	// Token: 0x040027E3 RID: 10211
	private CallLimiter[] _callLimiters;

	// Token: 0x02000503 RID: 1283
	public enum GameState
	{
		// Token: 0x040027E5 RID: 10213
		None,
		// Token: 0x040027E6 RID: 10214
		PreGame,
		// Token: 0x040027E7 RID: 10215
		Playing,
		// Token: 0x040027E8 RID: 10216
		PostScore,
		// Token: 0x040027E9 RID: 10217
		PostGame
	}

	// Token: 0x02000504 RID: 1284
	private enum RPC
	{
		// Token: 0x040027EB RID: 10219
		SetGameState,
		// Token: 0x040027EC RID: 10220
		RequestSetGameState,
		// Token: 0x040027ED RID: 10221
		RequestResetGame,
		// Token: 0x040027EE RID: 10222
		SetScore,
		// Token: 0x040027EF RID: 10223
		RequestSetTeam,
		// Token: 0x040027F0 RID: 10224
		SetTeam,
		// Token: 0x040027F1 RID: 10225
		SetRestrictBallToTeam,
		// Token: 0x040027F2 RID: 10226
		SetResetButton,
		// Token: 0x040027F3 RID: 10227
		Count
	}
}
