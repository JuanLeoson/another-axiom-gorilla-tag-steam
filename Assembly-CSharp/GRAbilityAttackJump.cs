using System;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
[Serializable]
public class GRAbilityAttackJump
{
	// Token: 0x0600257E RID: 9598 RVA: 0x000C92D4 File Offset: 0x000C74D4
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

	// Token: 0x0600257F RID: 9599 RVA: 0x000C930C File Offset: 0x000C750C
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.state = GRAbilityAttackJump.State.Tell;
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x000C9375 File Offset: 0x000C7575
	public void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x000C93AA File Offset: 0x000C75AA
	public bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x000C93C4 File Offset: 0x000C75C4
	public void Update(float dt)
	{
		double num = (double)((float)Time.timeAsDouble) - this.startTime;
		switch (this.state)
		{
		case GRAbilityAttackJump.State.Tell:
			if (num > (double)this.jumpTime)
			{
				this.targetPos = this.agent.transform.position + this.agent.transform.forward * 0.5f;
				if (this.target != null)
				{
					this.targetPos = this.target.transform.position;
				}
				float num2 = this.attackLandTime - this.jumpTime;
				num2 = Mathf.Max(0.1f, num2);
				this.initialPos = this.agent.transform.position;
				Vector3 vector = this.targetPos - this.initialPos;
				float y = vector.y;
				vector.y = 0f;
				float num3 = num2;
				float y2 = 0f;
				if (num3 > 0f)
				{
					Vector3 gravity = Physics.gravity;
					y2 = (y - 0.5f * gravity.y * num3 * num3) / num3;
				}
				this.initialVel = vector / num2;
				this.initialVel.y = y2;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.PlayAnim(this.jumpAnimName, 0.1f, this.animSpeed);
				this.jumpSound.Play(null);
				this.state = GRAbilityAttackJump.State.Jump;
			}
			break;
		case GRAbilityAttackJump.State.Jump:
		{
			float d = (float)(num - (double)this.jumpTime);
			Vector3 position = this.initialPos + this.initialVel * d + 0.5f * Physics.gravity * d * d;
			this.root.position = position;
			if (num > (double)this.attackLandTime)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				float num4 = this.attackReturnTime - this.attackLandTime;
				num4 = Mathf.Max(0.1f, num4);
				Vector3 a = this.initialPos;
				this.initialPos = this.agent.transform.position;
				this.initialVel = (a - this.initialPos) / num4;
				this.state = GRAbilityAttackJump.State.Return;
			}
			break;
		}
		case GRAbilityAttackJump.State.Return:
		{
			float d2 = (float)(num - (double)this.attackLandTime);
			Vector3 position2 = this.initialPos + this.initialVel * d2;
			this.root.position = position2;
			if (num > (double)this.attackReturnTime)
			{
				this.state = GRAbilityAttackJump.State.Done;
			}
			break;
		}
		}
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x000C9694 File Offset: 0x000C7894
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

	// Token: 0x06002584 RID: 9604 RVA: 0x000C96D4 File Offset: 0x000C78D4
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F76 RID: 12150
	public float duration;

	// Token: 0x04002F77 RID: 12151
	public float attackMoveSpeed;

	// Token: 0x04002F78 RID: 12152
	public float jumpTime;

	// Token: 0x04002F79 RID: 12153
	public float attackLandTime;

	// Token: 0x04002F7A RID: 12154
	public float attackReturnTime;

	// Token: 0x04002F7B RID: 12155
	public string animName;

	// Token: 0x04002F7C RID: 12156
	public float animSpeed;

	// Token: 0x04002F7D RID: 12157
	public float maxTurnSpeed;

	// Token: 0x04002F7E RID: 12158
	public string jumpAnimName;

	// Token: 0x04002F7F RID: 12159
	public AbilitySound jumpSound;

	// Token: 0x04002F80 RID: 12160
	public GameObject damageTrigger;

	// Token: 0x04002F81 RID: 12161
	private GameAgent agent;

	// Token: 0x04002F82 RID: 12162
	private Animation anim;

	// Token: 0x04002F83 RID: 12163
	private Transform root;

	// Token: 0x04002F84 RID: 12164
	private Transform target;

	// Token: 0x04002F85 RID: 12165
	private double startTime;

	// Token: 0x04002F86 RID: 12166
	private GRAbilityAttackJump.State state;

	// Token: 0x04002F87 RID: 12167
	public Vector3 targetPos;

	// Token: 0x04002F88 RID: 12168
	public Vector3 initialPos;

	// Token: 0x04002F89 RID: 12169
	public Vector3 initialVel;

	// Token: 0x020005F7 RID: 1527
	private enum State
	{
		// Token: 0x04002F8B RID: 12171
		Tell,
		// Token: 0x04002F8C RID: 12172
		Jump,
		// Token: 0x04002F8D RID: 12173
		Return,
		// Token: 0x04002F8E RID: 12174
		Done
	}
}
