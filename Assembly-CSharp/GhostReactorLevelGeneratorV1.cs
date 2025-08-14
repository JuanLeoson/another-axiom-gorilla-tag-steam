using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class GhostReactorLevelGeneratorV1 : MonoBehaviour
{
	// Token: 0x0600246C RID: 9324 RVA: 0x000C2BB4 File Offset: 0x000C0DB4
	private void Awake()
	{
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
		this.levelGeneration.sectionsToSpawn = new List<int>();
		this.levelGeneration.sectionIndexPerAnchor = new List<int>();
		this.levelGeneration.anchorOrderIndices = new List<int>();
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000C2C07 File Offset: 0x000C0E07
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000023F5 File Offset: 0x000005F5
	public void GenerateRandomLevelConfiguration(int seed)
	{
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000023F5 File Offset: 0x000005F5
	public void SpawnSectionsBasedOnLevelGenerationConfig()
	{
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000C2C10 File Offset: 0x000C0E10
	public void SpawnEntitiesInEachSection()
	{
		for (int i = 0; i < this.alwaysPresentSections.Count; i++)
		{
			this.alwaysPresentSections[i].SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, null);
		}
		for (int j = 0; j < this.spawnedSections.Count; j++)
		{
			this.spawnedSections[j].SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, null);
		}
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000C2C9C File Offset: 0x000C0E9C
	public GRPatrolPath GetPatrolPath(int patrolPathId)
	{
		int num = patrolPathId / 100;
		int patrolPathIndex = patrolPathId % 100;
		if (num < this.alwaysPresentSections.Count)
		{
			if (num < 0 || num >= this.alwaysPresentSections.Count)
			{
				return null;
			}
			return this.alwaysPresentSections[num].GetPatrolPath(patrolPathIndex);
		}
		else
		{
			int num2 = num - this.alwaysPresentSections.Count;
			if (num2 < 0 || num2 >= this.spawnedSections.Count)
			{
				return null;
			}
			return this.spawnedSections[num2].GetPatrolPath(patrolPathIndex);
		}
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000C2D1C File Offset: 0x000C0F1C
	private void RandomizeIndices(List<int> list, int count)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		this.randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000C2D50 File Offset: 0x000C0F50
	private void SpawnSection(int sectionIndex, int anchorIndex, int connectorIndex, GhostReactorLevelSectionConnector.Direction connectorDirection)
	{
		Transform transform = this.sectionAnchors[anchorIndex];
		GhostReactorLevelSection component = Object.Instantiate<GameObject>(this.sections[sectionIndex], base.transform).GetComponent<GhostReactorLevelSection>();
		component.hubAnchorIndex = anchorIndex;
		if (component == null || component.Anchor == null)
		{
			Debug.LogError("Ghost Reactor Level Sections need to have the component GhostReactorLevelSection at the root. That component needs the Anchor to be set.");
			return;
		}
		GhostReactorLevelSectionConnector component2;
		switch (connectorDirection)
		{
		case GhostReactorLevelSectionConnector.Direction.Down:
			connectorIndex = Mathf.Clamp(connectorIndex % this.downwardConnectors.Count, 0, this.downwardConnectors.Count - 1);
			component2 = Object.Instantiate<GameObject>(this.downwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		default:
			connectorIndex = Mathf.Clamp(connectorIndex % this.forwardConnectors.Count, 0, this.forwardConnectors.Count - 1);
			component2 = Object.Instantiate<GameObject>(this.forwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		case GhostReactorLevelSectionConnector.Direction.Up:
			connectorIndex = Mathf.Clamp(connectorIndex % this.upwardConnectors.Count, 0, this.upwardConnectors.Count - 1);
			component2 = Object.Instantiate<GameObject>(this.upwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		}
		Quaternion rotation = Quaternion.Inverse(component2.hubAnchor.localRotation) * transform.rotation;
		Vector3 position = rotation * -component2.hubAnchor.localPosition + transform.position;
		component2.transform.position = position;
		component2.transform.rotation = rotation;
		component2.Init(this.reactor.grManager);
		Quaternion rotation2 = Quaternion.Inverse(component.Anchor.localRotation) * component2.sectionAnchor.rotation;
		Vector3 position2 = rotation2 * -component.Anchor.localPosition + component2.sectionAnchor.position;
		component.transform.position = position2;
		component.transform.rotation = rotation2;
		component.sectionConnector = component2;
		this.spawnedSections.Add(component);
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000C2F6C File Offset: 0x000C116C
	private void SpawnBlockade(int blockadeIndex, int anchorIndex)
	{
		Transform transform = this.sectionAnchors[anchorIndex];
		GhostReactorLevelSection component = Object.Instantiate<GameObject>(this.blockades[blockadeIndex % this.blockades.Count], base.transform).GetComponent<GhostReactorLevelSection>();
		if (component == null || component.Anchor == null)
		{
			Debug.LogError("Ghost Reactor Level Sections need to have the component GhostReactorLevelSection at the root. That component needs the Anchor to be set.");
			return;
		}
		Quaternion rotation = Quaternion.Inverse(component.Anchor.localRotation) * transform.rotation;
		Vector3 position = rotation * -component.Anchor.localPosition + transform.position;
		component.transform.position = position;
		component.transform.rotation = rotation;
		this.spawnedBlockades.Add(component);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000C3034 File Offset: 0x000C1234
	public void ClearLevelSections()
	{
		for (int i = 0; i < this.spawnedSections.Count; i++)
		{
			this.spawnedSections[i].DeInit();
			Object.Destroy(this.spawnedSections[i].gameObject);
		}
		this.spawnedSections.Clear();
		for (int j = 0; j < this.spawnedBlockades.Count; j++)
		{
			this.spawnedBlockades[j].DeInit();
			Object.Destroy(this.spawnedBlockades[j].gameObject);
		}
		this.spawnedBlockades.Clear();
		this.sectionsSpawned = false;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000C30D8 File Offset: 0x000C12D8
	public void DebugSpawnSection()
	{
		if (this.debugSectionToSpawn >= this.sections.Count || this.debugSpawnAnchor >= this.sectionAnchors.Count)
		{
			Debug.LogError("Invalid sectionToSpawn or spawnAnchor index");
			return;
		}
		this.SpawnSection(this.debugSectionToSpawn, this.debugSpawnAnchor, 0, GhostReactorLevelSectionConnector.Direction.Forward);
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000C312A File Offset: 0x000C132A
	public void DebugClearSpawnedSections()
	{
		this.ClearLevelSections();
	}

	// Token: 0x04002DFB RID: 11771
	[SerializeField]
	[Tooltip("Must be ordered by adjacency. So anchors next to each other spatially should be next to each other in this list")]
	private List<Transform> sectionAnchors = new List<Transform>();

	// Token: 0x04002DFC RID: 11772
	[SerializeField]
	private List<GameObject> sections = new List<GameObject>();

	// Token: 0x04002DFD RID: 11773
	[SerializeField]
	private List<GhostReactorLevelSection> alwaysPresentSections = new List<GhostReactorLevelSection>();

	// Token: 0x04002DFE RID: 11774
	[SerializeField]
	private List<GameObject> blockades = new List<GameObject>();

	// Token: 0x04002DFF RID: 11775
	[SerializeField]
	private List<GameObject> forwardConnectors = new List<GameObject>();

	// Token: 0x04002E00 RID: 11776
	[SerializeField]
	private List<GameObject> upwardConnectors = new List<GameObject>();

	// Token: 0x04002E01 RID: 11777
	[SerializeField]
	private List<GameObject> downwardConnectors = new List<GameObject>();

	// Token: 0x04002E02 RID: 11778
	[SerializeField]
	private bool removeInterpenetratingSections = true;

	// Token: 0x04002E03 RID: 11779
	[SerializeField]
	private int debugSectionToSpawn;

	// Token: 0x04002E04 RID: 11780
	[SerializeField]
	private int debugSpawnAnchor;

	// Token: 0x04002E05 RID: 11781
	[SerializeField]
	private bool debugSectionOverlaps;

	// Token: 0x04002E06 RID: 11782
	private List<GhostReactorLevelSection> spawnedSections = new List<GhostReactorLevelSection>();

	// Token: 0x04002E07 RID: 11783
	private List<GhostReactorLevelSection> spawnedBlockades = new List<GhostReactorLevelSection>();

	// Token: 0x04002E08 RID: 11784
	[SerializeField]
	private List<GhostReactorLevelGeneratorV1.LevelGenerationIteration> levelGenerationIterations = new List<GhostReactorLevelGeneratorV1.LevelGenerationIteration>();

	// Token: 0x04002E09 RID: 11785
	private SRand randomGenerator;

	// Token: 0x04002E0A RID: 11786
	private List<int> randomizedIndices = new List<int>();

	// Token: 0x04002E0B RID: 11787
	private bool sectionsSpawned;

	// Token: 0x04002E0C RID: 11788
	private GhostReactorLevelSectionConnector.Direction[] connectorDirLookup = new GhostReactorLevelSectionConnector.Direction[]
	{
		GhostReactorLevelSectionConnector.Direction.Forward,
		GhostReactorLevelSectionConnector.Direction.Up,
		GhostReactorLevelSectionConnector.Direction.Forward,
		GhostReactorLevelSectionConnector.Direction.Down
	};

	// Token: 0x04002E0D RID: 11789
	public GhostReactorLevelGeneratorV1.LevelGeneration levelGeneration;

	// Token: 0x04002E0E RID: 11790
	private GhostReactor reactor;

	// Token: 0x020005CF RID: 1487
	[Serializable]
	public struct LevelGenerationIteration
	{
		// Token: 0x04002E0F RID: 11791
		public int minSections;

		// Token: 0x04002E10 RID: 11792
		public int maxSections;
	}

	// Token: 0x020005D0 RID: 1488
	public struct LevelGeneration
	{
		// Token: 0x04002E11 RID: 11793
		public int seed;

		// Token: 0x04002E12 RID: 11794
		public int connectionDirectionOffset;

		// Token: 0x04002E13 RID: 11795
		public int connectionSelectionOffset;

		// Token: 0x04002E14 RID: 11796
		public List<int> sectionsToSpawn;

		// Token: 0x04002E15 RID: 11797
		public List<int> sectionIndexPerAnchor;

		// Token: 0x04002E16 RID: 11798
		public List<int> anchorOrderIndices;
	}
}
