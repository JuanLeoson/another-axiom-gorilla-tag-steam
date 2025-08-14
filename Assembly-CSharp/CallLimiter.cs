using System;
using UnityEngine;

// Token: 0x02000A48 RID: 2632
[Serializable]
public class CallLimiter
{
	// Token: 0x06004068 RID: 16488 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimiter()
	{
	}

	// Token: 0x06004069 RID: 16489 RVA: 0x001467C4 File Offset: 0x001449C4
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = (double)latencyMax;
	}

	// Token: 0x0600406A RID: 16490 RVA: 0x00146814 File Offset: 0x00144A14
	public bool CheckCallServerTime(double time)
	{
		double simTime = NetworkSystem.Instance.SimTime;
		double num = this.maxLatency;
		double num2 = 4294967.295 - this.maxLatency;
		double num3;
		if (simTime > num || time < num)
		{
			if (time > simTime)
			{
				return false;
			}
			num3 = simTime - time;
		}
		else
		{
			double num4 = num2 + simTime;
			if (time > simTime && time < num4)
			{
				return false;
			}
			num3 = simTime + (4294967.295 - time);
		}
		if (num3 > this.maxLatency)
		{
			return false;
		}
		int num5 = (this.oldTimeIndex > 0) ? (this.oldTimeIndex - 1) : (this.callHistoryLength - 1);
		double num6 = (double)this.callTimeHistory[num5];
		if (num6 > num2 && time < num6)
		{
			this.Reset();
		}
		else if (time < num6)
		{
			return false;
		}
		return this.CheckCallTime((float)time);
	}

	// Token: 0x0600406B RID: 16491 RVA: 0x001468CC File Offset: 0x00144ACC
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x0600406C RID: 16492 RVA: 0x00146934 File Offset: 0x00144B34
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x04004C12 RID: 19474
	protected const double k_serverMaxTime = 4294967.295;

	// Token: 0x04004C13 RID: 19475
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x04004C14 RID: 19476
	[Space]
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x04004C15 RID: 19477
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x04004C16 RID: 19478
	[SerializeField]
	protected double maxLatency;

	// Token: 0x04004C17 RID: 19479
	private int oldTimeIndex;

	// Token: 0x04004C18 RID: 19480
	protected bool blockCall;

	// Token: 0x04004C19 RID: 19481
	protected float blockStartTime;
}
