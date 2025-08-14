using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005F3 RID: 1523
[Serializable]
public class GRAbilityAttackSwipe
{
	// Token: 0x0600256A RID: 9578 RVA: 0x000C8D55 File Offset: 0x000C6F55
	public void Setup(GameAgent agent, Animation anim, Transform root)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000C8D90 File Offset: 0x000C6F90
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.state = GRAbilityAttackSwipe.State.Tell;
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x000C8DF9 File Offset: 0x000C6FF9
	public void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000C8E2E File Offset: 0x000C702E
	public bool IsDone()
	{
		return this.state == GRAbilityAttackSwipe.State.Done;
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000C8E39 File Offset: 0x000C7039
	public void Update(float dt)
	{
		this.UpdateShared();
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000C8E39 File Offset: 0x000C7039
	public void UpdateRemote(float dt)
	{
		this.UpdateShared();
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000C8E44 File Offset: 0x000C7044
	private void UpdateShared()
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSwipe.State.Tell:
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackSwipe.State.Attack;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			break;
		case GRAbilityAttackSwipe.State.Attack:
		{
			float d = num - this.tellDuration;
			Vector3 vector = this.initialPos + this.initialVel * d;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, -1))
			{
				vector = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector, out navMeshHit, -1))
				{
					vector = navMeshHit.position;
				}
				this.root.position = vector;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				this.state = GRAbilityAttackSwipe.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackSwipe.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackSwipe.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000C8FF0 File Offset: 0x000C71F0
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
			}
		}
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000C9030 File Offset: 0x000C7230
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F53 RID: 12115
	public float duration;

	// Token: 0x04002F54 RID: 12116
	public float tellDuration;

	// Token: 0x04002F55 RID: 12117
	public float attackDuration;

	// Token: 0x04002F56 RID: 12118
	public float attackMoveSpeed;

	// Token: 0x04002F57 RID: 12119
	private GRAbilityAttackSwipe.State state;

	// Token: 0x04002F58 RID: 12120
	public string animName;

	// Token: 0x04002F59 RID: 12121
	public float animSpeed;

	// Token: 0x04002F5A RID: 12122
	public float maxTurnSpeed;

	// Token: 0x04002F5B RID: 12123
	public GameObject damageTrigger;

	// Token: 0x04002F5C RID: 12124
	private GameAgent agent;

	// Token: 0x04002F5D RID: 12125
	private Animation anim;

	// Token: 0x04002F5E RID: 12126
	private Transform root;

	// Token: 0x04002F5F RID: 12127
	private Transform target;

	// Token: 0x04002F60 RID: 12128
	private double startTime;

	// Token: 0x04002F61 RID: 12129
	public Vector3 targetPos;

	// Token: 0x04002F62 RID: 12130
	public Vector3 initialPos;

	// Token: 0x04002F63 RID: 12131
	public Vector3 initialVel;

	// Token: 0x020005F4 RID: 1524
	private enum State
	{
		// Token: 0x04002F65 RID: 12133
		Tell,
		// Token: 0x04002F66 RID: 12134
		Attack,
		// Token: 0x04002F67 RID: 12135
		FollowThrough,
		// Token: 0x04002F68 RID: 12136
		Done
	}
}
