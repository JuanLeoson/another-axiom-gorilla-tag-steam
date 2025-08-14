using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200062F RID: 1583
public class GREnemyPhantom : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameAgentComponent
{
	// Token: 0x060026E3 RID: 9955 RVA: 0x000D08BC File Offset: 0x000CEABC
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
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
		this.abilityMine.Setup(this.agent, this.anim, this.audioSource);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource);
		this.abilityRage.Setup(this.agent, this.anim, base.transform);
		this.abilityAlert.Setup(this.agent, this.anim, base.transform);
		this.abilityChase.Setup(this.agent, this.anim, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityReturn.Setup(this.agent, this.anim, base.transform);
		this.abilityAttack.Setup(this.agent, this.anim, base.transform);
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000D0A38 File Offset: 0x000CEC38
	public void OnEntityInit()
	{
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
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000D0B44 File Offset: 0x000CED44
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000D0B74 File Offset: 0x000CED74
	private void Setup(int patrolPathId)
	{
		this.SetPatrolPath(patrolPathId);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[0];
			this.idleLocation = this.target;
			this.SetBehavior(GREnemyPhantom.Behavior.Return, true);
		}
		else
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Mine, true);
		}
		this.SetBodyState(GREnemyPhantom.BodyState.Bones, true);
		if (this.attackLight != null)
		{
			this.attackLight.gameObject.SetActive(false);
		}
		if (this.negativeLight != null)
		{
			this.negativeLight.gameObject.SetActive(false);
		}
		GREnemyPhantom.Hide(this.bones, false);
		GREnemyPhantom.Hide(this.always, false);
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x000D0C43 File Offset: 0x000CEE43
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 7)
		{
			return;
		}
		this.SetBehavior((GREnemyPhantom.Behavior)newState, false);
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000D0C56 File Offset: 0x000CEE56
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 2)
		{
			return;
		}
		this.SetBodyState((GREnemyPhantom.BodyState)newState, false);
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000D0C6C File Offset: 0x000CEE6C
	public void SetPatrolPath(int patrolPathId)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(patrolPathId);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000D0C97 File Offset: 0x000CEE97
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000D0CA0 File Offset: 0x000CEEA0
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000D0CAC File Offset: 0x000CEEAC
	public void SetBehavior(GREnemyPhantom.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Stop();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Stop();
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Stop();
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Stop();
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Stop();
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Stop();
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(false);
			}
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Start();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Start();
			this.soundAlert.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Start();
			this.soundReturn.Play(this.audioSource);
			this.abilityReturn.SetTarget(this.idleLocation);
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Start();
			this.soundRage.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Start();
			this.soundChase.Play(this.audioSource);
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			this.soundAttack.Play(this.audioSource);
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(true);
			}
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000D0F30 File Offset: 0x000CF130
	public void SetBodyState(GREnemyPhantom.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000D0FAC File Offset: 0x000CF1AC
	private void RefreshBody()
	{
		GREnemyPhantom.BodyState bodyState = this.currBodyState;
		if (bodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			this.armor.SetHp(0);
			return;
		}
		if (bodyState != GREnemyPhantom.BodyState.Bones)
		{
			return;
		}
		this.armor.SetHp(0);
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000D0FE1 File Offset: 0x000CF1E1
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000D0FF0 File Offset: 0x000CF1F0
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPhantom.tempRigs.Clear();
		GREnemyPhantom.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPhantom.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPhantom.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Idle:
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Alert:
		case GREnemyPhantom.Behavior.Rage:
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.SetTarget(this.idleLocation);
			this.abilityReturn.Think(dt);
			if (this.senseNearby.IsAnyoneNearby())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Chase:
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

	// Token: 0x060026F3 RID: 9971 RVA: 0x000D112B File Offset: 0x000CF32B
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000D114C File Offset: 0x000CF34C
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Update(dt);
			if (this.idleLocation != null)
			{
				GameAgent.UpdateFacingDir(base.transform, this.agent.navAgent, this.idleLocation.forward, 180f);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyPhantom.Behavior.Alert:
			this.UpdateAlert(dt);
			return;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Update(dt);
			if (this.abilityReturn.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Mine, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Update(dt);
			if (this.abilityRage.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPhantom.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Update(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000D12C2 File Offset: 0x000CF4C2
	public void OnUpdateRemote(float dt)
	{
		if (this.currBehavior == GREnemyPhantom.Behavior.Attack)
		{
			this.abilityAttack.UpdateRemote(dt);
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000D12DC File Offset: 0x000CF4DC
	public void UpdateAlert(float dt)
	{
		this.abilityAlert.SetTargetPlayer(this.agent.targetPlayer);
		this.abilityAlert.Update(dt);
		double timeAsDouble = Time.timeAsDouble;
		if (!this.senseNearby.IsAnyoneNearby())
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
			return;
		}
		float num;
		if (this.abilityAlert.IsDone() && this.senseNearby.PickClosest(out num) != null)
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Rage, false);
		}
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000D1354 File Offset: 0x000CF554
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyPhantom.Behavior.Attack)
		{
			return;
		}
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

	// Token: 0x060026F8 RID: 9976 RVA: 0x000D145C File Offset: 0x000CF65C
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

	// Token: 0x060026F9 RID: 9977 RVA: 0x000D14A0 File Offset: 0x000CF6A0
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.hp;
		byte value4 = (byte)this.nextPatrolNode;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(value3);
		writer.Write(value4);
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000D14EC File Offset: 0x000CF6EC
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPhantom.Behavior newBehavior = (GREnemyPhantom.Behavior)reader.ReadByte();
		GREnemyPhantom.BodyState newBodyState = (GREnemyPhantom.BodyState)reader.ReadByte();
		int num = (int)reader.ReadByte();
		byte b = reader.ReadByte();
		this.SetPatrolPath((int)this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x04003189 RID: 12681
	public GameEntity entity;

	// Token: 0x0400318A RID: 12682
	public GameAgent agent;

	// Token: 0x0400318B RID: 12683
	public GRArmorEnemy armor;

	// Token: 0x0400318C RID: 12684
	public GRAttributes attributes;

	// Token: 0x0400318D RID: 12685
	public Animation anim;

	// Token: 0x0400318E RID: 12686
	public GRSenseNearby senseNearby;

	// Token: 0x0400318F RID: 12687
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003190 RID: 12688
	public GRAbilityIdle abilityMine;

	// Token: 0x04003191 RID: 12689
	public AbilitySound soundMine;

	// Token: 0x04003192 RID: 12690
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003193 RID: 12691
	public GRAbilityWatch abilityRage;

	// Token: 0x04003194 RID: 12692
	public AbilitySound soundRage;

	// Token: 0x04003195 RID: 12693
	public GRAbilityWatch abilityAlert;

	// Token: 0x04003196 RID: 12694
	public AbilitySound soundAlert;

	// Token: 0x04003197 RID: 12695
	public GRAbilityChase abilityChase;

	// Token: 0x04003198 RID: 12696
	public AbilitySound soundChase;

	// Token: 0x04003199 RID: 12697
	public GRAbilityMoveToTarget abilityReturn;

	// Token: 0x0400319A RID: 12698
	public AbilitySound soundReturn;

	// Token: 0x0400319B RID: 12699
	public GRAbilityAttackLatchOn abilityAttack;

	// Token: 0x0400319C RID: 12700
	public AbilitySound soundAttack;

	// Token: 0x0400319D RID: 12701
	public List<Renderer> bones;

	// Token: 0x0400319E RID: 12702
	public List<Renderer> always;

	// Token: 0x0400319F RID: 12703
	public Transform coreMarker;

	// Token: 0x040031A0 RID: 12704
	public GRCollectible corePrefab;

	// Token: 0x040031A1 RID: 12705
	public Transform headTransform;

	// Token: 0x040031A2 RID: 12706
	public float attackRange = 2f;

	// Token: 0x040031A3 RID: 12707
	public List<VRRig> rigsNearby;

	// Token: 0x040031A4 RID: 12708
	public GameLight attackLight;

	// Token: 0x040031A5 RID: 12709
	public GameLight negativeLight;

	// Token: 0x040031A6 RID: 12710
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x040031A7 RID: 12711
	private Transform idleLocation;

	// Token: 0x040031A8 RID: 12712
	public NavMeshAgent navAgent;

	// Token: 0x040031A9 RID: 12713
	public AudioSource audioSource;

	// Token: 0x040031AA RID: 12714
	public double lastStateChange;

	// Token: 0x040031AB RID: 12715
	private Transform target;

	// Token: 0x040031AC RID: 12716
	[ReadOnly]
	public int hp;

	// Token: 0x040031AD RID: 12717
	[ReadOnly]
	public GREnemyPhantom.Behavior currBehavior;

	// Token: 0x040031AE RID: 12718
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040031AF RID: 12719
	[ReadOnly]
	public GREnemyPhantom.BodyState currBodyState;

	// Token: 0x040031B0 RID: 12720
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x040031B1 RID: 12721
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040031B2 RID: 12722
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x040031B3 RID: 12723
	private Rigidbody rigidBody;

	// Token: 0x040031B4 RID: 12724
	private List<Collider> colliders;

	// Token: 0x040031B5 RID: 12725
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x02000630 RID: 1584
	public enum Behavior
	{
		// Token: 0x040031B7 RID: 12727
		Mine,
		// Token: 0x040031B8 RID: 12728
		Idle,
		// Token: 0x040031B9 RID: 12729
		Alert,
		// Token: 0x040031BA RID: 12730
		Return,
		// Token: 0x040031BB RID: 12731
		Rage,
		// Token: 0x040031BC RID: 12732
		Chase,
		// Token: 0x040031BD RID: 12733
		Attack,
		// Token: 0x040031BE RID: 12734
		Count
	}

	// Token: 0x02000631 RID: 1585
	public enum BodyState
	{
		// Token: 0x040031C0 RID: 12736
		Destroyed,
		// Token: 0x040031C1 RID: 12737
		Bones,
		// Token: 0x040031C2 RID: 12738
		Count
	}
}
