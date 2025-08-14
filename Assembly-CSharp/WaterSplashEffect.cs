using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x06000E25 RID: 3621 RVA: 0x00056C3B File Offset: 0x00054E3B
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00056C48 File Offset: 0x00054E48
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x00056C7C File Offset: 0x00054E7C
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x00056D84 File Offset: 0x00054F84
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x00056E4C File Offset: 0x0005504C
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x00056E78 File Offset: 0x00055078
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x00056EAC File Offset: 0x000550AC
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x00056F64 File Offset: 0x00055164
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x040016DB RID: 5851
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x040016DC RID: 5852
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x040016DD RID: 5853
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x040016DE RID: 5854
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x040016DF RID: 5855
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x040016E0 RID: 5856
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x040016E1 RID: 5857
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x040016E2 RID: 5858
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x040016E3 RID: 5859
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x040016E4 RID: 5860
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x040016E5 RID: 5861
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x040016E6 RID: 5862
	public float lifeTime = 1f;

	// Token: 0x040016E7 RID: 5863
	private float startTime = -1f;

	// Token: 0x040016E8 RID: 5864
	public AudioSource audioSource;

	// Token: 0x040016E9 RID: 5865
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x040016EA RID: 5866
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x040016EB RID: 5867
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x040016EC RID: 5868
	private WaterVolume waterVolume;
}
