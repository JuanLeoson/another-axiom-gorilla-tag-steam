using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class Oscillator : MonoBehaviour
{
	// Token: 0x0600007A RID: 122 RVA: 0x00004745 File Offset: 0x00002945
	public void Init(Vector3 center, Vector3 radius, Vector3 frequency, Vector3 startPhase)
	{
		this.Center = center;
		this.Radius = radius;
		this.Frequency = frequency;
		this.Phase = startPhase;
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004764 File Offset: 0x00002964
	private float SampleWave(float phase)
	{
		switch (this.WaveType)
		{
		case Oscillator.WaveTypeEnum.Sine:
			return Mathf.Sin(phase);
		case Oscillator.WaveTypeEnum.Square:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase >= 3.1415927f)
			{
				return -1f;
			}
			return 1f;
		case Oscillator.WaveTypeEnum.Triangle:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase < 1.5707964f)
			{
				return phase / 1.5707964f;
			}
			if (phase < 3.1415927f)
			{
				return 1f - (phase - 1.5707964f) / 1.5707964f;
			}
			if (phase < 4.712389f)
			{
				return (3.1415927f - phase) / 1.5707964f;
			}
			return (phase - 4.712389f) / 1.5707964f - 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x0000481F File Offset: 0x00002A1F
	public void OnEnable()
	{
		this.m_initCenter = base.transform.position;
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00004834 File Offset: 0x00002A34
	public void Update()
	{
		this.Phase += this.Frequency * 2f * 3.1415927f * Time.deltaTime;
		Vector3 position = this.UseCenter ? this.Center : this.m_initCenter;
		position.x += this.Radius.x * this.SampleWave(this.Phase.x);
		position.y += this.Radius.y * this.SampleWave(this.Phase.y);
		position.z += this.Radius.z * this.SampleWave(this.Phase.z);
		base.transform.position = position;
	}

	// Token: 0x0400008E RID: 142
	public Oscillator.WaveTypeEnum WaveType;

	// Token: 0x0400008F RID: 143
	private Vector3 m_initCenter;

	// Token: 0x04000090 RID: 144
	public bool UseCenter;

	// Token: 0x04000091 RID: 145
	public Vector3 Center;

	// Token: 0x04000092 RID: 146
	public Vector3 Radius;

	// Token: 0x04000093 RID: 147
	public Vector3 Frequency;

	// Token: 0x04000094 RID: 148
	public Vector3 Phase;

	// Token: 0x02000022 RID: 34
	public enum WaveTypeEnum
	{
		// Token: 0x04000096 RID: 150
		Sine,
		// Token: 0x04000097 RID: 151
		Square,
		// Token: 0x04000098 RID: 152
		Triangle
	}
}
