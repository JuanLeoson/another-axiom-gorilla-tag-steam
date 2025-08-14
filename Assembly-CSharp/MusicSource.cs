using System;
using UnityEngine;

// Token: 0x0200079E RID: 1950
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06003109 RID: 12553 RVA: 0x00100138 File Offset: 0x000FE338
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x0600310A RID: 12554 RVA: 0x00100140 File Offset: 0x000FE340
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x0600310B RID: 12555 RVA: 0x00100148 File Offset: 0x000FE348
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x00100155 File Offset: 0x000FE355
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.setDefaultVolumeFromAudioSourceOnAwake)
		{
			this.defaultVolume = this.audioSource.volume;
		}
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x0010018A File Offset: 0x000FE38A
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x001001A8 File Offset: 0x000FE3A8
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x001001C6 File Offset: 0x000FE3C6
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x001001EA File Offset: 0x000FE3EA
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = null;
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x04003CB6 RID: 15542
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04003CB7 RID: 15543
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04003CB8 RID: 15544
	private AudioSource audioSource;

	// Token: 0x04003CB9 RID: 15545
	private float? volumeOverride;
}
