using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E21 RID: 3617
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x060059DF RID: 23007 RVA: 0x001C54F0 File Offset: 0x001C36F0
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x060059E0 RID: 23008 RVA: 0x001C5518 File Offset: 0x001C3718
		private void Simulate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				Vector3 b = burstRopeNode.curPos - burstRopeNode.lastPos;
				burstRopeNode.lastPos = burstRopeNode.curPos;
				Vector3 vector = burstRopeNode.curPos + b;
				vector += this.gravity * this.fixedDeltaTime;
				burstRopeNode.curPos = vector;
				this.nodes[i] = burstRopeNode;
			}
		}

		// Token: 0x060059E1 RID: 23009 RVA: 0x001C55A4 File Offset: 0x001C37A4
		private void ApplyConstraint()
		{
			BurstRopeNode value = this.nodes[0];
			value.curPos = this.rootPos;
			this.nodes[0] = value;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				BurstRopeNode burstRopeNode2 = this.nodes[i + 1];
				float magnitude = (burstRopeNode.curPos - burstRopeNode2.curPos).magnitude;
				float d = Mathf.Abs(magnitude - this.nodeDistance);
				Vector3 a = Vector3.zero;
				if (magnitude > this.nodeDistance)
				{
					a = (burstRopeNode.curPos - burstRopeNode2.curPos).normalized;
				}
				else if (magnitude < this.nodeDistance)
				{
					a = (burstRopeNode2.curPos - burstRopeNode.curPos).normalized;
				}
				Vector3 a2 = a * d;
				burstRopeNode.curPos -= a2 * 0.5f;
				burstRopeNode2.curPos += a2 * 0.5f;
				this.nodes[i] = burstRopeNode;
				this.nodes[i + 1] = burstRopeNode2;
			}
		}

		// Token: 0x040064A3 RID: 25763
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x040064A4 RID: 25764
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x040064A5 RID: 25765
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x040064A6 RID: 25766
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x040064A7 RID: 25767
		[ReadOnly]
		public float nodeDistance;
	}
}
