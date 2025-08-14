using System;
using UnityEngine;

// Token: 0x020005F5 RID: 1525
[Serializable]
public class GRAbilityAttackLatchOn
{
	// Token: 0x06002574 RID: 9588 RVA: 0x000C9067 File Offset: 0x000C7267
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

	// Token: 0x06002575 RID: 9589 RVA: 0x000C90A0 File Offset: 0x000C72A0
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.navAgent.speed = this.tellMoveSpeed;
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000C90FF File Offset: 0x000C72FF
	public void Stop()
	{
		this.agent.transform.SetParent(null);
		this.agent.SetIsPathing(true, true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002577 RID: 9591 RVA: 0x000C9139 File Offset: 0x000C7339
	public bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002578 RID: 9592 RVA: 0x000C9153 File Offset: 0x000C7353
	public void Update(float dt)
	{
		this.UpdateNavSpeed();
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002579 RID: 9593 RVA: 0x000C917D File Offset: 0x000C737D
	public void UpdateRemote(float dt)
	{
		this.UpdateNavSpeed();
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000C9188 File Offset: 0x000C7388
	private void UpdateNavSpeed()
	{
		if (Time.timeAsDouble - this.startTime > (double)this.tellDuration)
		{
			this.agent.navAgent.velocity = this.agent.navAgent.velocity.normalized * this.attackMoveSpeed;
			this.agent.navAgent.speed = this.attackMoveSpeed;
			if (this.damageTrigger != null)
			{
				this.damageTrigger.SetActive(true);
			}
		}
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000C9210 File Offset: 0x000C7410
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.agent.transform.SetParent(grplayer.attachEnemy);
				this.agent.transform.localPosition = Vector3.zero;
				this.agent.transform.localRotation = Quaternion.identity;
				this.agent.SetIsPathing(false, true);
			}
		}
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000C929D File Offset: 0x000C749D
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F69 RID: 12137
	public float duration;

	// Token: 0x04002F6A RID: 12138
	public float attackMoveSpeed;

	// Token: 0x04002F6B RID: 12139
	public float tellDuration;

	// Token: 0x04002F6C RID: 12140
	public float tellMoveSpeed;

	// Token: 0x04002F6D RID: 12141
	public string animName;

	// Token: 0x04002F6E RID: 12142
	public float animSpeed;

	// Token: 0x04002F6F RID: 12143
	public float maxTurnSpeed;

	// Token: 0x04002F70 RID: 12144
	public GameObject damageTrigger;

	// Token: 0x04002F71 RID: 12145
	private GameAgent agent;

	// Token: 0x04002F72 RID: 12146
	private Animation anim;

	// Token: 0x04002F73 RID: 12147
	private Transform root;

	// Token: 0x04002F74 RID: 12148
	private Transform target;

	// Token: 0x04002F75 RID: 12149
	private double startTime;
}
