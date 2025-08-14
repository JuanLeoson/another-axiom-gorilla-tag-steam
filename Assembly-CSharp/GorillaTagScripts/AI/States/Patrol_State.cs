using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000CD4 RID: 3284
	public class Patrol_State : IState
	{
		// Token: 0x0600518C RID: 20876 RVA: 0x00196763 File Offset: 0x00194963
		public Patrol_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x0600518D RID: 20877 RVA: 0x00196784 File Offset: 0x00194984
		public void Tick()
		{
			if (this.agent.remainingDistance <= this.agent.stoppingDistance)
			{
				Vector3 position = this.entity.waypoints[Random.Range(0, this.entity.waypoints.Count - 1)].transform.position;
				this.agent.SetDestination(position);
			}
		}

		// Token: 0x0600518E RID: 20878 RVA: 0x001967EC File Offset: 0x001949EC
		public void OnEnter()
		{
			string str = "Current State: ";
			Type typeFromHandle = typeof(Patrol_State);
			Debug.Log(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			if (this.entity.waypoints.Count > 0)
			{
				this.agent.SetDestination(this.entity.waypoints[0].transform.position);
			}
		}

		// Token: 0x0600518F RID: 20879 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnExit()
		{
		}

		// Token: 0x04005B10 RID: 23312
		private AIEntity entity;

		// Token: 0x04005B11 RID: 23313
		private NavMeshAgent agent;
	}
}
