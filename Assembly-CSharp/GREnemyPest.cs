using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000629 RID: 1577
public class GREnemyPest : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x060026BC RID: 9916 RVA: 0x000CFAD8 File Offset: 0x000CDCD8
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource);
		this.abilityChase.Setup(this.agent, this.anim, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, base.transform);
		this.abilityWander.Setup(this.agent, this.anim, base.transform);
		this.abilityDie.Setup(this.agent, this.anim, base.transform);
		this.abilityGrabbed.Setup(this.agent, this.anim, this.audioSource);
		this.abilityThrown.Setup(this.agent, this.anim, this.audioSource);
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		base.Invoke("PlaySpawnAudio", 0.1f);
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000CFCC3 File Offset: 0x000CDEC3
	private void PlaySpawnAudio()
	{
		this.spawnSound.Play(null);
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000CFCD4 File Offset: 0x000CDED4
	public void OnEntityInit()
	{
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactor reactor = this.entity.manager.ghostReactorManager.reactor;
			foreach (GRBonusEntry entry in reactor.GetDepthLevelConfig(reactor.GetDepthLevel()).configGenOptions[0].enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000CFDE8 File Offset: 0x000CDFE8
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000CFE01 File Offset: 0x000CE001
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyPest.Behavior)newState, false);
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000CFE14 File Offset: 0x000CE014
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000CFE20 File Offset: 0x000CE020
	public void SetBehavior(GREnemyPest.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Stop();
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Stop();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Stop();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Start();
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Start();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Start();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000CFFC7 File Offset: 0x000CE1C7
	private void OnGrabbed()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Grabbed, false);
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000CFFDB File Offset: 0x000CE1DB
	private void OnReleased()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Thrown, false);
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000CFFEF File Offset: 0x000CE1EF
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000CFFFC File Offset: 0x000CE1FC
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPest.tempRigs.Clear();
		GREnemyPest.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPest.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPest.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Think(dt);
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000D0101 File Offset: 0x000CE301
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000D0120 File Offset: 0x000CE320
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Update(dt);
			return;
		case GREnemyPest.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPest.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Update(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Grabbed:
			break;
		case GREnemyPest.Behavior.Thrown:
			if (this.abilityThrown.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
			}
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000D025C File Offset: 0x000CE45C
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Update(dt);
			break;
		case GREnemyPest.Behavior.Chase:
		case GREnemyPest.Behavior.Grabbed:
		case GREnemyPest.Behavior.Thrown:
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Update(dt);
			return;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			return;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000D02C8 File Offset: 0x000CE4C8
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.hp;
		writer.Write(value);
		writer.Write(value2);
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000D02F4 File Offset: 0x000CE4F4
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPest.Behavior newBehavior = (GREnemyPest.Behavior)reader.ReadByte();
		int num = (int)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000D0320 File Offset: 0x000CE520
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000D037C File Offset: 0x000CE57C
	public void OnHitByClub(GameHitData hit)
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
			return;
		}
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyPest.Behavior.Stagger, false);
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000D03FC File Offset: 0x000CE5FC
	public void OnHitByFlash(GameHitData hit)
	{
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyPest.Behavior.Stagger, false);
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000D044C File Offset: 0x000CE64C
	public void OnHitByShield(GameHitData hit)
	{
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyPest.Behavior.Stagger, false);
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x000D049C File Offset: 0x000CE69C
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyPest.Behavior.Attack)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			Vector3 enemyAttackDirection = this.abilityAttack.targetPos - this.abilityAttack.initialPos;
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(base.transform.position, enemyAttackDirection, component2);
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

	// Token: 0x060026D4 RID: 9940 RVA: 0x000D0601 File Offset: 0x000CE801
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyPest.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000D0618 File Offset: 0x000CE818
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

	// Token: 0x060026D6 RID: 9942 RVA: 0x000D0659 File Offset: 0x000CE859
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x04003147 RID: 12615
	public GameEntity entity;

	// Token: 0x04003148 RID: 12616
	public GameAgent agent;

	// Token: 0x04003149 RID: 12617
	public GRArmorEnemy armor;

	// Token: 0x0400314A RID: 12618
	public GRAttributes attributes;

	// Token: 0x0400314B RID: 12619
	public Animation anim;

	// Token: 0x0400314C RID: 12620
	public GRSenseNearby senseNearby;

	// Token: 0x0400314D RID: 12621
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x0400314E RID: 12622
	public GRAbilityIdle abilityIdle;

	// Token: 0x0400314F RID: 12623
	public GRAbilityChase abilityChase;

	// Token: 0x04003150 RID: 12624
	public GRAbilityWander abilityWander;

	// Token: 0x04003151 RID: 12625
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x04003152 RID: 12626
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003153 RID: 12627
	public GRAbilityDie abilityDie;

	// Token: 0x04003154 RID: 12628
	public GRAbilityGrabbed abilityGrabbed;

	// Token: 0x04003155 RID: 12629
	public GRAbilityThrown abilityThrown;

	// Token: 0x04003156 RID: 12630
	public AbilitySound spawnSound;

	// Token: 0x04003157 RID: 12631
	public List<Renderer> bones;

	// Token: 0x04003158 RID: 12632
	public List<Renderer> always;

	// Token: 0x04003159 RID: 12633
	public Transform coreMarker;

	// Token: 0x0400315A RID: 12634
	public GRCollectible corePrefab;

	// Token: 0x0400315B RID: 12635
	public Transform headTransform;

	// Token: 0x0400315C RID: 12636
	public float attackRange = 2f;

	// Token: 0x0400315D RID: 12637
	public List<VRRig> rigsNearby;

	// Token: 0x0400315E RID: 12638
	public NavMeshAgent navAgent;

	// Token: 0x0400315F RID: 12639
	public AudioSource audioSource;

	// Token: 0x04003160 RID: 12640
	[ReadOnly]
	public int hp;

	// Token: 0x04003161 RID: 12641
	[ReadOnly]
	public GREnemyPest.Behavior currBehavior;

	// Token: 0x04003162 RID: 12642
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003163 RID: 12643
	[ReadOnly]
	public GREnemyPest.BodyState currBodyState;

	// Token: 0x04003164 RID: 12644
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003165 RID: 12645
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003166 RID: 12646
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04003167 RID: 12647
	private Rigidbody rigidBody;

	// Token: 0x04003168 RID: 12648
	private List<Collider> colliders;

	// Token: 0x04003169 RID: 12649
	private float lastHitPlayerTime;

	// Token: 0x0400316A RID: 12650
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x0400316B RID: 12651
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400316C RID: 12652
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x0200062A RID: 1578
	public enum Behavior
	{
		// Token: 0x0400316E RID: 12654
		Idle,
		// Token: 0x0400316F RID: 12655
		Wander,
		// Token: 0x04003170 RID: 12656
		Chase,
		// Token: 0x04003171 RID: 12657
		Attack,
		// Token: 0x04003172 RID: 12658
		Stagger,
		// Token: 0x04003173 RID: 12659
		Grabbed,
		// Token: 0x04003174 RID: 12660
		Thrown,
		// Token: 0x04003175 RID: 12661
		Destroyed,
		// Token: 0x04003176 RID: 12662
		Count
	}

	// Token: 0x0200062B RID: 1579
	public enum BodyState
	{
		// Token: 0x04003178 RID: 12664
		Destroyed,
		// Token: 0x04003179 RID: 12665
		Bones,
		// Token: 0x0400317A RID: 12666
		Count
	}
}
