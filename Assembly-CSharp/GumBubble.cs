using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000494 RID: 1172
public class GumBubble : LerpComponent
{
	// Token: 0x06001D00 RID: 7424 RVA: 0x0009BF65 File Offset: 0x0009A165
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x0009BF7A File Offset: 0x0009A17A
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x0009BF88 File Offset: 0x0009A188
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x0009BFA8 File Offset: 0x0009A1A8
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxInflate, 1f);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x0009C02C File Offset: 0x0009A22C
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxPop, 1f);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x0009C0B0 File Offset: 0x0009A2B0
	private void Update()
	{
		float t = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, t);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num)
		{
			this.Pop();
		}
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x0009C144 File Offset: 0x0009A344
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError<string>("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x04002564 RID: 9572
	public Transform target;

	// Token: 0x04002565 RID: 9573
	public Vector3 targetScale = Vector3.one;

	// Token: 0x04002566 RID: 9574
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x04002567 RID: 9575
	public AudioSource audioSource;

	// Token: 0x04002568 RID: 9576
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04002569 RID: 9577
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x0400256A RID: 9578
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x0400256B RID: 9579
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x0400256C RID: 9580
	[SerializeField]
	private bool _animating;

	// Token: 0x0400256D RID: 9581
	public UnityEvent onPop;

	// Token: 0x0400256E RID: 9582
	public UnityEvent onInflate;

	// Token: 0x0400256F RID: 9583
	[NonSerialized]
	private bool _done;

	// Token: 0x04002570 RID: 9584
	[NonSerialized]
	private TimeSince _sinceInflate;
}
