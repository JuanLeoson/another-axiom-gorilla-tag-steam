using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x060009B8 RID: 2488 RVA: 0x000352E9 File Offset: 0x000334E9
	private void Reset()
	{
		this._source = base.GetComponent<AudioSource>();
		if (this._source)
		{
			this._source.playOnAwake = false;
		}
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00035310 File Offset: 0x00033510
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00035318 File Offset: 0x00033518
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00035320 File Offset: 0x00033520
	public void Play()
	{
		if (this._loop && this._clips.Length == 1 && this._loopDelay == Vector2.zero)
		{
			this._source.clip = this._clips[0];
			this._source.loop = true;
			this._source.GTPlay();
			return;
		}
		this._source.loop = false;
		if (this._loop)
		{
			base.StartCoroutine(this.DoLoop());
			return;
		}
		this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
		this._source.GTPlay();
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000353CA File Offset: 0x000335CA
	private IEnumerator DoLoop()
	{
		while (base.enabled)
		{
			this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
			this._source.GTPlay();
			while (this._source.isPlaying)
			{
				yield return null;
			}
			float num = Random.Range(this._loopDelay.x, this._loopDelay.y);
			if (num > 0f)
			{
				float waitEndTime = Time.time + num;
				while (Time.time < waitEndTime)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x000353D9 File Offset: 0x000335D9
	public void Stop()
	{
		this._source.GTStop();
		this._source.loop = false;
	}

	// Token: 0x04000B8F RID: 2959
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04000B90 RID: 2960
	[SerializeField]
	private AudioClip[] _clips;

	// Token: 0x04000B91 RID: 2961
	[SerializeField]
	private bool _loop;

	// Token: 0x04000B92 RID: 2962
	[SerializeField]
	private Vector2 _loopDelay;
}
