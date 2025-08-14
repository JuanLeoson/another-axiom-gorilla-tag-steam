using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000CD3 RID: 3283
	public class CircularPatrol_State : IState
	{
		// Token: 0x06005188 RID: 20872 RVA: 0x001966BC File Offset: 0x001948BC
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		// Token: 0x06005189 RID: 20873 RVA: 0x001966CC File Offset: 0x001948CC
		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float x = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float z = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(x, y, z);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		// Token: 0x0600518A RID: 20874 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnEnter()
		{
		}

		// Token: 0x0600518B RID: 20875 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnExit()
		{
		}

		// Token: 0x04005B0E RID: 23310
		private AIEntity entity;

		// Token: 0x04005B0F RID: 23311
		private float angle;
	}
}
