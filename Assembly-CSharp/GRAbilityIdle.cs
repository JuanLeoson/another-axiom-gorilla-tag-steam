using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020005ED RID: 1517
[Serializable]
public class GRAbilityIdle
{
	// Token: 0x0600253D RID: 9533 RVA: 0x000C854E File Offset: 0x000C674E
	public void Setup(GameAgent agent, Animation anim, AudioSource audioSource)
	{
		this.agent = agent;
		this.anim = anim;
		this.audioSource = audioSource;
		this.animLoops = 0;
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000C856C File Offset: 0x000C676C
	public void Start()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		this.animLoops = 0;
		this.events.Reset();
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Stop()
	{
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000C85A2 File Offset: 0x000C67A2
	public bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000C85D0 File Offset: 0x000C67D0
	public void Update(float dt)
	{
		this.UpdateShared(dt);
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000C85D0 File Offset: 0x000C67D0
	public void UpdateRemote(float dt)
	{
		this.UpdateShared(dt);
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000C85DC File Offset: 0x000C67DC
	private void UpdateShared(float dt)
	{
		float abilityTime = (float)(Time.timeAsDouble - this.startTime);
		if (this.anim != null && this.anim[this.animName] != null)
		{
			if ((int)this.anim[this.animName].normalizedTime > this.animLoops)
			{
				this.events.Reset();
				this.animLoops = (int)this.anim[this.animName].normalizedTime;
			}
			abilityTime = this.anim[this.animName].time - this.anim[this.animName].length * (float)this.animLoops;
		}
		this.events.TryPlay(abilityTime, this.audioSource);
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000C86B0 File Offset: 0x000C68B0
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x04002F1B RID: 12059
	public float duration;

	// Token: 0x04002F1C RID: 12060
	public string animName;

	// Token: 0x04002F1D RID: 12061
	public float animSpeed;

	// Token: 0x04002F1E RID: 12062
	public GameAbilityEvents events;

	// Token: 0x04002F1F RID: 12063
	private GameAgent agent;

	// Token: 0x04002F20 RID: 12064
	private Animation anim;

	// Token: 0x04002F21 RID: 12065
	private AudioSource audioSource;

	// Token: 0x04002F22 RID: 12066
	[ReadOnly]
	public double startTime;

	// Token: 0x04002F23 RID: 12067
	[ReadOnly]
	public int animLoops;
}
