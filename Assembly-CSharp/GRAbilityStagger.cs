using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
[Serializable]
public class GRAbilityStagger
{
	// Token: 0x06002586 RID: 9606 RVA: 0x000C970C File Offset: 0x000C790C
	public void Setup(Vector3 staggerVel, GameAgent agent, Animation anim, Transform root, Rigidbody rb)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		this.rb = rb;
		this.staggerMovement.Setup(root);
		this.staggerMovement.InitFromVelocityAndDuration(staggerVel, this.duration);
		this.staggerMovement.interpolationType = GRAbilityInterpolatedMovement.InterpType.EaseOut;
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000C9764 File Offset: 0x000C7964
	public void Start()
	{
		if (this.animData.Count > 0)
		{
			int index = Random.Range(0, this.animData.Count);
			this.duration = this.animData[index].duration;
			this.PlayAnim(this.animData[index].animName, 0.1f, this.animData[index].speed);
		}
		else
		{
			this.duration = 0.5f;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.duration);
		this.staggerMovement.Start();
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000C9822 File Offset: 0x000C7A22
	public void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000C983D File Offset: 0x000C7A3D
	public bool IsDone()
	{
		return this.staggerMovement.IsDone();
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x000C984A File Offset: 0x000C7A4A
	public void Update(float dt)
	{
		this.staggerMovement.Update(dt);
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x000C9858 File Offset: 0x000C7A58
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F8F RID: 12175
	private float duration;

	// Token: 0x04002F90 RID: 12176
	public List<AnimationData> animData;

	// Token: 0x04002F91 RID: 12177
	private GameAgent agent;

	// Token: 0x04002F92 RID: 12178
	private Animation anim;

	// Token: 0x04002F93 RID: 12179
	private Transform root;

	// Token: 0x04002F94 RID: 12180
	private Rigidbody rb;

	// Token: 0x04002F95 RID: 12181
	public float maxStaggerImpulse = 2f;

	// Token: 0x04002F96 RID: 12182
	public GRAbilityInterpolatedMovement staggerMovement;
}
