using System;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02000EAB RID: 3755
	public class ShakeReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06005DD6 RID: 24022 RVA: 0x001D9AB2 File Offset: 0x001D7CB2
		private float loopSoundTotalDuration
		{
			get
			{
				return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06005DD7 RID: 24023 RVA: 0x001D9AC8 File Offset: 0x001D7CC8
		// (set) Token: 0x06005DD8 RID: 24024 RVA: 0x001D9AD0 File Offset: 0x001D7CD0
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005DD9 RID: 24025 RVA: 0x001D9ADC File Offset: 0x001D7CDC
		protected void Awake()
		{
			this.sampleHistoryPos = new Vector3[256];
			this.sampleHistoryTime = new float[256];
			this.sampleHistoryVel = new Vector3[256];
			if (this.particles != null)
			{
				this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
			}
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x06005DDA RID: 24026 RVA: 0x001D9B5C File Offset: 0x001D7D5C
		protected void OnEnable()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			for (int i = 0; i < 256; i++)
			{
				this.sampleHistoryTime[i] = unscaledTime;
				this.sampleHistoryPos[i] = position;
				this.sampleHistoryVel[i] = Vector3.zero;
			}
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.loop = true;
				this.loopSoundAudioSource.GTPlay();
			}
			this.hasLoopSound = (this.loopSoundAudioSource != null);
			this.hasShakeSound = (this.shakeSoundBankPlayer != null);
			this.hasParticleSystem = (this.particles != null);
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005DDB RID: 24027 RVA: 0x001D9C13 File Offset: 0x001D7E13
		protected void OnDisable()
		{
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.GTStop();
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005DDC RID: 24028 RVA: 0x00100D37 File Offset: 0x000FEF37
		private void HandleApplicationQuitting()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005DDD RID: 24029 RVA: 0x001D9C34 File Offset: 0x001D7E34
		void ITickSystemPost.PostTick()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			int num = (this.currentIndex - 1 + 256) % 256;
			this.currentIndex = (this.currentIndex + 1) % 256;
			this.sampleHistoryTime[this.currentIndex] = unscaledTime;
			float num2 = unscaledTime - this.sampleHistoryTime[num];
			this.sampleHistoryPos[this.currentIndex] = position;
			if (num2 > 0f)
			{
				Vector3 a = position - this.sampleHistoryPos[num];
				this.sampleHistoryVel[this.currentIndex] = a / num2;
			}
			else
			{
				this.sampleHistoryVel[this.currentIndex] = Vector3.zero;
			}
			float sqrMagnitude = (this.sampleHistoryVel[num] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
			this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
			float num3 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
			if (sqrMagnitude >= num3)
			{
				this.lastShakeTime = unscaledTime;
			}
			float num4 = unscaledTime - this.lastShakeTime;
			float time = Mathf.Clamp01(num4 / this.particleDuration);
			if (this.hasParticleSystem)
			{
				this.particles.emission.rateOverTime = this.emissionCurve.Evaluate(time) * this.maxEmissionRate;
			}
			if (this.hasShakeSound && this.lastShakeTime - this.lastShakeSoundTime > this.shakeSoundCooldown)
			{
				this.shakeSoundBankPlayer.Play();
				this.lastShakeSoundTime = unscaledTime;
			}
			if (this.hasLoopSound)
			{
				if (num4 < this.loopSoundFadeInDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num4 / this.loopSoundFadeInDuration));
					return;
				}
				if (num4 < this.loopSoundFadeInDuration + this.loopSoundSustainDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
					return;
				}
				this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num4 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
			}
		}

		// Token: 0x040067BC RID: 26556
		[SerializeField]
		private Transform shakeXform;

		// Token: 0x040067BD RID: 26557
		[SerializeField]
		private float velocityThreshold = 5f;

		// Token: 0x040067BE RID: 26558
		[SerializeField]
		private SoundBankPlayer shakeSoundBankPlayer;

		// Token: 0x040067BF RID: 26559
		[SerializeField]
		private float shakeSoundCooldown = 1f;

		// Token: 0x040067C0 RID: 26560
		[SerializeField]
		private AudioSource loopSoundAudioSource;

		// Token: 0x040067C1 RID: 26561
		[SerializeField]
		private float loopSoundBaseVolume = 1f;

		// Token: 0x040067C2 RID: 26562
		[SerializeField]
		private float loopSoundSustainDuration = 1f;

		// Token: 0x040067C3 RID: 26563
		[SerializeField]
		private float loopSoundFadeInDuration = 1f;

		// Token: 0x040067C4 RID: 26564
		[SerializeField]
		private AnimationCurve loopSoundFadeInCurve;

		// Token: 0x040067C5 RID: 26565
		[SerializeField]
		private float loopSoundFadeOutDuration = 1f;

		// Token: 0x040067C6 RID: 26566
		[SerializeField]
		private AnimationCurve loopSoundFadeOutCurve;

		// Token: 0x040067C7 RID: 26567
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x040067C8 RID: 26568
		[SerializeField]
		private AnimationCurve emissionCurve;

		// Token: 0x040067C9 RID: 26569
		[SerializeField]
		private float particleDuration = 5f;

		// Token: 0x040067CB RID: 26571
		private const int sampleHistorySize = 256;

		// Token: 0x040067CC RID: 26572
		private float[] sampleHistoryTime;

		// Token: 0x040067CD RID: 26573
		private Vector3[] sampleHistoryPos;

		// Token: 0x040067CE RID: 26574
		private Vector3[] sampleHistoryVel;

		// Token: 0x040067CF RID: 26575
		private int currentIndex;

		// Token: 0x040067D0 RID: 26576
		private float lastShakeSoundTime = float.MinValue;

		// Token: 0x040067D1 RID: 26577
		private float lastShakeTime = float.MinValue;

		// Token: 0x040067D2 RID: 26578
		private float maxEmissionRate;

		// Token: 0x040067D3 RID: 26579
		private bool hasLoopSound;

		// Token: 0x040067D4 RID: 26580
		private bool hasShakeSound;

		// Token: 0x040067D5 RID: 26581
		private bool hasParticleSystem;

		// Token: 0x040067D6 RID: 26582
		[DebugReadout]
		private float poopVelocity;
	}
}
