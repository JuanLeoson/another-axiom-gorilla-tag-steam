using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200079B RID: 1947
public class MusicManager : MonoBehaviour
{
	// Token: 0x060030F5 RID: 12533 RVA: 0x000FFD53 File Offset: 0x000FDF53
	private void Awake()
	{
		if (MusicManager.Instance == null)
		{
			MusicManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x000FFD73 File Offset: 0x000FDF73
	public void RegisterMusicSource(MusicSource musicSource)
	{
		if (!this.activeSources.Contains(musicSource))
		{
			this.activeSources.Add(musicSource);
		}
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x000FFD90 File Offset: 0x000FDF90
	public void UnregisterMusicSource(MusicSource musicSource)
	{
		if (this.activeSources.Contains(musicSource))
		{
			this.activeSources.Remove(musicSource);
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x000FFDB4 File Offset: 0x000FDFB4
	public void FadeOutMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeOutVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.SetVolumeOverride(0f);
		}
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x000FFE28 File Offset: 0x000FE028
	public void FadeInMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeInVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x000FFE98 File Offset: 0x000FE098
	private IEnumerator FadeInVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, musicSource.DefaultVolume, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != musicSource.DefaultVolume)
				{
					complete = false;
				}
			}
			yield return null;
		}
		using (HashSet<MusicSource>.Enumerator enumerator = this.activeSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MusicSource musicSource2 = enumerator.Current;
				musicSource2.UnsetVolumeOverride();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x000FFEAE File Offset: 0x000FE0AE
	private IEnumerator FadeOutVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, 0f, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != 0f)
				{
					complete = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04003CAA RID: 15530
	[OnEnterPlay_SetNull]
	public static volatile MusicManager Instance;

	// Token: 0x04003CAB RID: 15531
	private HashSet<MusicSource> activeSources = new HashSet<MusicSource>();
}
