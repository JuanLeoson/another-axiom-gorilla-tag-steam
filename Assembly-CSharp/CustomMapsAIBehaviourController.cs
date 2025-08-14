using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000801 RID: 2049
public class CustomMapsAIBehaviourController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x0600333C RID: 13116 RVA: 0x0010AEB3 File Offset: 0x001090B3
	// (set) Token: 0x0600333B RID: 13115 RVA: 0x0010AEAA File Offset: 0x001090AA
	public GRPlayer TargetPlayer { get; set; }

	// Token: 0x0600333D RID: 13117 RVA: 0x0010AEBB File Offset: 0x001090BB
	private void Awake()
	{
		this.TargetPlayer = VRRig.LocalRig.GetComponent<GRPlayer>();
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x0010AEE4 File Offset: 0x001090E4
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x0600333F RID: 13119 RVA: 0x0010AEFD File Offset: 0x001090FD
	private void Update()
	{
		this.OnThink();
		this.UpdateAnimators();
	}

	// Token: 0x06003340 RID: 13120 RVA: 0x0010AF0B File Offset: 0x0010910B
	private void InitAnimators()
	{
		this.animators = base.gameObject.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06003341 RID: 13121 RVA: 0x0010AF20 File Offset: 0x00109120
	private void UpdateAnimators()
	{
		if (this.animators.IsNullOrEmpty<Animator>())
		{
			return;
		}
		float magnitude = this.agent.navAgent.velocity.magnitude;
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].SetFloat(CustomMapsAIBehaviourController.movementSpeedParamIndex, magnitude);
		}
	}

	// Token: 0x06003342 RID: 13122 RVA: 0x0010AF7C File Offset: 0x0010917C
	public void PlayAnimation(string stateName)
	{
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].Play(stateName);
		}
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x0010AFAC File Offset: 0x001091AC
	public void SetupBehaviours(AIAgent aiAgent)
	{
		List<AgentBehaviours> list = new List<AgentBehaviours>(8);
		for (int i = 0; i < aiAgent.agentBehaviours.Count; i++)
		{
			if (!list.Contains(aiAgent.agentBehaviours[i]))
			{
				AgentBehaviours agentBehaviours = aiAgent.agentBehaviours[i];
				if (agentBehaviours != AgentBehaviours.Search)
				{
					if (agentBehaviours != AgentBehaviours.Chase)
					{
						goto IL_96;
					}
					this.behaviourArray.Add(new CustomMapsChaseBehaviour(this, aiAgent.sightOffset, aiAgent.sightDist, aiAgent.loseSightDist, aiAgent.stopDist));
				}
				else
				{
					this.behaviourArray.Add(new CustomMapsSearchBehaviour(this, aiAgent.sightOffset, aiAgent.sightDist, aiAgent.sightFOV));
				}
				list.Add(aiAgent.agentBehaviours[i]);
			}
			IL_96:;
		}
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x0010B064 File Offset: 0x00109264
	public void RequestDestination(Vector3 destination)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.agent.RequestDestination(destination);
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x0010B080 File Offset: 0x00109280
	private void OnThink()
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (this.behaviourArray.IsNullOrEmpty<CustomMapsBehaviourBase>())
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < this.behaviourArray.Count; i++)
		{
			if (this.behaviourArray[i].CanExecute())
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		this.currentBehaviourIndex = num;
		this.behaviourArray[this.currentBehaviourIndex].Execute();
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnNetworkBehaviourStateChanged(byte newstate)
	{
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x0010B0FC File Offset: 0x001092FC
	public void OnEntityInit()
	{
		GTDev.Log<string>("CustomMapsAIBehaviourController::OnEntityInit", null);
		this.entity.transform.parent = AISpawnManager.instance.transform;
		byte enemyTypeIndex;
		AIAgent.UnpackCreateData(this.entity.createData, out enemyTypeIndex, out this.luaAgentID);
		AIAgent aiagent;
		if (!AISpawnManager.instance.SpawnEnemy((int)enemyTypeIndex, out aiagent))
		{
			GTDev.LogError<string>("CustomMapsAIBehaviourController::OnEntityInit could not spawn enemy", null);
			Object.Destroy(base.gameObject);
			return;
		}
		aiagent.gameObject.SetActive(true);
		aiagent.transform.parent = this.entity.transform;
		aiagent.transform.localPosition = Vector3.zero;
		aiagent.transform.localRotation = Quaternion.identity;
		this.InitAnimators();
		NavMeshAgent component = this.entity.gameObject.GetComponent<NavMeshAgent>();
		if (component.IsNull())
		{
			GTDev.LogError<string>("nav mesh agent is null", null);
			Object.Destroy(base.gameObject);
			return;
		}
		component.agentTypeID = this.GetNavAgentType(aiagent.navAgentType);
		component.speed = aiagent.movementSpeed;
		component.angularSpeed = aiagent.turnSpeed;
		component.acceleration = aiagent.acceleration;
		this.SetupBehaviours(aiagent);
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x0010B224 File Offset: 0x00109424
	private int GetNavAgentType(NavAgentType navType)
	{
		int settingsCount = NavMesh.GetSettingsCount();
		int agentTypeID = NavMesh.GetSettingsByIndex(0).agentTypeID;
		for (int i = 0; i < settingsCount; i++)
		{
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(i);
			if (NavMesh.GetSettingsNameFromID(settingsByIndex.agentTypeID) == navType.ToString())
			{
				agentTypeID = settingsByIndex.agentTypeID;
				break;
			}
		}
		return agentTypeID;
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x04004051 RID: 16465
	private static readonly int movementSpeedParamIndex = Animator.StringToHash("MovementSpeed");

	// Token: 0x04004052 RID: 16466
	public GameEntity entity;

	// Token: 0x04004053 RID: 16467
	public GameAgent agent;

	// Token: 0x04004054 RID: 16468
	private Animator[] animators;

	// Token: 0x04004055 RID: 16469
	public short luaAgentID;

	// Token: 0x04004057 RID: 16471
	private List<CustomMapsBehaviourBase> behaviourArray = new List<CustomMapsBehaviourBase>(8);

	// Token: 0x04004058 RID: 16472
	private int currentBehaviourIndex;

	// Token: 0x02000802 RID: 2050
	public enum CustomMapsAIBehaviour
	{
		// Token: 0x0400405A RID: 16474
		Search,
		// Token: 0x0400405B RID: 16475
		Chase
	}
}
