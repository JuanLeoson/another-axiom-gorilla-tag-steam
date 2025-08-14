using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000E85 RID: 3717
	public class VolcanoEffects : BaseGuidedRefTargetMono
	{
		// Token: 0x06005D1A RID: 23834 RVA: 0x001D6E30 File Offset: 0x001D5030
		protected override void Awake()
		{
			base.Awake();
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
			{
				this.LogNullsFoundInArray("lavaSpewParticleSystems");
			}
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
			{
				this.LogNullsFoundInArray("smokeParticleSystems");
			}
			this.hasVolcanoAudioSrc = (this.volcanoAudioSource != null);
			this.hasForestSpeakerAudioSrc = (this.forestSpeakerAudioSrc != null);
			this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
			this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
			this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
				this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
				this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					ParticleSystem.Burst burst = emission.GetBurst(j);
					this.lavaSpewDefaultEmitBursts[i][j] = burst;
					this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
					this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
				}
				this.lavaSpewEmissionModules[i] = emission;
			}
			this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
			this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
			this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
			for (int k = 0; k < this.smokeParticleSystems.Length; k++)
			{
				this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
				this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
				this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
			}
			this.InitState(this.drainedStateFX);
			this.InitState(this.eruptingStateFX);
			this.InitState(this.risingStateFX);
			this.InitState(this.fullStateFX);
			this.InitState(this.drainingStateFX);
			this.currentStateFX = this.drainedStateFX;
			this.UpdateDrainedState(0f);
		}

		// Token: 0x06005D1B RID: 23835 RVA: 0x001D70C0 File Offset: 0x001D52C0
		public void OnVolcanoBellyEmpty()
		{
			if (!this.hasForestSpeakerAudioSrc)
			{
				return;
			}
			if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
			{
				return;
			}
			this.forestSpeakerAudioSrc.gameObject.SetActive(true);
			this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied, 1f);
		}

		// Token: 0x06005D1C RID: 23836 RVA: 0x001D7118 File Offset: 0x001D5318
		public void OnStoneAccepted(double activationProgress)
		{
			if (!this.hasVolcanoAudioSrc)
			{
				return;
			}
			this.volcanoAudioSource.gameObject.SetActive(true);
			if (activationProgress > 1.0)
			{
				this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone, 1f);
				return;
			}
			this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone, 1f);
		}

		// Token: 0x06005D1D RID: 23837 RVA: 0x001D7178 File Offset: 0x001D5378
		private void InitState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundExists = (fx.startSound != null);
			fx.endSoundExists = (fx.endSound != null);
			fx.loop1Exists = (fx.loop1AudioSrc != null);
			fx.loop2Exists = (fx.loop2AudioSrc != null);
			if (fx.loop1Exists)
			{
				fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
				fx.loop1AudioSrc.volume = 0f;
			}
			if (fx.loop2Exists)
			{
				fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
				fx.loop2AudioSrc.volume = 0f;
			}
		}

		// Token: 0x06005D1E RID: 23838 RVA: 0x001D7220 File Offset: 0x001D5420
		private void SetLavaAudioEnabled(bool toEnable)
		{
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x06005D1F RID: 23839 RVA: 0x001D7250 File Offset: 0x001D5450
		private void SetLavaAudioEnabled(bool toEnable, float volume)
		{
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = volume;
				audioSource.gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x001D7288 File Offset: 0x001D5488
		private void ResetState()
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			this.currentStateFX.startSoundPlayed = false;
			this.currentStateFX.endSoundPlayed = false;
			if (this.currentStateFX.startSoundExists)
			{
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.endSoundExists)
			{
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005D21 RID: 23841 RVA: 0x001D7344 File Offset: 0x001D5544
		private void UpdateState(float time, float timeRemaining, float progress)
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && time >= this.currentStateFX.startSoundDelay)
			{
				this.currentStateFX.startSoundPlayed = true;
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
			}
			if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && timeRemaining <= this.currentStateFX.endSound.length + this.currentStateFX.endSoundPadTime)
			{
				this.currentStateFX.endSoundPlayed = true;
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
				if (!this.currentStateFX.loop1AudioSrc.isPlaying)
				{
					this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop1AudioSrc.GTPlay();
				}
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
				if (!this.currentStateFX.loop2AudioSrc.isPlaying)
				{
					this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop2AudioSrc.GTPlay();
				}
			}
			for (int i = 0; i < this.smokeMainModules.Length; i++)
			{
				this.smokeMainModules[i].startColor = this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
				this.smokeEmissionModules[i].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
			}
			this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
			if (this.applyShaderGlobals)
			{
				Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
				Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
			}
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x001D7619 File Offset: 0x001D5819
		public void SetDrainedState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false);
			this.currentStateFX = this.drainedStateFX;
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x001D7634 File Offset: 0x001D5834
		public void UpdateDrainedState(float time)
		{
			this.ResetState();
			this.UpdateState(time, float.MaxValue, float.MinValue);
		}

		// Token: 0x06005D24 RID: 23844 RVA: 0x001D764D File Offset: 0x001D584D
		public void SetEruptingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false, 0f);
			this.currentStateFX = this.eruptingStateFX;
		}

		// Token: 0x06005D25 RID: 23845 RVA: 0x001D766D File Offset: 0x001D586D
		public void UpdateEruptingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x001D7678 File Offset: 0x001D5878
		public void SetRisingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 0f);
			this.currentStateFX = this.risingStateFX;
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x001D7698 File Offset: 0x001D5898
		public void UpdateRisingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
			}
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x001D76E0 File Offset: 0x001D58E0
		public void SetFullState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.fullStateFX;
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x001D766D File Offset: 0x001D586D
		public void UpdateFullState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x001D7700 File Offset: 0x001D5900
		public void SetDrainingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.drainingStateFX;
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x001D7720 File Offset: 0x001D5920
		public void UpdateDrainingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(1f, 0f, progress);
			}
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x001D7764 File Offset: 0x001D5964
		private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
		{
			for (int i = 0; i < emissionModules.Length; i++)
			{
				emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
				int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
				for (int j = 0; j < num; j++)
				{
					adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
				}
				emissionModules[i].SetBursts(adjustedEmitBursts[i]);
			}
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001D77E4 File Offset: 0x001D59E4
		private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
		{
			List<T> list = new List<T>(array.Length);
			foreach (T t in array)
			{
				if (t != null)
				{
					list.Add(t);
				}
			}
			int num = array.Length;
			array = list.ToArray();
			return num != array.Length;
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001D783E File Offset: 0x001D5A3E
		private void LogNullsFoundInArray(string nameOfArray)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Null reference found in ",
				nameOfArray,
				" array of component: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
		}

		// Token: 0x04006715 RID: 26389
		[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
		[SerializeField]
		private bool applyShaderGlobals = true;

		// Token: 0x04006716 RID: 26390
		[Tooltip("Game trigger notification sounds will play through this.")]
		[SerializeField]
		private AudioSource forestSpeakerAudioSrc;

		// Token: 0x04006717 RID: 26391
		[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
		[SerializeField]
		private AudioClip warnVolcanoBellyEmptied;

		// Token: 0x04006718 RID: 26392
		[Tooltip("Accept stone sounds will play through here.")]
		[SerializeField]
		private AudioSource volcanoAudioSource;

		// Token: 0x04006719 RID: 26393
		[Tooltip("volcano ate rock but needs more.")]
		[SerializeField]
		private AudioClip volcanoAcceptStone;

		// Token: 0x0400671A RID: 26394
		[Tooltip("volcano ate last needed rock.")]
		[SerializeField]
		private AudioClip volcanoAcceptLastStone;

		// Token: 0x0400671B RID: 26395
		[Tooltip("This will be faded in while lava is rising.")]
		[SerializeField]
		private AudioSource[] lavaSurfaceAudioSrcs;

		// Token: 0x0400671C RID: 26396
		[Tooltip("Emission will be adjusted for these particles during eruption.")]
		[SerializeField]
		private ParticleSystem[] lavaSpewParticleSystems;

		// Token: 0x0400671D RID: 26397
		[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
		[SerializeField]
		private ParticleSystem[] smokeParticleSystems;

		// Token: 0x0400671E RID: 26398
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainedStateFX;

		// Token: 0x0400671F RID: 26399
		[SerializeField]
		private VolcanoEffects.LavaStateFX eruptingStateFX;

		// Token: 0x04006720 RID: 26400
		[SerializeField]
		private VolcanoEffects.LavaStateFX risingStateFX;

		// Token: 0x04006721 RID: 26401
		[SerializeField]
		private VolcanoEffects.LavaStateFX fullStateFX;

		// Token: 0x04006722 RID: 26402
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainingStateFX;

		// Token: 0x04006723 RID: 26403
		private VolcanoEffects.LavaStateFX currentStateFX;

		// Token: 0x04006724 RID: 26404
		private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

		// Token: 0x04006725 RID: 26405
		private float[] lavaSpewEmissionDefaultRateMultipliers;

		// Token: 0x04006726 RID: 26406
		private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

		// Token: 0x04006727 RID: 26407
		private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

		// Token: 0x04006728 RID: 26408
		private ParticleSystem.MainModule[] smokeMainModules;

		// Token: 0x04006729 RID: 26409
		private ParticleSystem.EmissionModule[] smokeEmissionModules;

		// Token: 0x0400672A RID: 26410
		private float[] smokeEmissionDefaultRateMultipliers;

		// Token: 0x0400672B RID: 26411
		private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

		// Token: 0x0400672C RID: 26412
		private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

		// Token: 0x0400672D RID: 26413
		private float timeVolcanoBellyWasLastEmpty;

		// Token: 0x0400672E RID: 26414
		private bool hasVolcanoAudioSrc;

		// Token: 0x0400672F RID: 26415
		private bool hasForestSpeakerAudioSrc;

		// Token: 0x02000E86 RID: 3718
		[Serializable]
		public class LavaStateFX
		{
			// Token: 0x04006730 RID: 26416
			public AudioClip startSound;

			// Token: 0x04006731 RID: 26417
			public AudioSource startSoundAudioSrc;

			// Token: 0x04006732 RID: 26418
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float startSoundVol = 1f;

			// Token: 0x04006733 RID: 26419
			[FormerlySerializedAs("startSoundPad")]
			public float startSoundDelay;

			// Token: 0x04006734 RID: 26420
			public AudioClip endSound;

			// Token: 0x04006735 RID: 26421
			public AudioSource endSoundAudioSrc;

			// Token: 0x04006736 RID: 26422
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float endSoundVol = 1f;

			// Token: 0x04006737 RID: 26423
			[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
			public float endSoundPadTime;

			// Token: 0x04006738 RID: 26424
			public AudioSource loop1AudioSrc;

			// Token: 0x04006739 RID: 26425
			public AnimationCurve loop1VolAnim;

			// Token: 0x0400673A RID: 26426
			public AudioSource loop2AudioSrc;

			// Token: 0x0400673B RID: 26427
			public AnimationCurve loop2VolAnim;

			// Token: 0x0400673C RID: 26428
			public AnimationCurve lavaSpewEmissionAnim;

			// Token: 0x0400673D RID: 26429
			public AnimationCurve smokeEmissionAnim;

			// Token: 0x0400673E RID: 26430
			public Gradient smokeStartColorAnim;

			// Token: 0x0400673F RID: 26431
			public Gradient lavaLightColor;

			// Token: 0x04006740 RID: 26432
			public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

			// Token: 0x04006741 RID: 26433
			public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

			// Token: 0x04006742 RID: 26434
			[NonSerialized]
			public bool startSoundExists;

			// Token: 0x04006743 RID: 26435
			[NonSerialized]
			public bool startSoundPlayed;

			// Token: 0x04006744 RID: 26436
			[NonSerialized]
			public bool endSoundExists;

			// Token: 0x04006745 RID: 26437
			[NonSerialized]
			public bool endSoundPlayed;

			// Token: 0x04006746 RID: 26438
			[NonSerialized]
			public bool loop1Exists;

			// Token: 0x04006747 RID: 26439
			[NonSerialized]
			public float loop1DefaultVolume;

			// Token: 0x04006748 RID: 26440
			[NonSerialized]
			public bool loop2Exists;

			// Token: 0x04006749 RID: 26441
			[NonSerialized]
			public float loop2DefaultVolume;
		}
	}
}
