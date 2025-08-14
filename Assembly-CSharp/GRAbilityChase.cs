using System;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
[Serializable]
public class GRAbilityChase
{
	// Token: 0x06002561 RID: 9569 RVA: 0x000C8B9C File Offset: 0x000C6D9C
	public void Setup(GameAgent agent, Animation anim, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		this.head = head;
		this.lineOfSight = lineOfSight;
		this.targetPlayer = null;
		this.lastSeenTargetTime = 0.0;
		this.lastSeenTargetPosition = Vector3.zero;
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000C8BF0 File Offset: 0x000C6DF0
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.navAgent.speed = this.chaseSpeed;
		this.lastSeenTargetTime = Time.timeAsDouble;
		this.movementSound.Play(null);
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Stop()
	{
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000C8C41 File Offset: 0x000C6E41
	public bool IsDone()
	{
		return this.targetPlayer == null || Time.timeAsDouble - this.lastSeenTargetTime >= (double)this.giveUpDelay;
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000C8C68 File Offset: 0x000C6E68
	public void Think(float dt)
	{
		GRPlayer grplayer = GRPlayer.Get(this.targetPlayer);
		if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
		{
			Vector3 position = grplayer.transform.position;
			if (this.lineOfSight.HasLineOfSight(this.head.position, position))
			{
				this.lastSeenTargetTime = Time.timeAsDouble;
			}
			if ((float)(Time.timeAsDouble - this.lastSeenTargetTime) <= this.loseVisibilityDelay)
			{
				this.lastSeenTargetPosition = position;
			}
		}
		this.agent.RequestDestination(this.lastSeenTargetPosition);
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000C8CF1 File Offset: 0x000C6EF1
	public void Update(float dt)
	{
		GameAgent.UpdateFacing(this.root, this.agent.navAgent, this.targetPlayer, this.maxTurnSpeed);
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000C8D15 File Offset: 0x000C6F15
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.targetPlayer = targetPlayer;
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x000C8D1E File Offset: 0x000C6F1E
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F44 RID: 12100
	public float chaseSpeed;

	// Token: 0x04002F45 RID: 12101
	public string animName;

	// Token: 0x04002F46 RID: 12102
	public float animSpeed;

	// Token: 0x04002F47 RID: 12103
	public float maxTurnSpeed;

	// Token: 0x04002F48 RID: 12104
	public float loseVisibilityDelay;

	// Token: 0x04002F49 RID: 12105
	public float giveUpDelay;

	// Token: 0x04002F4A RID: 12106
	public AbilitySound movementSound;

	// Token: 0x04002F4B RID: 12107
	private GameAgent agent;

	// Token: 0x04002F4C RID: 12108
	private Animation anim;

	// Token: 0x04002F4D RID: 12109
	private Transform root;

	// Token: 0x04002F4E RID: 12110
	private Transform head;

	// Token: 0x04002F4F RID: 12111
	private NetPlayer targetPlayer;

	// Token: 0x04002F50 RID: 12112
	private GRSenseLineOfSight lineOfSight;

	// Token: 0x04002F51 RID: 12113
	private double lastSeenTargetTime;

	// Token: 0x04002F52 RID: 12114
	private Vector3 lastSeenTargetPosition;
}
