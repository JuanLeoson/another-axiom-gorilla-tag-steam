using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts.AI.States;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.AI.Entities
{
	// Token: 0x02000CD5 RID: 3285
	public class TestShark : AIEntity
	{
		// Token: 0x06005190 RID: 20880 RVA: 0x00196858 File Offset: 0x00194A58
		private new void Awake()
		{
			base.Awake();
			this.chasingTimer = 0f;
			this._stateMachine = new StateMachine();
			this.circularPatrol = new CircularPatrol_State(this);
			this.patrol = new Patrol_State(this);
			this.chase = new Chase_State(this);
			this._stateMachine.AddTransition(this.patrol, this.chase, this.<Awake>g__ShouldChase|7_0());
			this._stateMachine.AddTransition(this.chase, this.patrol, this.<Awake>g__ShouldPatrol|7_1());
			this._stateMachine.SetState(this.patrol);
		}

		// Token: 0x06005191 RID: 20881 RVA: 0x001968F0 File Offset: 0x00194AF0
		private void Update()
		{
			this._stateMachine.Tick();
			this.shouldChase = false;
			this.chasingTimer += Time.deltaTime;
			if (this.chasingTimer >= this.nextTimeToChasePlayer)
			{
				base.ChooseClosestTarget();
				if (this.followTarget != null)
				{
					this.chase.FollowTarget = this.followTarget;
					this.shouldChase = true;
				}
				this.chasingTimer = 0f;
			}
		}

		// Token: 0x06005193 RID: 20883 RVA: 0x00196979 File Offset: 0x00194B79
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x06005195 RID: 20885 RVA: 0x00196998 File Offset: 0x00194B98
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04005B12 RID: 23314
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04005B13 RID: 23315
		private float chasingTimer;

		// Token: 0x04005B14 RID: 23316
		private bool shouldChase;

		// Token: 0x04005B15 RID: 23317
		private StateMachine _stateMachine;

		// Token: 0x04005B16 RID: 23318
		private CircularPatrol_State circularPatrol;

		// Token: 0x04005B17 RID: 23319
		private Patrol_State patrol;

		// Token: 0x04005B18 RID: 23320
		private Chase_State chase;
	}
}
