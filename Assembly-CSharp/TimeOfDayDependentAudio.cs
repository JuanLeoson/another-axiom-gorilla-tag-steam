using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class TimeOfDayDependentAudio : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06000DE9 RID: 3561 RVA: 0x00054BCC File Offset: 0x00052DCC
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00054C1C File Offset: 0x00052E1C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
		base.StopAllCoroutines();
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00054C2B File Offset: 0x00052E2B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00054C41 File Offset: 0x00052E41
	public void SliceUpdate()
	{
		this.isModified = false;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00054C4A File Offset: 0x00052E4A
	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		for (;;)
		{
			if (BetterDayNightManager.instance != null)
			{
				if (this.isModified)
				{
					this.positionMultiplier = this.positionMultiplierSet;
				}
				else
				{
					this.positionMultiplier = 1f;
				}
				if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather || BetterDayNightManager.instance.NextWeather() == this.myWeather)
				{
					if (!this.dependentStuff.activeSelf && (!this.includesAudio || this.dependentStuff != this.timeOfDayDependent))
					{
						this.dependentStuff.SetActive(true);
					}
					if (this.includesAudio)
					{
						if (this.timeOfDayDependent != null)
						{
							if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] == 0f)
							{
								if (this.timeOfDayDependent.activeSelf)
								{
									this.timeOfDayDependent.SetActive(false);
								}
							}
							else if (!this.timeOfDayDependent.activeSelf)
							{
								this.timeOfDayDependent.SetActive(true);
							}
						}
						if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] != this.audioSources[0].volume)
						{
							if (BetterDayNightManager.instance.currentLerp < 0.05f)
							{
								this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
							}
							else
							{
								this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
							}
						}
					}
					if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather)
					{
						if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.NextWeather() == this.myWeather)
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = this.startingEmissionRate;
							}
							if (this.includesAudio && this.myParticleSystem != null)
							{
								this.currentVolume = Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], BetterDayNightManager.instance.currentLerp);
							}
							else if (this.includesAudio)
							{
								if (BetterDayNightManager.instance.currentLerp < 0.05f)
								{
									this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
								}
								else
								{
									this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
								}
							}
						}
						else
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.startingEmissionRate, 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
							if (this.includesAudio)
							{
								this.currentVolume = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
						}
					}
					else
					{
						if (this.myParticleSystem != null)
						{
							this.newRate = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.startingEmissionRate, (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
						if (this.includesAudio)
						{
							this.currentVolume = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
					}
					if (this.myParticleSystem != null)
					{
						this.myEmissionModule = this.myParticleSystem.emission;
						this.myEmissionModule.rateOverTime = this.newRate;
					}
					if (this.includesAudio)
					{
						for (int i = 0; i < this.audioSources.Length; i++)
						{
							MusicSource component = this.audioSources[i].gameObject.GetComponent<MusicSource>();
							if (!(component != null) || !component.VolumeOverridden)
							{
								this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
								this.audioSources[i].enabled = (this.currentVolume != 0f);
							}
						}
					}
				}
				else if (this.dependentStuff.activeSelf)
				{
					this.dependentStuff.SetActive(false);
				}
			}
			yield return new WaitForSeconds(this.stepTime);
		}
		yield break;
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00054C5C File Offset: 0x00052E5C
	public bool BuildValidationCheck()
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (this.audioSources[i] == null)
			{
				Debug.LogError("audio source array contains null references", this);
				return false;
			}
		}
		return true;
	}

	// Token: 0x040015B0 RID: 5552
	public AudioSource[] audioSources;

	// Token: 0x040015B1 RID: 5553
	public float[] volumes;

	// Token: 0x040015B2 RID: 5554
	public float currentVolume;

	// Token: 0x040015B3 RID: 5555
	public float stepTime;

	// Token: 0x040015B4 RID: 5556
	public BetterDayNightManager.WeatherType myWeather;

	// Token: 0x040015B5 RID: 5557
	public GameObject dependentStuff;

	// Token: 0x040015B6 RID: 5558
	public GameObject timeOfDayDependent;

	// Token: 0x040015B7 RID: 5559
	public bool includesAudio;

	// Token: 0x040015B8 RID: 5560
	public ParticleSystem myParticleSystem;

	// Token: 0x040015B9 RID: 5561
	private float startingEmissionRate;

	// Token: 0x040015BA RID: 5562
	private int lastEmission;

	// Token: 0x040015BB RID: 5563
	private int nextEmission;

	// Token: 0x040015BC RID: 5564
	private ParticleSystem.MinMaxCurve newCurve;

	// Token: 0x040015BD RID: 5565
	private ParticleSystem.EmissionModule myEmissionModule;

	// Token: 0x040015BE RID: 5566
	private float newRate;

	// Token: 0x040015BF RID: 5567
	public float positionMultiplierSet;

	// Token: 0x040015C0 RID: 5568
	public float positionMultiplier = 1f;

	// Token: 0x040015C1 RID: 5569
	public bool isModified;
}
