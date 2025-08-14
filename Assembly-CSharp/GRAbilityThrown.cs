using System;
using UnityEngine;

// Token: 0x020005FC RID: 1532
[Serializable]
public class GRAbilityThrown
{
	// Token: 0x060025A8 RID: 9640 RVA: 0x000C9FB0 File Offset: 0x000C81B0
	public void Setup(GameAgent agent, Animation anim, AudioSource audioSource)
	{
		this.agent = agent;
		this.idleAbility.Setup(agent, anim, audioSource);
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000C9FC7 File Offset: 0x000C81C7
	public void Start()
	{
		this.agent.SetIsPathing(false, false);
		this.idleAbility.Start();
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000C9FE1 File Offset: 0x000C81E1
	public void Stop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, false);
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x000C9FFB File Offset: 0x000C81FB
	public bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000CA008 File Offset: 0x000C8208
	public void Update(float dt)
	{
		this.idleAbility.Update(dt);
	}

	// Token: 0x04002FB0 RID: 12208
	private GameAgent agent;

	// Token: 0x04002FB1 RID: 12209
	public GRAbilityIdle idleAbility;
}
