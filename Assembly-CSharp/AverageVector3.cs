using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A93 RID: 2707
public class AverageVector3
{
	// Token: 0x060041AF RID: 16815 RVA: 0x0014B381 File Offset: 0x00149581
	public AverageVector3(float averagingWindow = 0.1f)
	{
		this.timeWindow = averagingWindow;
	}

	// Token: 0x060041B0 RID: 16816 RVA: 0x0014B3A8 File Offset: 0x001495A8
	public void AddSample(Vector3 sample, float time)
	{
		this.samples.Add(new AverageVector3.Sample
		{
			timeStamp = time,
			value = sample
		});
		this.RefreshSamples();
	}

	// Token: 0x060041B1 RID: 16817 RVA: 0x0014B3E0 File Offset: 0x001495E0
	public Vector3 GetAverage()
	{
		this.RefreshSamples();
		Vector3 a = Vector3.zero;
		for (int i = 0; i < this.samples.Count; i++)
		{
			a += this.samples[i].value;
		}
		return a / (float)this.samples.Count;
	}

	// Token: 0x060041B2 RID: 16818 RVA: 0x0014B43B File Offset: 0x0014963B
	public void Clear()
	{
		this.samples.Clear();
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x0014B448 File Offset: 0x00149648
	private void RefreshSamples()
	{
		float num = Time.time - this.timeWindow;
		for (int i = this.samples.Count - 1; i >= 0; i--)
		{
			if (this.samples[i].timeStamp < num)
			{
				this.samples.RemoveAt(i);
			}
		}
	}

	// Token: 0x04004D27 RID: 19751
	private List<AverageVector3.Sample> samples = new List<AverageVector3.Sample>();

	// Token: 0x04004D28 RID: 19752
	private float timeWindow = 0.1f;

	// Token: 0x02000A94 RID: 2708
	public struct Sample
	{
		// Token: 0x04004D29 RID: 19753
		public float timeStamp;

		// Token: 0x04004D2A RID: 19754
		public Vector3 value;
	}
}
