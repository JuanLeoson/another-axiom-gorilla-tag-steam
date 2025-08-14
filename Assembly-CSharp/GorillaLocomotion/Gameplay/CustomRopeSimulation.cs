using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E1F RID: 3615
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x060059DB RID: 23003 RVA: 0x001C5310 File Offset: 0x001C3510
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x060059DC RID: 23004 RVA: 0x001C53C0 File Offset: 0x001C35C0
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x060059DD RID: 23005 RVA: 0x001C53D0 File Offset: 0x001C35D0
		private void Update()
		{
			new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			}.Run<SolveRopeJob>();
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 a = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -a;
				}
			}
		}

		// Token: 0x0400649B RID: 25755
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x0400649C RID: 25756
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x0400649D RID: 25757
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x0400649E RID: 25758
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x0400649F RID: 25759
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x040064A0 RID: 25760
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
