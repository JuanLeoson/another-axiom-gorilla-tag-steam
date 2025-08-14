using System;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class CrossFadeAudioSources : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x06002245 RID: 8773 RVA: 0x000B951C File Offset: 0x000B771C
	public void Play()
	{
		if (this.source1)
		{
			this.source1.Play();
		}
		if (this.source2)
		{
			this.source2.Play();
		}
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000B954E File Offset: 0x000B774E
	public void Stop()
	{
		if (this.source1)
		{
			this.source1.Stop();
		}
		if (this.source2)
		{
			this.source2.Stop();
		}
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000B9580 File Offset: 0x000B7780
	private void Update()
	{
		if (!this.source1 || !this.source2)
		{
			return;
		}
		float num = this._curve.Evaluate(this._lerp);
		float num2;
		if (this.tween)
		{
			num2 = MathUtils.Xlerp(this._lastT, num, Time.deltaTime, this.tweenSpeed);
		}
		else
		{
			num2 = (this.lerpByClipLength ? this._curve.Evaluate((float)this.source1.timeSamples / (float)this.source1.clip.samples) : num);
		}
		this._lastT = num2;
		this.source2.volume = num2;
		this.source1.volume = 1f - num2;
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000B9636 File Offset: 0x000B7836
	public float Get()
	{
		return this._lerp;
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000B963E File Offset: 0x000B783E
	public void Set(float f)
	{
		this._lerp = Mathf.Clamp01(f);
	}

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x0600224A RID: 8778 RVA: 0x000B964C File Offset: 0x000B784C
	// (set) Token: 0x0600224B RID: 8779 RVA: 0x000023F5 File Offset: 0x000005F5
	public float Min
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x0600224C RID: 8780 RVA: 0x000B9653 File Offset: 0x000B7853
	// (set) Token: 0x0600224D RID: 8781 RVA: 0x000023F5 File Offset: 0x000005F5
	public float Max
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x0600224E RID: 8782 RVA: 0x000B9653 File Offset: 0x000B7853
	public float Range
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x0600224F RID: 8783 RVA: 0x000B965A File Offset: 0x000B785A
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x04002BB1 RID: 11185
	[SerializeField]
	private float _lerp;

	// Token: 0x04002BB2 RID: 11186
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002BB3 RID: 11187
	[Space]
	[SerializeField]
	private AudioSource source1;

	// Token: 0x04002BB4 RID: 11188
	[SerializeField]
	private AudioSource source2;

	// Token: 0x04002BB5 RID: 11189
	[Space]
	public bool lerpByClipLength;

	// Token: 0x04002BB6 RID: 11190
	public bool tween;

	// Token: 0x04002BB7 RID: 11191
	public float tweenSpeed = 16f;

	// Token: 0x04002BB8 RID: 11192
	private float _lastT;
}
