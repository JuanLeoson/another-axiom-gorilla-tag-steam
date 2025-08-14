using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class SoundEffects : MonoBehaviour
{
	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001DAE RID: 7598 RVA: 0x0009F43A File Offset: 0x0009D63A
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x0009F46D File Offset: 0x0009D66D
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x0009F490 File Offset: 0x0009D690
	public void Stop()
	{
		if (this.source)
		{
			this.source.GTStop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x0009F4BC File Offset: 0x0009D6BC
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float delay = this._rnd.NextFloat(delayMin, delayMax);
		float volume = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(delay, volume);
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x0009F4F0 File Offset: 0x0009D6F0
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.GTStop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.GTPlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.GTPlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x06001DB3 RID: 7603 RVA: 0x0009F604 File Offset: 0x0009D804
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x04002646 RID: 9798
	public AudioSource source;

	// Token: 0x04002647 RID: 9799
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04002648 RID: 9800
	public string seed = "0x1337C0D3";

	// Token: 0x04002649 RID: 9801
	[Space]
	public bool distinct = true;

	// Token: 0x0400264A RID: 9802
	[SerializeField]
	private float _minDelay;

	// Token: 0x0400264B RID: 9803
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x0400264C RID: 9804
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x0400264D RID: 9805
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x0400264E RID: 9806
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
