using System;
using UnityEngine;

// Token: 0x02000A8D RID: 2701
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x06004190 RID: 16784 RVA: 0x0014B025 File Offset: 0x00149225
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x06004191 RID: 16785 RVA: 0x0014B033 File Offset: 0x00149233
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x06004192 RID: 16786 RVA: 0x0014B03C File Offset: 0x0014923C
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].GTPlay();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x06004193 RID: 16787 RVA: 0x0014B0B0 File Offset: 0x001492B0
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x04004D1B RID: 19739
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x04004D1C RID: 19740
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04004D1D RID: 19741
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x04004D1E RID: 19742
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x04004D1F RID: 19743
	private float timer;

	// Token: 0x04004D20 RID: 19744
	private float timerTarget;
}
