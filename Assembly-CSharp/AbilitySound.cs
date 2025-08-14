using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200062D RID: 1581
[Serializable]
public class AbilitySound
{
	// Token: 0x060026DF RID: 9951 RVA: 0x000D0790 File Offset: 0x000CE990
	public bool IsValid()
	{
		return this.sounds != null && this.sounds.Count > 0;
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000D07AC File Offset: 0x000CE9AC
	private void UpdateNextSound()
	{
		AbilitySound.SoundSelectMode soundSelectMode = this.soundSelectMode;
		if (soundSelectMode == AbilitySound.SoundSelectMode.Sequential)
		{
			this.nextSound = (this.nextSound + 1) % this.sounds.Count;
			return;
		}
		if (soundSelectMode != AbilitySound.SoundSelectMode.Random)
		{
			return;
		}
		this.nextSound = Random.Range(0, this.sounds.Count);
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000D07FC File Offset: 0x000CE9FC
	public void Play(AudioSource audioSourceIn)
	{
		AudioSource audioSource = (audioSourceIn != null) ? audioSourceIn : this.audioSource;
		if (this.sounds != null && audioSource != null)
		{
			AudioClip clip = null;
			if (this.sounds.Count > 0)
			{
				if (this.nextSound < 0)
				{
					this.UpdateNextSound();
				}
				clip = this.sounds[this.nextSound];
				this.UpdateNextSound();
			}
			audioSource.clip = clip;
			audioSource.volume = this.volume;
			audioSource.pitch = this.pitch;
			audioSource.loop = this.loop;
			audioSource.Play();
		}
	}

	// Token: 0x0400317F RID: 12671
	public float volume = 1f;

	// Token: 0x04003180 RID: 12672
	public float pitch = 1f;

	// Token: 0x04003181 RID: 12673
	public bool loop;

	// Token: 0x04003182 RID: 12674
	public List<AudioClip> sounds;

	// Token: 0x04003183 RID: 12675
	public AudioSource audioSource;

	// Token: 0x04003184 RID: 12676
	private int nextSound = -1;

	// Token: 0x04003185 RID: 12677
	public AbilitySound.SoundSelectMode soundSelectMode;

	// Token: 0x0200062E RID: 1582
	public enum SoundSelectMode
	{
		// Token: 0x04003187 RID: 12679
		Sequential,
		// Token: 0x04003188 RID: 12680
		Random
	}
}
