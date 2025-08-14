using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000625 RID: 1573
public class GREnemyChaser : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent
{
	// Token: 0x06002691 RID: 9873 RVA: 0x000CE660 File Offset: 0x000CC860
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource);
		this.abilityChase.Setup(this.agent, this.anim, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource);
		this.abilityAttackSwipe.Setup(this.agent, this.anim, base.transform);
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
		this.abilityDie.Setup(this.agent, this.anim, base.transform);
		this.senseNearby.Setup(this.headTransform);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x000CE7E8 File Offset: 0x000CC9E8
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

	// Token: 0x06002693 RID: 9875 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000CE8E0 File Offset: 0x000CCAE0
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)(this.entity.GetNetId() + 1));
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)(this.entity.GetNetId() + 10));
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000CE913 File Offset: 0x000CCB13
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000CE944 File Offset: 0x000CCB44
	public void Setup(int patrolPathId)
	{
		this.SetPatrolPath(patrolPathId);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000CE9CE File Offset: 0x000CCBCE
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000CE9E1 File Offset: 0x000CCBE1
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000CE9F4 File Offset: 0x000CCBF4
	public void SetPatrolPath(int patrolPathId)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(patrolPathId);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000CEA1F File Offset: 0x000CCC1F
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000CEA28 File Offset: 0x000CCC28
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000CEA34 File Offset: 0x000CCC34
	public void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.behaviorEndTime = 1.0;
			this.abilityDie.Stop();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Stop();
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.PlayAnim("GREnemyChaserCrawl", 0.3f, 0.5f);
			this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
			this.CalculateNextPatrolGroan();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.PlayAnim("GREnemyChaserIdle", 0.1f, 1f);
			this.behaviorEndTime = 1.0;
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Start();
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Start();
			this.abilityAttackSwipe.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.PlayAnim("GREnemyChaserFlashed", 0.1f, 1f);
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

	// Token: 0x0600269E RID: 9886 RVA: 0x000CECD2 File Offset: 0x000CCED2
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000CED04 File Offset: 0x000CCF04
	public void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x000CEDDC File Offset: 0x000CCFDC
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemyChaser.Hide(this.bones, false);
			GREnemyChaser.Hide(this.always, false);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemyChaser.Hide(this.bones, false);
			GREnemyChaser.Hide(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemyChaser.Hide(this.bones, true);
			GREnemyChaser.Hide(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000CEE76 File Offset: 0x000CD076
	public void CalculateNextPatrolGroan()
	{
		if (this.lastPartrolAmbientSoundTime < this.lastStateChange)
		{
			this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
		}
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000CEEAC File Offset: 0x000CD0AC
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000CEF02 File Offset: 0x000CD102
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000CEF10 File Offset: 0x000CD110
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyChaser.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Patrol:
			if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Stagger:
		case GREnemyChaser.Behavior.Dying:
			break;
		case GREnemyChaser.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			break;
		case GREnemyChaser.Behavior.Search:
			if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000CF040 File Offset: 0x000CD240
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000CF060 File Offset: 0x000CD260
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.UpdatePatrol();
			GameAgent.UpdateFacing(base.transform, this.navAgent, null, this.turnSpeed);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Update(dt);
			break;
		case GREnemyChaser.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Update(dt);
			if (this.abilitySearch.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Update(dt);
			if (this.abilityAttackSwipe.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Flashed:
			if (Time.timeAsDouble >= this.behaviorEndTime)
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x000CF1FC File Offset: 0x000CD3FC
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.UpdatePatrolRemote();
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			return;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Update(dt);
			break;
		case GREnemyChaser.Behavior.Chase:
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000CF27C File Offset: 0x000CD47C
	public void UpdatePatrol()
	{
		if (this.patrolPath == null)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Idle, false);
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

	// Token: 0x060026A9 RID: 9897 RVA: 0x000CF355 File Offset: 0x000CD555
	public void UpdatePatrolRemote()
	{
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000CF36C File Offset: 0x000CD56C
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
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
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
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
		this.SetBehavior(GREnemyChaser.Behavior.Stagger, false);
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000CF4BC File Offset: 0x000CD6BC
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
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
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
				if (grTool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
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
		this.SetBehavior(GREnemyChaser.Behavior.Flashed, false);
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000CF644 File Offset: 0x000CD844
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyChaser.Behavior.Stagger, false);
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000CF694 File Offset: 0x000CD894
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position
				};
				component5.RequestHit(hitData);
			}
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000CF7EC File Offset: 0x000CD9EC
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyChaser.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000CF804 File Offset: 0x000CDA04
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

	// Token: 0x060026B0 RID: 9904 RVA: 0x000CF848 File Offset: 0x000CDA48
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

	// Token: 0x060026B1 RID: 9905 RVA: 0x000CF8B4 File Offset: 0x000CDAB4
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyChaser.Behavior newBehavior = (GREnemyChaser.Behavior)reader.ReadByte();
		GREnemyChaser.BodyState newBodyState = (GREnemyChaser.BodyState)reader.ReadByte();
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

	// Token: 0x060026B2 RID: 9906 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000CF928 File Offset: 0x000CDB28
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x040030FC RID: 12540
	public GameEntity entity;

	// Token: 0x040030FD RID: 12541
	public GameAgent agent;

	// Token: 0x040030FE RID: 12542
	public GRArmorEnemy armor;

	// Token: 0x040030FF RID: 12543
	public GameHittable hittable;

	// Token: 0x04003100 RID: 12544
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003101 RID: 12545
	public GRSenseNearby senseNearby;

	// Token: 0x04003102 RID: 12546
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003103 RID: 12547
	public Animation anim;

	// Token: 0x04003104 RID: 12548
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003105 RID: 12549
	public GRAbilityChase abilityChase;

	// Token: 0x04003106 RID: 12550
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003107 RID: 12551
	public GRAbilityAttackSwipe abilityAttackSwipe;

	// Token: 0x04003108 RID: 12552
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003109 RID: 12553
	public GRAbilityDie abilityDie;

	// Token: 0x0400310A RID: 12554
	public List<Renderer> bones;

	// Token: 0x0400310B RID: 12555
	public List<Renderer> always;

	// Token: 0x0400310C RID: 12556
	public Transform coreMarker;

	// Token: 0x0400310D RID: 12557
	public GRCollectible corePrefab;

	// Token: 0x0400310E RID: 12558
	public Transform headTransform;

	// Token: 0x0400310F RID: 12559
	public float turnSpeed = 540f;

	// Token: 0x04003110 RID: 12560
	public SoundBankPlayer chaseSoundBank;

	// Token: 0x04003111 RID: 12561
	public float attackRange = 1.5f;

	// Token: 0x04003112 RID: 12562
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003113 RID: 12563
	public NavMeshAgent navAgent;

	// Token: 0x04003114 RID: 12564
	public AudioSource audioSource;

	// Token: 0x04003115 RID: 12565
	public AudioClip damagedSound;

	// Token: 0x04003116 RID: 12566
	public float damagedSoundVolume;

	// Token: 0x04003117 RID: 12567
	public GameObject fxDamaged;

	// Token: 0x04003118 RID: 12568
	public double lastStateChange;

	// Token: 0x04003119 RID: 12569
	public float ambientSoundVolume = 0.5f;

	// Token: 0x0400311A RID: 12570
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x0400311B RID: 12571
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x0400311C RID: 12572
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x0400311D RID: 12573
	private double lastPartrolAmbientSoundTime;

	// Token: 0x0400311E RID: 12574
	private double nextPatrolGroanTime;

	// Token: 0x0400311F RID: 12575
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x04003120 RID: 12576
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x04003121 RID: 12577
	private Transform target;

	// Token: 0x04003122 RID: 12578
	[ReadOnly]
	public int hp;

	// Token: 0x04003123 RID: 12579
	[ReadOnly]
	public GREnemyChaser.Behavior currBehavior;

	// Token: 0x04003124 RID: 12580
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003125 RID: 12581
	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	// Token: 0x04003126 RID: 12582
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003127 RID: 12583
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003128 RID: 12584
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003129 RID: 12585
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x0400312A RID: 12586
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400312B RID: 12587
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x0400312C RID: 12588
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x0400312D RID: 12589
	private LayerMask visibilityLayerMask;

	// Token: 0x0400312E RID: 12590
	private Rigidbody rigidBody;

	// Token: 0x0400312F RID: 12591
	private List<Collider> colliders;

	// Token: 0x04003130 RID: 12592
	private float lastHitPlayerTime;

	// Token: 0x04003131 RID: 12593
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003132 RID: 12594
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003133 RID: 12595
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x02000626 RID: 1574
	public enum Behavior
	{
		// Token: 0x04003135 RID: 12597
		Idle,
		// Token: 0x04003136 RID: 12598
		Patrol,
		// Token: 0x04003137 RID: 12599
		Stagger,
		// Token: 0x04003138 RID: 12600
		Dying,
		// Token: 0x04003139 RID: 12601
		Chase,
		// Token: 0x0400313A RID: 12602
		Search,
		// Token: 0x0400313B RID: 12603
		Attack,
		// Token: 0x0400313C RID: 12604
		Flashed,
		// Token: 0x0400313D RID: 12605
		Count
	}

	// Token: 0x02000627 RID: 1575
	public enum BodyState
	{
		// Token: 0x0400313F RID: 12607
		Destroyed,
		// Token: 0x04003140 RID: 12608
		Bones,
		// Token: 0x04003141 RID: 12609
		Shell,
		// Token: 0x04003142 RID: 12610
		Count
	}
}
