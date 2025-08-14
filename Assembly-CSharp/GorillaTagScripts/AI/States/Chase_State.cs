using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000CD2 RID: 3282
	public class Chase_State : IState
	{
		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06005182 RID: 20866 RVA: 0x0019661C File Offset: 0x0019481C
		// (set) Token: 0x06005183 RID: 20867 RVA: 0x00196624 File Offset: 0x00194824
		public Transform FollowTarget { get; set; }

		// Token: 0x06005184 RID: 20868 RVA: 0x0019662D File Offset: 0x0019482D
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x06005185 RID: 20869 RVA: 0x0019664D File Offset: 0x0019484D
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x06005186 RID: 20870 RVA: 0x00196685 File Offset: 0x00194885
		public void OnEnter()
		{
			this.chaseOver = false;
			string str = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x06005187 RID: 20871 RVA: 0x001966B3 File Offset: 0x001948B3
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x04005B0A RID: 23306
		private AIEntity entity;

		// Token: 0x04005B0B RID: 23307
		private NavMeshAgent agent;

		// Token: 0x04005B0D RID: 23309
		public bool chaseOver;
	}
}
