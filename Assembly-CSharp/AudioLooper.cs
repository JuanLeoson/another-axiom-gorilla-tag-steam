using System;
using UnityEngine;

// Token: 0x02000A92 RID: 2706
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x060041AC RID: 16812 RVA: 0x0014B2D7 File Offset: 0x001494D7
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060041AD RID: 16813 RVA: 0x0014B2E8 File Offset: 0x001494E8
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			if (this.audioSource.clip == this.loopClip && this.interjectionClips.Length != 0 && Random.value < this.interjectionLikelyhood)
			{
				this.audioSource.clip = this.interjectionClips[Random.Range(0, this.interjectionClips.Length)];
			}
			else
			{
				this.audioSource.clip = this.loopClip;
			}
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x04004D23 RID: 19747
	private AudioSource audioSource;

	// Token: 0x04004D24 RID: 19748
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04004D25 RID: 19749
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x04004D26 RID: 19750
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
