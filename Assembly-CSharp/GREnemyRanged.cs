using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x02000632 RID: 1586
public class GREnemyRanged : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent
{
	// Token: 0x060026FD RID: 9981 RVA: 0x000D1568 File Offset: 0x000CF768
	private bool IsMoving()
	{
		return this.navAgent.velocity.sqrMagnitude > 0f;
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000D1590 File Offset: 0x000CF790
	private void SoftResetThrowableHead()
	{
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
		this.headLightReset = true;
		this.spitterLightTurnOffTime = Time.timeAsDouble + this.spitterLightTurnOffDelay;
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000D15FC File Offset: 0x000CF7FC
	private void ForceResetThrowableHead()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000D1660 File Offset: 0x000CF860
	private void ForceHeadToDeadState()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000D16C4 File Offset: 0x000CF8C4
	private void EnableVFXForShoulderHead()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(true);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000D1720 File Offset: 0x000CF920
	private void EnableVFXForHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(true);
		this.spitterHeadInHandLight.SetActive(true);
		this.spitterHeadInHandVFX.SetActive(true);
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000D177C File Offset: 0x000CF97C
	private void DisableHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadInHand.SetActive(false);
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000D1794 File Offset: 0x000CF994
	private void DisableHeadOnShoulderAndHeadInHand()
	{
		this.headLightReset = false;
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000D17F8 File Offset: 0x000CF9F8
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000D18C4 File Offset: 0x000CFAC4
	public void OnEntityInit()
	{
		this.InitializeRandoms();
		int patrolPathId = (int)this.entity.createData;
		this.Setup(patrolPathId);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactor reactor = this.entity.manager.ghostReactorManager.reactor;
			foreach (GRBonusEntry entry in reactor.GetDepthLevelConfig(reactor.GetDepthLevel()).configGenOptions[0].enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000D19BC File Offset: 0x000CFBBC
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)(this.entity.GetNetId() + 1));
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)(this.entity.GetNetId() + 10));
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000D19F0 File Offset: 0x000CFBF0
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
		if (this.rangedProjectileInstance != null)
		{
			this.rangedProjectileInstance.Destroy();
		}
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000D1A44 File Offset: 0x000CFC44
	public void Setup(int patrolPathId)
	{
		this.SetPatrolPath(patrolPathId);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, true);
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		else
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Shell, true);
		}
		else
		{
			this.SetBodyState(GREnemyRanged.BodyState.Bones, true);
		}
		this.abilityDie.Setup(this.agent, this.anim, base.transform);
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000D1AEC File Offset: 0x000CFCEC
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 9)
		{
			return;
		}
		this.SetBehavior((GREnemyRanged.Behavior)newState, false);
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000D1B00 File Offset: 0x000CFD00
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyRanged.BodyState)newState, false);
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000D1B13 File Offset: 0x000CFD13
	public void SetPatrolPath(int patrolPathId)
	{
		this.patrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(patrolPathId);
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000D1B31 File Offset: 0x000CFD31
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000D1B3A File Offset: 0x000CFD3A
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000D1B44 File Offset: 0x000CFD44
	public void SetBehavior(GREnemyRanged.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttack)
			{
				this.SoftResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttackCooldown)
			{
				this.ForceResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.ForceResetThrowableHead();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdle", 0.1f, 1f);
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 0.5f);
			this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
			this.CalculateNextPatrolGroan();
			break;
		case GREnemyRanged.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 0.5f);
			this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
			this.lastMoving = false;
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Start();
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
			this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ChaseSpeed) / 10f;
			this.EnableVFXForShoulderHead();
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.PlayAnim("GREnemyRangedAttack", 0.1f, 1f);
			this.navAgent.speed = 0f;
			this.navAgent.velocity = Vector3.zero;
			this.headRemovaltime = PhotonNetwork.Time + (double)this.headRemovalFrame;
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
			this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ChaseSpeed) / 10f;
			this.lastMoving = true;
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.PlayAnim("GREnemyRangedFlashed", 0.1f, 1f);
			this.behaviorEndTime = Time.timeAsDouble + 0.25;
			this.agent.SetIsPathing(false, true);
			this.agent.SetDisableNetworkSync(true);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000D1E60 File Offset: 0x000D0060
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000D1E90 File Offset: 0x000D0090
	public void SetBodyState(GREnemyRanged.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.ForceResetThrowableHead();
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.DisableHeadOnShoulderAndHeadInHand();
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000D1FA0 File Offset: 0x000D01A0
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemyRanged.Hide(this.bones, true);
			GREnemyRanged.Hide(this.always, true);
			this.DisableHeadOnShoulderAndHeadInHand();
			return;
		case GREnemyRanged.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemyRanged.Hide(this.bones, false);
			GREnemyRanged.Hide(this.always, false);
			return;
		case GREnemyRanged.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemyRanged.Hide(this.bones, true);
			GREnemyRanged.Hide(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000D2040 File Offset: 0x000D0240
	public void CalculateNextPatrolGroan()
	{
		if (this.lastPartrolAmbientSoundTime < this.lastStateChange)
		{
			this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
		}
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000D2074 File Offset: 0x000D0274
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000D20CA File Offset: 0x000D02CA
	private void Update()
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(Time.deltaTime);
		}
		else
		{
			this.OnUpdateRemote(Time.deltaTime);
		}
		this.UpdateShared();
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000D20F8 File Offset: 0x000D02F8
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		float num = float.MaxValue;
		this.bestTargetPlayer = null;
		this.bestTargetNetPlayer = null;
		if ((!GhostReactorManager.AggroDisabled && this.currBehavior == GREnemyRanged.Behavior.Patrol) || this.currBehavior == GREnemyRanged.Behavior.Search)
		{
			GREnemyRanged.tempRigs.Clear();
			GREnemyRanged.tempRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GREnemyRanged.tempRigs);
			Vector3 position = base.transform.position;
			Vector3 rhs = base.transform.rotation * Vector3.forward;
			float num2 = this.sightDist * this.sightDist;
			float num3 = Mathf.Cos(this.sightFOV * 0.017453292f);
			for (int i = 0; i < GREnemyRanged.tempRigs.Count; i++)
			{
				VRRig vrrig = GREnemyRanged.tempRigs[i];
				GRPlayer component = vrrig.GetComponent<GRPlayer>();
				if (component.State != GRPlayer.GRPlayerState.Ghost)
				{
					Vector3 vector = vrrig.transform.position - position;
					float sqrMagnitude = vector.sqrMagnitude;
					if (sqrMagnitude <= num2)
					{
						float num4 = 0f;
						if (sqrMagnitude > 0f)
						{
							num4 = Mathf.Sqrt(sqrMagnitude);
							if (Vector3.Dot(vector / num4, rhs) < num3)
							{
								goto IL_174;
							}
						}
						if (num4 < num && Physics.RaycastNonAlloc(new Ray(this.headTransform.position, vector), GREnemyChaser.visibilityHits, vector.magnitude, this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
						{
							num = num4;
							this.bestTargetPlayer = component;
							this.bestTargetNetPlayer = vrrig.OwningNetPlayer;
						}
					}
				}
				IL_174:;
			}
		}
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000D2290 File Offset: 0x000D0490
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.UpdatePatrol();
			if (this.bestTargetPlayer != null)
			{
				this.targetPlayer = this.bestTargetNetPlayer;
				this.lastSeenTargetTime = Time.timeAsDouble;
				this.audioSource.clip = this.chaseSound;
				this.audioSource.volume = this.chaseSoundVolume;
				this.audioSource.Play();
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
			}
			break;
		case GREnemyRanged.Behavior.Search:
			this.UpdateSearch();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Update(dt);
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
		{
			bool flag = true;
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
				{
					Vector3 position = grplayer.transform.position;
					Vector3 position2 = base.transform.position;
					Vector3 a = position - position2;
					float magnitude = a.magnitude;
					if (magnitude < this.loseSightDist)
					{
						flag = false;
					}
					bool flag2 = Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position - this.headTransform.position), GREnemyChaser.visibilityHits, Mathf.Min(Vector3.Distance(position, this.headTransform.position), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1;
					if (flag2)
					{
						this.lastSeenTargetTime = Time.timeAsDouble;
					}
					if (flag2)
					{
						this.lastSeenTargetPosition = position;
						this.lastSeenTargetTime = Time.timeAsDouble;
					}
					if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
					{
						this.searchPosition = position;
					}
					else
					{
						flag = true;
					}
					this.agent.RequestDestination(this.lastSeenTargetPosition);
					Vector3 a2 = a / magnitude;
					float d = Mathf.Lerp(this.rangedAttackDistMin, this.rangedAttackDistMax, 0.5f);
					this.rangedFiringPosition = position - a2 * d;
					this.rangedTargetPosition = position;
					Vector3 b = Vector3.up * 0.4f;
					this.rangedTargetPosition += b;
					if (magnitude < this.rangedAttackDistMax)
					{
						this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackChargeTime;
						this.SetBehavior(GREnemyRanged.Behavior.RangedAttack, false);
						GhostReactorManager.Get(this.entity).RequestFireProjectile(this.entity.id, this.rangedProjectileFirePoint.position, this.rangedTargetPosition, PhotonNetwork.Time + (double)this.rangedAttackChargeTime);
					}
				}
			}
			this.agent.RequestDestination(this.rangedFiringPosition);
			if (flag)
			{
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
			}
			break;
		}
		case GREnemyRanged.Behavior.RangedAttack:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				if (this.targetPlayer != null)
				{
					GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
					if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
					{
						this.rangedTargetPosition = grplayer2.transform.position;
					}
				}
				this.agent.RequestDestination(this.rangedFiringPosition);
				this.SetBehavior(GREnemyRanged.Behavior.RangedAttackCooldown, false);
				this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackRecoverTime;
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				this.behaviorEndTime = Time.timeAsDouble;
			}
			if (this.IsMoving())
			{
				if (!this.lastMoving)
				{
					this.PlayAnim("GREnemyRangedCrawl", 0.1f, 0.5f);
					this.lastMoving = true;
				}
			}
			else if (this.lastMoving)
			{
				this.PlayAnim("GREnemyRangedIdle", 0.1f, 1f);
				this.lastMoving = false;
			}
			this.agent.RequestDestination(this.rangedFiringPosition);
			break;
		case GREnemyRanged.Behavior.Flashed:
			if (Time.timeAsDouble >= this.behaviorEndTime)
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		}
		GameAgent.UpdateFacing(base.transform, this.navAgent, this.targetPlayer, this.turnSpeed);
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000D26E4 File Offset: 0x000D08E4
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.UpdatePatrolRemote();
			break;
		case GREnemyRanged.Behavior.Search:
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			return;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Update(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000D2734 File Offset: 0x000D0934
	public void UpdateShared()
	{
		if (this.rangedAttackQueued)
		{
			if (!this.headRemoved && this.currBehavior == GREnemyRanged.Behavior.RangedAttack && PhotonNetwork.Time >= this.headRemovaltime)
			{
				this.headRemoved = true;
				this.EnableVFXForHeadInHand();
			}
			if (PhotonNetwork.Time > this.queuedFiringTime)
			{
				this.rangedAttackQueued = false;
				this.FireRangedAttack(this.queuedFiringPosition, this.queuedTargetPosition);
			}
		}
		if (this.headLightReset && Time.timeAsDouble > this.spitterLightTurnOffTime)
		{
			this.spitterHeadOnShouldersLight.SetActive(false);
			this.headLightReset = false;
		}
		if (this.projectileHasImpacted && Time.timeAsDouble > this.projectileImpactTime + 2.0)
		{
			Object.Destroy(this.rangedProjectileInstance);
			this.rangedProjectileInstance = null;
		}
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000D27F4 File Offset: 0x000D09F4
	public void UpdatePatrol()
	{
		if (this.patrolPath == null)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, false);
			return;
		}
		if (this.target == null)
		{
			return;
		}
		if ((this.target.transform.position - base.transform.position).sqrMagnitude < 0.25f)
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		if (this.target != null)
		{
			this.agent.RequestDestination(this.target.transform.position);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000D28CD File Offset: 0x000D0ACD
	public void UpdatePatrolRemote()
	{
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x000D28E4 File Offset: 0x000D0AE4
	public void UpdateSearch()
	{
		Vector3 vector = this.searchPosition - base.transform.position;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude < 0.15f)
		{
			Vector3 b = this.lastSeenTargetPosition - this.searchPosition;
			b.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + b;
		}
		if (this.IsMoving())
		{
			if (!this.lastMoving)
			{
				this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
				this.lastMoving = true;
			}
		}
		else if (this.lastMoving)
		{
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
			this.lastMoving = false;
		}
		this.agent.RequestDestination(this.searchPosition);
		if (Time.timeAsDouble - this.lastSeenTargetTime > (double)this.searchTime)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, false);
		}
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000D29E0 File Offset: 0x000D0BE0
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyRanged.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyRanged.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		this.audioSource.PlayOneShot(this.damagedSound, this.damagedSoundVolume);
		if (this.fxDamaged != null)
		{
			this.fxDamaged.SetActive(false);
			this.fxDamaged.SetActive(true);
		}
		if (this.hp <= 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyRanged.Behavior.Stagger, false);
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000D2B30 File Offset: 0x000D0D30
	public void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyRanged.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyRanged.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
			}
			else if (tool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = tool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				this.RefreshBody();
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		this.SetBehavior(GREnemyRanged.Behavior.Flashed, false);
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000D2CC0 File Offset: 0x000D0EC0
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.hp;
		byte value4 = (byte)this.nextPatrolNode;
		int value5 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(value3);
		writer.Write(value4);
		writer.Write(value5);
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000D2D2C File Offset: 0x000D0F2C
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyRanged.Behavior newBehavior = (GREnemyRanged.Behavior)reader.ReadByte();
		GREnemyRanged.BodyState newBodyState = (GREnemyRanged.BodyState)reader.ReadByte();
		int num = (int)reader.ReadByte();
		byte b = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath((int)this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x000D2DA0 File Offset: 0x000D0FA0
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			if (hitTypeId == GameHitType.Club)
			{
				this.OnHitByClub(gameComponent, hit);
				return;
			}
			this.OnHitByFlash(gameComponent, hit);
		}
	}

	// Token: 0x06002725 RID: 10021 RVA: 0x000D2DE8 File Offset: 0x000D0FE8
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000D2E29 File Offset: 0x000D1029
	public void RequestRangedAttack(Vector3 firingPosition, Vector3 targetPosition, double fireTime)
	{
		this.rangedAttackQueued = true;
		this.queuedFiringTime = fireTime;
		this.queuedFiringPosition = firingPosition;
		this.queuedTargetPosition = targetPosition;
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000D2E48 File Offset: 0x000D1048
	private void FireRangedAttack(Vector3 launchPosition, Vector3 targetPosition)
	{
		this.DisableHeadInHand();
		if (this.rangedProjectileInstance != null)
		{
			Object.Destroy(this.rangedProjectileInstance);
			this.rangedProjectileInstance = null;
		}
		this.rangedProjectileInstance = Object.Instantiate<GameObject>(this.rangedProjectilePrefab, launchPosition, Quaternion.identity);
		this.projectileHasImpacted = false;
		Collider componentInChildren = this.rangedProjectileInstance.GetComponentInChildren<Collider>();
		if (componentInChildren != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(componentInChildren, this.colliders[i]);
			}
		}
		this.rangedProjectileInstance.GetComponent<CollisionEventNotifier>().CollisionEnterEvent += this.OnProjectileCollisionEnter;
		this.rangedProjectileInstance.GetComponentInChildren<TriggerEventNotifier>().TriggerEnterEvent += this.OnProjectileTriggerEnter;
		targetPosition.y = launchPosition.y;
		float num = Vector3.Distance(launchPosition, targetPosition);
		float angle = 0.5f * Mathf.Asin(Mathf.Clamp01(9.8f * num / (this.projectileSpeed * this.projectileSpeed))) * 57.29578f;
		Vector3 vector = (targetPosition - launchPosition).normalized;
		vector = Quaternion.AngleAxis(angle, Vector3.Cross(vector, Vector3.up)) * vector;
		this.rangedProjectileInstance.GetComponent<Rigidbody>().velocity = vector * this.projectileSpeed;
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000D2F94 File Offset: 0x000D1194
	private void OnProjectileCollisionEnter(CollisionEventNotifier eventNotifier, Collision collision)
	{
		eventNotifier.CollisionEnterEvent -= this.OnProjectileCollisionEnter;
		ParticleSystem componentInChildren = eventNotifier.gameObject.GetComponentInChildren<ParticleSystem>();
		AudioSource componentInChildren2 = eventNotifier.gameObject.GetComponentInChildren<AudioSource>();
		if (componentInChildren != null)
		{
			componentInChildren.Play();
		}
		if (componentInChildren2 != null)
		{
			componentInChildren2.Play();
		}
		MeshRenderer componentInChildren3 = eventNotifier.gameObject.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren3 != null)
		{
			componentInChildren3.enabled = false;
		}
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		Vector3 position = eventNotifier.gameObject.transform.position;
		if ((VRRig.LocalRig.GetMouthPosition() - position).sqrMagnitude < this.projectileHitRadius * this.projectileHitRadius && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Ranged, this.entity.id, VRRig.LocalRig.GetComponent<GRPlayer>(), position);
		}
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000D3098 File Offset: 0x000D1298
	private void OnProjectileTriggerEnter(TriggerEventNotifier notifier, Collider collider)
	{
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			ParticleSystem componentInChildren = this.rangedProjectileInstance.GetComponentInChildren<ParticleSystem>();
			AudioSource componentInChildren2 = this.rangedProjectileInstance.GetComponentInChildren<AudioSource>();
			if (componentInChildren != null)
			{
				componentInChildren.Play();
			}
			if (componentInChildren2 != null)
			{
				componentInChildren2.Play();
			}
			MeshRenderer componentInChildren3 = this.rangedProjectileInstance.GetComponentInChildren<MeshRenderer>();
			if (componentInChildren3 != null)
			{
				componentInChildren3.enabled = false;
			}
			this.projectileHasImpacted = true;
			this.projectileImpactTime = Time.timeAsDouble;
			Rigidbody component2 = this.rangedProjectileInstance.GetComponent<Rigidbody>();
			if (component2 != null)
			{
				component2.velocity = -component2.velocity.normalized * component.KnockbackVelocity;
			}
			component.OnEnemyBlocked(component2.transform.position);
			notifier.TriggerEnterEvent -= this.OnProjectileTriggerEnter;
		}
	}

	// Token: 0x040031C3 RID: 12739
	public GameEntity entity;

	// Token: 0x040031C4 RID: 12740
	public GameAgent agent;

	// Token: 0x040031C5 RID: 12741
	public GRArmorEnemy armor;

	// Token: 0x040031C6 RID: 12742
	public GameHittable hittable;

	// Token: 0x040031C7 RID: 12743
	public GRAttributes attributes;

	// Token: 0x040031C8 RID: 12744
	public Animation anim;

	// Token: 0x040031C9 RID: 12745
	public GRAbilityStagger abilityStagger;

	// Token: 0x040031CA RID: 12746
	public GRAbilityDie abilityDie;

	// Token: 0x040031CB RID: 12747
	public List<Renderer> bones;

	// Token: 0x040031CC RID: 12748
	public List<Renderer> always;

	// Token: 0x040031CD RID: 12749
	public Transform coreMarker;

	// Token: 0x040031CE RID: 12750
	public GRCollectible corePrefab;

	// Token: 0x040031CF RID: 12751
	public Transform headTransform;

	// Token: 0x040031D0 RID: 12752
	public float sightDist;

	// Token: 0x040031D1 RID: 12753
	public float loseSightDist;

	// Token: 0x040031D2 RID: 12754
	public float sightFOV;

	// Token: 0x040031D3 RID: 12755
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x040031D4 RID: 12756
	public float searchTime = 5f;

	// Token: 0x040031D5 RID: 12757
	public float turnSpeed = 540f;

	// Token: 0x040031D6 RID: 12758
	public Color chaseColor = Color.red;

	// Token: 0x040031D7 RID: 12759
	public AudioClip chaseSound;

	// Token: 0x040031D8 RID: 12760
	public float chaseSoundVolume;

	// Token: 0x040031D9 RID: 12761
	public float rangedAttackDistMin = 6f;

	// Token: 0x040031DA RID: 12762
	public float rangedAttackDistMax = 8f;

	// Token: 0x040031DB RID: 12763
	public float rangedAttackChargeTime = 0.5f;

	// Token: 0x040031DC RID: 12764
	public float rangedAttackRecoverTime = 2f;

	// Token: 0x040031DD RID: 12765
	public float projectileSpeed = 5f;

	// Token: 0x040031DE RID: 12766
	public float projectileHitRadius = 1f;

	// Token: 0x040031DF RID: 12767
	public GameObject rangedProjectilePrefab;

	// Token: 0x040031E0 RID: 12768
	public Transform rangedProjectileFirePoint;

	// Token: 0x040031E1 RID: 12769
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x040031E2 RID: 12770
	public NavMeshAgent navAgent;

	// Token: 0x040031E3 RID: 12771
	public AudioSource audioSource;

	// Token: 0x040031E4 RID: 12772
	public AudioClip damagedSound;

	// Token: 0x040031E5 RID: 12773
	public float damagedSoundVolume;

	// Token: 0x040031E6 RID: 12774
	public GameObject fxDamaged;

	// Token: 0x040031E7 RID: 12775
	public bool lastMoving;

	// Token: 0x040031E8 RID: 12776
	public double lastStateChange;

	// Token: 0x040031E9 RID: 12777
	public float ambientSoundVolume = 0.5f;

	// Token: 0x040031EA RID: 12778
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x040031EB RID: 12779
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x040031EC RID: 12780
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x040031ED RID: 12781
	private double lastPartrolAmbientSoundTime;

	// Token: 0x040031EE RID: 12782
	private double nextPatrolGroanTime;

	// Token: 0x040031EF RID: 12783
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x040031F0 RID: 12784
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x040031F1 RID: 12785
	public bool debugLog;

	// Token: 0x040031F2 RID: 12786
	public GameObject spitterHeadOnShoulders;

	// Token: 0x040031F3 RID: 12787
	public GameObject spitterHeadOnShouldersLight;

	// Token: 0x040031F4 RID: 12788
	public GameObject spitterHeadOnShouldersVFX;

	// Token: 0x040031F5 RID: 12789
	public GameObject spitterHeadInHand;

	// Token: 0x040031F6 RID: 12790
	public GameObject spitterHeadInHandLight;

	// Token: 0x040031F7 RID: 12791
	public GameObject spitterHeadInHandVFX;

	// Token: 0x040031F8 RID: 12792
	public double spitterLightTurnOffDelay = 0.75;

	// Token: 0x040031F9 RID: 12793
	private bool headLightReset;

	// Token: 0x040031FA RID: 12794
	private double spitterLightTurnOffTime;

	// Token: 0x040031FB RID: 12795
	[FormerlySerializedAs("headRemovalInterval")]
	public float headRemovalFrame = 0.23333333f;

	// Token: 0x040031FC RID: 12796
	private double headRemovaltime;

	// Token: 0x040031FD RID: 12797
	private bool headRemoved;

	// Token: 0x040031FE RID: 12798
	private Transform target;

	// Token: 0x040031FF RID: 12799
	[ReadOnly]
	public int hp;

	// Token: 0x04003200 RID: 12800
	[ReadOnly]
	public GREnemyRanged.Behavior currBehavior;

	// Token: 0x04003201 RID: 12801
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003202 RID: 12802
	[ReadOnly]
	public GREnemyRanged.BodyState currBodyState;

	// Token: 0x04003203 RID: 12803
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003204 RID: 12804
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003205 RID: 12805
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003206 RID: 12806
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003207 RID: 12807
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003208 RID: 12808
	[ReadOnly]
	public Vector3 rangedFiringPosition;

	// Token: 0x04003209 RID: 12809
	[ReadOnly]
	public Vector3 rangedTargetPosition;

	// Token: 0x0400320A RID: 12810
	[ReadOnly]
	private GRPlayer bestTargetPlayer;

	// Token: 0x0400320B RID: 12811
	[ReadOnly]
	private NetPlayer bestTargetNetPlayer;

	// Token: 0x0400320C RID: 12812
	private bool rangedAttackQueued;

	// Token: 0x0400320D RID: 12813
	private double queuedFiringTime;

	// Token: 0x0400320E RID: 12814
	private Vector3 queuedFiringPosition;

	// Token: 0x0400320F RID: 12815
	private Vector3 queuedTargetPosition;

	// Token: 0x04003210 RID: 12816
	private GameObject rangedProjectileInstance;

	// Token: 0x04003211 RID: 12817
	private bool projectileHasImpacted;

	// Token: 0x04003212 RID: 12818
	private double projectileImpactTime;

	// Token: 0x04003213 RID: 12819
	private Rigidbody rigidBody;

	// Token: 0x04003214 RID: 12820
	private List<Collider> colliders;

	// Token: 0x04003215 RID: 12821
	private LayerMask visibilityLayerMask;

	// Token: 0x04003216 RID: 12822
	private Color defaultColor;

	// Token: 0x04003217 RID: 12823
	private float lastHitPlayerTime;

	// Token: 0x04003218 RID: 12824
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003219 RID: 12825
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400321A RID: 12826
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x02000633 RID: 1587
	public enum Behavior
	{
		// Token: 0x0400321C RID: 12828
		Idle,
		// Token: 0x0400321D RID: 12829
		Patrol,
		// Token: 0x0400321E RID: 12830
		Search,
		// Token: 0x0400321F RID: 12831
		Stagger,
		// Token: 0x04003220 RID: 12832
		Dying,
		// Token: 0x04003221 RID: 12833
		SeekRangedAttackPosition,
		// Token: 0x04003222 RID: 12834
		RangedAttack,
		// Token: 0x04003223 RID: 12835
		RangedAttackCooldown,
		// Token: 0x04003224 RID: 12836
		Flashed,
		// Token: 0x04003225 RID: 12837
		Count
	}

	// Token: 0x02000634 RID: 1588
	public enum BodyState
	{
		// Token: 0x04003227 RID: 12839
		Destroyed,
		// Token: 0x04003228 RID: 12840
		Bones,
		// Token: 0x04003229 RID: 12841
		Shell,
		// Token: 0x0400322A RID: 12842
		Count
	}
}
