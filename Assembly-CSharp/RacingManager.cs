using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class RacingManager : NetworkSceneObject, ITickSystemTick
{
	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000CDA RID: 3290 RVA: 0x00044B25 File Offset: 0x00042D25
	// (set) Token: 0x06000CDB RID: 3291 RVA: 0x00044B2C File Offset: 0x00042D2C
	public static RacingManager instance { get; private set; }

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000CDC RID: 3292 RVA: 0x00044B34 File Offset: 0x00042D34
	// (set) Token: 0x06000CDD RID: 3293 RVA: 0x00044B3C File Offset: 0x00042D3C
	public bool TickRunning { get; set; }

	// Token: 0x06000CDE RID: 3294 RVA: 0x00044B48 File Offset: 0x00042D48
	private void Awake()
	{
		RacingManager.instance = this;
		HashSet<int> actorsInAnyRace = new HashSet<int>();
		this.races = new RacingManager.Race[this.raceSetups.Length];
		for (int i = 0; i < this.raceSetups.Length; i++)
		{
			this.races[i] = new RacingManager.Race(i, this.raceSetups[i], actorsInAnyRace, this.photonView);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomJoin);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00044BDE File Offset: 0x00042DDE
	protected override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddTickCallback(this);
		base.OnEnable();
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00044BF2 File Offset: 0x00042DF2
	protected override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
		base.OnDisable();
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00044C08 File Offset: 0x00042E08
	private void OnRoomJoin()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Clear();
		}
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00044C38 File Offset: 0x00042E38
	private void OnPlayerJoined(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].SendStateToNewPlayer(player);
		}
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00044C74 File Offset: 0x00042E74
	public void RegisterVisual(RaceVisual visual)
	{
		int raceId = visual.raceId;
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].RegisterVisual(visual);
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00044CA5 File Offset: 0x00042EA5
	public void Button_StartRace(int raceId, int laps)
	{
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Button_StartRace(laps);
		}
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00044CC4 File Offset: 0x00042EC4
	[PunRPC]
	private void RequestRaceStart_RPC(int raceId, int laps, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestRaceStart_RPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Host_RequestRaceStart(laps, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00044D1C File Offset: 0x00042F1C
	[PunRPC]
	private void RaceBeginCountdown_RPC(byte raceId, byte laps, double startTime, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceBeginCountdown_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (!double.IsFinite(startTime))
		{
			return;
		}
		if (startTime < PhotonNetwork.Time || startTime > PhotonNetwork.Time + 4.0)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].BeginCountdown(startTime, (int)laps);
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00044D94 File Offset: 0x00042F94
	[PunRPC]
	private void RaceLockInParticipants_RPC(byte raceId, int[] participantActorNumbers, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceLockInParticipants_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (participantActorNumbers.Length > 10)
		{
			return;
		}
		for (int i = 1; i < participantActorNumbers.Length; i++)
		{
			if (participantActorNumbers[i] <= participantActorNumbers[i - 1])
			{
				return;
			}
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].LockInParticipants(participantActorNumbers, false);
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00044DF9 File Offset: 0x00042FF9
	public void OnCheckpointPassed(int raceId, int checkpointIndex)
	{
		this.photonView.RPC("PassCheckpoint_RPC", RpcTarget.All, new object[]
		{
			(byte)raceId,
			(byte)checkpointIndex
		});
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00044E26 File Offset: 0x00043026
	[PunRPC]
	private void PassCheckpoint_RPC(byte raceId, byte checkpointIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "PassCheckpoint_RPC");
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].PassCheckpoint(info.Sender, (int)checkpointIndex, info.SentServerTime);
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00044E5D File Offset: 0x0004305D
	[PunRPC]
	private void RaceEnded_RPC(byte raceId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceEnded_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].RaceEnded();
		}
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00044E94 File Offset: 0x00043094
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Tick();
		}
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00044EC4 File Offset: 0x000430C4
	public bool IsActorLockedIntoAnyRace(int actorNumber)
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			if (this.races[i].IsActorLockedIntoRace(actorNumber))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000FCB RID: 4043
	[SerializeField]
	private RacingManager.RaceSetup[] raceSetups;

	// Token: 0x04000FCC RID: 4044
	private const int MinPlayersInRace = 1;

	// Token: 0x04000FCD RID: 4045
	private const float ResultsDuration = 10f;

	// Token: 0x04000FCE RID: 4046
	private RacingManager.Race[] races;

	// Token: 0x02000224 RID: 548
	[Serializable]
	private struct RaceSetup
	{
		// Token: 0x04000FCF RID: 4047
		public BoxCollider startVolume;

		// Token: 0x04000FD0 RID: 4048
		public int numCheckpoints;

		// Token: 0x04000FD1 RID: 4049
		public float dqBaseDuration;

		// Token: 0x04000FD2 RID: 4050
		public float dqInterval;
	}

	// Token: 0x02000225 RID: 549
	private struct RacerData
	{
		// Token: 0x04000FD3 RID: 4051
		public int actorNumber;

		// Token: 0x04000FD4 RID: 4052
		public string playerName;

		// Token: 0x04000FD5 RID: 4053
		public int numCheckpointsPassed;

		// Token: 0x04000FD6 RID: 4054
		public double latestCheckpointTime;

		// Token: 0x04000FD7 RID: 4055
		public bool isDisqualified;
	}

	// Token: 0x02000226 RID: 550
	private class RacerComparer : IComparer<RacingManager.RacerData>
	{
		// Token: 0x06000CEE RID: 3310 RVA: 0x00044F00 File Offset: 0x00043100
		public int Compare(RacingManager.RacerData a, RacingManager.RacerData b)
		{
			int num = a.isDisqualified.CompareTo(b.isDisqualified);
			if (num != 0)
			{
				return num;
			}
			int num2 = a.numCheckpointsPassed.CompareTo(b.numCheckpointsPassed);
			if (num2 != 0)
			{
				return -num2;
			}
			if (a.numCheckpointsPassed > 0)
			{
				return a.latestCheckpointTime.CompareTo(b.latestCheckpointTime);
			}
			return a.actorNumber.CompareTo(b.actorNumber);
		}

		// Token: 0x04000FD8 RID: 4056
		public static RacingManager.RacerComparer instance = new RacingManager.RacerComparer();
	}

	// Token: 0x02000227 RID: 551
	public enum RacingState
	{
		// Token: 0x04000FDA RID: 4058
		Inactive,
		// Token: 0x04000FDB RID: 4059
		Countdown,
		// Token: 0x04000FDC RID: 4060
		InProgress,
		// Token: 0x04000FDD RID: 4061
		Results
	}

	// Token: 0x02000228 RID: 552
	private class Race
	{
		// Token: 0x06000CF1 RID: 3313 RVA: 0x00044F78 File Offset: 0x00043178
		public Race(int raceIndex, RacingManager.RaceSetup setup, HashSet<int> actorsInAnyRace, PhotonView photonView)
		{
			this.raceIndex = raceIndex;
			this.numCheckpoints = setup.numCheckpoints;
			this.raceStartZone = setup.startVolume;
			this.dqBaseDuration = setup.dqBaseDuration;
			this.dqInterval = setup.dqInterval;
			this.photonView = photonView;
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000CF2 RID: 3314 RVA: 0x0004500A File Offset: 0x0004320A
		// (set) Token: 0x06000CF3 RID: 3315 RVA: 0x00045012 File Offset: 0x00043212
		public RacingManager.RacingState racingState { get; private set; }

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0004501B File Offset: 0x0004321B
		public void RegisterVisual(RaceVisual visual)
		{
			this.raceVisual = visual;
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x00045024 File Offset: 0x00043224
		public void Clear()
		{
			this.hasLockedInParticipants = false;
			this.racers.Clear();
			this.playerLookup.Clear();
			this.racingState = RacingManager.RacingState.Inactive;
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0004504C File Offset: 0x0004324C
		public bool IsActorLockedIntoRace(int actorNumber)
		{
			if (this.racingState != RacingManager.RacingState.InProgress || !this.hasLockedInParticipants)
			{
				return false;
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				if (this.racers[i].actorNumber == actorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x0004509C File Offset: 0x0004329C
		public void SendStateToNewPlayer(NetPlayer newPlayer)
		{
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
			case RacingManager.RacingState.Results:
				return;
			case RacingManager.RacingState.Countdown:
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)this.numLapsSelected,
					this.raceStartTime
				});
				return;
			case RacingManager.RacingState.InProgress:
				return;
			default:
				return;
			}
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x0004510C File Offset: 0x0004330C
		public void Tick()
		{
			if (Time.time >= this.nextTickTimestamp)
			{
				this.nextTickTimestamp = Time.time + this.TickWithNextDelay();
			}
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x00045130 File Offset: 0x00043330
		public float TickWithNextDelay()
		{
			bool flag = this.raceVisual != null;
			if (flag)
			{
				this.raceVisual.ActivateStartingWall(this.racingState == RacingManager.RacingState.Countdown);
			}
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
				if (flag)
				{
					this.RefreshStartingPlayerList();
				}
				return 1f;
			case RacingManager.RacingState.Countdown:
				if (this.raceStartTime > PhotonNetwork.Time)
				{
					if (flag)
					{
						this.RefreshStartingPlayerList();
						this.raceVisual.UpdateCountdown(Mathf.CeilToInt((float)(this.raceStartTime - PhotonNetwork.Time)));
					}
				}
				else
				{
					this.RaceCountdownEnds();
				}
				return 0.1f;
			case RacingManager.RacingState.InProgress:
				if (PhotonNetwork.IsMasterClient)
				{
					if (PhotonNetwork.Time > this.abortRaceAtTimestamp)
					{
						this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[]
						{
							(byte)this.raceIndex
						});
					}
					else
					{
						int num = 0;
						for (int i = 0; i < this.racers.Count; i++)
						{
							if (this.racers[i].numCheckpointsPassed < this.numCheckpointsToWin)
							{
								num++;
							}
						}
						if (num == 0)
						{
							this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[]
							{
								(byte)this.raceIndex
							});
						}
					}
				}
				return 1f;
			case RacingManager.RacingState.Results:
				if (Time.time >= this.resultsEndTimestamp)
				{
					if (flag)
					{
						this.raceVisual.OnRaceReset();
					}
					this.racingState = RacingManager.RacingState.Inactive;
				}
				return 1f;
			default:
				return 1f;
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x000452A4 File Offset: 0x000434A4
		public void RaceEnded()
		{
			if (this.racingState != RacingManager.RacingState.InProgress)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Results;
			this.resultsEndTimestamp = Time.time + 10f;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceEnded();
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				RacingManager.RacerData racerData = this.racers[i];
				if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
				{
					racerData.isDisqualified = true;
					this.racers[i] = racerData;
				}
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
			for (int j = 0; j < this.racers.Count; j++)
			{
				if (this.racers[j].actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					VRRig.LocalRig.hoverboardVisual.SetRaceDisplay("");
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
					return;
				}
			}
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x000453A4 File Offset: 0x000435A4
		private void RefreshStartingPlayerList()
		{
			if (this.raceVisual != null && this.UpdateActorsInStartZone())
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.stringBuilder.AppendLine("NEXT RACE LINEUP");
				for (int i = 0; i < this.actorsInStartZone.Count; i++)
				{
					RacingManager.Race.stringBuilder.Append("    ");
					RacingManager.Race.stringBuilder.AppendLine(this.playerNamesInStartZone[this.actorsInStartZone[i]]);
				}
				this.raceVisual.SetRaceStartScoreboardText(RacingManager.Race.stringBuilder.ToString(), "");
			}
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00045447 File Offset: 0x00043647
		public void Button_StartRace(int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.photonView.RPC("RequestRaceStart_RPC", RpcTarget.MasterClient, new object[]
			{
				this.raceIndex,
				laps
			});
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00045480 File Offset: 0x00043680
		public void Host_RequestRaceStart(int laps, int requestedByActorNumber)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.UpdateActorsInStartZone();
			if (this.actorsInStartZone.Contains(requestedByActorNumber))
			{
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)laps,
					PhotonNetwork.Time + 4.0
				});
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x000454F4 File Offset: 0x000436F4
		public void BeginCountdown(double startTime, int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Countdown;
			this.raceStartTime = startTime;
			this.abortRaceAtTimestamp = startTime + (double)this.dqBaseDuration;
			this.numLapsSelected = laps;
			this.numCheckpointsToWin = this.numCheckpoints * laps + 1;
			this.hasLockedInParticipants = false;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnCountdownStart(laps, (float)(startTime - PhotonNetwork.Time));
			}
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x00045568 File Offset: 0x00043768
		public void RaceCountdownEnds()
		{
			if (this.racingState != RacingManager.RacingState.Countdown)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.InProgress;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceStart();
			}
			this.UpdateActorsInStartZone();
			if (PhotonNetwork.IsMasterClient)
			{
				this.photonView.RPC("RaceLockInParticipants_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					this.actorsInStartZone.ToArray()
				});
				return;
			}
			if (this.actorsInStartZone.Count >= 1)
			{
				this.LockInParticipants(this.actorsInStartZone.ToArray(), true);
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x00045604 File Offset: 0x00043804
		public void LockInParticipants(int[] participantActorNumbers, bool isProvisional = false)
		{
			if (this.hasLockedInParticipants)
			{
				return;
			}
			if (!isProvisional && participantActorNumbers.Length < 1)
			{
				this.racingState = RacingManager.RacingState.Inactive;
				return;
			}
			this.racers.Clear();
			if (participantActorNumbers.Length != 0)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					int actorNumber = vrrig.OwningNetPlayer.ActorNumber;
					if (participantActorNumbers.BinarySearch(actorNumber) >= 0 && !RacingManager.instance.IsActorLockedIntoAnyRace(actorNumber))
					{
						this.racers.Add(new RacingManager.RacerData
						{
							actorNumber = actorNumber,
							playerName = vrrig.OwningNetPlayer.SanitizedNickName,
							latestCheckpointTime = this.raceStartTime
						});
					}
				}
			}
			if (!isProvisional)
			{
				if (this.racers.Count < 1)
				{
					this.racingState = RacingManager.RacingState.Inactive;
					return;
				}
				this.hasLockedInParticipants = true;
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x00045718 File Offset: 0x00043918
		public void PassCheckpoint(Player player, int checkpointIndex, double time)
		{
			if (this.racingState == RacingManager.RacingState.Inactive)
			{
				return;
			}
			if (time < this.raceStartTime || time < PhotonNetwork.Time - 5.0 || time > PhotonNetwork.Time + 0.10000000149011612)
			{
				return;
			}
			if (this.abortRaceAtTimestamp < time + (double)this.dqInterval)
			{
				this.abortRaceAtTimestamp = time + (double)this.dqInterval;
			}
			RacingManager.RacerData racerData = default(RacingManager.RacerData);
			int i = 0;
			while (i < this.racers.Count)
			{
				racerData = this.racers[i];
				if (racerData.actorNumber == player.ActorNumber)
				{
					if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || racerData.isDisqualified)
					{
						return;
					}
					if (checkpointIndex != racerData.numCheckpointsPassed % this.numCheckpoints)
					{
						return;
					}
					RigContainer rigContainer;
					if (this.raceVisual != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer) && !this.raceVisual.IsPlayerNearCheckpoint(rigContainer.Rig, checkpointIndex))
					{
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (racerData.actorNumber != player.ActorNumber)
			{
				return;
			}
			racerData.numCheckpointsPassed++;
			racerData.latestCheckpointTime = time;
			this.racers[i] = racerData;
			if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || (i > 0 && RacingManager.RacerComparer.instance.Compare(this.racers[i - 1], racerData) > 0))
			{
				this.racers.Sort(RacingManager.RacerComparer.instance);
				this.OnRacerOrderChanged();
			}
			if (player.IsLocal)
			{
				if (checkpointIndex == this.numCheckpoints - 1)
				{
					int num = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num > this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINISH");
						this.raceVisual.EnableRaceEndSound();
						return;
					}
					if (num == this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINAL LAP");
						return;
					}
					this.raceVisual.ShowFinishLineText("NEXT LAP");
					return;
				}
				else if (checkpointIndex == 0)
				{
					int num2 = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num2 > this.numLapsSelected)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
						return;
					}
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay(string.Format("LAP {0}/{1}", num2, this.numLapsSelected));
				}
			}
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0004595C File Offset: 0x00043B5C
		private void OnRacerOrderChanged()
		{
			if (this.raceVisual != null)
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.AppendLine("");
				bool flag = false;
				switch (this.racingState)
				{
				case RacingManager.RacingState.Inactive:
					return;
				case RacingManager.RacingState.Countdown:
					RacingManager.Race.stringBuilder.AppendLine("STARTING LINEUP");
					flag = true;
					break;
				case RacingManager.RacingState.InProgress:
					RacingManager.Race.stringBuilder.AppendLine("RACE LEADERBOARD");
					break;
				case RacingManager.RacingState.Results:
					RacingManager.Race.stringBuilder.AppendLine("RACE RESULTS");
					break;
				}
				for (int i = 0; i < this.racers.Count; i++)
				{
					RacingManager.RacerData racerData = this.racers[i];
					if (racerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceDisplay(racerData.isDisqualified ? "DQ" : (i + 1).ToString());
					}
					string text = racerData.isDisqualified ? "DQ. " : (flag ? "    " : ((i + 1).ToString() + ". "));
					RacingManager.Race.stringBuilder.Append(text);
					if (text.Length <= 3)
					{
						RacingManager.Race.stringBuilder.Append(" ");
					}
					RacingManager.Race.stringBuilder.AppendLine(racerData.playerName);
					if (racerData.isDisqualified)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("--.--");
					}
					else if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("");
					}
					else
					{
						RacingManager.Race.timesStringBuilder.AppendLine(string.Format("{0:0.00}", racerData.latestCheckpointTime - this.raceStartTime));
					}
				}
				string mainText = RacingManager.Race.stringBuilder.ToString();
				string timesText = RacingManager.Race.timesStringBuilder.ToString();
				this.raceVisual.SetScoreboardText(mainText, timesText);
				this.raceVisual.SetRaceStartScoreboardText(mainText, timesText);
			}
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x00045B68 File Offset: 0x00043D68
		private bool UpdateActorsInStartZone()
		{
			if (Time.time < this.nextStartZoneUpdateTimestamp)
			{
				return false;
			}
			this.nextStartZoneUpdateTimestamp = Time.time + 0.1f;
			List<int> list = this.actorsInStartZone2;
			List<int> list2 = this.actorsInStartZone;
			this.actorsInStartZone = list;
			this.actorsInStartZone2 = list2;
			this.actorsInStartZone.Clear();
			this.playerNamesInStartZone.Clear();
			int num = Physics.OverlapBoxNonAlloc(this.raceStartZone.transform.position, this.raceStartZone.size / 2f, RacingManager.Race.overlapColliders, this.raceStartZone.transform.rotation, RacingManager.Race.playerLayerMask);
			num = Mathf.Min(num, RacingManager.Race.overlapColliders.Length);
			for (int i = 0; i < num; i++)
			{
				Collider collider = RacingManager.Race.overlapColliders[i];
				if (!(collider == null))
				{
					VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
					int count = this.actorsInStartZone.Count;
					if (!(component == null))
					{
						if (component.isLocal)
						{
							if (NetworkSystem.Instance.LocalPlayer == null)
							{
								RacingManager.Race.overlapColliders[i] = null;
								goto IL_1E2;
							}
							if (RacingManager.instance.IsActorLockedIntoAnyRace(NetworkSystem.Instance.LocalPlayer.ActorNumber))
							{
								goto IL_1E2;
							}
							this.actorsInStartZone.AddSortedUnique(NetworkSystem.Instance.LocalPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(NetworkSystem.Instance.LocalPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						else
						{
							if (RacingManager.instance.IsActorLockedIntoAnyRace(component.OwningNetPlayer.ActorNumber))
							{
								goto IL_1E2;
							}
							this.actorsInStartZone.AddSortedUnique(component.OwningNetPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(component.OwningNetPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						RacingManager.Race.overlapColliders[i] = null;
					}
				}
				IL_1E2:;
			}
			if (this.actorsInStartZone2.Count != this.actorsInStartZone.Count)
			{
				return true;
			}
			for (int j = 0; j < this.actorsInStartZone.Count; j++)
			{
				if (this.actorsInStartZone[j] != this.actorsInStartZone2[j])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000FDE RID: 4062
		private int raceIndex;

		// Token: 0x04000FDF RID: 4063
		private int numCheckpoints;

		// Token: 0x04000FE0 RID: 4064
		private float dqBaseDuration;

		// Token: 0x04000FE1 RID: 4065
		private float dqInterval;

		// Token: 0x04000FE2 RID: 4066
		private BoxCollider raceStartZone;

		// Token: 0x04000FE3 RID: 4067
		private PhotonView photonView;

		// Token: 0x04000FE4 RID: 4068
		private List<RacingManager.RacerData> racers = new List<RacingManager.RacerData>(10);

		// Token: 0x04000FE5 RID: 4069
		private Dictionary<NetPlayer, int> playerLookup = new Dictionary<NetPlayer, int>();

		// Token: 0x04000FE6 RID: 4070
		private List<int> actorsInStartZone = new List<int>();

		// Token: 0x04000FE7 RID: 4071
		private List<int> actorsInStartZone2 = new List<int>();

		// Token: 0x04000FE8 RID: 4072
		private Dictionary<int, string> playerNamesInStartZone = new Dictionary<int, string>();

		// Token: 0x04000FE9 RID: 4073
		private int numLapsSelected = 1;

		// Token: 0x04000FEB RID: 4075
		private double raceStartTime;

		// Token: 0x04000FEC RID: 4076
		private double abortRaceAtTimestamp;

		// Token: 0x04000FED RID: 4077
		private float resultsEndTimestamp;

		// Token: 0x04000FEE RID: 4078
		private bool isInstanceLoaded;

		// Token: 0x04000FEF RID: 4079
		private int numCheckpointsToWin;

		// Token: 0x04000FF0 RID: 4080
		private RaceVisual raceVisual;

		// Token: 0x04000FF1 RID: 4081
		private bool hasLockedInParticipants;

		// Token: 0x04000FF2 RID: 4082
		private float nextTickTimestamp;

		// Token: 0x04000FF3 RID: 4083
		private static StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04000FF4 RID: 4084
		private static StringBuilder timesStringBuilder = new StringBuilder();

		// Token: 0x04000FF5 RID: 4085
		private static Collider[] overlapColliders = new Collider[20];

		// Token: 0x04000FF6 RID: 4086
		private static int playerLayerMask = UnityLayer.GorillaBodyCollider.ToLayerMask() | UnityLayer.GorillaTagCollider.ToLayerMask();

		// Token: 0x04000FF7 RID: 4087
		private float nextStartZoneUpdateTimestamp;
	}
}
