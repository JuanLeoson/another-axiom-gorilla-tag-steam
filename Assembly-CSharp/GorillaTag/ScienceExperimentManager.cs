using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CjLib;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag
{
	// Token: 0x02000E78 RID: 3704
	[NetworkBehaviourWeaved(76)]
	public class ScienceExperimentManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06005CAB RID: 23723 RVA: 0x001D2C8C File Offset: 0x001D0E8C
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Erupting || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising && this.riseProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining && this.riseProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06005CAC RID: 23724 RVA: 0x001D2CF0 File Offset: 0x001D0EF0
		public ScienceExperimentManager.RisingLiquidState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06005CAD RID: 23725 RVA: 0x001D2CFD File Offset: 0x001D0EFD
		public float RiseProgress
		{
			get
			{
				return this.riseProgress;
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06005CAE RID: 23726 RVA: 0x001D2D05 File Offset: 0x001D0F05
		public float RiseProgressLinear
		{
			get
			{
				return this.riseProgressLinear;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06005CAF RID: 23727 RVA: 0x001D2D10 File Offset: 0x001D0F10
		private int PlayerCount
		{
			get
			{
				int result = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentNetPlayerArray != null)
				{
					result = gorillaGameManager.currentNetPlayerArray.Length;
				}
				return result;
			}
		}

		// Token: 0x06005CB0 RID: 23728 RVA: 0x001D2D40 File Offset: 0x001D0F40
		protected override void Awake()
		{
			base.Awake();
			if (ScienceExperimentManager.instance == null)
			{
				ScienceExperimentManager.instance = this;
				NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
				this.riseTimeLookup = new float[]
				{
					this.riseTimeFast,
					this.riseTimeMedium,
					this.riseTimeSlow,
					this.riseTimeExtraSlow
				};
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
				GorillaGameManager.OnTouch += this.OnPlayerTagged;
				RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
				RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
				this.rotatingRings = new ScienceExperimentManager.RotatingRingState[this.ringParent.childCount];
				for (int i = 0; i < this.rotatingRings.Length; i++)
				{
					this.rotatingRings[i].ringTransform = this.ringParent.GetChild(i);
					this.rotatingRings[i].initialAngle = 0f;
					this.rotatingRings[i].resultingAngle = 0f;
				}
				this.gameAreaTriggerNotifier.CompositeTriggerEnter += this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit += this.OnColliderExitedVolume;
				this.liquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				this.entryLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				if (this.bottleLiquidVolume != null)
				{
					this.bottleLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
					this.bottleLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				}
				if (this.refreshWaterVolume != null)
				{
					this.refreshWaterVolume.ColliderEnteredWater += this.OnColliderEnteredRefreshWater;
					this.refreshWaterVolume.ColliderExitedWater += this.OnColliderExitedRefreshWater;
				}
				if (this.sodaWaterProjectileTriggerNotifier != null)
				{
					this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter += this.OnProjectileEnteredSodaWater;
				}
				float num = Vector3.Distance(this.drainBlockerClosedPosition.position, this.drainBlockerOpenPosition.position);
				this.drainBlockerSlideSpeed = num / this.drainBlockerSlideTime;
				return;
			}
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x06005CB1 RID: 23729 RVA: 0x001D2FE0 File Offset: 0x001D11E0
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005CB2 RID: 23730 RVA: 0x001D2FF4 File Offset: 0x001D11F4
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005CB3 RID: 23731 RVA: 0x001D3008 File Offset: 0x001D1208
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			GorillaGameManager.OnTouch -= this.OnPlayerTagged;
			if (this.gameAreaTriggerNotifier != null)
			{
				this.gameAreaTriggerNotifier.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit -= this.OnColliderExitedVolume;
			}
			if (this.liquidVolume != null)
			{
				this.liquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.entryLiquidVolume != null)
			{
				this.entryLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.bottleLiquidVolume != null)
			{
				this.bottleLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.bottleLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.refreshWaterVolume != null)
			{
				this.refreshWaterVolume.ColliderEnteredWater -= this.OnColliderEnteredRefreshWater;
				this.refreshWaterVolume.ColliderExitedWater -= this.OnColliderExitedRefreshWater;
			}
			if (this.sodaWaterProjectileTriggerNotifier != null)
			{
				this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter -= this.OnProjectileEnteredSodaWater;
			}
		}

		// Token: 0x06005CB4 RID: 23732 RVA: 0x001D3180 File Offset: 0x001D1380
		public void InitElements(ScienceExperimentSceneElements elements)
		{
			this.elements = elements;
			this.fizzParticleEmission = elements.sodaFizzParticles.emission;
			elements.sodaFizzParticles.gameObject.SetActive(false);
			elements.sodaEruptionParticles.gameObject.SetActive(false);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		}

		// Token: 0x06005CB5 RID: 23733 RVA: 0x001D31E2 File Offset: 0x001D13E2
		public void DeInitElements()
		{
			this.elements = null;
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x001D31EC File Offset: 0x001D13EC
		public Transform GetElement(ScienceExperimentElementID elementID)
		{
			switch (elementID)
			{
			case ScienceExperimentElementID.Platform1:
				return this.rotatingRings[0].ringTransform;
			case ScienceExperimentElementID.Platform2:
				return this.rotatingRings[1].ringTransform;
			case ScienceExperimentElementID.Platform3:
				return this.rotatingRings[2].ringTransform;
			case ScienceExperimentElementID.Platform4:
				return this.rotatingRings[3].ringTransform;
			case ScienceExperimentElementID.Platform5:
				return this.rotatingRings[4].ringTransform;
			case ScienceExperimentElementID.LiquidMesh:
				return this.liquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberLiquidMesh:
				return this.entryWayLiquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberBridgeQuad:
				return this.entryWayBridgeQuadTransform;
			case ScienceExperimentElementID.DrainBlocker:
				return this.drainBlocker;
			default:
				Debug.LogError(string.Format("Unhandled ScienceExperiment element ID! {0}", elementID));
				return null;
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06005CB7 RID: 23735 RVA: 0x001D32B1 File Offset: 0x001D14B1
		// (set) Token: 0x06005CB8 RID: 23736 RVA: 0x001D32B9 File Offset: 0x001D14B9
		bool ITickSystemTick.TickRunning { get; set; }

		// Token: 0x06005CB9 RID: 23737 RVA: 0x001D32C4 File Offset: 0x001D14C4
		void ITickSystemTick.Tick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.unscaledTimeAsDouble);
			this.lastInfrequentUpdateTime = ((this.lastInfrequentUpdateTime > this.currentTime) ? this.currentTime : this.lastInfrequentUpdateTime);
			if (this.currentTime > this.lastInfrequentUpdateTime + (double)this.infrequentUpdatePeriod)
			{
				this.InfrequentUpdate();
				this.lastInfrequentUpdateTime = (double)((float)this.currentTime);
			}
			if (base.IsMine)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagRiseProgressOffset = Mathf.MoveTowards(this.localLagRiseProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLiquid(this.riseProgress + this.localLagRiseProgressOffset);
			this.UpdateRotatingRings(this.ringRotationProgress);
			this.UpdateRefreshWater();
			this.UpdateDrainBlocker(this.currentTime);
			this.DisableObjectsInContactWithLava(this.liquidMeshTransform.localScale.z);
			this.UpdateEffects();
			if (this.debugDrawPlayerGameState)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					NetPlayer netPlayer = null;
					if (NetworkSystem.Instance.InRoom)
					{
						netPlayer = NetworkSystem.Instance.GetPlayer(this.inGamePlayerStates[i].playerId);
					}
					else if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
					{
						netPlayer = NetworkSystem.Instance.LocalPlayer;
					}
					RigContainer rigContainer;
					if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						float num = 0.03f;
						DebugUtil.DrawSphere(rigContainer.Rig.transform.position + Vector3.up * 0.5f * num, 0.16f * num, 12, 12, this.inGamePlayerStates[i].touchedLiquid ? Color.red : Color.green, true, DebugUtil.Style.SolidColor);
					}
				}
			}
		}

		// Token: 0x06005CBA RID: 23738 RVA: 0x001D34E4 File Offset: 0x001D16E4
		private void InfrequentUpdate()
		{
			this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
			if (base.IsMine)
			{
				for (int i = this.inGamePlayerCount - 1; i >= 0; i--)
				{
					int playerId = this.inGamePlayerStates[i].playerId;
					bool flag = false;
					for (int j = 0; j < this.allPlayersInRoom.Length; j++)
					{
						if (this.allPlayersInRoom[j].ActorNumber == playerId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (i < this.inGamePlayerCount - 1)
						{
							this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						}
						this.inGamePlayerStates[this.inGamePlayerCount - 1] = default(ScienceExperimentManager.PlayerGameState);
						this.inGamePlayerCount--;
					}
				}
			}
			if (this.optPlayersOutOfRoomGameMode)
			{
				for (int k = 0; k < this.allPlayersInRoom.Length; k++)
				{
					bool flag2 = false;
					for (int l = 0; l < this.inGamePlayerCount; l++)
					{
						if (this.allPlayersInRoom[k].ActorNumber == this.inGamePlayerStates[l].playerId)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						GorillaGameModes.GameMode.OptOut(this.allPlayersInRoom[k]);
					}
					else
					{
						GorillaGameModes.GameMode.OptIn(this.allPlayersInRoom[k]);
					}
				}
			}
		}

		// Token: 0x06005CBB RID: 23739 RVA: 0x001D3630 File Offset: 0x001D1830
		private bool PlayerInGame(Player player)
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005CBC RID: 23740 RVA: 0x001D366C File Offset: 0x001D186C
		private void UpdateReliableState(double currentTime, ref ScienceExperimentManager.SyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
			{
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() > 0 && syncData.activationProgress > 1.0)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Erupting;
					syncData.stateStartTime = currentTime;
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.activationProgress = 1.0;
					return;
				}
				float num = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.fullyDrainedWaitTime)
				{
					this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Rising;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = Mathf.Clamp01((float)((currentTime - syncData.stateStartTime) / (double)this.riseTime));
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Full;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Full:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0 || currentTime > syncData.stateStartTime + (double)this.maxFullTime)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				if (currentTime > syncData.stateStartTime + (double)this.preDrainWaitTime)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Draining;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
					for (int i = 0; i < this.rotatingRings.Length; i++)
					{
						float num2 = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
						float num3 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
						float num4 = (Random.Range(0f, 1f) > 0.5f) ? 1f : -1f;
						this.rotatingRings[i].initialAngle = num2;
						this.rotatingRings[i].resultingAngle = num2 + num4 * num3;
					}
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num5 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				if (currentTime + num5 > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Drained;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
				}
				break;
			}
			}
		}

		// Token: 0x06005CBD RID: 23741 RVA: 0x001D3948 File Offset: 0x001D1B48
		private void UpdateLocalState(double currentTime, ScienceExperimentManager.SyncData syncData)
		{
			switch (syncData.state)
			{
			default:
				this.riseProgressLinear = 0f;
				this.riseProgress = 0f;
				if (!this.debugRandomizingRings)
				{
					this.ringRotationProgress = 1f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
			{
				double num = (currentTime - syncData.stateStartTime) / (double)this.riseTime;
				this.riseProgressLinear = Mathf.Clamp01((float)num);
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Full:
				this.riseProgressLinear = 1f;
				this.riseProgress = 1f;
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				this.riseProgressLinear = syncData.stateStartLiquidProgressLinear;
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num2 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				double num3 = (currentTime + num2 - syncData.stateStartTime) / (double)this.drainTime;
				this.riseProgressLinear = Mathf.Clamp01((float)(1.0 - num3));
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = (float)(currentTime - syncData.stateStartTime) / (this.drainTime * syncData.stateStartLiquidProgressLinear);
				break;
			}
			}
		}

		// Token: 0x06005CBE RID: 23742 RVA: 0x001D3AB4 File Offset: 0x001D1CB4
		private void UpdateLiquid(float fillProgress)
		{
			float num = Mathf.Lerp(this.minScale, this.maxScale, fillProgress);
			this.liquidMeshTransform.localScale = new Vector3(1f, 1f, num);
			bool active = this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Full || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.PreDrainDelay || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining;
			this.liquidMeshTransform.gameObject.SetActive(active);
			if (this.entryWayLiquidMeshTransform != null)
			{
				float y = 0f;
				float z;
				float z2;
				if (num < this.entryLiquidScaleSyncOpeningBottom.y)
				{
					z = this.entryLiquidScaleSyncOpeningBottom.x;
					z2 = this.entryBridgeQuadMinMaxZHeight.x;
				}
				else if (num < this.entryLiquidScaleSyncOpeningTop.y)
				{
					float num2 = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningBottom.y, this.entryLiquidScaleSyncOpeningTop.y, num);
					z = Mathf.Lerp(this.entryLiquidScaleSyncOpeningBottom.x, this.entryLiquidScaleSyncOpeningTop.x, num2);
					z2 = Mathf.Lerp(this.entryBridgeQuadMinMaxZHeight.x, this.entryBridgeQuadMinMaxZHeight.y, num2);
					y = this.entryBridgeQuadMaxScaleY * Mathf.Sin(num2 * 3.1415927f);
				}
				else
				{
					float t = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningTop.y, 0.6f * this.maxScale, num);
					z = Mathf.Lerp(this.entryLiquidScaleSyncOpeningTop.x, this.entryLiquidMaxScale, t);
					z2 = this.entryBridgeQuadMinMaxZHeight.y;
				}
				this.entryWayLiquidMeshTransform.localScale = new Vector3(this.entryWayLiquidMeshTransform.localScale.x, this.entryWayLiquidMeshTransform.localScale.y, z);
				this.entryWayBridgeQuadTransform.localScale = new Vector3(this.entryWayBridgeQuadTransform.localScale.x, y, this.entryWayBridgeQuadTransform.localScale.z);
				this.entryWayBridgeQuadTransform.localPosition = new Vector3(this.entryWayBridgeQuadTransform.localPosition.x, this.entryWayBridgeQuadTransform.localPosition.y, z2);
			}
		}

		// Token: 0x06005CBF RID: 23743 RVA: 0x001D3CD8 File Offset: 0x001D1ED8
		private void UpdateRotatingRings(float rotationProgress)
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float angle = Mathf.Lerp(this.rotatingRings[i].initialAngle, this.rotatingRings[i].resultingAngle, rotationProgress);
				this.rotatingRings[i].ringTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
			}
		}

		// Token: 0x06005CC0 RID: 23744 RVA: 0x001D3D44 File Offset: 0x001D1F44
		private void UpdateDrainBlocker(double currentTime)
		{
			if (this.reliableState.state != ScienceExperimentManager.RisingLiquidState.Draining)
			{
				this.drainBlocker.position = this.drainBlockerClosedPosition.position;
				return;
			}
			float num = (float)(currentTime - this.reliableState.stateStartTime);
			float num2 = (1f - this.reliableState.stateStartLiquidProgressLinear) * this.drainTime;
			if (this.drainTime - (num + num2) < this.drainBlockerSlideTime)
			{
				this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerClosedPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
				return;
			}
			this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerOpenPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
		}

		// Token: 0x06005CC1 RID: 23745 RVA: 0x001D3E18 File Offset: 0x001D2018
		private void UpdateEffects()
		{
			switch (this.reliableState.state)
			{
			case ScienceExperimentManager.RisingLiquidState.Drained:
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.elements.sodaFizzParticles.gameObject.SetActive(true);
					if (this.reliableState.activationProgress > 0.0010000000474974513)
					{
						this.fizzParticleEmission.rateOverTimeMultiplier = Mathf.Lerp(this.sodaFizzParticleEmissionMinMax.x, this.sodaFizzParticleEmissionMinMax.y, (float)this.reliableState.activationProgress);
						return;
					}
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (!this.hasPlayedEruptionEffects)
				{
					this.eruptionAudioSource.loop = true;
					this.eruptionAudioSource.GTPlay();
					this.hasPlayedEruptionEffects = true;
					if (this.elements != null)
					{
						this.elements.sodaEruptionParticles.gameObject.SetActive(true);
						this.fizzParticleEmission.rateOverTimeMultiplier = this.sodaFizzParticleEmissionMinMax.y;
						return;
					}
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.elements != null)
				{
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			default:
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
				this.hasPlayedEruptionEffects = false;
				this.eruptionAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				if (!this.hasPlayedDrainEffects)
				{
					this.drainAudioSource.loop = true;
					this.drainAudioSource.GTPlay();
					this.rotatingRingsAudioSource.loop = true;
					this.rotatingRingsAudioSource.GTPlay();
					this.hasPlayedDrainEffects = true;
				}
				break;
			}
		}

		// Token: 0x06005CC2 RID: 23746 RVA: 0x001D40B4 File Offset: 0x001D22B4
		private void DisableObjectsInContactWithLava(float lavaScale)
		{
			if (this.elements == null)
			{
				return;
			}
			Plane plane = new Plane(this.liquidSurfacePlane.up, this.liquidSurfacePlane.position);
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				for (int i = 0; i < this.elements.disableByLiquidList.Count; i++)
				{
					if (!plane.GetSide(this.elements.disableByLiquidList[i].target.position + this.elements.disableByLiquidList[i].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[i].target.gameObject.SetActive(false);
					}
				}
				return;
			}
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining)
			{
				for (int j = 0; j < this.elements.disableByLiquidList.Count; j++)
				{
					if (plane.GetSide(this.elements.disableByLiquidList[j].target.position + this.elements.disableByLiquidList[j].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[j].target.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06005CC3 RID: 23747 RVA: 0x001D4220 File Offset: 0x001D2420
		private void UpdateWinner()
		{
			float num = -1f;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
					break;
				}
				if (this.inGamePlayerStates[i].touchedLiquidAtProgress > num)
				{
					num = this.inGamePlayerStates[i].touchedLiquidAtProgress;
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
				}
			}
			this.RefreshWinnerName();
		}

		// Token: 0x06005CC4 RID: 23748 RVA: 0x001D42B4 File Offset: 0x001D24B4
		private void RefreshWinnerName()
		{
			NetPlayer playerFromId = this.GetPlayerFromId(this.lastWinnerId);
			if (playerFromId != null)
			{
				this.lastWinnerName = playerFromId.NickName;
				return;
			}
			this.lastWinnerName = "None";
		}

		// Token: 0x06005CC5 RID: 23749 RVA: 0x001D42E9 File Offset: 0x001D24E9
		private NetPlayer GetPlayerFromId(int id)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				return NetworkSystem.Instance.GetPlayer(id);
			}
			if (id == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return NetworkSystem.Instance.LocalPlayer;
			}
			return null;
		}

		// Token: 0x06005CC6 RID: 23750 RVA: 0x001D4324 File Offset: 0x001D2524
		private void UpdateRefreshWater()
		{
			if (this.refreshWaterVolume != null)
			{
				if (this.RefreshWaterAvailable && !this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(true);
					return;
				}
				if (!this.RefreshWaterAvailable && this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005CC7 RID: 23751 RVA: 0x001D4398 File Offset: 0x001D2598
		private void ResetGame()
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
				playerGameState.touchedLiquid = false;
				playerGameState.touchedLiquidAtProgress = -1f;
				this.inGamePlayerStates[i] = playerGameState;
			}
		}

		// Token: 0x06005CC8 RID: 23752 RVA: 0x001D43E4 File Offset: 0x001D25E4
		public void RestartGame()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
				this.ResetGame();
			}
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x001D446C File Offset: 0x001D266C
		public void DebugErupt()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
			}
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x001D44F0 File Offset: 0x001D26F0
		public void RandomizeRings()
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float num = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
				float num2 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
				float num3 = (Random.Range(0f, 1f) > 0.5f) ? 1f : -1f;
				this.rotatingRings[i].initialAngle = num;
				float num4 = num + num3 * num2;
				if (this.rotatingRingQuantizeAngles)
				{
					num4 = Mathf.Round(num4 / this.rotatingRingAngleSnapDegrees) * this.rotatingRingAngleSnapDegrees;
				}
				this.rotatingRings[i].resultingAngle = num4;
			}
			if (this.rotateRingsCoroutine != null)
			{
				base.StopCoroutine(this.rotateRingsCoroutine);
			}
			this.rotateRingsCoroutine = base.StartCoroutine(this.RotateRingsCoroutine());
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x001D45E2 File Offset: 0x001D27E2
		private IEnumerator RotateRingsCoroutine()
		{
			if (this.debugRotateRingsTime > 0.01f)
			{
				float routineStartTime = Time.time;
				this.ringRotationProgress = 0f;
				this.debugRandomizingRings = true;
				while (this.ringRotationProgress < 1f)
				{
					this.ringRotationProgress = (Time.time - routineStartTime) / this.debugRotateRingsTime;
					yield return null;
				}
			}
			this.debugRandomizingRings = false;
			this.ringRotationProgress = 1f;
			yield break;
		}

		// Token: 0x06005CCC RID: 23756 RVA: 0x001D45F4 File Offset: 0x001D27F4
		public bool GetMaterialIfPlayerInGame(int playerActorNumber, out int materialIndex)
		{
			int i = 0;
			while (i < this.inGamePlayerCount)
			{
				if (this.inGamePlayerStates[i].playerId == playerActorNumber)
				{
					if (this.inGamePlayerStates[i].touchedLiquid)
					{
						materialIndex = 12;
						return true;
					}
					materialIndex = 0;
					return true;
				}
				else
				{
					i++;
				}
			}
			materialIndex = 0;
			return false;
		}

		// Token: 0x06005CCD RID: 23757 RVA: 0x001D4648 File Offset: 0x001D2848
		private void OnPlayerTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (base.IsMine)
			{
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == taggedPlayer.ActorNumber)
					{
						num = i;
					}
					else if (this.inGamePlayerStates[i].playerId == taggingPlayer.ActorNumber)
					{
						num2 = i;
					}
					if (num != -1 && num2 != -1)
					{
						break;
					}
				}
				if (num == -1 || num2 == -1)
				{
					return;
				}
				switch (this.tagBehavior)
				{
				case ScienceExperimentManager.TagBehavior.None:
					break;
				case ScienceExperimentManager.TagBehavior.Infect:
					if (this.inGamePlayerStates[num2].touchedLiquid && !this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[num];
						playerGameState.touchedLiquid = true;
						playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						this.inGamePlayerStates[num] = playerGameState;
						return;
					}
					break;
				case ScienceExperimentManager.TagBehavior.Revive:
					if (!this.inGamePlayerStates[num2].touchedLiquid && this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState2 = this.inGamePlayerStates[num];
						playerGameState2.touchedLiquid = false;
						playerGameState2.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[num] = playerGameState2;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x001D478C File Offset: 0x001D298C
		private void OnColliderEnteredVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerEnteredGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x001D47CC File Offset: 0x001D29CC
		private void OnColliderExitedVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerExitedGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x001D480C File Offset: 0x001D2A0C
		private void OnColliderEnteredSoda(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedLava(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnColliderExitedSoda(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x06005CD2 RID: 23762 RVA: 0x001D4860 File Offset: 0x001D2A60
		private void OnColliderEnteredRefreshWater(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedRefreshWater(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedRefreshWaterRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06005CD3 RID: 23763 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnColliderExitedRefreshWater(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x06005CD4 RID: 23764 RVA: 0x001D48B3 File Offset: 0x001D2AB3
		private void OnProjectileEnteredSodaWater(SlingshotProjectile projectile, Collider collider)
		{
			if (projectile.gameObject.CompareTag(this.mentoProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x06005CD5 RID: 23765 RVA: 0x001D48DC File Offset: 0x001D2ADC
		private void AddLavaRock(int playerId)
		{
			if (base.IsMine && this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (!this.inGamePlayerStates[i].touchedLiquid)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
					this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				}
			}
		}

		// Token: 0x06005CD6 RID: 23766 RVA: 0x001D4950 File Offset: 0x001D2B50
		public void OnWaterBalloonHitPlayer(NetPlayer hitPlayer)
		{
			bool flag = false;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == hitPlayer.ActorNumber)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (hitPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.ValidateLocalPlayerWaterBalloonHit(hitPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("ValidateLocalPlayerWaterBalloonHitRPC", RpcTarget.Others, new object[]
				{
					hitPlayer.ActorNumber
				});
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06005CD7 RID: 23767 RVA: 0x001D49CD File Offset: 0x001D2BCD
		// (set) Token: 0x06005CD8 RID: 23768 RVA: 0x001D49F7 File Offset: 0x001D2BF7
		[Networked]
		[NetworkedWeaved(0, 76)]
		private unsafe ScienceExperimentManager.ScienceManagerData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005CD9 RID: 23769 RVA: 0x001D4A24 File Offset: 0x001D2C24
		public override void WriteDataFusion()
		{
			ScienceExperimentManager.ScienceManagerData data = new ScienceExperimentManager.ScienceManagerData((int)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.stateStartLiquidProgressLinear, this.reliableState.activationProgress, (int)this.nextRoundRiseSpeed, this.riseTime, this.lastWinnerId, this.inGamePlayerCount, this.inGamePlayerStates, this.rotatingRings);
			this.Data = data;
		}

		// Token: 0x06005CDA RID: 23770 RVA: 0x001D4A90 File Offset: 0x001D2C90
		public override void ReadDataFusion()
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)this.Data.reliableState;
			this.reliableState.stateStartTime = this.Data.stateStartTime;
			this.reliableState.stateStartLiquidProgressLinear = this.Data.stateStartLiquidProgressLinear.ClampSafe(0f, 1f);
			this.reliableState.activationProgress = this.Data.activationProgress.GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)this.Data.nextRoundRiseSpeed;
			this.riseTime = this.Data.riseTime.GetFinite();
			this.lastWinnerId = this.Data.lastWinnerId;
			this.inGamePlayerCount = Mathf.Clamp(this.Data.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = this.Data.playerIdArray[i];
				this.inGamePlayerStates[i].touchedLiquid = this.Data.touchedLiquidArray[i];
				this.inGamePlayerStates[i].touchedLiquidAtProgress = this.Data.touchedLiquidAtProgressArray[i].ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = this.Data.initialAngleArray[j].GetFinite();
				this.rotatingRings[j].resultingAngle = this.Data.resultingAngleArray[j].GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x06005CDB RID: 23771 RVA: 0x001D4CB8 File Offset: 0x001D2EB8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.stateStartLiquidProgressLinear);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext((int)this.nextRoundRiseSpeed);
			stream.SendNext(this.riseTime);
			stream.SendNext(this.lastWinnerId);
			stream.SendNext(this.inGamePlayerCount);
			for (int i = 0; i < 10; i++)
			{
				stream.SendNext(this.inGamePlayerStates[i].playerId);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquid);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquidAtProgress);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				stream.SendNext(this.rotatingRings[j].initialAngle);
				stream.SendNext(this.rotatingRings[j].resultingAngle);
			}
		}

		// Token: 0x06005CDC RID: 23772 RVA: 0x001D4E10 File Offset: 0x001D3010
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.stateStartLiquidProgressLinear = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)((int)stream.ReceiveNext());
			this.riseTime = ((float)stream.ReceiveNext()).GetFinite();
			this.lastWinnerId = (int)stream.ReceiveNext();
			this.inGamePlayerCount = (int)stream.ReceiveNext();
			this.inGamePlayerCount = Mathf.Clamp(this.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = (int)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquid = (bool)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquidAtProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = ((float)stream.ReceiveNext()).GetFinite();
				this.rotatingRings[j].resultingAngle = ((float)stream.ReceiveNext()).GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x06005CDD RID: 23773 RVA: 0x001D4FF8 File Offset: 0x001D31F8
		private void PlayerEnteredGameArea(int pId)
		{
			if (base.IsMine)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == pId)
					{
						flag = true;
						break;
					}
				}
				if (!flag && this.inGamePlayerCount < 10)
				{
					bool touchedLiquid = false;
					this.inGamePlayerStates[this.inGamePlayerCount] = new ScienceExperimentManager.PlayerGameState
					{
						playerId = pId,
						touchedLiquid = touchedLiquid,
						touchedLiquidAtProgress = -1f
					};
					this.inGamePlayerCount++;
					if (this.optPlayersOutOfRoomGameMode)
					{
						GorillaGameModes.GameMode.OptOut(pId);
					}
				}
			}
		}

		// Token: 0x06005CDE RID: 23774 RVA: 0x001D509C File Offset: 0x001D329C
		private void PlayerExitedGameArea(int playerId)
		{
			if (base.IsMine)
			{
				int i = 0;
				while (i < this.inGamePlayerCount)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						this.inGamePlayerCount--;
						if (this.optPlayersOutOfRoomGameMode)
						{
							GorillaGameModes.GameMode.OptIn(playerId);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x06005CDF RID: 23775 RVA: 0x001D5112 File Offset: 0x001D3312
		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

		// Token: 0x06005CE0 RID: 23776 RVA: 0x001D5130 File Offset: 0x001D3330
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public unsafe void RPC_PlayerTouchedLava(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
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
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			PhotonMessageInfoWrapped infoWrapped = new PhotonMessageInfoWrapped(info);
			GorillaNot.IncrementRPCCall(infoWrapped, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(infoWrapped.Sender.ActorNumber);
		}

		// Token: 0x06005CE1 RID: 23777 RVA: 0x001D5274 File Offset: 0x001D3474
		private void PlayerTouchedLava(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						if (!playerGameState.touchedLiquid)
						{
							playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						}
						playerGameState.touchedLiquid = true;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06005CE2 RID: 23778 RVA: 0x001D52E6 File Offset: 0x001D34E6
		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

		// Token: 0x06005CE3 RID: 23779 RVA: 0x001D5304 File Offset: 0x001D3504
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RPC_PlayerTouchedRefreshWater(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								int capacityInBytes = 8;
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, capacityInBytes);
								byte* data = SimulationMessage.GetData(ptr);
								int num = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2), data);
								ptr->Offset = num * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			PhotonMessageInfoWrapped infoWrapped = new PhotonMessageInfoWrapped(info);
			GorillaNot.IncrementRPCCall(infoWrapped, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(infoWrapped.Sender.ActorNumber);
		}

		// Token: 0x06005CE4 RID: 23780 RVA: 0x001D5448 File Offset: 0x001D3648
		private void PlayerTouchedRefreshWater(int playerId)
		{
			if (base.IsMine && this.RefreshWaterAvailable)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06005CE5 RID: 23781 RVA: 0x001D54B9 File Offset: 0x001D36B9
		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

		// Token: 0x06005CE6 RID: 23782 RVA: 0x001D54E0 File Offset: 0x001D36E0
		[Rpc(InvokeLocal = false)]
		private unsafe void RPC_ValidateLocalPlayerWaterBalloonHit(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (this.InvokeRpc)
			{
				this.InvokeRpc = false;
				GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "ValidateLocalPlayerWaterBalloonHitRPC");
				if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.ValidateLocalPlayerWaterBalloonHit(playerId);
				}
				return;
			}
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else if (base.Runner.HasAnyActiveConnections())
				{
					int num = 8;
					num += 4;
					SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3), data);
					*(int*)(data + num2) = playerId;
					num2 += 4;
					ptr->Offset = num2 * 8;
					base.Runner.SendRpc(ptr);
				}
			}
		}

		// Token: 0x06005CE7 RID: 23783 RVA: 0x001D561C File Offset: 0x001D381C
		private void ValidateLocalPlayerWaterBalloonHit(int playerId)
		{
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber && !GTPlayer.Instance.InWater)
			{
				if (base.IsMine)
				{
					this.PlayerHitByWaterBalloon(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.MasterClient, new object[]
				{
					PhotonNetwork.LocalPlayer.ActorNumber
				});
			}
		}

		// Token: 0x06005CE8 RID: 23784 RVA: 0x001D568E File Offset: 0x001D388E
		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x06005CE9 RID: 23785 RVA: 0x001D56A4 File Offset: 0x001D38A4
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RPC_PlayerHitByWaterBalloon(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								int num = 8;
								num += 4;
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* data = SimulationMessage.GetData(ptr);
								int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 4), data);
								*(int*)(data + num2) = playerId;
								num2 += 4;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x06005CEA RID: 23786 RVA: 0x001D57FC File Offset: 0x001D39FC
		private void PlayerHitByWaterBalloon(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06005CEB RID: 23787 RVA: 0x001D5865 File Offset: 0x001D3A65
		public void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		// Token: 0x06005CEC RID: 23788 RVA: 0x001D5874 File Offset: 0x001D3A74
		public void OnLeftRoom()
		{
			this.inGamePlayerCount = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.inGamePlayerStates[0] = this.inGamePlayerStates[i];
					this.inGamePlayerCount = 1;
					return;
				}
			}
		}

		// Token: 0x06005CED RID: 23789 RVA: 0x001D58DC File Offset: 0x001D3ADC
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!Utils.PlayerInRoom(this.inGamePlayerStates[i].playerId))
				{
					this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
					this.inGamePlayerCount--;
					i--;
				}
			}
		}

		// Token: 0x06005CEF RID: 23791 RVA: 0x001D5B58 File Offset: 0x001D3D58
		[CompilerGenerated]
		private int <UpdateReliableState>g__GetAlivePlayerCount|105_0()
		{
			int num = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06005CF0 RID: 23792 RVA: 0x001D5B90 File Offset: 0x001D3D90
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005CF1 RID: 23793 RVA: 0x001D5BA8 File Offset: 0x001D3DA8
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x06005CF2 RID: 23794 RVA: 0x001D5BBC File Offset: 0x001D3DBC
		[NetworkRpcWeavedInvoker(1, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedLava@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedLava(info);
		}

		// Token: 0x06005CF3 RID: 23795 RVA: 0x001D5C10 File Offset: 0x001D3E10
		[NetworkRpcWeavedInvoker(2, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedRefreshWater@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedRefreshWater(info);
		}

		// Token: 0x06005CF4 RID: 23796 RVA: 0x001D5C64 File Offset: 0x001D3E64
		[NetworkRpcWeavedInvoker(3, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_ValidateLocalPlayerWaterBalloonHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int playerId = num2;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_ValidateLocalPlayerWaterBalloonHit(playerId, info);
		}

		// Token: 0x06005CF5 RID: 23797 RVA: 0x001D5CD4 File Offset: 0x001D3ED4
		[NetworkRpcWeavedInvoker(4, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerHitByWaterBalloon@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int playerId = num2;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerHitByWaterBalloon(playerId, info);
		}

		// Token: 0x04006669 RID: 26217
		public static volatile ScienceExperimentManager instance;

		// Token: 0x0400666A RID: 26218
		[SerializeField]
		private ScienceExperimentManager.TagBehavior tagBehavior = ScienceExperimentManager.TagBehavior.Infect;

		// Token: 0x0400666B RID: 26219
		[SerializeField]
		private float minScale = 1f;

		// Token: 0x0400666C RID: 26220
		[SerializeField]
		private float maxScale = 10f;

		// Token: 0x0400666D RID: 26221
		[SerializeField]
		private float riseTimeFast = 30f;

		// Token: 0x0400666E RID: 26222
		[SerializeField]
		private float riseTimeMedium = 60f;

		// Token: 0x0400666F RID: 26223
		[SerializeField]
		private float riseTimeSlow = 120f;

		// Token: 0x04006670 RID: 26224
		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		// Token: 0x04006671 RID: 26225
		[SerializeField]
		private float preDrainWaitTime = 3f;

		// Token: 0x04006672 RID: 26226
		[SerializeField]
		private float maxFullTime = 5f;

		// Token: 0x04006673 RID: 26227
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04006674 RID: 26228
		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		// Token: 0x04006675 RID: 26229
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04006676 RID: 26230
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006677 RID: 26231
		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		// Token: 0x04006678 RID: 26232
		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		// Token: 0x04006679 RID: 26233
		[SerializeField]
		private float entryLiquidMaxScale = 5f;

		// Token: 0x0400667A RID: 26234
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningTop = Vector2.zero;

		// Token: 0x0400667B RID: 26235
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningBottom = Vector2.zero;

		// Token: 0x0400667C RID: 26236
		[SerializeField]
		private float entryBridgeQuadMaxScaleY = 0.0915f;

		// Token: 0x0400667D RID: 26237
		[SerializeField]
		private Vector2 entryBridgeQuadMinMaxZHeight = new Vector2(0.245f, 0.337f);

		// Token: 0x0400667E RID: 26238
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400667F RID: 26239
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006680 RID: 26240
		[SerializeField]
		public GameObject waterBalloonPrefab;

		// Token: 0x04006681 RID: 26241
		[SerializeField]
		private Vector2 rotatingRingRandomAngleRange = Vector2.zero;

		// Token: 0x04006682 RID: 26242
		[SerializeField]
		private bool rotatingRingQuantizeAngles;

		// Token: 0x04006683 RID: 26243
		[SerializeField]
		private float rotatingRingAngleSnapDegrees = 9f;

		// Token: 0x04006684 RID: 26244
		[SerializeField]
		private float drainBlockerSlideTime = 4f;

		// Token: 0x04006685 RID: 26245
		[SerializeField]
		private Vector2 sodaFizzParticleEmissionMinMax = new Vector2(30f, 100f);

		// Token: 0x04006686 RID: 26246
		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		// Token: 0x04006687 RID: 26247
		[SerializeField]
		private bool optPlayersOutOfRoomGameMode;

		// Token: 0x04006688 RID: 26248
		[SerializeField]
		private bool debugDrawPlayerGameState;

		// Token: 0x04006689 RID: 26249
		private ScienceExperimentSceneElements elements;

		// Token: 0x0400668A RID: 26250
		private NetPlayer[] allPlayersInRoom;

		// Token: 0x0400668B RID: 26251
		private ScienceExperimentManager.RotatingRingState[] rotatingRings = new ScienceExperimentManager.RotatingRingState[0];

		// Token: 0x0400668C RID: 26252
		private const int maxPlayerCount = 10;

		// Token: 0x0400668D RID: 26253
		private ScienceExperimentManager.PlayerGameState[] inGamePlayerStates = new ScienceExperimentManager.PlayerGameState[10];

		// Token: 0x0400668E RID: 26254
		private int inGamePlayerCount;

		// Token: 0x0400668F RID: 26255
		private int lastWinnerId = -1;

		// Token: 0x04006690 RID: 26256
		private string lastWinnerName = "None";

		// Token: 0x04006691 RID: 26257
		private List<ScienceExperimentManager.PlayerGameState> sortedPlayerStates = new List<ScienceExperimentManager.PlayerGameState>();

		// Token: 0x04006692 RID: 26258
		private ScienceExperimentManager.SyncData reliableState;

		// Token: 0x04006693 RID: 26259
		private ScienceExperimentManager.RiseSpeed nextRoundRiseSpeed = ScienceExperimentManager.RiseSpeed.Slow;

		// Token: 0x04006694 RID: 26260
		private float riseTime = 120f;

		// Token: 0x04006695 RID: 26261
		private float riseProgress;

		// Token: 0x04006696 RID: 26262
		private float riseProgressLinear;

		// Token: 0x04006697 RID: 26263
		private float localLagRiseProgressOffset;

		// Token: 0x04006698 RID: 26264
		private double lastInfrequentUpdateTime = -10.0;

		// Token: 0x04006699 RID: 26265
		private string mentoProjectileTag = "ScienceCandyProjectile";

		// Token: 0x0400669A RID: 26266
		private double currentTime;

		// Token: 0x0400669B RID: 26267
		private double prevTime;

		// Token: 0x0400669C RID: 26268
		private float ringRotationProgress = 1f;

		// Token: 0x0400669D RID: 26269
		private float drainBlockerSlideSpeed;

		// Token: 0x0400669E RID: 26270
		private float[] riseTimeLookup;

		// Token: 0x0400669F RID: 26271
		[Header("Scene References")]
		public Transform ringParent;

		// Token: 0x040066A0 RID: 26272
		public Transform liquidMeshTransform;

		// Token: 0x040066A1 RID: 26273
		public Transform liquidSurfacePlane;

		// Token: 0x040066A2 RID: 26274
		public Transform entryWayLiquidMeshTransform;

		// Token: 0x040066A3 RID: 26275
		public Transform entryWayBridgeQuadTransform;

		// Token: 0x040066A4 RID: 26276
		public Transform drainBlocker;

		// Token: 0x040066A5 RID: 26277
		public Transform drainBlockerClosedPosition;

		// Token: 0x040066A6 RID: 26278
		public Transform drainBlockerOpenPosition;

		// Token: 0x040066A7 RID: 26279
		public WaterVolume liquidVolume;

		// Token: 0x040066A8 RID: 26280
		public WaterVolume entryLiquidVolume;

		// Token: 0x040066A9 RID: 26281
		public WaterVolume bottleLiquidVolume;

		// Token: 0x040066AA RID: 26282
		public WaterVolume refreshWaterVolume;

		// Token: 0x040066AB RID: 26283
		public CompositeTriggerEvents gameAreaTriggerNotifier;

		// Token: 0x040066AC RID: 26284
		public SlingshotProjectileHitNotifier sodaWaterProjectileTriggerNotifier;

		// Token: 0x040066AD RID: 26285
		public AudioSource eruptionAudioSource;

		// Token: 0x040066AE RID: 26286
		public AudioSource drainAudioSource;

		// Token: 0x040066AF RID: 26287
		public AudioSource rotatingRingsAudioSource;

		// Token: 0x040066B0 RID: 26288
		private ParticleSystem.EmissionModule fizzParticleEmission;

		// Token: 0x040066B1 RID: 26289
		private bool hasPlayedEruptionEffects;

		// Token: 0x040066B2 RID: 26290
		private bool hasPlayedDrainEffects;

		// Token: 0x040066B4 RID: 26292
		[SerializeField]
		private float debugRotateRingsTime = 10f;

		// Token: 0x040066B5 RID: 26293
		private Coroutine rotateRingsCoroutine;

		// Token: 0x040066B6 RID: 26294
		private bool debugRandomizingRings;

		// Token: 0x040066B7 RID: 26295
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 76)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ScienceExperimentManager.ScienceManagerData _Data;

		// Token: 0x02000E79 RID: 3705
		public enum RisingLiquidState
		{
			// Token: 0x040066B9 RID: 26297
			Drained,
			// Token: 0x040066BA RID: 26298
			Erupting,
			// Token: 0x040066BB RID: 26299
			Rising,
			// Token: 0x040066BC RID: 26300
			Full,
			// Token: 0x040066BD RID: 26301
			PreDrainDelay,
			// Token: 0x040066BE RID: 26302
			Draining
		}

		// Token: 0x02000E7A RID: 3706
		private enum RiseSpeed
		{
			// Token: 0x040066C0 RID: 26304
			Fast,
			// Token: 0x040066C1 RID: 26305
			Medium,
			// Token: 0x040066C2 RID: 26306
			Slow,
			// Token: 0x040066C3 RID: 26307
			ExtraSlow
		}

		// Token: 0x02000E7B RID: 3707
		private enum TagBehavior
		{
			// Token: 0x040066C5 RID: 26309
			None,
			// Token: 0x040066C6 RID: 26310
			Infect,
			// Token: 0x040066C7 RID: 26311
			Revive
		}

		// Token: 0x02000E7C RID: 3708
		[Serializable]
		public struct PlayerGameState
		{
			// Token: 0x040066C8 RID: 26312
			public int playerId;

			// Token: 0x040066C9 RID: 26313
			public bool touchedLiquid;

			// Token: 0x040066CA RID: 26314
			public float touchedLiquidAtProgress;
		}

		// Token: 0x02000E7D RID: 3709
		private struct SyncData
		{
			// Token: 0x040066CB RID: 26315
			public ScienceExperimentManager.RisingLiquidState state;

			// Token: 0x040066CC RID: 26316
			public double stateStartTime;

			// Token: 0x040066CD RID: 26317
			public float stateStartLiquidProgressLinear;

			// Token: 0x040066CE RID: 26318
			public double activationProgress;
		}

		// Token: 0x02000E7E RID: 3710
		private struct RotatingRingState
		{
			// Token: 0x040066CF RID: 26319
			public Transform ringTransform;

			// Token: 0x040066D0 RID: 26320
			public float initialAngle;

			// Token: 0x040066D1 RID: 26321
			public float resultingAngle;
		}

		// Token: 0x02000E7F RID: 3711
		[Serializable]
		private struct DisableByLiquidData
		{
			// Token: 0x040066D2 RID: 26322
			public Transform target;

			// Token: 0x040066D3 RID: 26323
			public float heightOffset;
		}

		// Token: 0x02000E80 RID: 3712
		[NetworkStructWeaved(76)]
		[StructLayout(LayoutKind.Explicit, Size = 304)]
		private struct ScienceManagerData : INetworkStruct
		{
			// Token: 0x170008FF RID: 2303
			// (get) Token: 0x06005CF6 RID: 23798 RVA: 0x001D5D44 File Offset: 0x001D3F44
			[Networked]
			[Capacity(10)]
			public NetworkArray<int> playerIdArray
			{
				get
				{
					return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerIdArray), 10, ReaderWriter@System_Int32.GetInstance());
				}
			}

			// Token: 0x17000900 RID: 2304
			// (get) Token: 0x06005CF7 RID: 23799 RVA: 0x001D5D6C File Offset: 0x001D3F6C
			[Networked]
			[Capacity(10)]
			public NetworkArray<bool> touchedLiquidArray
			{
				get
				{
					return new NetworkArray<bool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidArray), 10, ReaderWriter@System_Boolean.GetInstance());
				}
			}

			// Token: 0x17000901 RID: 2305
			// (get) Token: 0x06005CF8 RID: 23800 RVA: 0x001D5D94 File Offset: 0x001D3F94
			[Networked]
			[Capacity(10)]
			public NetworkArray<float> touchedLiquidAtProgressArray
			{
				get
				{
					return new NetworkArray<float>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidAtProgressArray), 10, ReaderWriter@System_Single.GetInstance());
				}
			}

			// Token: 0x17000902 RID: 2306
			// (get) Token: 0x06005CF9 RID: 23801 RVA: 0x001D5DBC File Offset: 0x001D3FBC
			[Networked]
			[Capacity(5)]
			public NetworkLinkedList<float> initialAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._initialAngleArray), 5, ReaderWriter@System_Single.GetInstance());
				}
			}

			// Token: 0x17000903 RID: 2307
			// (get) Token: 0x06005CFA RID: 23802 RVA: 0x001D5DE0 File Offset: 0x001D3FE0
			[Networked]
			[Capacity(5)]
			public NetworkLinkedList<float> resultingAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._resultingAngleArray), 5, ReaderWriter@System_Single.GetInstance());
				}
			}

			// Token: 0x06005CFB RID: 23803 RVA: 0x001D5E04 File Offset: 0x001D4004
			public ScienceManagerData(int reliableState, double stateStartTime, float stateStartLiquidProgressLinear, double activationProgress, int nextRoundRiseSpeed, float riseTime, int lastWinnerId, int inGamePlayerCount, ScienceExperimentManager.PlayerGameState[] playerStates, ScienceExperimentManager.RotatingRingState[] rings)
			{
				this.reliableState = reliableState;
				this.stateStartTime = stateStartTime;
				this.stateStartLiquidProgressLinear = stateStartLiquidProgressLinear;
				this.activationProgress = activationProgress;
				this.nextRoundRiseSpeed = nextRoundRiseSpeed;
				this.riseTime = riseTime;
				this.lastWinnerId = lastWinnerId;
				this.inGamePlayerCount = inGamePlayerCount;
				foreach (ScienceExperimentManager.RotatingRingState rotatingRingState in rings)
				{
					this.initialAngleArray.Add(rotatingRingState.initialAngle);
					this.resultingAngleArray.Add(rotatingRingState.resultingAngle);
				}
				int[] array = new int[10];
				bool[] array2 = new bool[10];
				float[] array3 = new float[10];
				for (int j = 0; j < 10; j++)
				{
					array[j] = playerStates[j].playerId;
					array2[j] = playerStates[j].touchedLiquid;
					array3[j] = playerStates[j].touchedLiquidAtProgress;
				}
				this.playerIdArray.CopyFrom(array, 0, array.Length);
				this.touchedLiquidArray.CopyFrom(array2, 0, array2.Length);
				this.touchedLiquidAtProgressArray.CopyFrom(array3, 0, array3.Length);
			}

			// Token: 0x040066D4 RID: 26324
			[FieldOffset(0)]
			public int reliableState;

			// Token: 0x040066D5 RID: 26325
			[FieldOffset(4)]
			public double stateStartTime;

			// Token: 0x040066D6 RID: 26326
			[FieldOffset(12)]
			public float stateStartLiquidProgressLinear;

			// Token: 0x040066D7 RID: 26327
			[FieldOffset(16)]
			public double activationProgress;

			// Token: 0x040066D8 RID: 26328
			[FieldOffset(24)]
			public int nextRoundRiseSpeed;

			// Token: 0x040066D9 RID: 26329
			[FieldOffset(28)]
			public float riseTime;

			// Token: 0x040066DA RID: 26330
			[FieldOffset(32)]
			public int lastWinnerId;

			// Token: 0x040066DB RID: 26331
			[FieldOffset(36)]
			public int inGamePlayerCount;

			// Token: 0x040066DC RID: 26332
			[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ReaderWriter@System_Int32), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(40)]
			private FixedStorage@10 _playerIdArray;

			// Token: 0x040066DD RID: 26333
			[FixedBufferProperty(typeof(NetworkArray<bool>), typeof(UnityArraySurrogate@ReaderWriter@System_Boolean), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(80)]
			private FixedStorage@10 _touchedLiquidArray;

			// Token: 0x040066DE RID: 26334
			[FixedBufferProperty(typeof(NetworkArray<float>), typeof(UnityArraySurrogate@ReaderWriter@System_Single), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(120)]
			private FixedStorage@10 _touchedLiquidAtProgressArray;

			// Token: 0x040066DF RID: 26335
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Single), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(160)]
			private FixedStorage@18 _initialAngleArray;

			// Token: 0x040066E0 RID: 26336
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ReaderWriter@System_Single), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(232)]
			private FixedStorage@18 _resultingAngleArray;
		}
	}
}
