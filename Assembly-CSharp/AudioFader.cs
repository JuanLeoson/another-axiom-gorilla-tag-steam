using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class AudioFader : MonoBehaviour
{
	// Token: 0x060009C7 RID: 2503 RVA: 0x000356C7 File Offset: 0x000338C7
	private void Start()
	{
		this.fadeInSpeed = this.maxVolume / this.fadeInDuration;
		this.fadeOutSpeed = this.maxVolume / this.fadeOutDuration;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000356F0 File Offset: 0x000338F0
	public void FadeIn()
	{
		this.targetVolume = this.maxVolume;
		if (this.fadeInDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeInSpeed;
		}
		else
		{
			this.currentVolume = this.maxVolume;
		}
		this.audioToFade.volume = this.currentVolume;
		if (!this.audioToFade.isPlaying)
		{
			this.audioToFade.GTPlay();
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00035760 File Offset: 0x00033960
	public void FadeOut()
	{
		this.targetVolume = 0f;
		if (this.fadeOutDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeOutSpeed;
		}
		else
		{
			this.currentVolume = 0f;
			if (this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
		if (this.outro != null && this.currentVolume > 0f)
		{
			this.outro.volume = this.currentVolume;
			this.outro.GTPlay();
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x000357F4 File Offset: 0x000339F4
	private void Update()
	{
		this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.targetVolume, this.currentFadeSpeed * Time.deltaTime);
		this.audioToFade.volume = this.currentVolume;
		if (this.currentVolume == this.targetVolume)
		{
			base.enabled = false;
			if (this.currentVolume == 0f && this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
	}

	// Token: 0x04000B99 RID: 2969
	[SerializeField]
	private AudioSource audioToFade;

	// Token: 0x04000B9A RID: 2970
	[SerializeField]
	private AudioSource outro;

	// Token: 0x04000B9B RID: 2971
	[SerializeField]
	private float fadeInDuration = 0.3f;

	// Token: 0x04000B9C RID: 2972
	[SerializeField]
	private float fadeOutDuration = 0.3f;

	// Token: 0x04000B9D RID: 2973
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04000B9E RID: 2974
	private float currentVolume;

	// Token: 0x04000B9F RID: 2975
	private float targetVolume;

	// Token: 0x04000BA0 RID: 2976
	private float currentFadeSpeed;

	// Token: 0x04000BA1 RID: 2977
	private float fadeInSpeed;

	// Token: 0x04000BA2 RID: 2978
	private float fadeOutSpeed;
}
