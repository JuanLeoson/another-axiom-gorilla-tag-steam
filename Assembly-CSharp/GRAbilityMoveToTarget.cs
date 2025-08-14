using System;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
[Serializable]
public class GRAbilityMoveToTarget
{
	// Token: 0x06002555 RID: 9557 RVA: 0x000C89FB File Offset: 0x000C6BFB
	public void Setup(GameAgent agent, Animation anim, Transform root)
	{
		this.agent = agent;
		this.anim = anim;
		this.root = root;
		this.target = null;
		this.targetPos = agent.transform.position;
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000C8A2C File Offset: 0x000C6C2C
	public void Start()
	{
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.agent.navAgent.speed = this.moveSpeed;
		this.targetPos = this.agent.transform.position;
		this.movementSound.Play(null);
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Stop()
	{
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x000C8A88 File Offset: 0x000C6C88
	public bool IsDone()
	{
		return (this.targetPos - this.root.position).sqrMagnitude < 0.25f;
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Think(float dt)
	{
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000C8ABC File Offset: 0x000C6CBC
	public void Update(float dt)
	{
		if (this.target != null)
		{
			this.targetPos = this.target.position;
			this.agent.RequestDestination(this.targetPos);
		}
		Transform transform = (this.lookAtTarget != null) ? this.lookAtTarget : this.target;
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, transform, this.maxTurnSpeed);
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000C8B33 File Offset: 0x000C6D33
	public void SetTarget(Transform transform)
	{
		this.target = transform;
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000C8B3C File Offset: 0x000C6D3C
	public void SetTargetPos(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		this.agent.RequestDestination(targetPos);
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000C8B51 File Offset: 0x000C6D51
	public Vector3 GetTargetPos()
	{
		return this.targetPos;
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x000C8B59 File Offset: 0x000C6D59
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x000C8B62 File Offset: 0x000C6D62
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F39 RID: 12089
	public float moveSpeed;

	// Token: 0x04002F3A RID: 12090
	public string animName;

	// Token: 0x04002F3B RID: 12091
	public float animSpeed;

	// Token: 0x04002F3C RID: 12092
	public float maxTurnSpeed;

	// Token: 0x04002F3D RID: 12093
	public AbilitySound movementSound;

	// Token: 0x04002F3E RID: 12094
	private GameAgent agent;

	// Token: 0x04002F3F RID: 12095
	private Animation anim;

	// Token: 0x04002F40 RID: 12096
	private Transform root;

	// Token: 0x04002F41 RID: 12097
	private Vector3 targetPos;

	// Token: 0x04002F42 RID: 12098
	private Transform target;

	// Token: 0x04002F43 RID: 12099
	private Transform lookAtTarget;
}
