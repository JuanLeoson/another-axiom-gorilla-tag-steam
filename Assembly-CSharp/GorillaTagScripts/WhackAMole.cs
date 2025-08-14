using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000BEE RID: 3054
	[NetworkBehaviourWeaved(210)]
	public class WhackAMole : NetworkComponent
	{
		// Token: 0x06004A21 RID: 18977 RVA: 0x00168268 File Offset: 0x00166468
		private void UpdateMeshRendererList()
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			ZoneBasedObject[] array = this.zoneBasedVisuals;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>(true))
				{
					if (meshRenderer.enabled)
					{
						list.Add(meshRenderer);
					}
				}
			}
			this.zoneBasedMeshRenderers = list.ToArray();
		}

		// Token: 0x06004A22 RID: 18978 RVA: 0x001682D0 File Offset: 0x001664D0
		protected override void Awake()
		{
			base.Awake();
			if (this.molesContainerRight != null)
			{
				this.rightMolesList = new List<Mole>(this.molesContainerRight.GetComponentsInChildren<Mole>());
				if (this.rightMolesList.Count > 0)
				{
					this.molesList.AddRange(this.rightMolesList);
				}
			}
			if (this.molesContainerLeft != null)
			{
				this.leftMolesList = new List<Mole>(this.molesContainerLeft.GetComponentsInChildren<Mole>());
				if (this.leftMolesList.Count > 0)
				{
					this.molesList.AddRange(this.leftMolesList);
					foreach (Mole mole in this.leftMolesList)
					{
						mole.IsLeftSideMole = true;
					}
				}
			}
			this.currentLevelIndex = -1;
			foreach (Mole mole2 in this.molesList)
			{
				mole2.OnTapped += this.OnMoleTapped;
			}
			List<Mole> list = this.leftMolesList;
			bool flag;
			if (list != null && list.Count > 0)
			{
				list = this.rightMolesList;
				flag = (list != null && list.Count > 0);
			}
			else
			{
				flag = false;
			}
			this.isMultiplayer = flag;
			this.welcomeUI.SetActive(false);
			this.ongoingGameUI.SetActive(false);
			this.levelEndedUI.SetActive(false);
			this.ContinuePressedUI.SetActive(false);
			this.multiplyareScoresUI.SetActive(false);
			this.bestScore = 0;
			this.bestScoreText.text = string.Empty;
			this.highScorePlayerName = string.Empty;
			this.victoryParticles = this.victoryFX.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x001684A0 File Offset: 0x001666A0
		protected override void Start()
		{
			base.Start();
			this.SwitchState(WhackAMole.GameState.Off);
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Register(this);
			}
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x001684C8 File Offset: 0x001666C8
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (Mole mole in this.molesList)
			{
				mole.OnTapped -= this.OnMoleTapped;
			}
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Unregister(this);
			}
			this.molesList.Clear();
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x0016854C File Offset: 0x0016674C
		public void InvokeUpdate()
		{
			bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
			bool flag = this.zoneBasedVisuals[0].IsLocalPlayerInZone();
			if (isMasterClient != this.wasMasterClient || flag != this.wasLocalPlayerInZone)
			{
				MeshRenderer[] array = this.zoneBasedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = flag;
				}
				bool active = isMasterClient || flag;
				ZoneBasedObject[] array2 = this.zoneBasedVisuals;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(active);
				}
				this.wasMasterClient = isMasterClient;
				this.wasLocalPlayerInZone = flag;
			}
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x001685E4 File Offset: 0x001667E4
		private void SwitchState(WhackAMole.GameState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.ResetGame();
				this.currentLevelIndex = -1;
				this.currentLevel = null;
				this.UpdateLevelUI(1);
				break;
			case WhackAMole.GameState.ContinuePressed:
				this.continuePressedTime = Time.time;
				this.audioSource.GTStop();
				this.audioSource.GTPlayOneShot(this.counterClip, 1f);
				if (base.IsMine)
				{
					this.pickedMolesIndex.Clear();
				}
				this.ResetGame();
				if (base.IsMine)
				{
					this.LoadNextLevel();
				}
				break;
			case WhackAMole.GameState.Ongoing:
				this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
				break;
			case WhackAMole.GameState.TimesUp:
				if (this.currentLevel != null)
				{
					foreach (Mole mole in this.molesList)
					{
						mole.HideMole(false);
					}
					this.curentGameResult = this.GetGameResult();
					this.UpdateResultUI(this.curentGameResult);
					this.levelEndedTotalScoreText.text = "SCORE " + this.totalScore.ToString();
					this.levelEndedCurrentScoreText.text = string.Format("{0}/{1}", this.currentScore, this.currentLevel.GetMinScore(this.isMultiplayer));
					if (this.totalScore > this.bestScore)
					{
						this.bestScore = this.totalScore;
						this.highScorePlayerName = this.playerName;
					}
					this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
					this.audioSource.GTStop();
					if (this.curentGameResult == WhackAMole.GameResult.LevelComplete)
					{
						this.audioSource.GTPlayOneShot(this.levelCompleteClip, 1f);
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					else if (this.curentGameResult == WhackAMole.GameResult.GameOver)
					{
						this.audioSource.GTPlayOneShot(this.gameOverClip, 1f);
					}
					else if (this.curentGameResult == WhackAMole.GameResult.Win)
					{
						this.audioSource.GTPlayOneShot(this.winClip, 1f);
						if (this.victoryFX)
						{
							ParticleSystem[] array = this.victoryParticles;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].Play();
							}
						}
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					int minScore = this.currentLevel.GetMinScore(this.isMultiplayer);
					if (this.levelGoodMolesPicked < minScore)
					{
						GTDev.LogError<string>(string.Format("[WAM] Lvl:{0} Only Picked {1}/{2} good moles!", this.currentLevel.levelNumber, this.levelGoodMolesPicked, minScore), null);
					}
					if (base.IsMine)
					{
						GorillaTelemetry.WamLevelEnd(this.playerId, this.gameId, this.machineId, this.currentLevel.levelNumber, this.levelGoodMolesPicked, this.levelHazardMolesPicked, minScore, this.currentScore, this.levelHazardMolesHit, this.curentGameResult.ToString());
					}
				}
				break;
			}
			this.UpdateScreenData();
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x001689AC File Offset: 0x00166BAC
		private void UpdateScreenData()
		{
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.welcomeUI.SetActive(true);
				this.ContinuePressedUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.levelEndedUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				return;
			case WhackAMole.GameState.ContinuePressed:
				this.levelEndedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				this.ContinuePressedUI.SetActive(true);
				break;
			case WhackAMole.GameState.Ongoing:
				this.ContinuePressedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(true);
				this.levelEndedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
					return;
				}
				break;
			case WhackAMole.GameState.PickMoles:
				break;
			case WhackAMole.GameState.TimesUp:
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.ContinuePressedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
				}
				this.levelEndedUI.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x00168AE4 File Offset: 0x00166CE4
		public static int CreateNewGameID()
		{
			int num = (int)((DateTime.Now - WhackAMole.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
			if (num <= WhackAMole.lastAssignedID)
			{
				WhackAMole.lastAssignedID++;
				return WhackAMole.lastAssignedID;
			}
			WhackAMole.lastAssignedID = num;
			return num;
		}

		// Token: 0x06004A29 RID: 18985 RVA: 0x00168B44 File Offset: 0x00166D44
		private void OnMoleTapped(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeftHand)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.Off || gameState == WhackAMole.GameState.TimesUp)
			{
				return;
			}
			AudioClip clip = moleType.isHazard ? this.whackHazardClips[Random.Range(0, this.whackHazardClips.Length)] : this.whackMonkeClips[Random.Range(0, this.whackMonkeClips.Length)];
			if (moleType.isHazard)
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
				this.levelHazardMolesHit++;
			}
			else
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
			}
			if (moleType.monkeMoleHitMaterial != null)
			{
				moleType.MeshRenderer.material = moleType.monkeMoleHitMaterial;
			}
			this.currentScore += moleType.scorePoint;
			this.totalScore += moleType.scorePoint;
			if (moleType.IsLeftSideMoleType)
			{
				this.leftPlayerScore += moleType.scorePoint;
			}
			else
			{
				this.rightPlayerScore += moleType.scorePoint;
			}
			this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
			moleType.MoleContainerParent.HideMole(true);
		}

		// Token: 0x06004A2A RID: 18986 RVA: 0x00168C68 File Offset: 0x00166E68
		public void HandleOnTimerStopped()
		{
			this.gameEndedTime = Time.time;
			this.SwitchState(WhackAMole.GameState.TimesUp);
		}

		// Token: 0x06004A2B RID: 18987 RVA: 0x00168C7C File Offset: 0x00166E7C
		private IEnumerator PlayHazardAudio(AudioClip clip)
		{
			this.audioSource.clip = clip;
			this.audioSource.GTPlay();
			yield return new WaitForSeconds(this.audioSource.clip.length);
			this.audioSource.clip = this.errorClip;
			this.audioSource.GTPlay();
			yield break;
		}

		// Token: 0x06004A2C RID: 18988 RVA: 0x00168C94 File Offset: 0x00166E94
		private bool PickMoles()
		{
			WhackAMole.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.pickedMolesIndex.Clear();
			float passedTime = this.timer.GetPassedTime();
			if (passedTime > this.currentLevel.levelDuration - this.currentLevel.showMoleDuration)
			{
				return true;
			}
			float t = passedTime / this.currentLevel.levelDuration;
			CS$<>8__locals1.minMoleCount = Mathf.Lerp(this.currentLevel.minimumMoleCount.x, this.currentLevel.minimumMoleCount.y, t);
			CS$<>8__locals1.maxMoleCount = Mathf.Lerp(this.currentLevel.maximumMoleCount.x, this.currentLevel.maximumMoleCount.y, t);
			this.curentTime = Time.time;
			CS$<>8__locals1.hazardMoleChance = Mathf.Lerp(this.currentLevel.hazardMoleChance.x, this.currentLevel.hazardMoleChance.y, t);
			if (this.isMultiplayer)
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.rightMolesList, ref CS$<>8__locals1);
				this.<PickMoles>g__PickMolesFrom|85_0(this.leftMolesList, ref CS$<>8__locals1);
			}
			else
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.molesList, ref CS$<>8__locals1);
			}
			return this.pickedMolesIndex.Count != 0;
		}

		// Token: 0x06004A2D RID: 18989 RVA: 0x00168DC0 File Offset: 0x00166FC0
		private void LoadNextLevel()
		{
			if (this.currentLevel != null)
			{
				this.resetToFirstLevel = (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer));
				if (this.resetToFirstLevel)
				{
					this.currentLevelIndex = 0;
				}
				else
				{
					this.currentLevelIndex++;
				}
				if (this.currentLevelIndex >= this.allLevels.Length)
				{
					this.currentLevelIndex = 0;
				}
			}
			else
			{
				this.currentLevelIndex++;
			}
			this.currentLevel = this.allLevels[this.currentLevelIndex];
			this.timer.SetTimerDuration(this.currentLevel.levelDuration);
			this.timer.RestartTimer();
			this.curentTime = Time.time;
			this.currentScore = 0;
			this.leftPlayerScore = 0;
			this.rightPlayerScore = 0;
			this.levelGoodMolesPicked = (this.levelHazardMolesPicked = 0);
			this.levelHazardMolesHit = 0;
			if (this.currentLevelIndex == 0)
			{
				this.totalScore = 0;
			}
			if (this.currentLevelIndex == 0 && base.IsMine)
			{
				this.gameId = WhackAMole.CreateNewGameID();
				Debug.LogWarning("GAME ID" + this.gameId.ToString());
			}
		}

		// Token: 0x06004A2E RID: 18990 RVA: 0x00168EF0 File Offset: 0x001670F0
		private bool PickSingleMole(int randomMoleIndex, float hazardMoleChance)
		{
			bool flag = hazardMoleChance > 0f && Random.value <= hazardMoleChance;
			int moleTypeIndex = this.molesList[randomMoleIndex].GetMoleTypeIndex(flag);
			this.molesList[randomMoleIndex].ShowMole(this.currentLevel.showMoleDuration, moleTypeIndex);
			this.pickedMolesIndex.Add(randomMoleIndex, moleTypeIndex);
			if (flag)
			{
				this.levelHazardMolesPicked++;
			}
			else
			{
				this.levelGoodMolesPicked++;
			}
			return flag;
		}

		// Token: 0x06004A2F RID: 18991 RVA: 0x00168F74 File Offset: 0x00167174
		private void ResetGame()
		{
			foreach (Mole mole in this.molesList)
			{
				mole.ResetPosition();
			}
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x00168FC4 File Offset: 0x001671C4
		private void UpdateScoreUI(int totalScore, int _leftPlayerScore, int _rightPlayerScore)
		{
			if (this.currentLevel != null)
			{
				this.scoreText.text = string.Format("SCORE\n{0}/{1}", totalScore, this.currentLevel.GetMinScore(this.isMultiplayer));
				this.leftPlayerScoreText.text = _leftPlayerScore.ToString();
				this.rightPlayerScoreText.text = _rightPlayerScore.ToString();
			}
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x00169034 File Offset: 0x00167234
		private void UpdateLevelUI(int levelNumber)
		{
			this.arrowTargetRotation = Quaternion.Euler(0f, 0f, (float)(18 * (levelNumber - 1)));
			this.arrowRotationNeedsUpdate = true;
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x0016905C File Offset: 0x0016725C
		private void UpdateArrowRotation()
		{
			Quaternion quaternion = Quaternion.Slerp(this.levelArrow.transform.localRotation, this.arrowTargetRotation, Time.deltaTime * 5f);
			if (Quaternion.Angle(quaternion, this.arrowTargetRotation) < 0.1f)
			{
				quaternion = this.arrowTargetRotation;
				this.arrowRotationNeedsUpdate = false;
			}
			this.levelArrow.transform.localRotation = quaternion;
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x001690C2 File Offset: 0x001672C2
		private void UpdateTimerUI(int time)
		{
			if (time == this.previousTime)
			{
				return;
			}
			this.timeText.text = "TIME " + time.ToString();
			this.previousTime = time;
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x001690F1 File Offset: 0x001672F1
		private void UpdateResultUI(WhackAMole.GameResult gameResult)
		{
			if (gameResult == WhackAMole.GameResult.LevelComplete)
			{
				this.resultText.text = "LEVEL COMPLETE";
				return;
			}
			if (gameResult == WhackAMole.GameResult.Win)
			{
				this.resultText.text = "YOU WIN!";
				return;
			}
			if (gameResult == WhackAMole.GameResult.GameOver)
			{
				this.resultText.text = "GAME OVER";
			}
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x00169130 File Offset: 0x00167330
		public void OnStartButtonPressed()
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.TimesUp || gameState == WhackAMole.GameState.Off)
			{
				base.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, Array.Empty<object>());
			}
		}

		// Token: 0x06004A36 RID: 18998 RVA: 0x00169161 File Offset: 0x00167361
		[PunRPC]
		private void WhackAMoleButtonPressed(PhotonMessageInfo info)
		{
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06004A37 RID: 18999 RVA: 0x00169170 File Offset: 0x00167370
		[Rpc]
		private unsafe void RPC_WhackAMoleButtonPressed(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int capacityInBytes = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, capacityInBytes);
							byte* data = SimulationMessage.GetData(ptr);
							int num = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							ptr->Offset = num * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_12;
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06004A38 RID: 19000 RVA: 0x00169290 File Offset: 0x00167490
		private void WhackAMoleButtonPressedShared(PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "WhackAMoleButtonPressedShared");
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (vrrig)
			{
				this.playerName = vrrig.playerNameVisible;
				if (this.currentState == WhackAMole.GameState.Off)
				{
					this.playerId = info.Sender.UserId;
					if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
					{
						PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
					}
				}
			}
			this.SwitchState(WhackAMole.GameState.ContinuePressed);
		}

		// Token: 0x06004A39 RID: 19001 RVA: 0x00169311 File Offset: 0x00167511
		private WhackAMole.GameResult GetGameResult()
		{
			if (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer))
			{
				return WhackAMole.GameResult.GameOver;
			}
			if (this.currentLevelIndex >= this.allLevels.Length - 1)
			{
				return WhackAMole.GameResult.Win;
			}
			return WhackAMole.GameResult.LevelComplete;
		}

		// Token: 0x06004A3A RID: 19002 RVA: 0x00169343 File Offset: 0x00167543
		public int GetCurrentLevel()
		{
			if (this.currentLevel != null)
			{
				return this.currentLevel.levelNumber;
			}
			return 0;
		}

		// Token: 0x06004A3B RID: 19003 RVA: 0x00169360 File Offset: 0x00167560
		public int GetTotalLevelNumbers()
		{
			if (this.allLevels != null)
			{
				return this.allLevels.Length;
			}
			return 0;
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06004A3C RID: 19004 RVA: 0x00169374 File Offset: 0x00167574
		// (set) Token: 0x06004A3D RID: 19005 RVA: 0x0016939E File Offset: 0x0016759E
		[Networked]
		[NetworkedWeaved(0, 210)]
		public unsafe WhackAMole.WhackAMoleData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(WhackAMole.WhackAMoleData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(WhackAMole.WhackAMoleData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004A3E RID: 19006 RVA: 0x001693CC File Offset: 0x001675CC
		public override void WriteDataFusion()
		{
			this.Data = new WhackAMole.WhackAMoleData(this.currentState, this.currentLevelIndex, this.currentScore, this.totalScore, this.bestScore, this.rightPlayerScore, this.highScorePlayerName, this.timer.GetRemainingTime(), this.gameEndedTime, this.gameId, this.pickedMolesIndex);
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x06004A3F RID: 19007 RVA: 0x00169438 File Offset: 0x00167638
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentLevelIndex, this.Data.CurrentScore, this.Data.TotalScore, this.Data.BestScore, this.Data.RightPlayerScore, this.Data.HighScorePlayerName.Value, this.Data.RemainingTime, this.Data.GameEndedTime, this.Data.GameId);
			for (int i = 0; i < this.Data.PickedMolesIndexCount; i++)
			{
				int randomMoleTypeIndex = this.Data.PickedMolesIndex[i];
				if (i >= 0 && i < this.molesList.Count && this.currentLevel)
				{
					this.molesList[i].ShowMole(this.currentLevel.showMoleDuration, randomMoleTypeIndex);
				}
			}
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x00169550 File Offset: 0x00167750
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x00169560 File Offset: 0x00167760
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06004A42 RID: 19010 RVA: 0x00169570 File Offset: 0x00167770
		private void ReadDataShared(WhackAMole.GameState _currentState, int _currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float _remainingTime, float endedTime, int _gameId)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (_currentState != gameState)
			{
				this.SwitchState(_currentState);
			}
			this.currentLevelIndex = _currentLevelIndex;
			if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
			{
				this.currentLevel = this.allLevels[this.currentLevelIndex];
				this.UpdateLevelUI(this.currentLevel.levelNumber);
			}
			this.currentScore = cScore;
			this.totalScore = tScore;
			this.bestScore = bScore;
			this.rightPlayerScore = rPScore;
			this.leftPlayerScore = this.currentScore - this.rightPlayerScore;
			this.highScorePlayerName = hScorePName;
			this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
			this.remainingTime = _remainingTime;
			if (float.IsFinite(this.remainingTime) && this.currentLevel)
			{
				this.remainingTime = this.remainingTime.ClampSafe(0f, this.currentLevel.levelDuration);
				this.UpdateTimerUI((int)this.remainingTime);
			}
			if (float.IsFinite(endedTime))
			{
				this.gameEndedTime = endedTime.ClampSafe(0f, Time.time);
			}
			this.gameId = _gameId;
		}

		// Token: 0x06004A43 RID: 19011 RVA: 0x001696C4 File Offset: 0x001678C4
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.timer.RestartTimer();
				this.timer.SetTimerDuration(this.remainingTime);
				this.curentTime = Time.time;
				if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
				{
					this.currentLevel = this.allLevels[this.currentLevelIndex];
				}
				this.SwitchState(this.currentState);
			}
		}

		// Token: 0x06004A46 RID: 19014 RVA: 0x001697C8 File Offset: 0x001679C8
		[CompilerGenerated]
		private void <PickMoles>g__PickMolesFrom|85_0(List<Mole> moles, ref WhackAMole.<>c__DisplayClass85_0 A_2)
		{
			int a = Mathf.RoundToInt(Random.Range(A_2.minMoleCount, A_2.maxMoleCount));
			this.potentialMoles.Clear();
			foreach (Mole mole in moles)
			{
				if (mole.CanPickMole())
				{
					this.potentialMoles.Add(mole);
				}
			}
			int num = Mathf.Min(a, this.potentialMoles.Count);
			int num2 = Mathf.CeilToInt((float)num * A_2.hazardMoleChance);
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, this.potentialMoles.Count);
				if (this.PickSingleMole(this.molesList.IndexOf(this.potentialMoles[index]), (num3 < num2) ? A_2.hazardMoleChance : 0f))
				{
					num3++;
				}
				this.potentialMoles.RemoveAt(index);
			}
		}

		// Token: 0x06004A47 RID: 19015 RVA: 0x001698D4 File Offset: 0x00167AD4
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004A48 RID: 19016 RVA: 0x001698EC File Offset: 0x00167AEC
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x06004A49 RID: 19017 RVA: 0x00169900 File Offset: 0x00167B00
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_WhackAMoleButtonPressed@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((WhackAMole)behaviour).RPC_WhackAMoleButtonPressed(info);
		}

		// Token: 0x040052F5 RID: 21237
		public string machineId = "default";

		// Token: 0x040052F6 RID: 21238
		public GameObject molesContainerRight;

		// Token: 0x040052F7 RID: 21239
		[Tooltip("Only for co-op version")]
		public GameObject molesContainerLeft;

		// Token: 0x040052F8 RID: 21240
		public int betweenLevelPauseDuration = 3;

		// Token: 0x040052F9 RID: 21241
		public int countdownDuration = 5;

		// Token: 0x040052FA RID: 21242
		public WhackAMoleLevelSO[] allLevels;

		// Token: 0x040052FB RID: 21243
		[SerializeField]
		private GorillaTimer timer;

		// Token: 0x040052FC RID: 21244
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040052FD RID: 21245
		public GameObject levelArrow;

		// Token: 0x040052FE RID: 21246
		public GameObject victoryFX;

		// Token: 0x040052FF RID: 21247
		public ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x04005300 RID: 21248
		[SerializeField]
		private MeshRenderer[] zoneBasedMeshRenderers;

		// Token: 0x04005301 RID: 21249
		[Space]
		public AudioClip backgroundLoop;

		// Token: 0x04005302 RID: 21250
		public AudioClip errorClip;

		// Token: 0x04005303 RID: 21251
		public AudioClip counterClip;

		// Token: 0x04005304 RID: 21252
		public AudioClip levelCompleteClip;

		// Token: 0x04005305 RID: 21253
		public AudioClip winClip;

		// Token: 0x04005306 RID: 21254
		public AudioClip gameOverClip;

		// Token: 0x04005307 RID: 21255
		public AudioClip[] whackHazardClips;

		// Token: 0x04005308 RID: 21256
		public AudioClip[] whackMonkeClips;

		// Token: 0x04005309 RID: 21257
		[Space]
		public GameObject welcomeUI;

		// Token: 0x0400530A RID: 21258
		public GameObject ongoingGameUI;

		// Token: 0x0400530B RID: 21259
		public GameObject levelEndedUI;

		// Token: 0x0400530C RID: 21260
		public GameObject ContinuePressedUI;

		// Token: 0x0400530D RID: 21261
		public GameObject multiplyareScoresUI;

		// Token: 0x0400530E RID: 21262
		[Space]
		public TextMeshPro scoreText;

		// Token: 0x0400530F RID: 21263
		public TextMeshPro bestScoreText;

		// Token: 0x04005310 RID: 21264
		[Tooltip("Only for co-op version")]
		public TextMeshPro rightPlayerScoreText;

		// Token: 0x04005311 RID: 21265
		[Tooltip("Only for co-op version")]
		public TextMeshPro leftPlayerScoreText;

		// Token: 0x04005312 RID: 21266
		public TextMeshPro timeText;

		// Token: 0x04005313 RID: 21267
		public TextMeshPro counterText;

		// Token: 0x04005314 RID: 21268
		public TextMeshPro resultText;

		// Token: 0x04005315 RID: 21269
		public TextMeshPro levelEndedOptionsText;

		// Token: 0x04005316 RID: 21270
		public TextMeshPro levelEndedCountdownText;

		// Token: 0x04005317 RID: 21271
		public TextMeshPro levelEndedTotalScoreText;

		// Token: 0x04005318 RID: 21272
		public TextMeshPro levelEndedCurrentScoreText;

		// Token: 0x04005319 RID: 21273
		private List<Mole> rightMolesList;

		// Token: 0x0400531A RID: 21274
		private List<Mole> leftMolesList;

		// Token: 0x0400531B RID: 21275
		private List<Mole> molesList = new List<Mole>();

		// Token: 0x0400531C RID: 21276
		private WhackAMoleLevelSO currentLevel;

		// Token: 0x0400531D RID: 21277
		private int currentScore;

		// Token: 0x0400531E RID: 21278
		private int totalScore;

		// Token: 0x0400531F RID: 21279
		private int leftPlayerScore;

		// Token: 0x04005320 RID: 21280
		private int rightPlayerScore;

		// Token: 0x04005321 RID: 21281
		private int bestScore;

		// Token: 0x04005322 RID: 21282
		private float curentTime;

		// Token: 0x04005323 RID: 21283
		private int currentLevelIndex;

		// Token: 0x04005324 RID: 21284
		private float continuePressedTime;

		// Token: 0x04005325 RID: 21285
		private bool resetToFirstLevel;

		// Token: 0x04005326 RID: 21286
		private Quaternion arrowTargetRotation;

		// Token: 0x04005327 RID: 21287
		private bool arrowRotationNeedsUpdate;

		// Token: 0x04005328 RID: 21288
		private List<Mole> potentialMoles = new List<Mole>();

		// Token: 0x04005329 RID: 21289
		private Dictionary<int, int> pickedMolesIndex = new Dictionary<int, int>();

		// Token: 0x0400532A RID: 21290
		private WhackAMole.GameState currentState;

		// Token: 0x0400532B RID: 21291
		private WhackAMole.GameState lastState;

		// Token: 0x0400532C RID: 21292
		private float remainingTime;

		// Token: 0x0400532D RID: 21293
		private int previousTime = -1;

		// Token: 0x0400532E RID: 21294
		private bool isMultiplayer;

		// Token: 0x0400532F RID: 21295
		private float gameEndedTime;

		// Token: 0x04005330 RID: 21296
		private WhackAMole.GameResult curentGameResult;

		// Token: 0x04005331 RID: 21297
		private string playerName = string.Empty;

		// Token: 0x04005332 RID: 21298
		private string highScorePlayerName = string.Empty;

		// Token: 0x04005333 RID: 21299
		private ParticleSystem[] victoryParticles;

		// Token: 0x04005334 RID: 21300
		private int levelHazardMolesPicked;

		// Token: 0x04005335 RID: 21301
		private int levelGoodMolesPicked;

		// Token: 0x04005336 RID: 21302
		private string playerId;

		// Token: 0x04005337 RID: 21303
		private int gameId;

		// Token: 0x04005338 RID: 21304
		private int levelHazardMolesHit;

		// Token: 0x04005339 RID: 21305
		private static DateTime epoch = new DateTime(2024, 1, 1);

		// Token: 0x0400533A RID: 21306
		private static int lastAssignedID;

		// Token: 0x0400533B RID: 21307
		private bool wasMasterClient;

		// Token: 0x0400533C RID: 21308
		private bool wasLocalPlayerInZone = true;

		// Token: 0x0400533D RID: 21309
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 210)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private WhackAMole.WhackAMoleData _Data;

		// Token: 0x02000BEF RID: 3055
		public enum GameState
		{
			// Token: 0x0400533F RID: 21311
			Off,
			// Token: 0x04005340 RID: 21312
			ContinuePressed,
			// Token: 0x04005341 RID: 21313
			Ongoing,
			// Token: 0x04005342 RID: 21314
			PickMoles,
			// Token: 0x04005343 RID: 21315
			TimesUp,
			// Token: 0x04005344 RID: 21316
			LevelStarted
		}

		// Token: 0x02000BF0 RID: 3056
		private enum GameResult
		{
			// Token: 0x04005346 RID: 21318
			GameOver,
			// Token: 0x04005347 RID: 21319
			Win,
			// Token: 0x04005348 RID: 21320
			LevelComplete,
			// Token: 0x04005349 RID: 21321
			Unknown
		}

		// Token: 0x02000BF1 RID: 3057
		[NetworkStructWeaved(210)]
		[StructLayout(LayoutKind.Explicit, Size = 840)]
		public struct WhackAMoleData : INetworkStruct
		{
			// Token: 0x17000724 RID: 1828
			// (get) Token: 0x06004A4A RID: 19018 RVA: 0x00169953 File Offset: 0x00167B53
			// (set) Token: 0x06004A4B RID: 19019 RVA: 0x0016995B File Offset: 0x00167B5B
			public WhackAMole.GameState CurrentState { readonly get; set; }

			// Token: 0x17000725 RID: 1829
			// (get) Token: 0x06004A4C RID: 19020 RVA: 0x00169964 File Offset: 0x00167B64
			// (set) Token: 0x06004A4D RID: 19021 RVA: 0x0016996C File Offset: 0x00167B6C
			public int CurrentLevelIndex { readonly get; set; }

			// Token: 0x17000726 RID: 1830
			// (get) Token: 0x06004A4E RID: 19022 RVA: 0x00169975 File Offset: 0x00167B75
			// (set) Token: 0x06004A4F RID: 19023 RVA: 0x0016997D File Offset: 0x00167B7D
			public int CurrentScore { readonly get; set; }

			// Token: 0x17000727 RID: 1831
			// (get) Token: 0x06004A50 RID: 19024 RVA: 0x00169986 File Offset: 0x00167B86
			// (set) Token: 0x06004A51 RID: 19025 RVA: 0x0016998E File Offset: 0x00167B8E
			public int TotalScore { readonly get; set; }

			// Token: 0x17000728 RID: 1832
			// (get) Token: 0x06004A52 RID: 19026 RVA: 0x00169997 File Offset: 0x00167B97
			// (set) Token: 0x06004A53 RID: 19027 RVA: 0x0016999F File Offset: 0x00167B9F
			public int BestScore { readonly get; set; }

			// Token: 0x17000729 RID: 1833
			// (get) Token: 0x06004A54 RID: 19028 RVA: 0x001699A8 File Offset: 0x00167BA8
			// (set) Token: 0x06004A55 RID: 19029 RVA: 0x001699B0 File Offset: 0x00167BB0
			public int RightPlayerScore { readonly get; set; }

			// Token: 0x1700072A RID: 1834
			// (get) Token: 0x06004A56 RID: 19030 RVA: 0x001699B9 File Offset: 0x00167BB9
			// (set) Token: 0x06004A57 RID: 19031 RVA: 0x001699CB File Offset: 0x00167BCB
			[Networked]
			public unsafe NetworkString<_128> HighScorePlayerName
			{
				readonly get
				{
					return *(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName);
				}
				set
				{
					*(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName) = value;
				}
			}

			// Token: 0x1700072B RID: 1835
			// (get) Token: 0x06004A58 RID: 19032 RVA: 0x001699DE File Offset: 0x00167BDE
			// (set) Token: 0x06004A59 RID: 19033 RVA: 0x001699E6 File Offset: 0x00167BE6
			public float RemainingTime { readonly get; set; }

			// Token: 0x1700072C RID: 1836
			// (get) Token: 0x06004A5A RID: 19034 RVA: 0x001699EF File Offset: 0x00167BEF
			// (set) Token: 0x06004A5B RID: 19035 RVA: 0x001699F7 File Offset: 0x00167BF7
			public float GameEndedTime { readonly get; set; }

			// Token: 0x1700072D RID: 1837
			// (get) Token: 0x06004A5C RID: 19036 RVA: 0x00169A00 File Offset: 0x00167C00
			// (set) Token: 0x06004A5D RID: 19037 RVA: 0x00169A08 File Offset: 0x00167C08
			public int GameId { readonly get; set; }

			// Token: 0x1700072E RID: 1838
			// (get) Token: 0x06004A5E RID: 19038 RVA: 0x00169A11 File Offset: 0x00167C11
			// (set) Token: 0x06004A5F RID: 19039 RVA: 0x00169A19 File Offset: 0x00167C19
			public int PickedMolesIndexCount { readonly get; set; }

			// Token: 0x1700072F RID: 1839
			// (get) Token: 0x06004A60 RID: 19040 RVA: 0x00169A24 File Offset: 0x00167C24
			[Networked]
			[Capacity(10)]
			public unsafe NetworkDictionary<int, int> PickedMolesIndex
			{
				get
				{
					return new NetworkDictionary<int, int>((int*)Native.ReferenceToPointer<FixedStorage@71>(ref this._PickedMolesIndex), 17, ReaderWriter@System_Int32.GetInstance(), ReaderWriter@System_Int32.GetInstance());
				}
			}

			// Token: 0x06004A61 RID: 19041 RVA: 0x00169A50 File Offset: 0x00167C50
			public WhackAMoleData(WhackAMole.GameState state, int currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float remainingTime, float endedTime, int gameId, Dictionary<int, int> moleIndexs)
			{
				this.CurrentState = state;
				this.CurrentLevelIndex = currentLevelIndex;
				this.CurrentScore = cScore;
				this.TotalScore = tScore;
				this.BestScore = bScore;
				this.RightPlayerScore = rPScore;
				this.HighScorePlayerName = hScorePName;
				this.RemainingTime = remainingTime;
				this.GameEndedTime = endedTime;
				this.GameId = gameId;
				this.PickedMolesIndexCount = moleIndexs.Count;
				foreach (KeyValuePair<int, int> keyValuePair in moleIndexs)
				{
					this.PickedMolesIndex.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x04005350 RID: 21328
			[FixedBufferProperty(typeof(NetworkString<_128>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(24)]
			private FixedStorage@129 _HighScorePlayerName;

			// Token: 0x04005355 RID: 21333
			[FixedBufferProperty(typeof(NetworkDictionary<int, int>), typeof(UnityDictionarySurrogate@ReaderWriter@System_Int32@ReaderWriter@System_Int32), 17, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(556)]
			private FixedStorage@71 _PickedMolesIndex;
		}
	}
}
