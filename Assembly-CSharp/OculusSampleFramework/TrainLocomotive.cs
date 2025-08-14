using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D33 RID: 3379
	public class TrainLocomotive : TrainCarBase
	{
		// Token: 0x060053A8 RID: 21416 RVA: 0x0019DCE0 File Offset: 0x0019BEE0
		private void Start()
		{
			this._standardRateOverTimeMultiplier = this._smoke1.emission.rateOverTimeMultiplier;
			this._standardMaxParticles = this._smoke1.main.maxParticles;
			base.Distance = 0f;
			this._speedDiv = 2.5f / (float)this._accelerationSounds.Length;
			this._currentSpeed = this._initialSpeed;
			base.UpdateCarPosition();
			this._smoke1.Stop();
			this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(true));
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x0019DD6F File Offset: 0x0019BF6F
		private void Update()
		{
			this.UpdatePosition();
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x0019DD78 File Offset: 0x0019BF78
		public override void UpdatePosition()
		{
			if (!this._isMoving)
			{
				return;
			}
			if (this._trainTrack != null)
			{
				this.UpdateDistance();
				base.UpdateCarPosition();
				base.RotateCarWheels();
			}
			TrainCarBase[] childCars = this._childCars;
			for (int i = 0; i < childCars.Length; i++)
			{
				childCars[i].UpdatePosition();
			}
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x0019DDCB File Offset: 0x0019BFCB
		public void StartStopStateChanged()
		{
			if (this._startStopTrainCr == null)
			{
				this._startStopTrainCr = base.StartCoroutine(this.StartStopTrain(!this._isMoving));
			}
		}

		// Token: 0x060053AC RID: 21420 RVA: 0x0019DDF0 File Offset: 0x0019BFF0
		private IEnumerator StartStopTrain(bool startTrain)
		{
			float endSpeed = startTrain ? this._initialSpeed : 0f;
			float timePeriodForSpeedChange = 3f;
			if (startTrain)
			{
				this._smoke1.Play();
				this._isMoving = true;
				ParticleSystem.EmissionModule emission = this._smoke1.emission;
				ParticleSystem.MainModule main = this._smoke1.main;
				emission.rateOverTimeMultiplier = this._standardRateOverTimeMultiplier;
				main.maxParticles = this._standardMaxParticles;
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Start);
			}
			else
			{
				timePeriodForSpeedChange = this.PlayEngineSound(TrainLocomotive.EngineSoundState.Stop);
			}
			this._engineAudioSource.loop = false;
			timePeriodForSpeedChange *= 0.9f;
			float startTime = Time.time;
			float endTime = Time.time + timePeriodForSpeedChange;
			float startSpeed = this._currentSpeed;
			while (Time.time < endTime)
			{
				float num = (Time.time - startTime) / timePeriodForSpeedChange;
				this._currentSpeed = startSpeed * (1f - num) + endSpeed * num;
				this.UpdateSmokeEmissionBasedOnSpeed();
				yield return null;
			}
			this._currentSpeed = endSpeed;
			this._startStopTrainCr = null;
			this._isMoving = startTrain;
			if (!this._isMoving)
			{
				this._smoke1.Stop();
			}
			else
			{
				this._engineAudioSource.loop = true;
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
			yield break;
		}

		// Token: 0x060053AD RID: 21421 RVA: 0x0019DE08 File Offset: 0x0019C008
		private float PlayEngineSound(TrainLocomotive.EngineSoundState engineSoundState)
		{
			AudioClip audioClip;
			if (engineSoundState == TrainLocomotive.EngineSoundState.Start)
			{
				audioClip = this._startUpSound;
			}
			else
			{
				AudioClip[] array = (engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed) ? this._accelerationSounds : this._decelerationSounds;
				int num = array.Length;
				int value = (int)Mathf.Round((this._currentSpeed - 0.2f) / this._speedDiv);
				audioClip = array[Mathf.Clamp(value, 0, num - 1)];
			}
			if (this._engineAudioSource.clip == audioClip && this._engineAudioSource.isPlaying && engineSoundState == TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed)
			{
				return 0f;
			}
			this._engineAudioSource.clip = audioClip;
			this._engineAudioSource.timeSamples = 0;
			this._engineAudioSource.Play();
			return audioClip.length;
		}

		// Token: 0x060053AE RID: 21422 RVA: 0x0019DEB4 File Offset: 0x0019C0B4
		private void UpdateDistance()
		{
			float num = this._reverse ? (-this._currentSpeed) : this._currentSpeed;
			base.Distance = (base.Distance + num * Time.deltaTime) % this._trainTrack.TrackLength;
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x0019DEFC File Offset: 0x0019C0FC
		public void DecreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed - this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x0019DF4C File Offset: 0x0019C14C
		public void IncreaseSpeedStateChanged()
		{
			if (this._startStopTrainCr == null && this._isMoving)
			{
				this._currentSpeed = Mathf.Clamp(this._currentSpeed + this._speedDiv, 0.2f, 2.7f);
				this.UpdateSmokeEmissionBasedOnSpeed();
				this.PlayEngineSound(TrainLocomotive.EngineSoundState.AccelerateOrSetProperSpeed);
			}
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x0019DF9C File Offset: 0x0019C19C
		private void UpdateSmokeEmissionBasedOnSpeed()
		{
			this._smoke1.emission.rateOverTimeMultiplier = this.GetCurrentSmokeEmission();
			this._smoke1.main.maxParticles = (int)Mathf.Lerp((float)this._standardMaxParticles, (float)(this._standardMaxParticles * 3), this._currentSpeed / 2.5f);
		}

		// Token: 0x060053B2 RID: 21426 RVA: 0x0019DFF7 File Offset: 0x0019C1F7
		private float GetCurrentSmokeEmission()
		{
			return Mathf.Lerp(this._standardRateOverTimeMultiplier, this._standardRateOverTimeMultiplier * 8f, this._currentSpeed / 2.5f);
		}

		// Token: 0x060053B3 RID: 21427 RVA: 0x0019E01C File Offset: 0x0019C21C
		public void SmokeButtonStateChanged()
		{
			if (this._isMoving)
			{
				this._smokeStackAudioSource.clip = this._smokeSound;
				this._smokeStackAudioSource.timeSamples = 0;
				this._smokeStackAudioSource.Play();
				this._smoke2.time = 0f;
				this._smoke2.Play();
			}
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x0019E074 File Offset: 0x0019C274
		public void WhistleButtonStateChanged()
		{
			if (this._whistleSound != null)
			{
				this._whistleAudioSource.clip = this._whistleSound;
				this._whistleAudioSource.timeSamples = 0;
				this._whistleAudioSource.Play();
			}
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x0019E0AC File Offset: 0x0019C2AC
		public void ReverseButtonStateChanged()
		{
			this._reverse = !this._reverse;
		}

		// Token: 0x04005CF8 RID: 23800
		private const float MIN_SPEED = 0.2f;

		// Token: 0x04005CF9 RID: 23801
		private const float MAX_SPEED = 2.7f;

		// Token: 0x04005CFA RID: 23802
		private const float SMOKE_SPEED_MULTIPLIER = 8f;

		// Token: 0x04005CFB RID: 23803
		private const int MAX_PARTICLES_MULTIPLIER = 3;

		// Token: 0x04005CFC RID: 23804
		[SerializeField]
		[Range(0.2f, 2.7f)]
		protected float _initialSpeed;

		// Token: 0x04005CFD RID: 23805
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04005CFE RID: 23806
		[SerializeField]
		private GameObject _decreaseSpeedButton;

		// Token: 0x04005CFF RID: 23807
		[SerializeField]
		private GameObject _increaseSpeedButton;

		// Token: 0x04005D00 RID: 23808
		[SerializeField]
		private GameObject _smokeButton;

		// Token: 0x04005D01 RID: 23809
		[SerializeField]
		private GameObject _whistleButton;

		// Token: 0x04005D02 RID: 23810
		[SerializeField]
		private GameObject _reverseButton;

		// Token: 0x04005D03 RID: 23811
		[SerializeField]
		private AudioSource _whistleAudioSource;

		// Token: 0x04005D04 RID: 23812
		[SerializeField]
		private AudioClip _whistleSound;

		// Token: 0x04005D05 RID: 23813
		[SerializeField]
		private AudioSource _engineAudioSource;

		// Token: 0x04005D06 RID: 23814
		[SerializeField]
		private AudioClip[] _accelerationSounds;

		// Token: 0x04005D07 RID: 23815
		[SerializeField]
		private AudioClip[] _decelerationSounds;

		// Token: 0x04005D08 RID: 23816
		[SerializeField]
		private AudioClip _startUpSound;

		// Token: 0x04005D09 RID: 23817
		[SerializeField]
		private AudioSource _smokeStackAudioSource;

		// Token: 0x04005D0A RID: 23818
		[SerializeField]
		private AudioClip _smokeSound;

		// Token: 0x04005D0B RID: 23819
		[SerializeField]
		private ParticleSystem _smoke1;

		// Token: 0x04005D0C RID: 23820
		[SerializeField]
		private ParticleSystem _smoke2;

		// Token: 0x04005D0D RID: 23821
		[SerializeField]
		private TrainCarBase[] _childCars;

		// Token: 0x04005D0E RID: 23822
		private bool _isMoving = true;

		// Token: 0x04005D0F RID: 23823
		private bool _reverse;

		// Token: 0x04005D10 RID: 23824
		private float _currentSpeed;

		// Token: 0x04005D11 RID: 23825
		private float _speedDiv;

		// Token: 0x04005D12 RID: 23826
		private float _standardRateOverTimeMultiplier;

		// Token: 0x04005D13 RID: 23827
		private int _standardMaxParticles;

		// Token: 0x04005D14 RID: 23828
		private Coroutine _startStopTrainCr;

		// Token: 0x02000D34 RID: 3380
		private enum EngineSoundState
		{
			// Token: 0x04005D16 RID: 23830
			Start,
			// Token: 0x04005D17 RID: 23831
			AccelerateOrSetProperSpeed,
			// Token: 0x04005D18 RID: 23832
			Stop
		}
	}
}
