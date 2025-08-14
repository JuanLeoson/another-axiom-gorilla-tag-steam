using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class RotationStepper : MonoBehaviour
{
	// Token: 0x0600007F RID: 127 RVA: 0x00004911 File Offset: 0x00002B11
	public void OnEnable()
	{
		this.m_phase = 0f;
		Random.InitState(0);
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004924 File Offset: 0x00002B24
	public void Update()
	{
		this.m_phase += this.Frequency * Time.deltaTime;
		RotationStepper.ModeEnum mode = this.Mode;
		if (mode == RotationStepper.ModeEnum.Fixed)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Repeat(this.m_phase, 2f) < 1f) ? -25f : 25f);
			return;
		}
		if (mode != RotationStepper.ModeEnum.Random)
		{
			return;
		}
		while (this.m_phase >= 1f)
		{
			Random.InitState(Time.frameCount);
			base.transform.rotation = Random.rotationUniform;
			this.m_phase -= 1f;
		}
	}

	// Token: 0x04000099 RID: 153
	public RotationStepper.ModeEnum Mode;

	// Token: 0x0400009A RID: 154
	[ConditionalField("Mode", RotationStepper.ModeEnum.Fixed, null, null, null, null, null)]
	public float Angle = 25f;

	// Token: 0x0400009B RID: 155
	public float Frequency;

	// Token: 0x0400009C RID: 156
	private float m_phase;

	// Token: 0x02000024 RID: 36
	public enum ModeEnum
	{
		// Token: 0x0400009E RID: 158
		Fixed,
		// Token: 0x0400009F RID: 159
		Random
	}
}
