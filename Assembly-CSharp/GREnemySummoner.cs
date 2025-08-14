using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000635 RID: 1589
public class GREnemySummoner : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameEntityDebugComponent
{
	// Token: 0x0600272C RID: 10028 RVA: 0x000D3268 File Offset: 0x000D1468
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		this.trackedEntities = new List<int>();
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent = base.GetComponent<GameAgent>();
		this.entity = base.GetComponent<GameEntity>();
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource);
		this.abilityWander.Setup(this.agent, this.anim, base.transform);
		this.abilityDie.Setup(this.agent, this.anim, base.transform);
		this.abilitySummon.Setup(this.agent, this.anim, base.transform);
		this.abilityKeepDistance.Setup(this.agent, this.anim);
		this.abilityMoveToTarget.Setup(this.agent, this.anim, base.transform);
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000D33EC File Offset: 0x000D15EC
	public void OnEntityInit()
	{
		this.SetBehavior(GREnemySummoner.Behavior.Idle, true);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactor reactor = this.entity.manager.ghostReactorManager.reactor;
			foreach (GRBonusEntry entry in reactor.GetDepthLevelConfig(reactor.GetDepthLevel()).configGenOptions[0].enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000D3500 File Offset: 0x000D1700
	private void OnDisable()
	{
		Debug.Log("PrintOnDisable: script was disabled");
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000D350C File Offset: 0x000D170C
	private void OnEnable()
	{
		Debug.Log("PrintOnEnable: script was enabled");
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000D3518 File Offset: 0x000D1718
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000D3531 File Offset: 0x000D1731
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 7)
		{
			return;
		}
		this.SetBehavior((GREnemySummoner.Behavior)newState, false);
	}

	// Token: 0x06002734 RID: 10036 RVA: 0x000D3544 File Offset: 0x000D1744
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000D3550 File Offset: 0x000D1750
	public void SetBehavior(GREnemySummoner.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Stop();
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(false);
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Stop();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Start();
			this.soundWander.Play(this.audioSource);
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Start();
			break;
		case GREnemySummoner.Behavior.Summon:
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(true);
			}
			this.lastSummonTime = Time.timeAsDouble;
			this.abilitySummon.SetLookAtTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilitySummon.Start();
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.SetTargetPlayer(this.agent.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.SetTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilityMoveToTarget.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000D375A File Offset: 0x000D195A
	private void Update()
	{
		this.OnThink(Time.deltaTime);
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000D3774 File Offset: 0x000D1974
	public void OnThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		GREnemySummoner.tempRigs.Clear();
		GREnemySummoner.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemySummoner.tempRigs);
		this.senseNearby.UpdateNearby(GREnemySummoner.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		double num2 = Time.timeAsDouble - this.behaviorStartTime;
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.ChooseNewBehavior();
				return;
			}
			if (num2 > (double)this.idleDuration)
			{
				this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Think(dt);
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.Stagger:
		case GREnemySummoner.Behavior.Destroyed:
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Think(dt);
			if (this.abilitySummon.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Think(dt);
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.ChooseNewBehavior();
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Think(dt);
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.ChooseNewBehavior();
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000D38F5 File Offset: 0x000D1AF5
	public bool CanSummon()
	{
		return Time.timeAsDouble - this.lastSummonTime >= (double)this.minSummonInterval && this.trackedEntities.Count < this.maxSimultaneousSummonedEntities;
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000D3924 File Offset: 0x000D1B24
	public Transform GetPlayerTransform(NetPlayer targetPlayer)
	{
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				return grplayer.transform;
			}
		}
		return null;
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000D395C File Offset: 0x000D1B5C
	private void ChooseNewBehavior()
	{
		float num = 0f;
		if (!(this.senseNearby.PickClosest(out num) != null))
		{
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			return;
		}
		float num2 = (this.currBehavior == GREnemySummoner.Behavior.KeepDistance) ? (this.keepDistanceThreshold + 1f) : this.keepDistanceThreshold;
		if (num < num2 * num2)
		{
			this.SetBehavior(GREnemySummoner.Behavior.KeepDistance, false);
			return;
		}
		if (this.CanSummon())
		{
			this.SetBehavior(GREnemySummoner.Behavior.Summon, false);
			return;
		}
		float num3 = this.tooFarDistanceThreshold * this.tooFarDistanceThreshold;
		if (num > num3)
		{
			this.SetBehavior(GREnemySummoner.Behavior.MoveToTarget, false);
			return;
		}
		this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000D39EE File Offset: 0x000D1BEE
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000D3A0C File Offset: 0x000D1C0C
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Update(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Update(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Update(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Update(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000D3AB4 File Offset: 0x000D1CB4
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Update(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			return;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Update(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Update(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Update(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000D3B38 File Offset: 0x000D1D38
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.hp;
		writer.Write(value);
		writer.Write(value2);
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000D3B64 File Offset: 0x000D1D64
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemySummoner.Behavior newBehavior = (GREnemySummoner.Behavior)reader.ReadByte();
		int num = (int)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000D3B90 File Offset: 0x000D1D90
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			if (hitTypeId == GameHitType.Club)
			{
				this.OnHitByClub(hit);
				return;
			}
			this.OnHitByFlash(hit);
		}
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000D3BD4 File Offset: 0x000D1DD4
	public void OnHitByClub(GameHitData hit)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Destroyed)
		{
			return;
		}
		this.hp--;
		if (this.hp <= 0)
		{
			Vector3 hitImpulse = hit.hitImpulse;
			hitImpulse.y = 0f;
			this.abilityDie.SetStaggerVelocity(hitImpulse);
			this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
			return;
		}
		Vector3 hitImpulse2 = hit.hitImpulse;
		hitImpulse2.y = 0f;
		Vector3 staggerVel = hitImpulse2;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemySummoner.Behavior.Stagger, false);
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000D3C6C File Offset: 0x000D1E6C
	public void OnHitByFlash(GameHitData hit)
	{
		Vector3 hitImpulse = hit.hitImpulse;
		hitImpulse.y = 0f;
		Vector3 staggerVel = hitImpulse;
		this.abilityStagger.Setup(staggerVel, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemySummoner.Behavior.Stagger, false);
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000D3CBC File Offset: 0x000D1EBC
	private void OnTriggerEnter(Collider collider)
	{
		Debug.LogFormat("Summoner Hitting player {0} {1}", new object[]
		{
			collider,
			collider.isTrigger
		});
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position
				};
				component3.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000D3DD4 File Offset: 0x000D1FD4
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

	// Token: 0x06002746 RID: 10054 RVA: 0x000D3E18 File Offset: 0x000D2018
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("Nearby rigs: <color=\"yellow\">{0}<color=\"white\">", this.senseNearby.rigsNearby.Count));
		strings.Add(string.Format("Spawned entities: <color=\"yellow\">{0}<color=\"white\">", this.trackedEntities.Count));
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x000D3EA0 File Offset: 0x000D20A0
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000D3EC0 File Offset: 0x000D20C0
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x0400322B RID: 12843
	private GameEntity entity;

	// Token: 0x0400322C RID: 12844
	private GameAgent agent;

	// Token: 0x0400322D RID: 12845
	public GRArmorEnemy armor;

	// Token: 0x0400322E RID: 12846
	public GRAttributes attributes;

	// Token: 0x0400322F RID: 12847
	public Animation anim;

	// Token: 0x04003230 RID: 12848
	public GRSenseNearby senseNearby;

	// Token: 0x04003231 RID: 12849
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003232 RID: 12850
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003233 RID: 12851
	public GRAbilityWander abilityWander;

	// Token: 0x04003234 RID: 12852
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x04003235 RID: 12853
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003236 RID: 12854
	public GRAbilityDie abilityDie;

	// Token: 0x04003237 RID: 12855
	public GRAbilitySummon abilitySummon;

	// Token: 0x04003238 RID: 12856
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x04003239 RID: 12857
	public GRAbilityMoveToTarget abilityMoveToTarget;

	// Token: 0x0400323A RID: 12858
	public AbilitySound soundWander;

	// Token: 0x0400323B RID: 12859
	public AbilitySound soundAttack;

	// Token: 0x0400323C RID: 12860
	public GameLight summonLight;

	// Token: 0x0400323D RID: 12861
	public List<Renderer> bones;

	// Token: 0x0400323E RID: 12862
	public List<Renderer> always;

	// Token: 0x0400323F RID: 12863
	public Transform coreMarker;

	// Token: 0x04003240 RID: 12864
	public GRCollectible corePrefab;

	// Token: 0x04003241 RID: 12865
	public Transform headTransform;

	// Token: 0x04003242 RID: 12866
	public float attackRange = 2f;

	// Token: 0x04003243 RID: 12867
	public List<VRRig> rigsNearby;

	// Token: 0x04003244 RID: 12868
	public NavMeshAgent navAgent;

	// Token: 0x04003245 RID: 12869
	public AudioSource audioSource;

	// Token: 0x04003246 RID: 12870
	public float idleDuration = 2f;

	// Token: 0x04003247 RID: 12871
	public float keepDistanceThreshold = 3f;

	// Token: 0x04003248 RID: 12872
	public float tooFarDistanceThreshold = 5f;

	// Token: 0x04003249 RID: 12873
	public double lastSummonTime;

	// Token: 0x0400324A RID: 12874
	public float minSummonInterval = 4f;

	// Token: 0x0400324B RID: 12875
	public int maxSimultaneousSummonedEntities = 3;

	// Token: 0x0400324C RID: 12876
	[ReadOnly]
	public int hp;

	// Token: 0x0400324D RID: 12877
	[ReadOnly]
	public GREnemySummoner.Behavior currBehavior;

	// Token: 0x0400324E RID: 12878
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x0400324F RID: 12879
	[ReadOnly]
	public GREnemySummoner.BodyState currBodyState;

	// Token: 0x04003250 RID: 12880
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003251 RID: 12881
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04003252 RID: 12882
	private Rigidbody rigidBody;

	// Token: 0x04003253 RID: 12883
	private List<Collider> colliders;

	// Token: 0x04003254 RID: 12884
	private List<int> trackedEntities;

	// Token: 0x04003255 RID: 12885
	private float lastUpdateTime;

	// Token: 0x04003256 RID: 12886
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x02000636 RID: 1590
	public enum Behavior
	{
		// Token: 0x04003258 RID: 12888
		Idle,
		// Token: 0x04003259 RID: 12889
		Wander,
		// Token: 0x0400325A RID: 12890
		Stagger,
		// Token: 0x0400325B RID: 12891
		Destroyed,
		// Token: 0x0400325C RID: 12892
		Summon,
		// Token: 0x0400325D RID: 12893
		KeepDistance,
		// Token: 0x0400325E RID: 12894
		MoveToTarget,
		// Token: 0x0400325F RID: 12895
		Count
	}

	// Token: 0x02000637 RID: 1591
	public enum BodyState
	{
		// Token: 0x04003261 RID: 12897
		Destroyed,
		// Token: 0x04003262 RID: 12898
		Bones,
		// Token: 0x04003263 RID: 12899
		Count
	}
}
