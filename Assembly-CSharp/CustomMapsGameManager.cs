using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000805 RID: 2053
public class CustomMapsGameManager : MonoBehaviour, IGameEntityZoneComponent
{
	// Token: 0x06003353 RID: 13139 RVA: 0x0010B2C6 File Offset: 0x001094C6
	private void Awake()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			Object.Destroy(this);
			return;
		}
		CustomMapsGameManager.instance = this;
		this.customMapsAgents = new Dictionary<int, AIAgent>(Constants.aiAgentLimit);
		CustomMapsGameManager.tempCreateEntitiesList = new List<GameEntityCreateData>(Constants.aiAgentLimit);
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x0010B300 File Offset: 0x00109500
	public void CreatePlacedAgents(List<AIAgent> agents)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("CustomMapsManager::CreateAIAgents not the authority", null);
			return;
		}
		int gameAgentCount = this.gameAgentManager.GetGameAgentCount();
		if (gameAgentCount >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>("[CustomMapsGameManager::CreateAIAgents] Failed to create agent. Max Agent count " + string.Format("({0}) has been reached!", Constants.aiAgentLimit), null);
			return;
		}
		CustomMapsGameManager.tempCreateEntitiesList.Clear();
		int b = (Constants.aiAgentLimit - gameAgentCount < 0) ? 0 : (Constants.aiAgentLimit - gameAgentCount);
		int num = Mathf.Min(agents.Count, b);
		if (num < agents.Count)
		{
			GTDev.LogWarning<string>(string.Format("[CustomMapsGameManager::CreateAIAgents] Only creating {0} out of the ", num) + string.Format("requested {0} agents. Max Agent count ({1}) has been reached.!", agents.Count, Constants.aiAgentLimit), null);
		}
		for (int i = 0; i < num; i++)
		{
			int staticHash = "CustomMapsAIAgent".GetStaticHash();
			if (!this.gameEntityManager.FactoryHasEntity(staticHash))
			{
				Debug.LogErrorFormat("[CustomMapsManager::CreateAIAgents] Cannot Find Entity in Factory {0} {1}", new object[]
				{
					agents[i].gameObject.name,
					staticHash
				});
			}
			else
			{
				GameEntityCreateData item = new GameEntityCreateData
				{
					entityTypeId = staticHash,
					localPosition = agents[i].transform.position,
					localRotation = agents[i].transform.rotation,
					createData = agents[i].GetPackedCreateData()
				};
				CustomMapsGameManager.tempCreateEntitiesList.Add(item);
			}
		}
		if (CustomMapsGameManager.tempCreateEntitiesList.Count > 0)
		{
			this.gameEntityManager.RequestCreateItems(CustomMapsGameManager.tempCreateEntitiesList);
			CustomMapsGameManager.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x0010B4B6 File Offset: 0x001096B6
	public void TEST_Spawning()
	{
		GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn starting spawn", null);
		base.StartCoroutine(this.TEST_Spawn());
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x0010B4D0 File Offset: 0x001096D0
	private IEnumerator TEST_Spawn()
	{
		while (this.spawnCount < 10)
		{
			yield return new WaitForSeconds(5f);
			GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn spawning enemy", null);
			this.TEST_index = ((this.TEST_index == 5) ? 3 : 5);
			this.SpawnEnemyFromPoint("79e43963", this.TEST_index);
			this.spawnCount++;
		}
		yield break;
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x0010B4E0 File Offset: 0x001096E0
	public GameEntityId SpawnEnemyFromPoint(string spawnPointId, int enemyTypeId)
	{
		AISpawnPoint aispawnPoint;
		if (!AISpawnManager.instance.GetSpawnPoint(spawnPointId, out aispawnPoint))
		{
			GTDev.LogError<string>("CustomMapsGameManager::SpawnEnemyFromPoint cannot find spawn point", null);
			return GameEntityId.Invalid;
		}
		return this.SpawnEnemyAtLocation(enemyTypeId, aispawnPoint.transform.position, aispawnPoint.transform.rotation);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x0010B52C File Offset: 0x0010972C
	public GameEntityId SpawnEnemyAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameAgentManager.GetGameAgentCount() >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Max Agents ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsAIAgent".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x0010B5C4 File Offset: 0x001097C4
	public void SpawnEnemyClient(int enemyTypeId, int agentId)
	{
		if (this.gameEntityManager.IsAuthority())
		{
			return;
		}
		if (enemyTypeId == -1)
		{
			return;
		}
		AIAgent aiagent;
		if (AISpawnManager.instance.SpawnEnemy(enemyTypeId, out aiagent))
		{
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
		}
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x0010B615 File Offset: 0x00109815
	private bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x0010B622 File Offset: 0x00109822
	private bool IsDriver()
	{
		return CustomMapsTerminal.IsDriver;
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x0010B629 File Offset: 0x00109829
	public void OnZoneInit()
	{
		if (CustomMapsGameManager.agentsToCreateOnZoneInit.IsNullOrEmpty<AIAgent>())
		{
			return;
		}
		this.CreatePlacedAgents(CustomMapsGameManager.agentsToCreateOnZoneInit);
		CustomMapsGameManager.agentsToCreateOnZoneInit.Clear();
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnZoneClear(ZoneClearReason reason)
	{
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x0010B64D File Offset: 0x0010984D
	public bool IsZoneReady()
	{
		return CustomMapLoader.CanLoadEntities && NetworkSystem.Instance.InRoom;
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x000023F5 File Offset: 0x000005F5
	private void SetupCollisions(GameObject go)
	{
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x000023F5 File Offset: 0x000005F5
	public void SerializeZoneData(BinaryWriter writer)
	{
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x000023F5 File Offset: 0x000005F5
	public void DeserializeZoneData(BinaryReader reader)
	{
	}

	// Token: 0x06003364 RID: 13156 RVA: 0x000023F5 File Offset: 0x000005F5
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x000023F5 File Offset: 0x000005F5
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x0010B662 File Offset: 0x00109862
	public static GameEntityManager GetEntityManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameEntityManager;
		}
		return null;
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x0010B67C File Offset: 0x0010987C
	public static GameAgentManager GetAgentManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06003368 RID: 13160 RVA: 0x0010B698 File Offset: 0x00109898
	public static CustomMapsAIBehaviourController GetBehaviorControllerForEntity(GameEntityId entityId)
	{
		GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
		if (entityManager.IsNull())
		{
			return null;
		}
		GameEntity gameEntity = entityManager.GetGameEntity(entityId);
		if (gameEntity.IsNull())
		{
			return null;
		}
		return gameEntity.gameObject.GetComponent<CustomMapsAIBehaviourController>();
	}

	// Token: 0x06003369 RID: 13161 RVA: 0x0010B6D2 File Offset: 0x001098D2
	public static void SetAgentsToCreate(List<AIAgent> agentsToCreate)
	{
		if (CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		if (agentsToCreate.IsNullOrEmpty<AIAgent>())
		{
			return;
		}
		CustomMapsGameManager.agentsToCreateOnZoneInit.Clear();
		CustomMapsGameManager.agentsToCreateOnZoneInit.AddRange(agentsToCreate);
	}

	// Token: 0x0400405C RID: 16476
	public GameEntityManager gameEntityManager;

	// Token: 0x0400405D RID: 16477
	public GameAgentManager gameAgentManager;

	// Token: 0x0400405E RID: 16478
	public static CustomMapsGameManager instance;

	// Token: 0x0400405F RID: 16479
	private const string AGENT_PREFAB_NAME = "CustomMapsAIAgent";

	// Token: 0x04004060 RID: 16480
	private Dictionary<int, AIAgent> customMapsAgents;

	// Token: 0x04004061 RID: 16481
	private static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(128);

	// Token: 0x04004062 RID: 16482
	private static List<AIAgent> agentsToCreateOnZoneInit = new List<AIAgent>(128);

	// Token: 0x04004063 RID: 16483
	private int TEST_index;

	// Token: 0x04004064 RID: 16484
	private int spawnCount;
}
