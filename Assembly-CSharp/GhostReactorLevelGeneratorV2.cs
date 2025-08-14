using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class GhostReactorLevelGeneratorV2 : MonoBehaviour
{
	// Token: 0x17000386 RID: 902
	// (get) Token: 0x06002479 RID: 9337 RVA: 0x000C31DC File Offset: 0x000C13DC
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> TreeLevels
	{
		get
		{
			if (this.depthConfigs == null || this.depthConfigs.Count <= 0)
			{
				return null;
			}
			return this.depthConfigs[Mathf.Clamp(this.reactor.GetDepthLevel(), 0, this.depthConfigs.Count - 1)].configGenOptions[0].treeLevels;
		}
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000C323C File Offset: 0x000C143C
	private void Awake()
	{
		GameObject gameObject = new GameObject("TestColliderA");
		this.testColliderA = gameObject.AddComponent<BoxCollider>();
		this.testColliderA.isTrigger = true;
		gameObject.transform.SetParent(base.transform);
		gameObject.gameObject.SetActive(false);
		GameObject gameObject2 = new GameObject("TestColliderB");
		this.testColliderB = gameObject2.AddComponent<BoxCollider>();
		this.testColliderB.isTrigger = true;
		gameObject2.transform.SetParent(base.transform);
		gameObject2.gameObject.SetActive(false);
		this.nextVisCheckNodeIndex = 0;
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000C32D0 File Offset: 0x000C14D0
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000C32DC File Offset: 0x000C14DC
	private void Update()
	{
		Vector3 position = VRRig.LocalRig.transform.position;
		int num = Mathf.Min(1, this.nodeList.Count);
		for (int i = 0; i < num; i++)
		{
			if (this.nextVisCheckNodeIndex >= this.nodeList.Count)
			{
				this.nextVisCheckNodeIndex = 0;
			}
			if (this.nodeList[this.nextVisCheckNodeIndex] != null)
			{
				if (this.nodeList[this.nextVisCheckNodeIndex].sectionInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].sectionInstance.UpdateDisable(position);
				}
				if (this.nodeList[this.nextVisCheckNodeIndex].connectorInstance != null)
				{
					this.nodeList[this.nextVisCheckNodeIndex].connectorInstance.UpdateDisable(position);
				}
				GhostReactorLevelGeneratorV2.Node[] children = this.nodeList[this.nextVisCheckNodeIndex].children;
				for (int j = 0; j < children.Length; j++)
				{
					if (children[j] != null)
					{
						if (children[j].sectionInstance != null)
						{
							children[j].sectionInstance.UpdateDisable(position);
						}
						if (children[j].connectorInstance != null)
						{
							children[j].connectorInstance.UpdateDisable(position);
						}
					}
				}
				this.nextVisCheckNodeIndex++;
			}
		}
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000C3440 File Offset: 0x000C1640
	private bool TestForCollision(GhostReactorLevelSection section, Vector3 position, Quaternion rotation, int selfi, int selfj, int selfk)
	{
		this.testColliderA.gameObject.SetActive(true);
		this.testColliderB.gameObject.SetActive(true);
		this.testColliderA.transform.position = position + rotation * section.BoundingCollider.transform.localPosition;
		this.testColliderA.transform.rotation = rotation * section.BoundingCollider.transform.localRotation;
		this.testColliderA.transform.localScale = section.BoundingCollider.transform.localScale;
		this.testColliderA.size = section.BoundingCollider.size;
		this.testColliderA.center = section.BoundingCollider.center;
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			for (int j = 0; j < this.nodeTree[i].Count; j++)
			{
				if (i != selfi || j != selfj || selfk != -1)
				{
					GhostReactorLevelGeneratorV2.Node node = this.nodeTree[i][j];
					for (int k = 0; k < node.children.Length; k++)
					{
						if (i != selfi || j != selfj || k != selfk)
						{
							GhostReactorLevelGeneratorV2.Node node2 = node.children[k];
							if (node2 != null && node2.sectionInstance != null && node2.sectionInstance.BoundingCollider != null && (node2.type == GhostReactorLevelGeneratorV2.NodeType.Blocker || node2.type == GhostReactorLevelGeneratorV2.NodeType.EndCap))
							{
								GhostReactorLevelSection sectionInstance = node2.sectionInstance;
								this.testColliderB.transform.position = sectionInstance.transform.position + sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localPosition;
								this.testColliderB.transform.rotation = sectionInstance.transform.rotation * sectionInstance.BoundingCollider.transform.localRotation;
								this.testColliderB.transform.localScale = sectionInstance.BoundingCollider.transform.localScale;
								this.testColliderB.size = sectionInstance.BoundingCollider.size;
								this.testColliderB.center = sectionInstance.BoundingCollider.center;
								Vector3 vector;
								float num;
								if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector, out num))
								{
									this.testColliderA.gameObject.SetActive(false);
									this.testColliderB.gameObject.SetActive(false);
									return true;
								}
							}
						}
					}
					if ((i != selfi || j != selfj) && node.sectionInstance != null && node.sectionInstance.BoundingCollider != null)
					{
						GhostReactorLevelSection sectionInstance2 = node.sectionInstance;
						this.testColliderB.transform.position = sectionInstance2.transform.position + sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localPosition;
						this.testColliderB.transform.rotation = sectionInstance2.transform.rotation * sectionInstance2.BoundingCollider.transform.localRotation;
						this.testColliderB.transform.localScale = sectionInstance2.BoundingCollider.transform.localScale;
						this.testColliderB.size = sectionInstance2.BoundingCollider.size;
						this.testColliderB.center = sectionInstance2.BoundingCollider.center;
						Vector3 vector2;
						float num2;
						if (this.testColliderA.bounds.Intersects(this.testColliderB.bounds) && Physics.ComputePenetration(this.testColliderA, this.testColliderA.transform.position, this.testColliderA.transform.rotation, this.testColliderB, this.testColliderB.transform.position, this.testColliderB.transform.rotation, out vector2, out num2))
						{
							this.testColliderA.gameObject.SetActive(false);
							this.testColliderB.gameObject.SetActive(false);
							return true;
						}
					}
				}
			}
		}
		this.testColliderA.gameObject.SetActive(false);
		this.testColliderB.gameObject.SetActive(false);
		return false;
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000C3914 File Offset: 0x000C1B14
	private void DebugGenerate()
	{
		this.Generate(this.seed);
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x000C3924 File Offset: 0x000C1B24
	public void Generate(int inputSeed)
	{
		this.ClearLevelSections();
		if (!Application.isPlaying)
		{
			return;
		}
		this.seed = inputSeed;
		this.randomGenerator = new SRand(this.seed);
		if (this.TreeLevels.Count < 1)
		{
			return;
		}
		this.hubCaches.Clear();
		this.spawnedHubHashSet.Clear();
		for (int i = 0; i < this.TreeLevels.Count; i++)
		{
			this.nodeTree.Add(new List<GhostReactorLevelGeneratorV2.Node>());
			this.hubCaches.Add(new List<GhostReactorLevelGeneratorV2.Hub>());
			GameObject gameObject = new GameObject(string.Format("Tree Level {0}", i));
			gameObject.transform.parent = base.transform;
			gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			this.treeParents.Add(gameObject.transform);
		}
		GhostReactorLevelGeneratorV2.Hub hub = new GhostReactorLevelGeneratorV2.Hub();
		hub.configIndex = -1;
		hub.anchorCount = this.mainHub.Anchors.Count;
		hub.anchorOrder = new List<int>();
		hub.needsOrderShuffle = true;
		hub.needsInstantiation = false;
		GhostReactorLevelGeneratorV2.Node node = new GhostReactorLevelGeneratorV2.Node();
		node.type = GhostReactorLevelGeneratorV2.NodeType.Hub;
		node.configIndex = -1;
		node.parentNodeIndex = -1;
		node.attachAnchorIndex = -1;
		node.parentAnchorIndex = -1;
		node.connectorIndex = -1;
		node.children = new GhostReactorLevelGeneratorV2.Node[hub.anchorCount];
		node.sectionInstance = this.mainHub;
		node.position = this.mainHub.transform.position;
		node.rotation = this.mainHub.transform.rotation;
		this.nodeTree[0].Add(node);
		this.nodeList.Add(node);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < this.TreeLevels.Count; j++)
		{
			List<GhostReactorLevelSection> list = this.TreeLevels[j].hubs;
			List<GhostReactorLevelSection> list2 = this.TreeLevels[j].endCaps;
			List<GhostReactorLevelSection> list3 = this.TreeLevels[j].blockers;
			List<GhostReactorLevelSectionConnector> list4 = this.TreeLevels[j].connectors;
			for (int k = 0; k < list.Count; k++)
			{
				GhostReactorLevelGeneratorV2.Hub hub2 = new GhostReactorLevelGeneratorV2.Hub();
				hub2.configIndex = k;
				hub2.anchorCount = list[hub2.configIndex].Anchors.Count;
				hub2.anchorOrder = new List<int>();
				hub2.needsOrderShuffle = true;
				hub2.needsInstantiation = true;
				this.hubCaches[j].Add(hub2);
			}
			this.RandomizeIndices(ref this.hubOrder, list.Count);
			this.RandomizeIndices(ref this.connectorOrder, list4.Count);
			this.RandomizeIndices(ref this.blockerOrder, list3.Count);
			this.RandomizeIndices(ref this.capOrder, list2.Count);
			int num5 = Mathf.Max(this.TreeLevels[j].maxHubs - this.TreeLevels[j].minHubs, 0);
			int num6 = Mathf.Max(this.TreeLevels[j].minHubs, 0) + this.randomGenerator.NextInt(num5 + 1);
			for (int l = 0; l < num6; l++)
			{
				int num7 = this.hubOrder[num % this.hubOrder.Count];
				num++;
				int num8 = this.connectorOrder[num2 % this.connectorOrder.Count];
				num2++;
				int num9 = (j == 0) ? -1 : (l % this.nodeTree[j].Count);
				GhostReactorLevelGeneratorV2.Node node2 = (num9 != -1) ? this.nodeTree[j][num9] : node;
				GhostReactorLevelGeneratorV2.Hub hub3 = (node2.configIndex != -1 && j > 0) ? this.hubCaches[j - 1][node2.configIndex] : hub;
				if (hub3.needsOrderShuffle)
				{
					this.RandomizeIndices(ref hub3.anchorOrder, hub3.anchorCount);
					hub3.needsOrderShuffle = false;
				}
				for (int m = 0; m < hub3.anchorOrder.Count; m++)
				{
					int num10 = hub3.anchorOrder[m];
					bool flag = this.spawnedHubHashSet.Contains(list[num7].gameObject.name);
					if (node2.children[num10] == null && node2.attachAnchorIndex != num10 && !flag)
					{
						Quaternion rhs = node2.sectionInstance.Anchors[num10].rotation * this.flip180;
						Vector3 position = node2.sectionInstance.Anchors[num10].position;
						GhostReactorLevelSectionConnector ghostReactorLevelSectionConnector = list4[num8];
						Quaternion quaternion = Quaternion.Inverse(ghostReactorLevelSectionConnector.hubAnchor.localRotation) * rhs;
						Vector3 vector = quaternion * -ghostReactorLevelSectionConnector.hubAnchor.localPosition + position;
						Vector3 b = quaternion * ghostReactorLevelSectionConnector.sectionAnchor.localPosition + vector;
						Quaternion rhs2 = quaternion * ghostReactorLevelSectionConnector.sectionAnchor.localRotation;
						GhostReactorLevelSection ghostReactorLevelSection = list[num7];
						bool flag2 = false;
						if (ghostReactorLevelSection.Anchors.Count > 0)
						{
							this.RandomizeIndices(ref this.entryAnchorOrder, ghostReactorLevelSection.Anchors.Count);
							for (int n = 0; n < this.entryAnchorOrder.Count; n++)
							{
								int num11 = this.entryAnchorOrder[n];
								Transform transform = ghostReactorLevelSection.Anchors[num11];
								Quaternion rotation = Quaternion.Inverse(transform.localRotation) * rhs2;
								Vector3 position2 = rotation * -transform.localPosition + b;
								if (!this.TestForCollision(ghostReactorLevelSection, position2, rotation, j, l, num10))
								{
									GhostReactorLevelGeneratorV2.Node node3 = new GhostReactorLevelGeneratorV2.Node();
									node3.type = GhostReactorLevelGeneratorV2.NodeType.Hub;
									node3.configIndex = num7;
									node3.connectorIndex = num8;
									node3.parentNodeIndex = num9;
									node3.children = new GhostReactorLevelGeneratorV2.Node[list[num7].Anchors.Count];
									node3.parentAnchorIndex = num10;
									node3.attachAnchorIndex = num11;
									GhostReactorLevelSectionConnector component = Object.Instantiate<GameObject>(ghostReactorLevelSectionConnector.gameObject, this.treeParents[j]).GetComponent<GhostReactorLevelSectionConnector>();
									component.transform.SetPositionAndRotation(vector, quaternion);
									node3.connectorInstance = component;
									GhostReactorLevelSection component2 = Object.Instantiate<GameObject>(ghostReactorLevelSection.gameObject, this.treeParents[j]).GetComponent<GhostReactorLevelSection>();
									component2.transform.SetPositionAndRotation(position2, rotation);
									node3.sectionInstance = component2;
									node2.children[node3.parentAnchorIndex] = node3;
									this.nodeTree[j + 1].Add(node3);
									this.nodeList.Add(node3);
									this.spawnedHubHashSet.Add(ghostReactorLevelSection.gameObject.name);
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							break;
						}
					}
				}
			}
		}
		for (int num12 = 0; num12 < this.nodeTree.Count; num12++)
		{
			List<GhostReactorLevelSection> list5 = this.TreeLevels[num12].endCaps;
			List<GhostReactorLevelSection> list6 = this.TreeLevels[num12].blockers;
			for (int num13 = 0; num13 < this.nodeTree[num12].Count; num13++)
			{
				GhostReactorLevelGeneratorV2.Node node4 = this.nodeTree[num12][num13];
				int num14 = Mathf.Max(this.TreeLevels[num12].maxCaps - this.TreeLevels[num12].minCaps, 0);
				int num15 = Mathf.Max(this.TreeLevels[num12].minCaps, 0) + this.randomGenerator.NextInt(num14 + 1);
				for (int num16 = 0; num16 < node4.children.Length; num16++)
				{
					if (node4.children[num16] == null && node4.attachAnchorIndex != num16)
					{
						bool flag3 = false;
						if (num15 > 0)
						{
							int num17 = this.capOrder[num4 % this.capOrder.Count];
							num4++;
							num15--;
							Quaternion rhs3 = node4.sectionInstance.Anchors[num16].rotation * this.flip180;
							Vector3 position3 = node4.sectionInstance.Anchors[num16].position;
							GhostReactorLevelSection ghostReactorLevelSection2 = list5[num17];
							Quaternion rotation2 = Quaternion.Inverse(ghostReactorLevelSection2.Anchor.localRotation) * rhs3;
							Vector3 position4 = rotation2 * -ghostReactorLevelSection2.Anchor.localPosition + position3;
							if (!this.TestForCollision(ghostReactorLevelSection2, position4, rotation2, num12, num13, num16))
							{
								GhostReactorLevelGeneratorV2.Node node5 = new GhostReactorLevelGeneratorV2.Node();
								node5.type = GhostReactorLevelGeneratorV2.NodeType.EndCap;
								node5.configIndex = num17;
								node5.parentNodeIndex = num13;
								node5.connectorIndex = -1;
								node5.parentAnchorIndex = num16;
								GhostReactorLevelSection component3 = Object.Instantiate<GameObject>(ghostReactorLevelSection2.gameObject, this.treeParents[num12]).GetComponent<GhostReactorLevelSection>();
								component3.transform.SetPositionAndRotation(position4, rotation2);
								node5.sectionInstance = component3;
								node4.children[num16] = node5;
								flag3 = true;
							}
						}
						if (!flag3)
						{
							int configIndex = this.blockerOrder[num3 % this.blockerOrder.Count];
							num3++;
							GhostReactorLevelGeneratorV2.Node node6 = new GhostReactorLevelGeneratorV2.Node();
							node6.type = GhostReactorLevelGeneratorV2.NodeType.Blocker;
							node6.configIndex = configIndex;
							node6.parentNodeIndex = num13;
							node6.connectorIndex = -1;
							node6.parentAnchorIndex = num16;
							Quaternion rhs4 = node4.sectionInstance.Anchors[num16].rotation * this.flip180;
							Vector3 position5 = node4.sectionInstance.Anchors[num16].position;
							GhostReactorLevelSection component4 = Object.Instantiate<GameObject>(list6[node6.configIndex].gameObject, this.treeParents[num12]).GetComponent<GhostReactorLevelSection>();
							Quaternion rotation3 = Quaternion.Inverse(component4.Anchor.localRotation) * rhs4;
							Vector3 position6 = rotation3 * -component4.Anchor.localPosition + position5;
							component4.transform.SetPositionAndRotation(position6, rotation3);
							node6.sectionInstance = component4;
							node4.children[num16] = node6;
						}
					}
				}
			}
		}
		for (int num18 = 0; num18 < this.nodeList.Count; num18++)
		{
			if (this.nodeList[num18].connectorInstance != null)
			{
				this.nodeList[num18].connectorInstance.Init(this.reactor.grManager);
			}
			this.nodeList[num18].sectionInstance.InitLevelSection(num18, this.reactor);
		}
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x000C4453 File Offset: 0x000C2653
	private void DebugClear()
	{
		this.ClearLevelSections();
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000C445C File Offset: 0x000C265C
	public void ClearLevelSections()
	{
		for (int i = 0; i < this.nodeList.Count; i++)
		{
			if (!(this.nodeList[i].sectionInstance == this.mainHub))
			{
				if (this.nodeList[i].connectorInstance != null)
				{
					this.nodeList[i].connectorInstance.DeInit();
					Object.Destroy(this.nodeList[i].connectorInstance.gameObject);
				}
				this.nodeList[i].sectionInstance.DeInit();
				Object.Destroy(this.nodeList[i].sectionInstance.gameObject);
			}
		}
		this.nodeList.Clear();
		for (int j = 0; j < this.nodeTree.Count; j++)
		{
			this.nodeTree[j].Clear();
		}
		this.nodeTree.Clear();
		for (int k = 0; k < this.treeParents.Count; k++)
		{
			Object.Destroy(this.treeParents[k].gameObject);
		}
		this.treeParents.Clear();
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000C4594 File Offset: 0x000C2794
	private void OnValidate()
	{
		if (this.depthConfigs.Count <= 0)
		{
			Debug.LogError("GhostReactorLevelGeneratorV2 has no depthConfigs");
		}
		for (int i = 0; i < this.depthConfigs.Count; i++)
		{
			List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = this.depthConfigs[i].configGenOptions[0].treeLevels;
			for (int j = 0; j < treeLevels.Count; j++)
			{
				GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig = treeLevels[j];
				treeLevelConfig.minHubs = Mathf.Abs(treeLevelConfig.minHubs);
				treeLevelConfig.maxHubs = Mathf.Abs(treeLevelConfig.maxHubs);
				treeLevelConfig.minCaps = Mathf.Abs(treeLevelConfig.minCaps);
				treeLevelConfig.maxCaps = Mathf.Abs(treeLevelConfig.maxCaps);
				if (treeLevelConfig.minHubs > treeLevelConfig.maxHubs)
				{
					treeLevelConfig.maxHubs = treeLevelConfig.minHubs;
				}
				if (treeLevelConfig.minCaps > treeLevelConfig.maxCaps)
				{
					treeLevelConfig.maxCaps = treeLevelConfig.minCaps;
				}
				treeLevels[j] = treeLevelConfig;
			}
			GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig2 = treeLevels[treeLevels.Count - 1];
			if (treeLevelConfig2.minHubs > 0 || treeLevelConfig2.maxHubs > 0)
			{
				Debug.LogError("Ghost Reactor Level Gen Setup Error: The last tree level can only spawn end caps around the furthest level of hubs. Otherwise it would spawn hubs without a further level to spawn end caps around them");
				treeLevelConfig2.minHubs = 0;
				treeLevelConfig2.maxHubs = 0;
				treeLevels[treeLevels.Count - 1] = treeLevelConfig2;
			}
		}
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000C46F0 File Offset: 0x000C28F0
	public void SpawnEntitiesInEachSection()
	{
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			int num = i - 1;
			List<GhostReactorSpawnConfig> sectionSpawnConfigs;
			if (num >= 0)
			{
				sectionSpawnConfigs = this.TreeLevels[num].sectionSpawnConfigs;
			}
			else
			{
				sectionSpawnConfigs = this.mainHubSpawnConfigs;
			}
			for (int j = 0; j < this.nodeTree[i].Count; j++)
			{
				GhostReactorLevelGeneratorV2.Node node = this.nodeTree[i][j];
				if (node != null && node.sectionInstance != null && node.type == GhostReactorLevelGeneratorV2.NodeType.Hub)
				{
					node.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, sectionSpawnConfigs);
				}
				for (int k = 0; k < node.children.Length; k++)
				{
					GhostReactorLevelGeneratorV2.Node node2 = node.children[k];
					if (node2 != null && node2.sectionInstance != null && node2.type == GhostReactorLevelGeneratorV2.NodeType.EndCap)
					{
						node2.sectionInstance.SpawnSectionEntities(ref this.randomGenerator, this.reactor.grManager.gameEntityManager, null);
					}
				}
			}
		}
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x000C4818 File Offset: 0x000C2A18
	public GRPatrolPath GetPatrolPath(int patrolPathId)
	{
		int num = patrolPathId / 100;
		int patrolPathIndex = patrolPathId % 100;
		if (num < 0 || num >= this.nodeList.Count)
		{
			return null;
		}
		return this.nodeList[num].sectionInstance.GetPatrolPath(patrolPathIndex);
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x000C485C File Offset: 0x000C2A5C
	private void RandomizeIndices(ref List<int> list, int count)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		this.randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x000C4894 File Offset: 0x000C2A94
	public bool GetExitFromCurrentSection(Vector3 pos, out Vector3 exitPos, out Quaternion exitRot, List<Vector3> connectorCorners)
	{
		exitPos = Vector3.zero;
		exitRot = Quaternion.identity;
		GhostReactorLevelGeneratorV2.Node currentNode = this.GetCurrentNode(pos);
		if (currentNode == null || currentNode.parentAnchorIndex < 0)
		{
			return false;
		}
		Transform anchor = currentNode.sectionInstance.GetAnchor(currentNode.attachAnchorIndex);
		exitPos = anchor.transform.position;
		exitRot = anchor.transform.rotation;
		GRLevelAnchor component = anchor.GetComponent<GRLevelAnchor>();
		if (component != null && component.navigablePoint != null)
		{
			exitPos = component.navigablePoint.position;
			exitRot = component.navigablePoint.rotation;
		}
		connectorCorners.Clear();
		if (currentNode.connectorInstance != null)
		{
			for (int i = 0; i < currentNode.connectorInstance.pathNodes.Count; i++)
			{
				connectorCorners.Add(currentNode.connectorInstance.pathNodes[i].position);
			}
		}
		return true;
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x000C4990 File Offset: 0x000C2B90
	private GhostReactorLevelGeneratorV2.Node GetCurrentNode(Vector3 pos)
	{
		float num = float.MaxValue;
		GhostReactorLevelGeneratorV2.Node result = null;
		for (int i = 0; i < this.nodeTree.Count; i++)
		{
			List<GhostReactorLevelGeneratorV2.Node> list = this.nodeTree[i];
			for (int j = 0; j < list.Count; j++)
			{
				GhostReactorLevelGeneratorV2.Node node = list[j];
				if (!(node.sectionInstance == null))
				{
					float distSq = node.sectionInstance.GetDistSq(pos);
					if (distSq < num)
					{
						num = distSq;
						result = node;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x04002E17 RID: 11799
	public List<GhostReactorLevelDepthConfig> depthConfigs;

	// Token: 0x04002E18 RID: 11800
	[SerializeField]
	private GhostReactorLevelGenConfig levelGenConfig;

	// Token: 0x04002E19 RID: 11801
	[SerializeField]
	private GhostReactorLevelSection mainHub = new GhostReactorLevelSection();

	// Token: 0x04002E1A RID: 11802
	[SerializeField]
	private List<GhostReactorSpawnConfig> mainHubSpawnConfigs;

	// Token: 0x04002E1B RID: 11803
	[SerializeField]
	private List<GhostReactorLevelSection> hubs = new List<GhostReactorLevelSection>();

	// Token: 0x04002E1C RID: 11804
	[SerializeField]
	private List<GhostReactorLevelSection> endCaps = new List<GhostReactorLevelSection>();

	// Token: 0x04002E1D RID: 11805
	[SerializeField]
	private List<GhostReactorLevelSection> blockers = new List<GhostReactorLevelSection>();

	// Token: 0x04002E1E RID: 11806
	[SerializeField]
	private List<GhostReactorLevelSectionConnector> connectors = new List<GhostReactorLevelSectionConnector>();

	// Token: 0x04002E1F RID: 11807
	public int seed = 2343;

	// Token: 0x04002E20 RID: 11808
	private List<List<GhostReactorLevelGeneratorV2.Hub>> hubCaches = new List<List<GhostReactorLevelGeneratorV2.Hub>>();

	// Token: 0x04002E21 RID: 11809
	private List<List<GhostReactorLevelGeneratorV2.Node>> nodeTree = new List<List<GhostReactorLevelGeneratorV2.Node>>();

	// Token: 0x04002E22 RID: 11810
	private List<GhostReactorLevelGeneratorV2.Node> nodeList = new List<GhostReactorLevelGeneratorV2.Node>();

	// Token: 0x04002E23 RID: 11811
	private HashSet<string> spawnedHubHashSet = new HashSet<string>();

	// Token: 0x04002E24 RID: 11812
	private List<int> hubOrder = new List<int>();

	// Token: 0x04002E25 RID: 11813
	private List<int> connectorOrder = new List<int>();

	// Token: 0x04002E26 RID: 11814
	private List<int> capOrder = new List<int>();

	// Token: 0x04002E27 RID: 11815
	private List<int> blockerOrder = new List<int>();

	// Token: 0x04002E28 RID: 11816
	private List<int> entryAnchorOrder = new List<int>();

	// Token: 0x04002E29 RID: 11817
	private List<Transform> treeParents = new List<Transform>();

	// Token: 0x04002E2A RID: 11818
	private string generationOutput = "";

	// Token: 0x04002E2B RID: 11819
	private SRand randomGenerator;

	// Token: 0x04002E2C RID: 11820
	private BoxCollider testColliderA;

	// Token: 0x04002E2D RID: 11821
	private BoxCollider testColliderB;

	// Token: 0x04002E2E RID: 11822
	private GhostReactor reactor;

	// Token: 0x04002E2F RID: 11823
	private Quaternion flip180 = Quaternion.AngleAxis(180f, Vector3.up);

	// Token: 0x04002E30 RID: 11824
	private const int MAX_VIS_CHECKS_PER_FRAME = 1;

	// Token: 0x04002E31 RID: 11825
	public int nextVisCheckNodeIndex;

	// Token: 0x020005D2 RID: 1490
	[Serializable]
	public struct TreeLevelConfig
	{
		// Token: 0x04002E32 RID: 11826
		public int minHubs;

		// Token: 0x04002E33 RID: 11827
		public int maxHubs;

		// Token: 0x04002E34 RID: 11828
		public int minCaps;

		// Token: 0x04002E35 RID: 11829
		public int maxCaps;

		// Token: 0x04002E36 RID: 11830
		public List<GhostReactorSpawnConfig> sectionSpawnConfigs;

		// Token: 0x04002E37 RID: 11831
		public List<GhostReactorLevelSection> hubs;

		// Token: 0x04002E38 RID: 11832
		public List<GhostReactorLevelSection> endCaps;

		// Token: 0x04002E39 RID: 11833
		public List<GhostReactorLevelSection> blockers;

		// Token: 0x04002E3A RID: 11834
		public List<GhostReactorLevelSectionConnector> connectors;
	}

	// Token: 0x020005D3 RID: 1491
	public enum NodeType
	{
		// Token: 0x04002E3C RID: 11836
		Hub,
		// Token: 0x04002E3D RID: 11837
		EndCap,
		// Token: 0x04002E3E RID: 11838
		Blocker
	}

	// Token: 0x020005D4 RID: 1492
	public class Node
	{
		// Token: 0x04002E3F RID: 11839
		public GhostReactorLevelGeneratorV2.NodeType type;

		// Token: 0x04002E40 RID: 11840
		public int configIndex;

		// Token: 0x04002E41 RID: 11841
		public int connectorIndex;

		// Token: 0x04002E42 RID: 11842
		public int parentNodeIndex;

		// Token: 0x04002E43 RID: 11843
		public int parentAnchorIndex;

		// Token: 0x04002E44 RID: 11844
		public int attachAnchorIndex;

		// Token: 0x04002E45 RID: 11845
		public Vector3 position;

		// Token: 0x04002E46 RID: 11846
		public Quaternion rotation;

		// Token: 0x04002E47 RID: 11847
		public GhostReactorLevelSection sectionInstance;

		// Token: 0x04002E48 RID: 11848
		public GhostReactorLevelSectionConnector connectorInstance;

		// Token: 0x04002E49 RID: 11849
		public GhostReactorLevelGeneratorV2.Node[] children;
	}

	// Token: 0x020005D5 RID: 1493
	public class Hub
	{
		// Token: 0x04002E4A RID: 11850
		public int configIndex;

		// Token: 0x04002E4B RID: 11851
		public bool needsOrderShuffle;

		// Token: 0x04002E4C RID: 11852
		public bool needsInstantiation;

		// Token: 0x04002E4D RID: 11853
		public int anchorCount;

		// Token: 0x04002E4E RID: 11854
		public List<int> anchorOrder;
	}
}
