using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000153 RID: 339
public sealed class GorillaPropHuntGameManager : GorillaTagManager
{
	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060008D9 RID: 2265 RVA: 0x000305D2 File Offset: 0x0002E7D2
	// (set) Token: 0x060008DA RID: 2266 RVA: 0x000305D9 File Offset: 0x0002E7D9
	public new static GorillaPropHuntGameManager instance { get; private set; }

	// Token: 0x060008DB RID: 2267 RVA: 0x00006691 File Offset: 0x00004891
	public override GameModeType GameType()
	{
		return GameModeType.PropHunt;
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x000305E1 File Offset: 0x0002E7E1
	public override string GameModeName()
	{
		return "PROP HUNT";
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060008DD RID: 2269 RVA: 0x000305E8 File Offset: 0x0002E7E8
	public PropPlacementRB PropDecoyPrefab
	{
		get
		{
			return this.m_ph_propDecoyPrefab;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060008DE RID: 2270 RVA: 0x000305F0 File Offset: 0x0002E7F0
	public float HandFollowDistance
	{
		get
		{
			return this.m_ph_hand_follow_distance;
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060008DF RID: 2271 RVA: 0x000305F8 File Offset: 0x0002E7F8
	public bool RoundIsPlaying
	{
		get
		{
			return this._roundIsPlaying;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060008E0 RID: 2272 RVA: 0x00030600 File Offset: 0x0002E800
	public string[] AllPropIDs_NoPool
	{
		get
		{
			return PropHuntPools.AllPropCosmeticIds;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00030607 File Offset: 0x0002E807
	// (set) Token: 0x060008E2 RID: 2274 RVA: 0x0003060F File Offset: 0x0002E80F
	[DebugReadout]
	private long _ph_timeRoundStartedMillis
	{
		get
		{
			return this.__ph_timeRoundStartedMillis__;
		}
		set
		{
			this.__ph_timeRoundStartedMillis__ = value;
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00030618 File Offset: 0x0002E818
	public int GetSeed()
	{
		return this._ph_randomSeed;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00030620 File Offset: 0x0002E820
	public override void Awake()
	{
		GorillaPropHuntGameManager.instance = this;
		PhotonNetwork.AddCallbackTarget(this);
		base.Awake();
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00030634 File Offset: 0x0002E834
	private void Start()
	{
		PropHuntPools.StartInitializingPropsList(this.m_ph_allCosmetics, this.m_ph_fallbackPropCosmeticSO);
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._Initialize_gorillaGhostBodyMaterialIndex();
		}
		this._Initialize_defaultStencilRefOfSkeletonMat();
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060008E6 RID: 2278 RVA: 0x0003065C File Offset: 0x0002E85C
	public bool IsReadyToSpawnProps_NoPool
	{
		get
		{
			return PropHuntPools.IsReady;
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00030663 File Offset: 0x0002E863
	private void _ProcessPropsList_NoPool(string titleDataPropsLines)
	{
		this._ph_allPropIDs_noPool = titleDataPropsLines.Split(GorillaPropHuntGameManager._g_ph_titleDataSeparators, StringSplitOptions.RemoveEmptyEntries);
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00030678 File Offset: 0x0002E878
	public override void StartPlaying()
	{
		base.StartPlaying();
		bool isMasterClient = PhotonNetwork.IsMasterClient;
		this._ResolveXSceneRefs();
		GameMode.ParticipatingPlayersChanged += this._OnParticipatingPlayersChanged;
		this._UpdateParticipatingPlayers();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000306CC File Offset: 0x0002E8CC
	public override void StopPlaying()
	{
		base.StopPlaying();
		this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode;
		GameMode.ParticipatingPlayersChanged -= this._OnParticipatingPlayersChanged;
		foreach (VRRig rig in GorillaParent.instance.vrrigs)
		{
			GorillaSkin.ApplyToRig(rig, null, GorillaSkin.SkinType.gameMode);
			this._ResetRigAppearance(rig);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = false;
			if (this._ph_playBoundary_initialPosition_isInitialized)
			{
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x000307BC File Offset: 0x0002E9BC
	public override bool CanPlayerParticipate(NetPlayer player)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			VRRig rig = rigContainer.Rig;
			return rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
		}
		return true;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00030804 File Offset: 0x0002EA04
	private void _OnParticipatingPlayersChanged(List<NetPlayer> addedPlayers, List<NetPlayer> removedPlayers)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < addedPlayers.Count; i++)
			{
				NetPlayer infectedPlayer = addedPlayers[i];
				this.AddInfectedPlayer(infectedPlayer, true);
			}
		}
		for (int j = 0; j < removedPlayers.Count; j++)
		{
			NetPlayer netPlayer = removedPlayers[j];
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				if (PhotonNetwork.IsMasterClient)
				{
					while (this.currentInfected.Contains(netPlayer))
					{
						this.currentInfected.Remove(netPlayer);
					}
				}
				VRRig rig = rigContainer.Rig;
				this._ResetRigAppearance(rig);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.UpdateInfectionState();
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x000308A3 File Offset: 0x0002EAA3
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool isCurrentlyTag = this.isCurrentlyTag;
			this.UpdateState();
			if (!isCurrentlyTag && !this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x000308D0 File Offset: 0x0002EAD0
	public override void Tick()
	{
		base.Tick();
		this._UpdateParticipatingPlayers();
		this._UpdateGameState();
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = this._ph_isLocalPlayerParticipating;
			float num = (this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? 0f : Mathf.Clamp01(this._ph_roundTime / this.m_ph_playBoundary_radiusScaleOverRoundTime_maxTime);
			this._ph_playBoundary.radiusScale = this.m_ph_playBoundary_radiusScaleOverRoundTime_curve.Evaluate(num);
			if (this._ph_playBoundary_hasTargetPositionForRound)
			{
				Vector3 position = Vector3.Lerp(this._ph_playBoundary_initialPosition, this._ph_playBoundary_currentTargetPosition, num);
				this._ph_playBoundary.transform.position = position;
			}
			if (this._ph_isLocalPlayerParticipating || (PhotonNetwork.IsMasterClient && GameMode.ParticipatingPlayers.Count > 0))
			{
				this._ph_playBoundary.UpdateSim();
			}
		}
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00030998 File Offset: 0x0002EB98
	public void _UpdateParticipatingPlayers()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[i];
			bool flag = vrrig.zoneEntity.currentZone == GTZone.bayou && vrrig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
			bool flag2 = GameMode.ParticipatingPlayers.Contains(vrrig.OwningNetPlayer);
			if (flag && !flag2)
			{
				GameMode.OptIn(vrrig.OwningNetPlayer.ActorNumber);
			}
			else if (!flag && flag2)
			{
				GameMode.OptOut(vrrig.OwningNetPlayer.ActorNumber);
				this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
			}
		}
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this.m_ph_soundNearBorder_audioSource.gameObject.SetActive(this._ph_isLocalPlayerParticipating);
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00030A80 File Offset: 0x0002EC80
	private void _UpdateGameState()
	{
		this._ph_gameState_lastUpdate = this._ph_gameState;
		long num = GTTime.TimeAsMilliseconds();
		if (GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers;
			this._ph_roundTime = 0f;
		}
		else if (this._ph_timeRoundStartedMillis <= 0L || num < this._ph_timeRoundStartedMillis)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart;
			this._ph_roundTime = 0f;
		}
		else
		{
			this._ph_roundTime = (float)(num - this._ph_timeRoundStartedMillis) / 1000f;
			this._ph_gameState = ((this._ph_roundTime < this.m_ph_hideState_duration) ? GorillaPropHuntGameManager.EPropHuntGameState.Hiding : GorillaPropHuntGameManager.EPropHuntGameState.Playing);
		}
		if (this._ph_gameState != this._ph_gameState_lastUpdate)
		{
			foreach (PlayableBoundaryTracker playableBoundaryTracker in GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.Values)
			{
				playableBoundaryTracker.ResetValues();
			}
		}
		PlayableBoundaryTracker playableBoundaryTracker2;
		if (!this._ph_isLocalPlayerParticipating && GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(VRRig.LocalRig.GetInstanceID(), out playableBoundaryTracker2))
		{
			playableBoundaryTracker2.ResetValues();
		}
		switch (this._ph_gameState)
		{
		case GorillaPropHuntGameManager.EPropHuntGameState.Invalid:
			Debug.LogError("ERROR!!!  GorillaPropHuntGameManager: " + string.Format("Game state was `{0}` but should only be that when the app ", GorillaPropHuntGameManager.EPropHuntGameState.Invalid) + "starts and then assigned during `StartPlaying` call.");
			return;
		case GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.StartingGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers:
			if (this._ph_gameState != this._ph_gameState_lastUpdate)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				this._ph_timeRoundStartedMillis = -1000L;
				this._ResetRigAppearance(rig);
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart:
			this._ph_hideState_warnSounds_timesPlayed = 0;
			if (PhotonNetwork.IsMasterClient && !this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.InfectionRoundEnd();
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.Hiding:
		{
			if (this._ph_gameState != this._ph_gameState_lastUpdate && this.m_ph_hideState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
			{
				this.m_ph_hideState_startSoundBank.Play();
				if (!this._ph_isLocalPlayerSkeleton)
				{
					this.m_ph_soundNearBorder_audioSource.volume = 0f;
				}
			}
			for (int i = 0; i < GameMode.ParticipatingPlayers.Count; i++)
			{
				NetPlayer netPlayer = GameMode.ParticipatingPlayers[i];
				if (this.currentInfected.Contains(netPlayer))
				{
					this._SetPlayerBlindfoldVisibility(netPlayer, true);
				}
			}
			int num2 = this.m_ph_hideState_warnSoundBank_playCount - this._ph_hideState_warnSounds_timesPlayed;
			if (num2 > 0)
			{
				float num3 = this.m_ph_hideState_duration - (float)num2;
				if (this._ph_roundTime > num3 && ZoneManagement.IsInZone(GTZone.bayou))
				{
					if (this.m_ph_hideState_warnSoundBank != null)
					{
						this.m_ph_hideState_warnSoundBank.Play();
					}
					this._ph_hideState_warnSounds_timesPlayed++;
					return;
				}
			}
			break;
		}
		case GorillaPropHuntGameManager.EPropHuntGameState.Playing:
		{
			if (this._ph_gameState_lastUpdate != GorillaPropHuntGameManager.EPropHuntGameState.Playing)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				this._ph_playState_startLightning_strikeTimes_index = 0;
				if (this.m_ph_playState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
				{
					this.m_ph_playState_startSoundBank.Play();
				}
				for (int j = 0; j < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; j++)
				{
					VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[j];
					this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
				}
			}
			int num4 = this.m_ph_playState_startLightning_strikeTimes.Length;
			int num5 = math.min(this._ph_playState_startLightning_strikeTimes_index, num4 - 1);
			if (num5 < num4 && this._ph_playState_startLightning_manager_isResolved)
			{
				float num6 = this._ph_roundTime - this.m_ph_hideState_duration;
				if (this.m_ph_playState_startLightning_strikeTimes[num5] <= num6)
				{
					this._ph_playState_startLightning_strikeTimes_index++;
					this._ph_playState_startLightning_manager.DoLightningStrike();
				}
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00030E04 File Offset: 0x0002F004
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		if (rig.zoneEntity.currentZone != GTZone.bayou || (rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone == GTSubZone.entrance_tunnel))
		{
			return;
		}
		List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
		bool flag = this._GetRigShouldBeSkeleton(rig, participatingPlayers);
		this._ph_isLocalPlayerSkeleton = (this._ph_isLocalPlayerParticipating && !base.IsInfected(NetworkSystem.Instance.LocalPlayer));
		GorillaBodyType gorillaBodyType = flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default;
		int num = flag ? this._ph_gorillaGhostBodyMaterialIndex : 0;
		if (gorillaBodyType != rig.bodyRenderer.gameModeBodyType)
		{
			rig.bodyRenderer.SetGameModeBodyType(gorillaBodyType);
			if (rig.setMatIndex != num)
			{
				rig.ChangeMaterialLocal(num);
			}
		}
		if (PropHuntPools.IsReady)
		{
			bool flag2 = flag;
			if (rig.propHuntHandFollower.hasProp != flag2)
			{
				if (flag2)
				{
					rig.propHuntHandFollower.CreateProp();
				}
				else
				{
					rig.propHuntHandFollower.DestroyProp();
				}
			}
		}
		float signedDistToBoundary = this._UpdateBoundaryProximityState(rig, flag);
		bool flag3 = this._ShouldRigBeVisible(rig, flag, signedDistToBoundary);
		if (!rig.isOfflineVRRig)
		{
			rig.SetInvisibleToLocalPlayer(!flag3);
			if (flag || GorillaBodyRenderer.ForceSkeleton)
			{
				rig.bodyRenderer.bodySkeleton.gameObject.SetActive(flag3);
			}
		}
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00030F35 File Offset: 0x0002F135
	private bool _GetRigShouldBeSkeleton(VRRig rig, List<NetPlayer> participatingPlayers)
	{
		return rig.zoneEntity.currentZone == GTZone.bayou && participatingPlayers.Count >= 2 && participatingPlayers.Contains(rig.OwningNetPlayer) && !base.IsInfected(rig.Creator);
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00030F6E File Offset: 0x0002F16E
	private bool _ShouldRigBeVisible(VRRig rig, bool shouldBeSkeleton, float signedDistToBoundary)
	{
		return this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding && (rig.isOfflineVRRig || !shouldBeSkeleton || signedDistToBoundary > 0f || this._ph_isLocalPlayerSkeleton);
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00030F98 File Offset: 0x0002F198
	private float _UpdateBoundaryProximityState(VRRig rig, bool isSkeleton)
	{
		float num = float.MinValue;
		float num2 = float.MinValue;
		if (isSkeleton)
		{
			PlayableBoundaryTracker playableBoundaryTracker;
			if (!GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(rig.GetInstanceID(), out playableBoundaryTracker))
			{
				rig.bodyTransform.GetOrAddComponent(out playableBoundaryTracker);
				GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers[rig.GetInstanceID()] = playableBoundaryTracker;
				if (this._ph_playBoundary_isResolved)
				{
					this._ph_playBoundary.tracked.AddIfNew(playableBoundaryTracker);
				}
			}
			num = playableBoundaryTracker.signedDistanceToBoundary;
			num2 = playableBoundaryTracker.prevSignedDistanceToBoundary;
			if (PhotonNetwork.IsMasterClient && !playableBoundaryTracker.IsInsideZone() && playableBoundaryTracker.timeSinceCrossingBorder > this.m_ph_playBoundary_timeLimit)
			{
				this.AddInfectedPlayer(rig.OwningNetPlayer, true);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(isSkeleton);
			if (isSkeleton)
			{
				float time = 1f - math.saturate(-num / this.m_ph_soundNearBorder_maxDistance);
				AudioSource ph_soundNearBorder_audioSource = this.m_ph_soundNearBorder_audioSource;
				GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
				ph_soundNearBorder_audioSource.volume = ((ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Hiding || ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? (this.m_ph_soundNearBorder_baseVolume * this.m_ph_soundNearBorder_volumeCurve.Evaluate(time)) : 0f);
				if (num >= 0f && num2 < 0f && !this.m_ph_planeCrossingSoundBank.isPlaying)
				{
					this.m_ph_planeCrossingSoundBank.Play();
				}
				this._UpdateControllerHaptics(num);
			}
			else
			{
				this.m_ph_soundNearBorder_audioSource.volume = 0f;
			}
		}
		return num;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x000310E4 File Offset: 0x0002F2E4
	private void _UpdateControllerHaptics(float signedDistToBoundary)
	{
		if (Time.unscaledTime < GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime || math.abs(signedDistToBoundary) > this.m_ph_hapticsNearBorder_borderProximity)
		{
			return;
		}
		float time = 1f - math.saturate(-signedDistToBoundary / this.m_ph_hapticsNearBorder_borderProximity);
		float num = this.m_ph_hapticsNearBorder_ampCurve.Evaluate(time);
		float amplitude = math.saturate(this.m_ph_hapticsNearBorder_baseAmp * num * (GorillaTagger.Instance.tapHapticStrength * 2f));
		GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime = Time.unscaledTime + 0.1f;
		InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0U, amplitude, 0.1f);
		InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0U, amplitude, 0.1f);
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x0003118C File Offset: 0x0002F38C
	private void _Initialize_defaultStencilRefOfSkeletonMat()
	{
		if (GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat == -1 && this._ph_gorillaGhostBodyMaterialIndex != -1)
		{
			Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
			if (materialsToChangeTo != null && materialsToChangeTo.Length >= 1 && VRRig.LocalRig.materialsToChangeTo[0] != null)
			{
				GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = (int)VRRig.LocalRig.materialsToChangeTo[this._ph_gorillaGhostBodyMaterialIndex].GetFloat(ShaderProps._StencilReference);
				return;
			}
		}
		else
		{
			GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = 7;
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x000311FC File Offset: 0x0002F3FC
	private void _Initialize_gorillaGhostBodyMaterialIndex()
	{
		this._ph_gorillaGhostBodyMaterialIndex = -1;
		Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
		for (int i = 0; i < materialsToChangeTo.Length; i++)
		{
			if (materialsToChangeTo[i].name.StartsWith(this.m_ph_gorillaGhostBodyMaterial.name))
			{
				this._ph_gorillaGhostBodyMaterialIndex = i;
				break;
			}
		}
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._ph_gorillaGhostBodyMaterialIndex = 15;
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00031260 File Offset: 0x0002F460
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
		if ((ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing && ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding) || !GameMode.ParticipatingPlayers.Contains(forPlayer) || base.IsInfected(forPlayer))
		{
			return 0;
		}
		return this._ph_gorillaGhostBodyMaterialIndex;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000312A0 File Offset: 0x0002F4A0
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.InfectionRoundEndCheck();
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x000312AE File Offset: 0x0002F4AE
	private void InfectionRoundEndCheck()
	{
		this._roundIsPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x000312C4 File Offset: 0x0002F4C4
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this._ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing && base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x000312DC File Offset: 0x0002F4DC
	private void _ResetRigAppearance(VRRig rig)
	{
		rig.bodyRenderer.bodySkeleton.gameObject.SetActive(true);
		rig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
		this._SetPlayerBlindfoldVisibility(rig, rig.OwningNetPlayer, false);
		rig.ChangeMaterialLocal(0);
		rig.SetInvisibleToLocalPlayer(false);
		if (rig == VRRig.LocalRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		}
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_allHandFollowers.Count; i++)
		{
			PropHuntHandFollower propHuntHandFollower = GorillaPropHuntGameManager._g_ph_allHandFollowers[i];
			if (propHuntHandFollower.attachedToRig == rig && propHuntHandFollower.hasProp)
			{
				propHuntHandFollower.DestroyProp();
			}
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x0003137E File Offset: 0x0002F57E
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.InfectionRoundStartCheck();
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x0003138C File Offset: 0x0002F58C
	private void InfectionRoundStartCheck()
	{
		this._roundIsPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._ph_randomSeed = UnityEngine.Random.Range(1, int.MaxValue);
			this.PH_OnRoundStartRPC(GTTime.TimeAsMilliseconds(), this._ph_randomSeed);
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x000313BE File Offset: 0x0002F5BE
	public override void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
	{
		base.AddInfectedPlayer(infectedPlayer, withTagStop);
		if (infectedPlayer.IsLocal)
		{
			this.m_ph_playState_taggedSoundBank.Play();
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000313DC File Offset: 0x0002F5DC
	private void _ResolveXSceneRefs()
	{
		if (!this._isListeningForXSceneRefLoadCallbacks)
		{
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_PlayBoundary));
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_PlayBoundary));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_LightningManager));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_LightningManager));
		}
		this._OnXSceneRefLoaded_PlayBoundary();
		if (VRRig.LocalRig.zoneEntity.currentZone == GTZone.bayou)
		{
			this._OnXSceneRefLoaded_LightningManager();
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x0003146C File Offset: 0x0002F66C
	private void _OnXSceneRefLoaded_PlayBoundary()
	{
		if (!this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary_isResolved = (this.m_ph_playBoundary_xSceneRef.TryResolve<PlayableBoundaryManager>(out this._ph_playBoundary) && this._ph_playBoundary != null);
			if (this._ph_playBoundary_isResolved)
			{
				PlayableBoundaryManager ph_playBoundary = this._ph_playBoundary;
				if (ph_playBoundary.tracked == null)
				{
					ph_playBoundary.tracked = new List<PlayableBoundaryTracker>(10);
				}
				this._ph_playBoundary.tracked.Clear();
				if (!this._ph_playBoundary_initialPosition_isInitialized)
				{
					this._ph_playBoundary_initialPosition_isInitialized = true;
					this._ph_playBoundary_initialPosition = this._ph_playBoundary.transform.position;
					this._ph_playBoundary_hasTargetPositionForRound = false;
				}
			}
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x0003150C File Offset: 0x0002F70C
	private void _OnXSceneRefUnloaded_PlayBoundary()
	{
		this._ph_playBoundary_isResolved = false;
		this._ph_playBoundary = null;
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00031523 File Offset: 0x0002F723
	private void _OnXSceneRefLoaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = (this.m_ph_playState_startLightning_manager_ref.TryResolve<LightningManager>(out this._ph_playState_startLightning_manager) && this._ph_playState_startLightning_manager != null);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x0003154D File Offset: 0x0002F74D
	private void _OnXSceneRefUnloaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = false;
		this._ph_playState_startLightning_manager = null;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00031560 File Offset: 0x0002F760
	public void PH_OnRoundEnd()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			this._ResetRigAppearance(GorillaPropHuntGameManager._g_ph_activePlayerRigs[i]);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (LckSocialCameraManager.Instance != null)
		{
			LckSocialCameraManager.Instance.SetForceHidden(false);
		}
		this._ph_timeRoundStartedMillis = -1000L;
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x0003162A File Offset: 0x0002F82A
	public void PH_OnRoundStartRPC(long timeRoundStartedMillis, int seed)
	{
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this._ph_timeRoundStartedMillis = timeRoundStartedMillis;
		this._ph_randomSeed = seed;
		this._PH_OnRoundStart();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0003165C File Offset: 0x0002F85C
	private void _PH_OnRoundStart()
	{
		if (this._ph_playBoundary_isResolved)
		{
			SRand srand = new SRand(this._ph_randomSeed);
			int index = srand.NextInt(this.m_ph_playBoundary_endPointTransforms.Count);
			Transform transform = this.m_ph_playBoundary_endPointTransforms[index];
			if (transform != null)
			{
				this._ph_playBoundary_currentTargetPosition = transform.position;
				this._ph_playBoundary_hasTargetPositionForRound = true;
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		else if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		if (PropHuntPools.IsReady)
		{
			this.SpawnProps();
		}
		else if (!this._isListeningTo_Pools_OnReady)
		{
			PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
		}
		if (this._ph_isLocalPlayerParticipating)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (LckSocialCameraManager.Instance != null)
			{
				LckSocialCameraManager.Instance.SetForceHidden(true);
			}
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0003175C File Offset: 0x0002F95C
	private void _Pools_OnReady()
	{
		if (PhotonNetwork.IsMasterClient || this._ph_isLocalPlayerParticipating)
		{
			this.SpawnProps();
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00031773 File Offset: 0x0002F973
	public static void RegisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Add(propZone);
		if (GorillaPropHuntGameManager.instance != null && PropHuntPools.IsReady)
		{
			propZone.OnRoundStart();
		}
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00031794 File Offset: 0x0002F994
	public static void UnregisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Remove(propZone);
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000317A2 File Offset: 0x0002F9A2
	public static void RegisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Add(hand);
		if (GorillaPropHuntGameManager.instance != null)
		{
			hand.OnRoundStart();
		}
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000317BC File Offset: 0x0002F9BC
	public static void UnregisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Remove(hand);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x000317CC File Offset: 0x0002F9CC
	public void SpawnProps()
	{
		if (!PropHuntPools.IsReady)
		{
			if (!this._isListeningTo_Pools_OnReady)
			{
				PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
			}
			return;
		}
		foreach (PropHuntPropZone propHuntPropZone in GorillaPropHuntGameManager._g_ph_allPropZones)
		{
			propHuntPropZone.OnRoundStart();
		}
		foreach (PropHuntHandFollower propHuntHandFollower in GorillaPropHuntGameManager._g_ph_allHandFollowers)
		{
			if (GameMode.ParticipatingPlayers.Contains(propHuntHandFollower.attachedToRig.OwningNetPlayer))
			{
				propHuntHandFollower.OnRoundStart();
			}
		}
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000318A4 File Offset: 0x0002FAA4
	public string GetCosmeticId(uint randomUInt)
	{
		if (PropHuntPools.AllPropCosmeticIds == null)
		{
			return this.m_ph_fallbackPropCosmeticSO.info.playFabID;
		}
		return PropHuntPools.AllPropCosmeticIds[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)PropHuntPools.AllPropCosmeticIds.Length)))))];
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x000318D0 File Offset: 0x0002FAD0
	public GTAssetRef<GameObject> GetPropRef_NoPool(uint randomUInt, out CosmeticSO out_debugCosmeticSO)
	{
		if (this.AllPropIDs_NoPool == null)
		{
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		string cosmeticID = this.AllPropIDs_NoPool[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)this.AllPropIDs_NoPool.Length)))))];
		return this.GetPropRefByCosmeticID_NoPool(cosmeticID, out out_debugCosmeticSO);
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00031928 File Offset: 0x0002FB28
	public GTAssetRef<GameObject> GetPropRefByCosmeticID_NoPool(string cosmeticID, out CosmeticSO out_debugCosmeticSO)
	{
		CosmeticSO cosmeticSO = this.m_ph_allCosmetics.SearchForCosmeticSO(cosmeticID);
		if (cosmeticSO == null)
		{
			GTDev.LogError<string>("ERROR!!!  GorillaPropHuntGameManager.GetPropRefByCosmeticID_NoPool: Got cosmetic id from title data, but could not find \"" + cosmeticID + "\".", null);
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		if (cosmeticSO.info.wardrobeParts.Length == 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Invalid prop ",
				cosmeticID,
				" ",
				cosmeticSO.info.displayName,
				" has no wardrobeParts"
			}));
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		out_debugCosmeticSO = cosmeticSO;
		return cosmeticSO.info.wardrobeParts[0].prefabAssetRef;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00031A0C File Offset: 0x0002FC0C
	private void _SetPlayerBlindfoldVisibility(NetPlayer netPlayer, bool shouldEnable)
	{
		VRRig vrrig = this.FindPlayerVRRig(netPlayer);
		if (vrrig == null && netPlayer.InRoom)
		{
			return;
		}
		this._SetPlayerBlindfoldVisibility(vrrig, netPlayer, shouldEnable);
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00031A3C File Offset: 0x0002FC3C
	private void _SetPlayerBlindfoldVisibility(VRRig vrRig, NetPlayer netPlayer, bool shouldEnable)
	{
		if (netPlayer == VRRig.LocalRig.OwningNetPlayer)
		{
			if (!this._ph_blindfold_forCamera_isInitialized)
			{
				this._InitializeBlindfoldForCamera();
			}
			if (this._ph_blindfold_forCamera_isInitialized)
			{
				this._ph_blindfold_forCamera_1p.SetActive(shouldEnable);
				this._ph_blindfold_forCamera_3p.SetActive(shouldEnable);
				return;
			}
		}
		else
		{
			GameObject gameObject;
			if (!this._ph_vrRig_to_blindfolds.TryGetValue(vrRig.GetInstanceID(), out gameObject))
			{
				Transform[] boneXforms;
				string text;
				if (!GTHardCodedBones.TryGetBoneXforms(vrRig, out boneXforms, out text))
				{
					return;
				}
				Transform parent;
				if (!GTHardCodedBones.TryGetBoneXform(boneXforms, GTHardCodedBones.EBone.head, out parent))
				{
					return;
				}
				if (this.m_ph_blindfold_forAvatarPrefab == null)
				{
					return;
				}
				gameObject = Object.Instantiate<GameObject>(this.m_ph_blindfold_forAvatarPrefab, parent);
				this._ph_vrRig_to_blindfolds[vrRig.GetInstanceID()] = gameObject;
			}
			gameObject.SetActive(shouldEnable);
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00031AE8 File Offset: 0x0002FCE8
	private void _InitializeBlindfoldForCamera()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		if (this.m_ph_blindfold_forCameraPrefab == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_1p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, mainCamera.transform);
		Camera camera = null;
		if (GorillaTagger.Instance.thirdPersonCamera != null)
		{
			camera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		}
		if (camera == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_3p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, camera.transform);
		this._ph_blindfold_forCamera_isInitialized = (this._ph_blindfold_forCamera_1p != null);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00031B98 File Offset: 0x0002FD98
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		this._ph_randomSeed = (int)stream.ReceiveNext();
		long ph_timeRoundStartedMillis = this._ph_timeRoundStartedMillis;
		this._ph_timeRoundStartedMillis = (long)stream.ReceiveNext();
		if (ph_timeRoundStartedMillis != this._ph_timeRoundStartedMillis)
		{
			if (this._ph_timeRoundStartedMillis > 0L)
			{
				this._PH_OnRoundStart();
				return;
			}
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00031BF4 File Offset: 0x0002FDF4
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this._ph_randomSeed);
		stream.SendNext(this._ph_timeRoundStartedMillis);
	}

	// Token: 0x04000A76 RID: 2678
	private const string preLog = "GorillaPropHuntGameManager: ";

	// Token: 0x04000A77 RID: 2679
	private const string preLogEd = "(editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x04000A78 RID: 2680
	private const string preLogBeta = "(beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x04000A79 RID: 2681
	private const string preErr = "ERROR!!!  GorillaPropHuntGameManager: ";

	// Token: 0x04000A7A RID: 2682
	private const string preErrEd = "ERROR!!!  (editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x04000A7B RID: 2683
	private const string preErrBeta = "ERROR!!!  (beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x04000A7C RID: 2684
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x04000A7E RID: 2686
	[FormerlySerializedAs("allCosmetics")]
	[SerializeField]
	private AllCosmeticsArraySO m_ph_allCosmetics;

	// Token: 0x04000A7F RID: 2687
	[FormerlySerializedAs("backupCosmetic")]
	[FormerlySerializedAs("m_ph_backupCosmetic")]
	[SerializeField]
	private CosmeticSO m_ph_fallbackPropCosmeticSO;

	// Token: 0x04000A80 RID: 2688
	[Tooltip("This us used by PropHuntPools as the parent gameobject that the cosmetic prefab instance will be parented to.")]
	[FormerlySerializedAs("m_ph_propPlacementPrefab")]
	[SerializeField]
	private PropPlacementRB m_ph_propDecoyPrefab;

	// Token: 0x04000A81 RID: 2689
	[Tooltip("The time that players have to hide before their props can be seen by the tagger monke.")]
	[FormerlySerializedAs("m_propHunt_hideState_duration")]
	[SerializeField]
	private float m_ph_hideState_duration = 10f;

	// Token: 0x04000A82 RID: 2690
	[Tooltip("Prefab that will be parented to the camera if the current player is not a ghost during hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_1stPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forCameraPrefab;

	// Token: 0x04000A83 RID: 2691
	private GameObject _ph_blindfold_forCamera_1p;

	// Token: 0x04000A84 RID: 2692
	private GameObject _ph_blindfold_forCamera_3p;

	// Token: 0x04000A85 RID: 2693
	private bool _ph_blindfold_forCamera_isInitialized;

	// Token: 0x04000A86 RID: 2694
	[Tooltip("Prefab to cover the eyes of the non-ghost gorilla's avatar during the hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_3rdPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forAvatarPrefab;

	// Token: 0x04000A87 RID: 2695
	private readonly Dictionary<int, GameObject> _ph_vrRig_to_blindfolds = new Dictionary<int, GameObject>(10);

	// Token: 0x04000A88 RID: 2696
	[Tooltip("A randomly picked sound in this soundbank will be played when the hide state starts.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_startSoundBank;

	// Token: 0x04000A89 RID: 2697
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank")]
	[Tooltip("A randomly picked Sound in this Sound Bank will be played to warn players that the hiding period is ending.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_warnSoundBank;

	// Token: 0x04000A8A RID: 2698
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank_playCount")]
	[Tooltip("How many times should the warning sound play before the hiding period ends? Will play every 1 second.")]
	[SerializeField]
	private int m_ph_hideState_warnSoundBank_playCount = 3;

	// Token: 0x04000A8B RID: 2699
	private int _ph_hideState_warnSounds_timesPlayed;

	// Token: 0x04000A8C RID: 2700
	[FormerlySerializedAs("m_propHunt_playState_startSoundBank")]
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the hiding state ends and the playing state has started.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_startSoundBank;

	// Token: 0x04000A8D RID: 2701
	[FormerlySerializedAs("m_propHunt_playState_startLightning_manager_ref")]
	[Tooltip("Lightning manager for doing lightning strike strikes when playing starts.")]
	[SerializeField]
	private XSceneRef m_ph_playState_startLightning_manager_ref;

	// Token: 0x04000A8E RID: 2702
	private LightningManager _ph_playState_startLightning_manager;

	// Token: 0x04000A8F RID: 2703
	private bool _ph_playState_startLightning_manager_isResolved;

	// Token: 0x04000A90 RID: 2704
	[Tooltip("How long after the playing starts should the lightning strikes happen?")]
	private float[] m_ph_playState_startLightning_strikeTimes = new float[]
	{
		1f,
		1.5f,
		1.8f
	};

	// Token: 0x04000A91 RID: 2705
	private int _ph_playState_startLightning_strikeTimes_index;

	// Token: 0x04000A92 RID: 2706
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the ghost is tagged by the hunter.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_taggedSoundBank;

	// Token: 0x04000A93 RID: 2707
	[Tooltip("Maximum distance prop can be from the center of the player's hand")]
	[SerializeField]
	private float m_ph_hand_follow_distance = 0.35f;

	// Token: 0x04000A94 RID: 2708
	[FormerlySerializedAs("_playBoundary_xSceneRef")]
	[FormerlySerializedAs("_playZone_xSceneRef")]
	[SerializeField]
	private XSceneRef m_ph_playBoundary_xSceneRef;

	// Token: 0x04000A95 RID: 2709
	[Tooltip("A list of Transforms representing potential end positions for the playable boundary each round.")]
	[SerializeField]
	private List<Transform> m_ph_playBoundary_endPointTransforms = new List<Transform>();

	// Token: 0x04000A96 RID: 2710
	private PlayableBoundaryManager _ph_playBoundary;

	// Token: 0x04000A97 RID: 2711
	private bool _ph_playBoundary_isResolved;

	// Token: 0x04000A98 RID: 2712
	private Vector3 _ph_playBoundary_initialPosition;

	// Token: 0x04000A99 RID: 2713
	private bool _ph_playBoundary_initialPosition_isInitialized;

	// Token: 0x04000A9A RID: 2714
	private Vector3 _ph_playBoundary_currentTargetPosition;

	// Token: 0x04000A9B RID: 2715
	private bool _ph_playBoundary_hasTargetPositionForRound;

	// Token: 0x04000A9C RID: 2716
	[Tooltip("The maximum time a player can be outside of the boundary before being tagged.")]
	[SerializeField]
	private float m_ph_playBoundary_timeLimit = 15f;

	// Token: 0x04000A9D RID: 2717
	[Tooltip("On the What does 1.0 on the X axis")]
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_maxTime")]
	[SerializeField]
	private float m_ph_playBoundary_radiusScaleOverRoundTime_maxTime = 180f;

	// Token: 0x04000A9E RID: 2718
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_curve")]
	[FormerlySerializedAs("_playZoneRadiusOverRoundTime")]
	[SerializeField]
	private AnimationCurve m_ph_playBoundary_radiusScaleOverRoundTime_curve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f, 1f, 1f, 0f, 0f),
		new Keyframe(0.9f, 0.01f, 1f, 0f, 0f, 0f),
		new Keyframe(1f, 0.01f, 1f, 0f, 0f, 0f)
	});

	// Token: 0x04000A9F RID: 2719
	[FormerlySerializedAs("_ph_gorillaGhostBodyMaterial")]
	[FormerlySerializedAs("gorillaGhostBodyMaterial")]
	[SerializeField]
	private Material m_ph_gorillaGhostBodyMaterial;

	// Token: 0x04000AA0 RID: 2720
	private int _ph_gorillaGhostBodyMaterialIndex = -1;

	// Token: 0x04000AA1 RID: 2721
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the spectral plane border is crossed.")]
	[SerializeField]
	private SoundBankPlayer m_ph_planeCrossingSoundBank;

	// Token: 0x04000AA2 RID: 2722
	[Tooltip("This AudioSource will only be heard by the local player and is non directional.")]
	[FormerlySerializedAs("m_soundNearBorder_audioSource")]
	[FormerlySerializedAs("soundNearBorderAudioSource")]
	[FormerlySerializedAs("soundNearBoundaryAudioSource")]
	[SerializeField]
	private AudioSource m_ph_soundNearBorder_audioSource;

	// Token: 0x04000AA3 RID: 2723
	[FormerlySerializedAs("m_soundNearBorder_maxDistance")]
	[FormerlySerializedAs("soundNearBorderMaxDistance")]
	[FormerlySerializedAs("soundNearBoundaryMaxDistance")]
	[SerializeField]
	private float m_ph_soundNearBorder_maxDistance = 2f;

	// Token: 0x04000AA4 RID: 2724
	[FormerlySerializedAs("m_soundNearBorder_volumeCurve")]
	[FormerlySerializedAs("soundNearBorderVolumeCurve")]
	[FormerlySerializedAs("soundNearBoundaryVolumeCurve")]
	[SerializeField]
	private AnimationCurve m_ph_soundNearBorder_volumeCurve = AnimationCurves.Linear;

	// Token: 0x04000AA5 RID: 2725
	[Tooltip("The resulting volume curve value is multiplied by this.")]
	[FormerlySerializedAs("m_soundNearBorder_baseVolume")]
	[SerializeField]
	private float m_ph_soundNearBorder_baseVolume = 0.5f;

	// Token: 0x04000AA6 RID: 2726
	[FormerlySerializedAs("m_hapticsNearBorder_borderProximity")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_borderProximity = 2f;

	// Token: 0x04000AA7 RID: 2727
	[FormerlySerializedAs("m_hapticsNearBorder_ampCurve")]
	[SerializeField]
	private AnimationCurve m_ph_hapticsNearBorder_ampCurve = AnimationCurves.Linear;

	// Token: 0x04000AA8 RID: 2728
	[FormerlySerializedAs("m_hapticsNearBorder_baseAmp")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_baseAmp = 1f;

	// Token: 0x04000AA9 RID: 2729
	private bool _ph_isLocalPlayerSkeleton;

	// Token: 0x04000AAA RID: 2730
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, PlayableBoundaryTracker> _g_ph_rig_to_propHuntZoneTrackers = new Dictionary<int, PlayableBoundaryTracker>(10);

	// Token: 0x04000AAB RID: 2731
	[OnEnterPlay_Set(0f)]
	private static float _g_ph_hapticsLastImpulseEndTime;

	// Token: 0x04000AAC RID: 2732
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> _g_ph_activePlayerRigs = new List<VRRig>(10);

	// Token: 0x04000AAD RID: 2733
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntPropZone> _g_ph_allPropZones = new List<PropHuntPropZone>();

	// Token: 0x04000AAE RID: 2734
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntHandFollower> _g_ph_allHandFollowers = new List<PropHuntHandFollower>();

	// Token: 0x04000AAF RID: 2735
	private static readonly string[] _g_ph_titleDataSeparators = new string[]
	{
		"\"",
		" ",
		"\\n"
	};

	// Token: 0x04000AB0 RID: 2736
	[OnEnterPlay_Set(-1)]
	private static int _g_ph_defaultStencilRefOfSkeletonMat = -1;

	// Token: 0x04000AB1 RID: 2737
	[DebugReadout]
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState;

	// Token: 0x04000AB2 RID: 2738
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState_lastUpdate;

	// Token: 0x04000AB3 RID: 2739
	private bool _roundIsPlaying;

	// Token: 0x04000AB4 RID: 2740
	private string[] _ph_allPropIDs_noPool;

	// Token: 0x04000AB5 RID: 2741
	[DebugReadout]
	private float _ph_roundTime;

	// Token: 0x04000AB6 RID: 2742
	private long __ph_timeRoundStartedMillis__;

	// Token: 0x04000AB7 RID: 2743
	private int _ph_randomSeed;

	// Token: 0x04000AB8 RID: 2744
	private bool _ph_isLocalPlayerParticipating;

	// Token: 0x04000AB9 RID: 2745
	private bool _isListeningTo_Pools_OnReady;

	// Token: 0x04000ABA RID: 2746
	private bool _isListeningForXSceneRefLoadCallbacks;

	// Token: 0x02000154 RID: 340
	private enum EPropHuntGameState
	{
		// Token: 0x04000ABC RID: 2748
		Invalid,
		// Token: 0x04000ABD RID: 2749
		StoppedGameMode,
		// Token: 0x04000ABE RID: 2750
		StartingGameMode,
		// Token: 0x04000ABF RID: 2751
		WaitingForMorePlayers,
		// Token: 0x04000AC0 RID: 2752
		WaitingForRoundToStart,
		// Token: 0x04000AC1 RID: 2753
		Hiding,
		// Token: 0x04000AC2 RID: 2754
		Playing
	}
}
