using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class GhostReactorLevelSection : MonoBehaviour
{
	// Token: 0x17000387 RID: 903
	// (get) Token: 0x0600248B RID: 9355 RVA: 0x000C4AF7 File Offset: 0x000C2CF7
	public Transform Anchor
	{
		get
		{
			return this.anchorTransform;
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x0600248C RID: 9356 RVA: 0x000C4AFF File Offset: 0x000C2CFF
	public List<Transform> Anchors
	{
		get
		{
			return this.anchors;
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x0600248D RID: 9357 RVA: 0x000C4B07 File Offset: 0x000C2D07
	public GhostReactorLevelSection.SectionType Type
	{
		get
		{
			return this.sectionType;
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x0600248E RID: 9358 RVA: 0x000C4B0F File Offset: 0x000C2D0F
	public BoxCollider BoundingCollider
	{
		get
		{
			return this.boundingCollider;
		}
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000C4B18 File Offset: 0x000C2D18
	private void Awake()
	{
		this.spawnPointGroupLookup = new GhostReactorLevelSection.SpawnPointGroup[8];
		for (int i = 0; i < this.spawnPointGroups.Count; i++)
		{
			this.spawnPointGroups[i].SpawnPointIndexes = new List<int>();
			int type = (int)this.spawnPointGroups[i].type;
			if (type < this.spawnPointGroupLookup.Length)
			{
				this.spawnPointGroupLookup[type] = this.spawnPointGroups[i];
			}
		}
		this.hazardousMaterials = new List<GRHazardousMaterial>(32);
		base.GetComponentsInChildren<GRHazardousMaterial>(this.hazardousMaterials);
		for (int j = 0; j < this.patrolPaths.Count; j++)
		{
			if (this.patrolPaths[j] == null)
			{
				Debug.LogErrorFormat("Why does {0} have a null patrol path at index {1}", new object[]
				{
					base.gameObject.name,
					j
				});
			}
			else
			{
				this.patrolPaths[j].index = j;
			}
		}
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int k = 0; k < this.prePlacedGameEntities.Count; k++)
		{
			this.prePlacedGameEntities[k].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(this.renderers);
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000C4CA4 File Offset: 0x000C2EA4
	public void DeInit()
	{
		if (this.sectionConnector != null)
		{
			this.sectionConnector.DeInit();
			Object.Destroy(this.sectionConnector.gameObject);
		}
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000C4CD0 File Offset: 0x000C2ED0
	public static void RandomizeIndices(List<int> list, int count, ref SRand randomGenerator)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000C4D00 File Offset: 0x000C2F00
	public void InitLevelSection(int sectionIndex, GhostReactor reactor)
	{
		this.index = sectionIndex;
		for (int i = 0; i < this.hazardousMaterials.Count; i++)
		{
			this.hazardousMaterials[i].Init(reactor);
		}
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000C4D3C File Offset: 0x000C2F3C
	public void SpawnSectionEntities(ref SRand randomGenerator, GameEntityManager gameEntityManager, List<GhostReactorSpawnConfig> spawnConfigs)
	{
		if (spawnConfigs == null)
		{
			spawnConfigs = this.spawnConfigs;
		}
		if (spawnConfigs != null && spawnConfigs.Count > 0)
		{
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
			GhostReactorSpawnConfig ghostReactorSpawnConfig = spawnConfigs[randomGenerator.NextInt(spawnConfigs.Count)];
			Debug.LogFormat("Spawn Ghost Reactor Level Section {0} {1}", new object[]
			{
				base.gameObject.name,
				ghostReactorSpawnConfig.name
			});
			for (int i = 0; i < this.spawnPointGroups.Count; i++)
			{
				this.spawnPointGroups[i].CurrentIndex = 0;
				this.spawnPointGroups[i].NeedsRandomization = true;
			}
			for (int j = 0; j < ghostReactorSpawnConfig.entitySpawnGroups.Count; j++)
			{
				int num = ghostReactorSpawnConfig.entitySpawnGroups[j].spawnCount;
				if (num > 0)
				{
					int spawnPointType = (int)ghostReactorSpawnConfig.entitySpawnGroups[j].spawnPointType;
					if (spawnPointType < this.spawnPointGroupLookup.Length)
					{
						GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[spawnPointType];
						if (spawnPointGroup != null)
						{
							if (spawnPointGroup.NeedsRandomization)
							{
								spawnPointGroup.NeedsRandomization = false;
								GhostReactorLevelSection.RandomizeIndices(spawnPointGroup.SpawnPointIndexes, spawnPointGroup.spawnPoints.Count, ref randomGenerator);
							}
							num = Mathf.Min(num, spawnPointGroup.spawnPoints.Count);
							for (int k = 0; k < num; k++)
							{
								int currentIndex = spawnPointGroup.CurrentIndex;
								GREntitySpawnPoint nextSpawnPoint = spawnPointGroup.GetNextSpawnPoint();
								nextSpawnPoint == null;
								GameEntity entity = ghostReactorSpawnConfig.entitySpawnGroups[j].entity;
								if (ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity != null)
								{
									ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity.TryForRandomItem(ref randomGenerator, out entity);
								}
								if (!(entity == null))
								{
									int staticHash = entity.name.GetStaticHash();
									long createData = -1L;
									if (nextSpawnPoint.applyScale)
									{
										createData = BitPackUtils.PackWorldPosForNetwork(nextSpawnPoint.transform.localScale);
									}
									else if (nextSpawnPoint.patrolPath != null)
									{
										createData = (long)(this.index * 100 + nextSpawnPoint.patrolPath.index);
									}
									GameEntityCreateData item = new GameEntityCreateData
									{
										entityTypeId = staticHash,
										localPosition = nextSpawnPoint.transform.position,
										localRotation = nextSpawnPoint.transform.rotation,
										createData = createData
									};
									GhostReactorLevelSection.tempCreateEntitiesList.Add(item);
								}
							}
						}
					}
				}
			}
			for (int l = 0; l < this.prePlacedGameEntities.Count; l++)
			{
				int staticHash2 = this.prePlacedGameEntities[l].gameObject.name.GetStaticHash();
				if (!gameEntityManager.FactoryHasEntity(staticHash2))
				{
					Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
					{
						this.prePlacedGameEntities[l].gameObject.name,
						staticHash2
					});
				}
				else
				{
					GameEntityCreateData item2 = new GameEntityCreateData
					{
						entityTypeId = staticHash2,
						localPosition = this.prePlacedGameEntities[l].transform.position,
						localRotation = this.prePlacedGameEntities[l].transform.rotation,
						createData = 0L
					};
					GhostReactorLevelSection.tempCreateEntitiesList.Add(item2);
				}
			}
			gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000C50AE File Offset: 0x000C32AE
	public GRPatrolPath GetPatrolPath(int patrolPathIndex)
	{
		if (patrolPathIndex >= 0 && patrolPathIndex < this.patrolPaths.Count)
		{
			return this.patrolPaths[patrolPathIndex];
		}
		return null;
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000C50D0 File Offset: 0x000C32D0
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000C511C File Offset: 0x000C331C
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float distSq = this.GetDistSq(playerPos);
		float num = 324f;
		float num2 = 484f;
		if (this.hidden && distSq < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && distSq > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000C5184 File Offset: 0x000C3384
	public float GetDistSq(Vector3 pos)
	{
		return (this.boundingCollider.ClosestPoint(pos) - pos).sqrMagnitude;
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000C51AB File Offset: 0x000C33AB
	public Transform GetAnchor(int anchorIndex)
	{
		return this.anchors[anchorIndex];
	}

	// Token: 0x04002E4F RID: 11855
	private const float SHOW_DIST = 18f;

	// Token: 0x04002E50 RID: 11856
	private const float HIDE_DIST = 22f;

	// Token: 0x04002E51 RID: 11857
	[SerializeField]
	private GhostReactorLevelSection.SectionType sectionType;

	// Token: 0x04002E52 RID: 11858
	[SerializeField]
	[Tooltip("Single Anchor Transform used for End Caps and Blockers")]
	private Transform anchorTransform;

	// Token: 0x04002E53 RID: 11859
	[SerializeField]
	[Tooltip("A List of Anchors used as in and out connections for Hubs")]
	private List<Transform> anchors = new List<Transform>();

	// Token: 0x04002E54 RID: 11860
	[SerializeField]
	private List<GhostReactorLevelSection.SpawnPointGroup> spawnPointGroups;

	// Token: 0x04002E55 RID: 11861
	[SerializeField]
	private List<GhostReactorSpawnConfig> spawnConfigs;

	// Token: 0x04002E56 RID: 11862
	[SerializeField]
	private List<GRPatrolPath> patrolPaths;

	// Token: 0x04002E57 RID: 11863
	[SerializeField]
	private BoxCollider boundingCollider;

	// Token: 0x04002E58 RID: 11864
	private List<Renderer> renderers;

	// Token: 0x04002E59 RID: 11865
	private bool hidden;

	// Token: 0x04002E5A RID: 11866
	private List<GRHazardousMaterial> hazardousMaterials;

	// Token: 0x04002E5B RID: 11867
	[HideInInspector]
	public GhostReactorLevelSectionConnector sectionConnector;

	// Token: 0x04002E5C RID: 11868
	[HideInInspector]
	public int hubAnchorIndex;

	// Token: 0x04002E5D RID: 11869
	private int index;

	// Token: 0x04002E5E RID: 11870
	private GhostReactorLevelSection.SpawnPointGroup[] spawnPointGroupLookup;

	// Token: 0x04002E5F RID: 11871
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x04002E60 RID: 11872
	public static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(1024);

	// Token: 0x020005D7 RID: 1495
	public enum SectionType
	{
		// Token: 0x04002E62 RID: 11874
		Hub,
		// Token: 0x04002E63 RID: 11875
		EndCap,
		// Token: 0x04002E64 RID: 11876
		Blocker
	}

	// Token: 0x020005D8 RID: 1496
	[Serializable]
	public class SpawnPointGroup
	{
		// Token: 0x1700038B RID: 907
		// (get) Token: 0x0600249B RID: 9371 RVA: 0x000C51DD File Offset: 0x000C33DD
		// (set) Token: 0x0600249C RID: 9372 RVA: 0x000C51E5 File Offset: 0x000C33E5
		public bool NeedsRandomization
		{
			get
			{
				return this.needsRandomization;
			}
			set
			{
				this.needsRandomization = value;
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x0600249D RID: 9373 RVA: 0x000C51EE File Offset: 0x000C33EE
		// (set) Token: 0x0600249E RID: 9374 RVA: 0x000C51F6 File Offset: 0x000C33F6
		public int CurrentIndex
		{
			get
			{
				return this.currentIndex;
			}
			set
			{
				this.currentIndex = value;
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x0600249F RID: 9375 RVA: 0x000C51FF File Offset: 0x000C33FF
		// (set) Token: 0x060024A0 RID: 9376 RVA: 0x000C5207 File Offset: 0x000C3407
		public List<int> SpawnPointIndexes
		{
			get
			{
				return this.spawnPointIndexes;
			}
			set
			{
				this.spawnPointIndexes = value;
			}
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x000C5210 File Offset: 0x000C3410
		public GREntitySpawnPoint GetNextSpawnPoint()
		{
			GREntitySpawnPoint result = this.spawnPoints[this.spawnPointIndexes[this.currentIndex]];
			this.currentIndex = (this.currentIndex + 1) % this.spawnPointIndexes.Count;
			return result;
		}

		// Token: 0x04002E65 RID: 11877
		public GhostReactorSpawnConfig.SpawnPointType type;

		// Token: 0x04002E66 RID: 11878
		public List<GREntitySpawnPoint> spawnPoints;

		// Token: 0x04002E67 RID: 11879
		private List<int> spawnPointIndexes;

		// Token: 0x04002E68 RID: 11880
		private bool needsRandomization;

		// Token: 0x04002E69 RID: 11881
		private int currentIndex;
	}
}
