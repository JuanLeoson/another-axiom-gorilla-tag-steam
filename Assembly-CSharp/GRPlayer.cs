using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200064C RID: 1612
public class GRPlayer : MonoBehaviour
{
	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002788 RID: 10120 RVA: 0x000D5322 File Offset: 0x000D3522
	public GRPlayer.GRPlayerState State
	{
		get
		{
			return this.state;
		}
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002789 RID: 10121 RVA: 0x000D532A File Offset: 0x000D352A
	public int MaxHp
	{
		get
		{
			return this.maxHp;
		}
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x0600278A RID: 10122 RVA: 0x000D5332 File Offset: 0x000D3532
	public int MaxShieldHp
	{
		get
		{
			return this.maxShieldHp;
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x0600278B RID: 10123 RVA: 0x000D533A File Offset: 0x000D353A
	public int Hp
	{
		get
		{
			return this.hp;
		}
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x0600278C RID: 10124 RVA: 0x000D5342 File Offset: 0x000D3542
	public int ShieldHp
	{
		get
		{
			return this.shieldHp;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x0600278D RID: 10125 RVA: 0x000D534A File Offset: 0x000D354A
	// (set) Token: 0x0600278E RID: 10126 RVA: 0x000D5352 File Offset: 0x000D3552
	public float ShiftPlayTime
	{
		get
		{
			return this.shiftPlayTime;
		}
		set
		{
			this.shiftPlayTime = value;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x0600278F RID: 10127 RVA: 0x000D535B File Offset: 0x000D355B
	// (set) Token: 0x06002790 RID: 10128 RVA: 0x000D5363 File Offset: 0x000D3563
	public int LastShiftCut
	{
		get
		{
			return this.lastShiftCut;
		}
		set
		{
			this.lastShiftCut = value;
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002791 RID: 10129 RVA: 0x000D536C File Offset: 0x000D356C
	// (set) Token: 0x06002792 RID: 10130 RVA: 0x000D5374 File Offset: 0x000D3574
	public GRPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
		set
		{
			this.currentProgression = value;
		}
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000D5380 File Offset: 0x000D3580
	private void Awake()
	{
		this.vrRig = base.GetComponent<VRRig>();
		this.lowHealthVisualPropertyBlock = new MaterialPropertyBlock();
		this.damageEffects = GTPlayer.Instance.mainCamera.GetComponent<GRPlayerDamageEffects>();
		this.lowHealthTintPropertyId = Shader.PropertyToID("_TintColor");
		this.currency = 0;
		this.isEmployee = false;
		this.hp = this.maxHp;
		this.shieldHp = 0;
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.requestCollectItemLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestChargeToolLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestDepositCurrencyLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestShiftStartLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestToolPurchaseStationLimiter = new CallLimiter(25, 1f, 0.5f);
		this.applyEnemyHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportLocalHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportBreakableBrokenLimiter = new CallLimiter(25, 1f, 0.5f);
		this.playerStateChangeLimiter = new CallLimiter(25, 1f, 0.5f);
		this.promotionBotLimiter = new CallLimiter(25, 1f, 0.5f);
		this.progressionBroadcastLimiter = new CallLimiter(25, 1f, 0.5f);
		this.scoreboardPageLimiter = new CallLimiter(25, 1f, 0.5f);
		this.fireShieldLimiter = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000D5544 File Offset: 0x000D3744
	private void Start()
	{
		if (this.gamePlayer != null && this.gamePlayer.IsLocal())
		{
			this.LoadMyProgression();
			return;
		}
		this.currentProgression = new GRPlayer.ProgressionData
		{
			points = 0,
			redeemedPoints = 0
		};
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000D5592 File Offset: 0x000D3792
	private void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000D559A File Offset: 0x000D379A
	public void Reset()
	{
		this.currency = 0;
		this.hp = this.maxHp;
		this.shieldHp = 0;
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.RefreshPlayerVisuals();
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000D55CC File Offset: 0x000D37CC
	public void OnPlayerHit(Vector3 hitPosition, GhostReactorManager manager)
	{
		if (this.State == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp > 0)
			{
				this.shieldHp = Mathf.Max(this.shieldHp - 1, 0);
				if (this.shieldHp > 0)
				{
					if (this.shieldDamagedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDamagedSound, this.shieldDamagedVolume);
					}
					this.shieldDamagedEffect.Play();
				}
				else
				{
					if (this.shieldDestroyedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDestroyedSound, this.shieldDestroyedVolume);
					}
					this.shieldDestroyedEffect.Play();
				}
				this.RefreshPlayerVisuals();
				return;
			}
			this.PlayHitFx(hitPosition);
			this.hp = Mathf.Max(this.hp - 1, 0);
			this.RefreshDamageVignetteVisual();
			if (this.hp <= 0)
			{
				this.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, manager);
			}
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000D56A8 File Offset: 0x000D38A8
	public void OnPlayerRevive(GhostReactorManager manager)
	{
		this.hp = this.maxHp;
		this.RefreshDamageVignetteVisual();
		this.ChangePlayerState(GRPlayer.GRPlayerState.Alive, manager);
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000D56C4 File Offset: 0x000D38C4
	public void ChangePlayerState(GRPlayer.GRPlayerState newState, GhostReactorManager manager)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			newState = GRPlayer.GRPlayerState.Alive;
		}
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState == GRPlayer.GRPlayerState.Ghost)
			{
				this.hp = 0;
				this.shieldHp = 0;
				this.RefreshDamageVignetteVisual();
				if (this.playerTurnedGhostEffect != null)
				{
					this.playerTurnedGhostEffect.Play();
				}
				this.playerTurnedGhostSoundBank.Play();
				manager.ReportPlayerDeath();
				this.deaths++;
			}
		}
		else
		{
			this.hp = this.maxHp;
			this.RefreshDamageVignetteVisual();
			if (this.playerRevivedEffect != null)
			{
				this.playerRevivedEffect.Play();
			}
			if (this.audioSource != null && this.playerRevivedSound != null)
			{
				this.audioSource.PlayOneShot(this.playerRevivedSound, this.playerRevivedVolume);
			}
		}
		this.RefreshPlayerVisuals();
		if (this.vrRig.isLocal)
		{
			this.vrRigs.Clear();
			VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
			for (int i = 0; i < this.vrRigs.Count; i++)
			{
				this.vrRigs[i].GetComponent<GRPlayer>().RefreshPlayerVisuals();
			}
		}
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000D5808 File Offset: 0x000D3A08
	public void RefreshPlayerVisuals()
	{
		this.RefreshDamageVignetteVisual();
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState != GRPlayer.GRPlayerState.Ghost)
			{
				return;
			}
			this.gamePlayer.DisableGrabbing(true);
			this.shieldHeadVisual.gameObject.SetActive(false);
			this.shieldBodyVisual.gameObject.SetActive(false);
			if (this.badge != null)
			{
				this.badge.Hide();
			}
			if (this.vrRig.isLocal)
			{
				GamePlayerLocal.instance.OnUpdateInteract();
				this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
				this.vrRig.ChangeMaterialLocal(13);
				this.vrRig.SetInvisibleToLocalPlayer(false);
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(true);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(true, this.deathTintColor);
				GameLightingManager.instance.SetAmbientLightDynamic(this.deathAmbientLightColor);
				return;
			}
			if (VRRigCache.Instance.localRig.GetComponent<GRPlayer>().State == GRPlayer.GRPlayerState.Ghost)
			{
				this.vrRig.ChangeMaterialLocal(13);
				this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
				this.vrRig.SetInvisibleToLocalPlayer(false);
				return;
			}
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Invisible);
			this.vrRig.SetInvisibleToLocalPlayer(true);
			return;
		}
		else
		{
			this.gamePlayer.DisableGrabbing(false);
			if (this.badge != null)
			{
				this.badge.UnHide();
			}
			this.vrRig.ChangeMaterialLocal(0);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			if (this.vrRig.isLocal)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(false, Color.black);
				Color ambientLightDynamic = Color.black;
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					int depthLevel = instance.GetDepthLevel();
					ambientLightDynamic = instance.GetDepthLevelConfig(depthLevel).configGenOptions[0].ambientLight;
				}
				GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
			}
			if (this.shieldHp > 0)
			{
				this.shieldHeadVisual.gameObject.SetActive(true);
				this.shieldBodyVisual.gameObject.SetActive(true);
				return;
			}
			this.shieldHeadVisual.gameObject.SetActive(false);
			this.shieldBodyVisual.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000D5A58 File Offset: 0x000D3C58
	public static GRPlayer Get(int actorNumber)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(actorNumber);
		if (gamePlayer == null)
		{
			return null;
		}
		return gamePlayer.GetComponent<GRPlayer>();
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000D5A7D File Offset: 0x000D3C7D
	public static GRPlayer Get(NetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		return GRPlayer.Get(player.ActorNumber);
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000D5A8F File Offset: 0x000D3C8F
	public static GRPlayer Get(VRRig vrRig)
	{
		if (!(vrRig != null))
		{
			return null;
		}
		return vrRig.GetComponent<GRPlayer>();
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000D5AA2 File Offset: 0x000D3CA2
	public void AttachBadge(GRBadge grBadge)
	{
		this.badge = grBadge;
		this.badge.transform.SetParent(this.badgeBodyAnchor);
		this.badge.GetComponent<Rigidbody>().isKinematic = true;
		this.badge.StartRetracting();
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000D5ADD File Offset: 0x000D3CDD
	public bool CanActivateShield(int shieldHitPoints)
	{
		return this.state == GRPlayer.GRPlayerState.Alive && shieldHitPoints > 0;
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000D5AF0 File Offset: 0x000D3CF0
	public bool TryActivateShield(int shieldHitpoints)
	{
		if (this.state == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp <= 0 && this.shieldActivatedSound != null)
			{
				this.audioSource.PlayOneShot(this.shieldActivatedSound, this.shieldActivatedVolume);
			}
			this.shieldHp = Mathf.Min(shieldHitpoints, this.maxShieldHp);
			this.RefreshPlayerVisuals();
			return true;
		}
		return false;
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000D5B50 File Offset: 0x000D3D50
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		writer.Write((byte)this.state);
		writer.Write((byte)this.hp);
		writer.Write((byte)this.shieldHp);
		writer.Write(this.currency);
		writer.Write(this.shiftJoinTime);
		writer.Write(this.isEmployee ? 1 : 0);
		writer.Write(this.CurrentProgression.points);
		writer.Write(this.CurrentProgression.redeemedPoints);
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000D5BD4 File Offset: 0x000D3DD4
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, GRPlayer player, GhostReactorManager grManager)
	{
		GRPlayer.GRPlayerState newState = (GRPlayer.GRPlayerState)reader.ReadByte();
		int num = (int)reader.ReadByte();
		int num2 = (int)reader.ReadByte();
		int num3 = reader.ReadInt32();
		double num4 = reader.ReadDouble();
		bool flag = reader.ReadByte() > 0;
		int points = reader.ReadInt32();
		int redeemedPoints = reader.ReadInt32();
		if (player != null)
		{
			player.currency = num3;
			player.hp = num;
			player.shieldHp = num2;
			player.isEmployee = flag;
			player.ChangePlayerState(newState, grManager);
			player.RefreshPlayerVisuals();
			if (!player.gamePlayer.IsLocal())
			{
				player.SetProgressionData(points, redeemedPoints, false);
			}
			if (double.IsNaN(num4) || double.IsInfinity(num4))
			{
				player.shiftJoinTime = PhotonNetwork.Time;
				return;
			}
			player.shiftJoinTime = Math.Min(num4, PhotonNetwork.Time);
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000D5C9C File Offset: 0x000D3E9C
	public void PlayHitFx(Vector3 attackLocation)
	{
		if (this.playerDamageAudioSource != null)
		{
			this.playerDamageAudioSource.PlayOneShot(this.playerDamageSound, this.playerDamageVolume);
		}
		if (this.bodyCenter != null)
		{
			Vector3 vector = attackLocation - this.bodyCenter.position;
			vector.y = 0f;
			Vector3 b = vector.normalized * this.playerDamageOffsetDist;
			if (this.playerDamageEffect != null)
			{
				this.playerDamageEffect.transform.position = this.bodyCenter.position + b;
				this.playerDamageEffect.Play();
			}
			if (this.vrRig.isLocal)
			{
				Vector3 normalized = Vector3.ProjectOnPlane(GTPlayer.Instance.mainCamera.transform.forward, Vector3.up).normalized;
				vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;
				float num = Vector3.SignedAngle(normalized, vector, Vector3.up);
				this.damageEffects.radialDamageEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -num);
				this.damageEffects.radialDamageEffect.Play();
			}
		}
		if (this.gamePlayer == GamePlayerLocal.instance.gamePlayer)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, 0.5f);
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000D5E20 File Offset: 0x000D4020
	public void SendShiftStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorShiftStart(this.gameId, this.currency, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000D5E70 File Offset: 0x000D4070
	public void SendShiftEndedTelemetry(bool isShiftActuallyEnding, float shiftStartTime, ZoneClearReason zoneClearReason)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorShiftEnd(this.gameId, this.currency, this.coresSpentWhileWaiting, this.coresCollectedFromGhosts, this.coresCollectedFromGathering, this.coresSpentOnItems, this.coresSpentOnGates, this.coresSpentOnLevels, this.coresGiven, this.coresReceived, this.gatesUnlocked, this.deaths, this.caughtByAnomaly, this.itemsPurchased, this.levelsUnlocked, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)(this.timeIntoShiftAtJoin + shiftStartTime)), this.wasPlayerInAtShiftStart, zoneClearReason, (float)this.coresCollected, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.itemTypesHeldThisShift);
		this.coresSpentWhileWaiting = 0;
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000D5F44 File Offset: 0x000D4144
	public void ResetTelemetryTracking(string newGameId, float timeSinceShiftStart)
	{
		this.gameId = newGameId;
		this.coresCollectedFromGhosts = 0;
		this.coresCollectedFromGathering = 0;
		this.coresCollected = 0;
		this.coresSpentOnItems = 0;
		this.coresSpentOnGates = 0;
		this.coresSpentOnLevels = 0;
		this.coresGiven = 0;
		this.coresReceived = 0;
		this.gatesUnlocked = 0;
		this.deaths = 0;
		this.caughtByAnomaly = false;
		this.itemsPurchased = new List<string>();
		this.levelsUnlocked = new List<string>();
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersInShift = this.vrRigs.Count + 1;
		this.timeIntoShiftAtJoin = timeSinceShiftStart;
		this.itemsHeldThisShift.Clear();
		this.itemTypesHeldThisShift.Clear();
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000D6008 File Offset: 0x000D4208
	public void GrabbedItem(GameEntityId id, string itemName)
	{
		if (this.itemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.itemsHeldThisShift.Add(id);
		if (this.itemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.itemTypesHeldThisShift[itemName] = this.itemTypesHeldThisShift[itemName] + 1;
			return;
		}
		this.itemTypesHeldThisShift[itemName] = 1;
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000D6068 File Offset: 0x000D4268
	[ContextMenu("Refresh Damage Vignette Visual")]
	public void RefreshDamageVignetteVisual()
	{
		if (this.vrRig.isLocal && this.currentHealthVisualValue != this.hp)
		{
			this.currentHealthVisualValue = this.hp;
			if (this.hp <= this.damageOverlayMaxHp && this.hp > 0)
			{
				if (this.lowHeathVisualCoroutine != null)
				{
					base.StopCoroutine(this.lowHeathVisualCoroutine);
				}
				this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(true);
				this.lowHeathVisualCoroutine = base.StartCoroutine(this.LowHeathVisualCoroutine());
				return;
			}
			this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x000D6109 File Offset: 0x000D4309
	private IEnumerator LowHeathVisualCoroutine()
	{
		int index = this.hp - 1;
		if (index >= 0 && index < this.damageOverlayValues.Count)
		{
			float startTime = Time.time;
			while (Time.time - startTime < this.damageOverlayValues[index].effectDuration)
			{
				float time = Mathf.Clamp01((Time.time - startTime) / this.damageOverlayValues[index].effectDuration);
				float num = this.damageOverlayValues[index].effectCurve.Evaluate(time);
				Color tint = this.damageOverlayValues[index].tint;
				tint.a *= num;
				this.damageEffects.lowHealthVisualRenderer.GetPropertyBlock(this.lowHealthVisualPropertyBlock);
				this.lowHealthVisualPropertyBlock.SetColor(this.lowHealthTintPropertyId, tint);
				this.damageEffects.lowHealthVisualRenderer.SetPropertyBlock(this.lowHealthVisualPropertyBlock);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000D6118 File Offset: 0x000D4318
	public void CollectShiftCut()
	{
		this.SetProgressionData(this.currentProgression.points + this.LastShiftCut, this.currentProgression.redeemedPoints, true);
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x000D6140 File Offset: 0x000D4340
	public void AttemptPromotion()
	{
		int num = GhostReactorProgression.TotalPointsForNextGrade(this.currentProgression.redeemedPoints);
		if (this.currentProgression.points >= num)
		{
			this.SetProgressionData(this.currentProgression.points, num, false);
		}
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x000D6180 File Offset: 0x000D4380
	public void SetProgressionData(int _points, int _redeemedPoints, bool saveProgression = false)
	{
		if (_points < 0 || _redeemedPoints < 0)
		{
			return;
		}
		this.currentProgression = new GRPlayer.ProgressionData
		{
			points = _points,
			redeemedPoints = _redeemedPoints
		};
		if (this.gamePlayer.IsLocal() && saveProgression)
		{
			this.SaveMyProgression();
		}
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x000D61CA File Offset: 0x000D43CA
	public void LoadMyProgression()
	{
		GhostReactorProgression.instance.GetStartingProgression(this);
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000D61D7 File Offset: 0x000D43D7
	public void SaveMyProgression()
	{
		GhostReactorProgression.instance.SetProgression(this.LastShiftCut, this);
	}

	// Token: 0x040032BD RID: 12989
	public GamePlayer gamePlayer;

	// Token: 0x040032BE RID: 12990
	private GRPlayer.GRPlayerState state;

	// Token: 0x040032BF RID: 12991
	public int currency;

	// Token: 0x040032C0 RID: 12992
	public int unlockPointsCurrency;

	// Token: 0x040032C1 RID: 12993
	public double shiftJoinTime;

	// Token: 0x040032C2 RID: 12994
	public bool isEmployee;

	// Token: 0x040032C3 RID: 12995
	public AudioSource audioSource;

	// Token: 0x040032C4 RID: 12996
	[Header("Hit / Revive Effects")]
	public ParticleSystem playerTurnedGhostEffect;

	// Token: 0x040032C5 RID: 12997
	public SoundBankPlayer playerTurnedGhostSoundBank;

	// Token: 0x040032C6 RID: 12998
	public ParticleSystem playerRevivedEffect;

	// Token: 0x040032C7 RID: 12999
	public AudioClip playerRevivedSound;

	// Token: 0x040032C8 RID: 13000
	public float playerRevivedVolume = 1f;

	// Token: 0x040032C9 RID: 13001
	public AudioSource playerDamageAudioSource;

	// Token: 0x040032CA RID: 13002
	public Transform bodyCenter;

	// Token: 0x040032CB RID: 13003
	public ParticleSystem playerDamageEffect;

	// Token: 0x040032CC RID: 13004
	public float playerDamageVolume = 1f;

	// Token: 0x040032CD RID: 13005
	public AudioClip playerDamageSound;

	// Token: 0x040032CE RID: 13006
	public float playerDamageOffsetDist = 0.25f;

	// Token: 0x040032CF RID: 13007
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathTintColor;

	// Token: 0x040032D0 RID: 13008
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathAmbientLightColor;

	// Token: 0x040032D1 RID: 13009
	[Header("Attach")]
	public Transform attachEnemy;

	// Token: 0x040032D2 RID: 13010
	[Header("Shield")]
	public Transform shieldHeadVisual;

	// Token: 0x040032D3 RID: 13011
	public Transform shieldBodyVisual;

	// Token: 0x040032D4 RID: 13012
	public AudioClip shieldActivatedSound;

	// Token: 0x040032D5 RID: 13013
	public float shieldActivatedVolume = 0.5f;

	// Token: 0x040032D6 RID: 13014
	public ParticleSystem shieldDamagedEffect;

	// Token: 0x040032D7 RID: 13015
	public AudioClip shieldDamagedSound;

	// Token: 0x040032D8 RID: 13016
	public float shieldDamagedVolume = 0.5f;

	// Token: 0x040032D9 RID: 13017
	public ParticleSystem shieldDestroyedEffect;

	// Token: 0x040032DA RID: 13018
	public AudioClip shieldDestroyedSound;

	// Token: 0x040032DB RID: 13019
	public float shieldDestroyedVolume = 0.5f;

	// Token: 0x040032DC RID: 13020
	[Header("Badge")]
	public Transform badgeBodyAnchor;

	// Token: 0x040032DD RID: 13021
	[SerializeField]
	private Transform badgeBodyStringAttach;

	// Token: 0x040032DE RID: 13022
	[Header("Health")]
	[SerializeField]
	private int maxHp = 1;

	// Token: 0x040032DF RID: 13023
	[SerializeField]
	private int maxShieldHp = 1;

	// Token: 0x040032E0 RID: 13024
	private int hp;

	// Token: 0x040032E1 RID: 13025
	private int shieldHp;

	// Token: 0x040032E2 RID: 13026
	[Header("Damage Vignette")]
	[SerializeField]
	[Tooltip("First entry is 1 hp, second entry is 2 hp, etc.")]
	private List<GRPlayer.DamageOverlayValues> damageOverlayValues = new List<GRPlayer.DamageOverlayValues>();

	// Token: 0x040032E3 RID: 13027
	[SerializeField]
	private int damageOverlayMaxHp = 1;

	// Token: 0x040032E4 RID: 13028
	[HideInInspector]
	public GRBadge badge;

	// Token: 0x040032E5 RID: 13029
	public CallLimiter requestCollectItemLimiter;

	// Token: 0x040032E6 RID: 13030
	public CallLimiter requestChargeToolLimiter;

	// Token: 0x040032E7 RID: 13031
	public CallLimiter requestDepositCurrencyLimiter;

	// Token: 0x040032E8 RID: 13032
	public CallLimiter requestShiftStartLimiter;

	// Token: 0x040032E9 RID: 13033
	public CallLimiter requestToolPurchaseStationLimiter;

	// Token: 0x040032EA RID: 13034
	public CallLimiter applyEnemyHitLimiter;

	// Token: 0x040032EB RID: 13035
	public CallLimiter reportLocalHitLimiter;

	// Token: 0x040032EC RID: 13036
	public CallLimiter reportBreakableBrokenLimiter;

	// Token: 0x040032ED RID: 13037
	public CallLimiter playerStateChangeLimiter;

	// Token: 0x040032EE RID: 13038
	public CallLimiter promotionBotLimiter;

	// Token: 0x040032EF RID: 13039
	public CallLimiter progressionBroadcastLimiter;

	// Token: 0x040032F0 RID: 13040
	public CallLimiter scoreboardPageLimiter;

	// Token: 0x040032F1 RID: 13041
	public CallLimiter fireShieldLimiter;

	// Token: 0x040032F2 RID: 13042
	private VRRig vrRig;

	// Token: 0x040032F3 RID: 13043
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x040032F4 RID: 13044
	private string gameId;

	// Token: 0x040032F5 RID: 13045
	public int coresSpentWhileWaiting;

	// Token: 0x040032F6 RID: 13046
	public int coresCollectedFromGhosts;

	// Token: 0x040032F7 RID: 13047
	public int coresCollectedFromGathering;

	// Token: 0x040032F8 RID: 13048
	public int coresSpentOnItems;

	// Token: 0x040032F9 RID: 13049
	public int coresSpentOnGates;

	// Token: 0x040032FA RID: 13050
	public int coresSpentOnLevels;

	// Token: 0x040032FB RID: 13051
	public int coresGiven;

	// Token: 0x040032FC RID: 13052
	public int coresReceived;

	// Token: 0x040032FD RID: 13053
	public int gatesUnlocked;

	// Token: 0x040032FE RID: 13054
	public int deaths;

	// Token: 0x040032FF RID: 13055
	public bool caughtByAnomaly;

	// Token: 0x04003300 RID: 13056
	public List<string> itemsPurchased;

	// Token: 0x04003301 RID: 13057
	public List<string> levelsUnlocked;

	// Token: 0x04003302 RID: 13058
	public float timeIntoShiftAtJoin;

	// Token: 0x04003303 RID: 13059
	public bool wasPlayerInAtShiftStart;

	// Token: 0x04003304 RID: 13060
	public int coresCollected;

	// Token: 0x04003305 RID: 13061
	public int sentientCoresCollected;

	// Token: 0x04003306 RID: 13062
	public int maxNumberOfPlayersInShift;

	// Token: 0x04003307 RID: 13063
	private HashSet<GameEntityId> itemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x04003308 RID: 13064
	private Dictionary<string, int> itemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x04003309 RID: 13065
	private GRPlayerDamageEffects damageEffects;

	// Token: 0x0400330A RID: 13066
	private MaterialPropertyBlock lowHealthVisualPropertyBlock;

	// Token: 0x0400330B RID: 13067
	private int lowHealthTintPropertyId;

	// Token: 0x0400330C RID: 13068
	private int currentHealthVisualValue;

	// Token: 0x0400330D RID: 13069
	private Coroutine lowHeathVisualCoroutine;

	// Token: 0x0400330E RID: 13070
	private GRPlayer.ProgressionData currentProgression;

	// Token: 0x0400330F RID: 13071
	private float shiftPlayTime;

	// Token: 0x04003310 RID: 13072
	private int lastShiftCut;

	// Token: 0x0200064D RID: 1613
	public enum GRPlayerState
	{
		// Token: 0x04003312 RID: 13074
		Alive,
		// Token: 0x04003313 RID: 13075
		Ghost,
		// Token: 0x04003314 RID: 13076
		Shielded
	}

	// Token: 0x0200064E RID: 1614
	[Serializable]
	private struct DamageOverlayValues
	{
		// Token: 0x04003315 RID: 13077
		public Color tint;

		// Token: 0x04003316 RID: 13078
		public float effectDuration;

		// Token: 0x04003317 RID: 13079
		public AnimationCurve effectCurve;
	}

	// Token: 0x0200064F RID: 1615
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x04003318 RID: 13080
		public int points;

		// Token: 0x04003319 RID: 13081
		public int redeemedPoints;
	}

	// Token: 0x02000650 RID: 1616
	[Serializable]
	public struct ProgressionLevels
	{
		// Token: 0x0400331A RID: 13082
		public int tierId;

		// Token: 0x0400331B RID: 13083
		public string tierName;

		// Token: 0x0400331C RID: 13084
		public int grades;

		// Token: 0x0400331D RID: 13085
		public int pointsPerGrade;
	}
}
