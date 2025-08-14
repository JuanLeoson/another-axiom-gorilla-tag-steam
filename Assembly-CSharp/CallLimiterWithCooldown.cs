using System;
using UnityEngine;

// Token: 0x02000A49 RID: 2633
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x0600406D RID: 16493 RVA: 0x00146981 File Offset: 0x00144B81
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown) : base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x0600406E RID: 16494 RVA: 0x00146997 File Offset: 0x00144B97
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax) : base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x0600406F RID: 16495 RVA: 0x001469AA File Offset: 0x00144BAA
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04004C1A RID: 19482
	[SerializeField]
	private float spamCoolDown;
}
