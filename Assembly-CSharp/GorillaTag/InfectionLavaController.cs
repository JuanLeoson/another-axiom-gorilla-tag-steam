using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E75 RID: 3701
	public class InfectionLavaController : MonoBehaviour, IGorillaSerializeableScene, IGorillaSerializeable, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06005C80 RID: 23680 RVA: 0x001D19DA File Offset: 0x001CFBDA
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06005C81 RID: 23681 RVA: 0x001D19E1 File Offset: 0x001CFBE1
		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06005C82 RID: 23682 RVA: 0x001D19F1 File Offset: 0x001CFBF1
		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06005C83 RID: 23683 RVA: 0x001D1A0E File Offset: 0x001CFC0E
		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06005C84 RID: 23684 RVA: 0x001D1A1C File Offset: 0x001CFC1C
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

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06005C85 RID: 23685 RVA: 0x001D1A4C File Offset: 0x001CFC4C
		private bool InCompetitiveQueue
		{
			get
			{
				return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
			}
		}

		// Token: 0x06005C86 RID: 23686 RVA: 0x001D1A70 File Offset: 0x001CFC70
		private void Awake()
		{
			if (InfectionLavaController.instance.IsNotNull())
			{
				Object.Destroy(base.gameObject);
				return;
			}
			InfectionLavaController.instance = this;
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
			((IGuidedRefObject)this).GuidedRefInitialize();
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x06005C87 RID: 23687 RVA: 0x001D1B21 File Offset: 0x001CFD21
		protected void OnEnable()
		{
			if (!this.guidedRefsFullyResolved)
			{
				return;
			}
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005C88 RID: 23688 RVA: 0x001D1B38 File Offset: 0x001CFD38
		void IGorillaSerializeableScene.OnSceneLinking(GorillaSerializerScene netObj)
		{
			this.networkObject = netObj;
		}

		// Token: 0x06005C89 RID: 23689 RVA: 0x00100D37 File Offset: 0x000FEF37
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005C8A RID: 23690 RVA: 0x001D1B44 File Offset: 0x001CFD44
		private void VerifyReferences()
		{
			this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
			}
		}

		// Token: 0x06005C8B RID: 23691 RVA: 0x001D1BF8 File Offset: 0x001CFDF8
		private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
		{
			if (obj != null)
			{
				return;
			}
			fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
			Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
			base.enabled = false;
		}

		// Token: 0x06005C8C RID: 23692 RVA: 0x001D1C48 File Offset: 0x001CFE48
		private void OnDestroy()
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
			TickSystem<object>.RemovePostTickCallback(this);
			this.UpdateLava(0f);
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06005C8D RID: 23693 RVA: 0x001D1CC3 File Offset: 0x001CFEC3
		// (set) Token: 0x06005C8E RID: 23694 RVA: 0x001D1CCB File Offset: 0x001CFECB
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005C8F RID: 23695 RVA: 0x001D1CD4 File Offset: 0x001CFED4
		void ITickSystemPost.PostTick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
			if (this.networkObject.HasAuthority)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
			this.UpdateVolcanoActivationLava((float)this.reliableState.activationProgress);
			this.CheckLocalPlayerAgainstLava(this.currentTime);
		}

		// Token: 0x06005C90 RID: 23696 RVA: 0x001D1D94 File Offset: 0x001CFF94
		private void JumpToState(InfectionLavaController.RisingLavaState state)
		{
			this.reliableState.state = state;
			switch (state)
			{
			case InfectionLavaController.RisingLavaState.Drained:
				for (int i = 0; i < this.volcanoEffects.Length; i++)
				{
					VolcanoEffects volcanoEffects = this.volcanoEffects[i];
					if (volcanoEffects != null)
					{
						volcanoEffects.SetDrainedState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Erupting:
				for (int j = 0; j < this.volcanoEffects.Length; j++)
				{
					VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.SetEruptingState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Rising:
				for (int k = 0; k < this.volcanoEffects.Length; k++)
				{
					VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.SetRisingState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Full:
				for (int l = 0; l < this.volcanoEffects.Length; l++)
				{
					VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.SetFullState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Draining:
				for (int m = 0; m < this.volcanoEffects.Length; m++)
				{
					VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.SetDrainingState();
					}
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06005C91 RID: 23697 RVA: 0x001D1E90 File Offset: 0x001D0090
		private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
				if (syncData.activationProgress > 1.0)
				{
					float playerCount = (float)this.PlayerCount;
					float num = this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue;
					int num2 = Mathf.RoundToInt(playerCount * num);
					if (this.lavaActivationVoteCount >= num2)
					{
						for (int i = 0; i < this.lavaActivationVoteCount; i++)
						{
							this.lavaActivationVotePlayerIds[i] = 0;
						}
						this.lavaActivationVoteCount = 0;
						syncData.state = InfectionLavaController.RisingLavaState.Erupting;
						syncData.stateStartTime = currentTime;
						syncData.activationProgress = 1.0;
						for (int j = 0; j < this.volcanoEffects.Length; j++)
						{
							VolcanoEffects volcanoEffects = this.volcanoEffects[j];
							if (volcanoEffects != null)
							{
								volcanoEffects.SetEruptingState();
							}
						}
						return;
					}
				}
				else
				{
					float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
					double activationProgress = syncData.activationProgress;
					syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
					if (activationProgress > 0.0 && syncData.activationProgress <= 5E-324)
					{
						VolcanoEffects[] array = this.volcanoEffects;
						for (int k = 0; k < array.Length; k++)
						{
							array[k].OnVolcanoBellyEmpty();
						}
						return;
					}
				}
				break;
			case InfectionLavaController.RisingLavaState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Rising;
					syncData.stateStartTime = currentTime;
					for (int l = 0; l < this.volcanoEffects.Length; l++)
					{
						VolcanoEffects volcanoEffects2 = this.volcanoEffects[l];
						if (volcanoEffects2 != null)
						{
							volcanoEffects2.SetRisingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Rising:
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Full;
					syncData.stateStartTime = currentTime;
					for (int m = 0; m < this.volcanoEffects.Length; m++)
					{
						VolcanoEffects volcanoEffects3 = this.volcanoEffects[m];
						if (volcanoEffects3 != null)
						{
							volcanoEffects3.SetFullState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Full:
				if (currentTime > syncData.stateStartTime + (double)this.fullTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Draining;
					syncData.stateStartTime = currentTime;
					for (int n = 0; n < this.volcanoEffects.Length; n++)
					{
						VolcanoEffects volcanoEffects4 = this.volcanoEffects[n];
						if (volcanoEffects4 != null)
						{
							volcanoEffects4.SetDrainingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Draining:
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * Time.deltaTime);
				if (currentTime > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Drained;
					syncData.stateStartTime = currentTime;
					for (int num4 = 0; num4 < this.volcanoEffects.Length; num4++)
					{
						VolcanoEffects volcanoEffects5 = this.volcanoEffects[num4];
						if (volcanoEffects5 != null)
						{
							volcanoEffects5.SetDrainedState();
						}
					}
				}
				break;
			}
		}

		// Token: 0x06005C92 RID: 23698 RVA: 0x001D217C File Offset: 0x001D037C
		private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float time = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
				{
					if (volcanoEffects != null)
					{
						volcanoEffects.UpdateDrainedState(time);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num = (float)(currentTime - syncData.stateStartTime);
				float progress = Mathf.Clamp01(num / this.eruptTime);
				foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
				{
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.UpdateEruptingState(num, this.eruptTime - num, progress);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				float value = (float)(currentTime - syncData.stateStartTime) / this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01(value);
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				float num2 = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
				{
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.UpdateRisingState(num2, this.riseTime - num2, this.lavaProgressLinear);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				this.lavaProgressLinear = 1f;
				this.lavaProgressSmooth = 1f;
				float num3 = (float)(currentTime - syncData.stateStartTime);
				float progress2 = Mathf.Clamp01(this.fullTime / num3);
				foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
				{
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.UpdateFullState(num3, this.fullTime - num3, progress2);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				float num4 = (float)(currentTime - syncData.stateStartTime);
				float num5 = Mathf.Clamp01(num4 / this.drainTime);
				this.lavaProgressLinear = 1f - num5;
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
				{
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.UpdateDrainingState(num4, this.riseTime - num4, num5);
					}
				}
				return;
			}
			}
		}

		// Token: 0x06005C93 RID: 23699 RVA: 0x001D23C4 File Offset: 0x001D05C4
		private void UpdateLava(float fillProgress)
		{
			this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
			}
		}

		// Token: 0x06005C94 RID: 23700 RVA: 0x001D2430 File Offset: 0x001D0630
		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		// Token: 0x06005C95 RID: 23701 RVA: 0x001D24AD File Offset: 0x001D06AD
		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		// Token: 0x06005C96 RID: 23702 RVA: 0x001D24DA File Offset: 0x001D06DA
		private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
			}
		}

		// Token: 0x06005C97 RID: 23703 RVA: 0x001D2514 File Offset: 0x001D0714
		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				GameMode.ReportHit();
			}
		}

		// Token: 0x06005C98 RID: 23704 RVA: 0x001D2566 File Offset: 0x001D0766
		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag("LavaRockProjectile"))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x06005C99 RID: 23705 RVA: 0x001D258C File Offset: 0x001D078C
		private void AddLavaRock(int playerId)
		{
			if (this.networkObject.HasAuthority && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
				this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				this.AddVoteForVolcanoActivation(playerId);
				VolcanoEffects[] array = this.volcanoEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnStoneAccepted(this.reliableState.activationProgress);
				}
			}
		}

		// Token: 0x06005C9A RID: 23706 RVA: 0x001D2608 File Offset: 0x001D0808
		private void AddVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority && this.lavaActivationVoteCount < 10)
			{
				bool flag = false;
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
					this.lavaActivationVoteCount++;
				}
			}
		}

		// Token: 0x06005C9B RID: 23707 RVA: 0x001D266C File Offset: 0x001D086C
		private void RemoveVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority)
			{
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
						this.lavaActivationVoteCount--;
						return;
					}
				}
			}
		}

		// Token: 0x06005C9C RID: 23708 RVA: 0x001D26C8 File Offset: 0x001D08C8
		void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext(this.lavaActivationVoteCount);
			stream.SendNext(this.lavaActivationVotePlayerIds[0]);
			stream.SendNext(this.lavaActivationVotePlayerIds[1]);
			stream.SendNext(this.lavaActivationVotePlayerIds[2]);
			stream.SendNext(this.lavaActivationVotePlayerIds[3]);
			stream.SendNext(this.lavaActivationVotePlayerIds[4]);
			stream.SendNext(this.lavaActivationVotePlayerIds[5]);
			stream.SendNext(this.lavaActivationVotePlayerIds[6]);
			stream.SendNext(this.lavaActivationVotePlayerIds[7]);
			stream.SendNext(this.lavaActivationVotePlayerIds[8]);
			stream.SendNext(this.lavaActivationVotePlayerIds[9]);
		}

		// Token: 0x06005C9D RID: 23709 RVA: 0x001D27E8 File Offset: 0x001D09E8
		void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			InfectionLavaController.RisingLavaState risingLavaState = (InfectionLavaController.RisingLavaState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).ClampSafe(0.0, 2.0);
			this.lavaActivationVoteCount = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[0] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[1] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[2] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[3] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[4] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[5] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[6] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[7] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[8] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[9] = (int)stream.ReceiveNext();
			float num = this.lavaProgressSmooth;
			if (risingLavaState != this.reliableState.state)
			{
				this.JumpToState(risingLavaState);
			}
			this.UpdateLocalState((double)((float)NetworkSystem.Instance.SimTime), this.reliableState);
			this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
		}

		// Token: 0x06005C9E RID: 23710 RVA: 0x001D295B File Offset: 0x001D0B5B
		public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		}

		// Token: 0x06005C9F RID: 23711 RVA: 0x001D296C File Offset: 0x001D0B6C
		private void OnLeftRoom()
		{
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] != NetworkSystem.Instance.LocalPlayerID)
				{
					this.RemoveVoteForVolcanoActivation(this.lavaActivationVotePlayerIds[i]);
				}
			}
		}

		// Token: 0x06005CA0 RID: 23712 RVA: 0x000023F5 File Offset: 0x000005F5
		void IGorillaSerializeableScene.OnNetworkObjectDisable()
		{
		}

		// Token: 0x06005CA1 RID: 23713 RVA: 0x000023F5 File Offset: 0x000005F5
		void IGorillaSerializeableScene.OnNetworkObjectEnable()
		{
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06005CA2 RID: 23714 RVA: 0x001D29AE File Offset: 0x001D0BAE
		// (set) Token: 0x06005CA3 RID: 23715 RVA: 0x001D29B6 File Offset: 0x001D0BB6
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06005CA4 RID: 23716 RVA: 0x001D29BF File Offset: 0x001D0BBF
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			this.guidedRefsFullyResolved = true;
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005CA5 RID: 23717 RVA: 0x001D29D4 File Offset: 0x001D0BD4
		public void OnGuidedRefTargetDestroyed(int fieldId)
		{
			this.guidedRefsFullyResolved = false;
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005CA6 RID: 23718 RVA: 0x001D29E4 File Offset: 0x001D0BE4
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaMeshTransform_gRef", ref this.lavaMeshTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaSurfacePlaneTransform_gRef", ref this.lavaSurfacePlaneTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaVolume_gRef", ref this.lavaVolume_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationRenderer_gRef", ref this.lavaActivationRenderer_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationStartPos_gRef", ref this.lavaActivationStartPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationEndPos_gRef", ref this.lavaActivationEndPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationProjectileHitNotifier_gRef", ref this.lavaActivationProjectileHitNotifier_gRef);
			GuidedRefHub.RegisterReceiverArray<InfectionLavaController, VolcanoEffects>(this, "volcanoEffects_gRefs", ref this.volcanoEffects, ref this.volcanoEffects_gRefs);
			GuidedRefHub.ReceiverFullyRegistered<InfectionLavaController>(this);
		}

		// Token: 0x06005CA7 RID: 23719 RVA: 0x001D2A88 File Offset: 0x001D0C88
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaMeshTransform, this.lavaMeshTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaSurfacePlaneTransform, this.lavaSurfacePlaneTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, WaterVolume>(this, ref this.lavaVolume, this.lavaVolume_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, MeshRenderer>(this, ref this.lavaActivationRenderer, this.lavaActivationRenderer_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationStartPos, this.lavaActivationStartPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationEndPos, this.lavaActivationEndPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, SlingshotProjectileHitNotifier>(this, ref this.lavaActivationProjectileHitNotifier, this.lavaActivationProjectileHitNotifier_gRef, target) || GuidedRefHub.TryResolveArrayItem<InfectionLavaController, VolcanoEffects>(this, this.volcanoEffects, this.volcanoEffects_gRefs, target);
		}

		// Token: 0x06005CA9 RID: 23721 RVA: 0x0005860D File Offset: 0x0005680D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005CAA RID: 23722 RVA: 0x0001745D File Offset: 0x0001565D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x04006630 RID: 26160
		[OnEnterPlay_SetNull]
		private static InfectionLavaController instance;

		// Token: 0x04006631 RID: 26161
		[SerializeField]
		private float lavaMeshMinScale = 3.17f;

		// Token: 0x04006632 RID: 26162
		[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
		[SerializeField]
		private float lavaMeshMaxScale = 8.941086f;

		// Token: 0x04006633 RID: 26163
		[SerializeField]
		private float eruptTime = 3f;

		// Token: 0x04006634 RID: 26164
		[SerializeField]
		private float riseTime = 10f;

		// Token: 0x04006635 RID: 26165
		[SerializeField]
		private float fullTime = 240f;

		// Token: 0x04006636 RID: 26166
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04006637 RID: 26167
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04006638 RID: 26168
		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006639 RID: 26169
		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		// Token: 0x0400663A RID: 26170
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		// Token: 0x0400663B RID: 26171
		[SerializeField]
		private Gradient lavaActivationGradient;

		// Token: 0x0400663C RID: 26172
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400663D RID: 26173
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400663E RID: 26174
		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		// Token: 0x0400663F RID: 26175
		[SerializeField]
		private bool debugLavaActivationVotes;

		// Token: 0x04006640 RID: 26176
		[Header("Scene References")]
		[SerializeField]
		private Transform lavaMeshTransform;

		// Token: 0x04006641 RID: 26177
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaMeshTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04006642 RID: 26178
		[SerializeField]
		private Transform lavaSurfacePlaneTransform;

		// Token: 0x04006643 RID: 26179
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaSurfacePlaneTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04006644 RID: 26180
		[SerializeField]
		private WaterVolume lavaVolume;

		// Token: 0x04006645 RID: 26181
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaVolume_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04006646 RID: 26182
		[SerializeField]
		private MeshRenderer lavaActivationRenderer;

		// Token: 0x04006647 RID: 26183
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationRenderer_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04006648 RID: 26184
		[SerializeField]
		private Transform lavaActivationStartPos;

		// Token: 0x04006649 RID: 26185
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationStartPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x0400664A RID: 26186
		[SerializeField]
		private Transform lavaActivationEndPos;

		// Token: 0x0400664B RID: 26187
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationEndPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x0400664C RID: 26188
		[SerializeField]
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		// Token: 0x0400664D RID: 26189
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationProjectileHitNotifier_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x0400664E RID: 26190
		[SerializeField]
		private VolcanoEffects[] volcanoEffects;

		// Token: 0x0400664F RID: 26191
		[SerializeField]
		private GuidedRefReceiverArrayInfo volcanoEffects_gRefs = new GuidedRefReceiverArrayInfo(true);

		// Token: 0x04006650 RID: 26192
		[DebugReadout]
		private InfectionLavaController.LavaSyncData reliableState;

		// Token: 0x04006651 RID: 26193
		private int[] lavaActivationVotePlayerIds = new int[10];

		// Token: 0x04006652 RID: 26194
		private int lavaActivationVoteCount;

		// Token: 0x04006653 RID: 26195
		private float localLagLavaProgressOffset;

		// Token: 0x04006654 RID: 26196
		[DebugReadout]
		private float lavaProgressLinear;

		// Token: 0x04006655 RID: 26197
		[DebugReadout]
		private float lavaProgressSmooth;

		// Token: 0x04006656 RID: 26198
		private double lastTagSelfRPCTime;

		// Token: 0x04006657 RID: 26199
		private const string lavaRockProjectileTag = "LavaRockProjectile";

		// Token: 0x04006658 RID: 26200
		private double currentTime;

		// Token: 0x04006659 RID: 26201
		private double prevTime;

		// Token: 0x0400665A RID: 26202
		private float activationProgessSmooth;

		// Token: 0x0400665B RID: 26203
		private float lavaScale;

		// Token: 0x0400665C RID: 26204
		private GorillaSerializerScene networkObject;

		// Token: 0x0400665E RID: 26206
		private bool guidedRefsFullyResolved;

		// Token: 0x02000E76 RID: 3702
		public enum RisingLavaState
		{
			// Token: 0x04006661 RID: 26209
			Drained,
			// Token: 0x04006662 RID: 26210
			Erupting,
			// Token: 0x04006663 RID: 26211
			Rising,
			// Token: 0x04006664 RID: 26212
			Full,
			// Token: 0x04006665 RID: 26213
			Draining
		}

		// Token: 0x02000E77 RID: 3703
		private struct LavaSyncData
		{
			// Token: 0x04006666 RID: 26214
			public InfectionLavaController.RisingLavaState state;

			// Token: 0x04006667 RID: 26215
			public double stateStartTime;

			// Token: 0x04006668 RID: 26216
			public double activationProgress;
		}
	}
}
