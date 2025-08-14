using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000797 RID: 1943
public class MusicFadeArea : MonoBehaviour
{
	// Token: 0x060030E3 RID: 12515 RVA: 0x000FF91C File Offset: 0x000FDB1C
	private void Awake()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Stop();
			this.sourcesToFadeIn[i].audioSource.volume = 0f;
		}
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x000FF970 File Offset: 0x000FDB70
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeOutMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeInSources());
			}
		}
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x000FF9D8 File Offset: 0x000FDBD8
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeInMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeOutSources());
			}
		}
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x000FFA3D File Offset: 0x000FDC3D
	private IEnumerator FadeInSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Play();
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress < 1f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 1f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.volume = this.sourcesToFadeIn[k].maxVolume;
		}
		yield break;
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x000FFA4C File Offset: 0x000FDC4C
	private IEnumerator FadeOutSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress > 0f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 0f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.Stop();
			this.sourcesToFadeIn[k].audioSource.volume = 0f;
		}
		yield break;
	}

	// Token: 0x04003C9E RID: 15518
	[SerializeField]
	private List<MusicFadeArea.AudioSourceEntry> sourcesToFadeIn = new List<MusicFadeArea.AudioSourceEntry>();

	// Token: 0x04003C9F RID: 15519
	[SerializeField]
	private float fadeDuration = 3f;

	// Token: 0x04003CA0 RID: 15520
	private float fadeProgress;

	// Token: 0x04003CA1 RID: 15521
	private Coroutine fadeCoroutine;

	// Token: 0x02000798 RID: 1944
	[Serializable]
	public struct AudioSourceEntry
	{
		// Token: 0x04003CA2 RID: 15522
		public AudioSource audioSource;

		// Token: 0x04003CA3 RID: 15523
		public float maxVolume;
	}
}
