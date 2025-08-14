using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000CCD RID: 3277
	public class AIEntity : MonoBehaviour
	{
		// Token: 0x0600516E RID: 20846 RVA: 0x00196154 File Offset: 0x00194354
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform item in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(item);
				}
			}
		}

		// Token: 0x0600516F RID: 20847 RVA: 0x001961BC File Offset: 0x001943BC
		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == GorillaParent.instance.vrrigs[randomTarget].creator);
			if (num == -1)
			{
				num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			}
			if (num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x06005170 RID: 20848 RVA: 0x001962AC File Offset: 0x001944AC
		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				if (vrrig2.head != null && !(vrrig2.head.rigTarget == null))
				{
					float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = vrrig2;
					}
				}
			}
			if (vrrig != null)
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x04005AF0 RID: 23280
		public GameObject waypointsContainer;

		// Token: 0x04005AF1 RID: 23281
		public Transform circleCenter;

		// Token: 0x04005AF2 RID: 23282
		public float circleRadius;

		// Token: 0x04005AF3 RID: 23283
		public float angularSpeed;

		// Token: 0x04005AF4 RID: 23284
		public float patrolSpeed;

		// Token: 0x04005AF5 RID: 23285
		public float fleeSpeed;

		// Token: 0x04005AF6 RID: 23286
		public NavMeshAgent navMeshAgent;

		// Token: 0x04005AF7 RID: 23287
		public Animator animator;

		// Token: 0x04005AF8 RID: 23288
		public float fleeRang;

		// Token: 0x04005AF9 RID: 23289
		public float fleeSpeedMult;

		// Token: 0x04005AFA RID: 23290
		public float minChaseRange;

		// Token: 0x04005AFB RID: 23291
		public float attackDistance;

		// Token: 0x04005AFC RID: 23292
		public float navMeshSampleRange = 5f;

		// Token: 0x04005AFD RID: 23293
		internal readonly List<Transform> waypoints = new List<Transform>();

		// Token: 0x04005AFE RID: 23294
		internal float defaultSpeed;

		// Token: 0x04005AFF RID: 23295
		public Transform followTarget;

		// Token: 0x04005B00 RID: 23296
		public NetPlayer targetPlayer;

		// Token: 0x04005B01 RID: 23297
		public bool targetIsOnNavMesh;
	}
}
