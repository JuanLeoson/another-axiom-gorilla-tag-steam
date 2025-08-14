using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
[Serializable]
public class GRAbilityWatch
{
	// Token: 0x0600254D RID: 9549 RVA: 0x000C88CA File Offset: 0x000C6ACA
	public void Setup(GameAgent agent, Animation anim, Transform root)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		this.target = null;
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x000C88E8 File Offset: 0x000C6AE8
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.endTime = -1.0;
		if (this.duration > 0f)
		{
			this.endTime = Time.timeAsDouble + (double)this.duration;
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Stop()
	{
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000C893B File Offset: 0x000C6B3B
	public bool IsDone()
	{
		return this.endTime > 0.0 && Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000C8960 File Offset: 0x000C6B60
	public void Update(float dt)
	{
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000C8984 File Offset: 0x000C6B84
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

	// Token: 0x06002553 RID: 9555 RVA: 0x000C89C4 File Offset: 0x000C6BC4
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F30 RID: 12080
	public float duration;

	// Token: 0x04002F31 RID: 12081
	public string animName;

	// Token: 0x04002F32 RID: 12082
	public float animSpeed;

	// Token: 0x04002F33 RID: 12083
	public float maxTurnSpeed;

	// Token: 0x04002F34 RID: 12084
	private GameAgent agent;

	// Token: 0x04002F35 RID: 12085
	private Animation anim;

	// Token: 0x04002F36 RID: 12086
	private Transform root;

	// Token: 0x04002F37 RID: 12087
	private Transform target;

	// Token: 0x04002F38 RID: 12088
	[ReadOnly]
	public double endTime;
}
