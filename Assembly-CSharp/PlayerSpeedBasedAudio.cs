using System;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class PlayerSpeedBasedAudio : MonoBehaviour
{
	// Token: 0x060009B5 RID: 2485 RVA: 0x0003521F File Offset: 0x0003341F
	private void Start()
	{
		this.fadeRate = 1f / this.fadeTime;
		this.baseVolume = this.audioSource.volume;
		this.localPlayerVelocityEstimator.TryResolve<GorillaVelocityEstimator>(out this.velocityEstimator);
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00035258 File Offset: 0x00033458
	private void Update()
	{
		this.currentFadeLevel = Mathf.MoveTowards(this.currentFadeLevel, Mathf.InverseLerp(this.minVolumeSpeed, this.fullVolumeSpeed, this.velocityEstimator.linearVelocity.magnitude), this.fadeRate * Time.deltaTime);
		if (this.baseVolume == 0f || this.currentFadeLevel == 0f)
		{
			this.audioSource.volume = 0.0001f;
			return;
		}
		this.audioSource.volume = this.baseVolume * this.currentFadeLevel;
	}

	// Token: 0x04000B86 RID: 2950
	[SerializeField]
	private float minVolumeSpeed;

	// Token: 0x04000B87 RID: 2951
	[SerializeField]
	private float fullVolumeSpeed;

	// Token: 0x04000B88 RID: 2952
	[SerializeField]
	private float fadeTime;

	// Token: 0x04000B89 RID: 2953
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000B8A RID: 2954
	[SerializeField]
	private XSceneRef localPlayerVelocityEstimator;

	// Token: 0x04000B8B RID: 2955
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000B8C RID: 2956
	private float baseVolume;

	// Token: 0x04000B8D RID: 2957
	private float fadeRate;

	// Token: 0x04000B8E RID: 2958
	private float currentFadeLevel;
}
