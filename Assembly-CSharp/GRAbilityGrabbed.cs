using System;
using UnityEngine;

// Token: 0x020005FB RID: 1531
[Serializable]
public class GRAbilityGrabbed
{
	// Token: 0x060025A2 RID: 9634 RVA: 0x000C9F4A File Offset: 0x000C814A
	public void Setup(GameAgent agent, Animation anim, AudioSource audioSource)
	{
		this.agent = agent;
		this.idleAbility.Setup(agent, anim, audioSource);
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000C9F61 File Offset: 0x000C8161
	public void Start()
	{
		this.agent.SetIsPathing(false, true);
		this.idleAbility.Start();
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000C9F7B File Offset: 0x000C817B
	public void Stop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, true);
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000C9F95 File Offset: 0x000C8195
	public bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x060025A6 RID: 9638 RVA: 0x000C9FA2 File Offset: 0x000C81A2
	public void Update(float dt)
	{
		this.idleAbility.Update(dt);
	}

	// Token: 0x04002FAE RID: 12206
	private GameAgent agent;

	// Token: 0x04002FAF RID: 12207
	public GRAbilityIdle idleAbility;
}
