using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class MarkOneMitts : HandTapBehaviour, ITickSystemTick
{
	// Token: 0x06000415 RID: 1045 RVA: 0x00017FDC File Offset: 0x000161DC
	private void Awake()
	{
		this.bursts = new ParticleSystem.Burst[2];
		this.burst.emission.GetBursts(this.bursts);
		this.burstTransform = this.burst.transform;
		this.flameTransform = this.flame.transform;
		this.rig = base.GetComponentInParent<VRRig>();
		this.vibrateController = (this.vibrateController && this.rig.isOfflineVRRig);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00018059 File Offset: 0x00016259
	private void OnEnable()
	{
		if (this.enableInterference)
		{
			TickSystem<object>.AddTickCallback(this);
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00018069 File Offset: 0x00016269
	private void OnDisable()
	{
		if (this.enableInterference)
		{
			TickSystem<object>.RemoveTickCallback(this);
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001807C File Offset: 0x0001627C
	private void SetProximitySparks(float scale, float speed, ParticleSystem.MinMaxCurve xy, float vibrationStrength)
	{
		if (!this.flame.isPlaying)
		{
			this.flame.Play();
			this.thermalSource.enabled = true;
		}
		this.flameTransform.localScale = this.flameScale * scale * Vector3.one;
		this.flame.main.startSpeed = speed;
		ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.flame.forceOverLifetime;
		forceOverLifetime.x = xy;
		forceOverLifetime.y = xy;
		this.thermalSource.celsius = this.heatMultiplier * scale;
		if (this.vibrateController && vibrationStrength > 0f)
		{
			GorillaTagger.Instance.StartVibration(this.isLeftHand, this.vibrationStrengthMult * vibrationStrength * scale, 0.1f);
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00018143 File Offset: 0x00016343
	private void StopProximitySparks()
	{
		if (this.flame.isPlaying && this.timer == 0f)
		{
			this.flame.Stop();
			this.thermalSource.enabled = false;
		}
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00018178 File Offset: 0x00016378
	private void StartProximityAudio()
	{
		if (!this.interferenceAudioSource.isPlaying)
		{
			if (!this.interferenceStartStopAudioSource.isPlaying)
			{
				this.interferenceStartStopAudioSource.clip = this.interferenceAudioStartClip;
				this.interferenceStartStopAudioSource.volume = this.interferenceAudioStartVolume;
				this.interferenceStartStopAudioSource.Play();
			}
			this.interferenceAudioSource.Play();
		}
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x000181D8 File Offset: 0x000163D8
	private void StopProximityAudio()
	{
		if (this.interferenceAudioSource.isPlaying)
		{
			if (!this.interferenceStartStopAudioSource.isPlaying)
			{
				this.interferenceStartStopAudioSource.clip = this.interferenceAudioStopClip;
				this.interferenceStartStopAudioSource.volume = this.interferenceAudioStopVolume;
				this.interferenceStartStopAudioSource.Play();
			}
			this.interferenceAudioSource.Stop();
			this.interferenceAudioSource.pitch = 0f;
			this.interferenceAudioSource.volume = 0f;
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00018258 File Offset: 0x00016458
	private void RunTimer()
	{
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			this.timer = 0f;
			if (!this.enableInterference)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			this.StopProximitySparks();
			return;
		}
		float num = this.lastTapStrength * this.timer;
		this.flameTransform.localScale = this.flameScale * num * Vector3.one;
		this.thermalSource.celsius = this.heatMultiplier * num;
		if (this.vibrateController)
		{
			GorillaTagger.Instance.StartVibration(this.isLeftHand, this.vibrationStrengthMult * num, 0.1f);
		}
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00018308 File Offset: 0x00016508
	private void RunInterference()
	{
		if (this.timer != 0f || this.otherMitts.timer != 0f)
		{
			this.StopProximitySparks();
			this.otherMitts.StopProximitySparks();
			this.StopProximityAudio();
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 a = (this.otherMitts.transform.position - position) / this.rig.scaleFactor;
		float magnitude = a.magnitude;
		Vector3 forward = base.transform.forward;
		float a2 = -Vector3.Dot(this.otherMitts.transform.forward, forward);
		float num = Vector3.Dot(a / magnitude, forward) * Mathf.Max(a2, 0f);
		float num2 = (num > 0f) ? (Mathf.Sqrt(num) - magnitude * 1.5f - 0.5f) : 0f;
		if (num2 > 0.1f)
		{
			float num3 = Mathf.Pow(num, 6f) / magnitude;
			ParticleSystem.MinMaxCurve xy = new ParticleSystem.MinMaxCurve(-num3, num3);
			float speed = this.flameSpeed * magnitude;
			this.SetProximitySparks(num2, speed, xy, 0.5f);
			this.otherMitts.SetProximitySparks(num2, speed, xy, 0.5f);
			this.interferenceAudioSource.transform.position = position + a / 2f;
			float t = 1f - Mathf.Exp(-this.interferenceAudioReactionSpeed * Time.deltaTime);
			this.interferenceAudioSource.pitch = Mathf.Lerp(this.interferenceAudioSource.pitch, num2 * this.interferenceAudioPitchMult + this.interferenceAudioPitchMin, t);
			this.interferenceAudioSource.volume = Mathf.Lerp(this.interferenceAudioSource.volume, num2 * this.interferenceAudioVolumeMult + this.interferenceAudioVolumeMin, t);
			this.StartProximityAudio();
			return;
		}
		this.StopProximitySparks();
		this.otherMitts.StopProximitySparks();
		this.StopProximityAudio();
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x0600041E RID: 1054 RVA: 0x000184FB File Offset: 0x000166FB
	// (set) Token: 0x0600041F RID: 1055 RVA: 0x00018503 File Offset: 0x00016703
	public bool TickRunning { get; set; }

	// Token: 0x06000420 RID: 1056 RVA: 0x0001850C File Offset: 0x0001670C
	public void Tick()
	{
		if (this.timer > 0f)
		{
			this.RunTimer();
		}
		if (this.isLeftHand && this.enableInterference)
		{
			this.RunInterference();
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00018538 File Offset: 0x00016738
	internal override void OnTap(HandEffectContext handContext)
	{
		if (this.handSpeedToEffectStrength.Evaluate(handContext.Speed) >= this.minEffectStrength)
		{
			if (!this.enableInterference)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.lastTapStrength = this.handSpeedToEffectStrength.Evaluate(handContext.Speed);
			this.timer = this.flameTime;
			this.bursts[0].count = this.lastTapStrength * 10f;
			this.bursts[1].count = this.lastTapStrength * 5f;
			this.burst.emission.SetBursts(this.bursts);
			this.burstTransform.localScale = this.lastTapStrength * Vector3.one;
			this.burst.Play();
			this.SetProximitySparks(this.lastTapStrength * this.flameScale * this.flameTime, this.flameSpeed, this.emptyParticleCurve, 0f);
			float value = this.handSpeedToEffectStrength.keys[this.handSpeedToEffectStrength.keys.Length - 1].value;
			handContext.soundPitch = Mathf.Clamp(value / this.lastTapStrength, 1f, 3f);
			return;
		}
		handContext.soundFX = null;
	}

	// Token: 0x0400048C RID: 1164
	[SerializeField]
	private ParticleSystem burst;

	// Token: 0x0400048D RID: 1165
	[SerializeField]
	private ParticleSystem flame;

	// Token: 0x0400048E RID: 1166
	[SerializeField]
	private ThermalSourceVolume thermalSource;

	// Token: 0x0400048F RID: 1167
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x04000490 RID: 1168
	[Space]
	[SerializeField]
	private AnimationCurve handSpeedToEffectStrength;

	// Token: 0x04000491 RID: 1169
	[SerializeField]
	private float minEffectStrength = 0.5f;

	// Token: 0x04000492 RID: 1170
	[SerializeField]
	private float flameScale = 3f;

	// Token: 0x04000493 RID: 1171
	[SerializeField]
	private float flameTime = 0.5f;

	// Token: 0x04000494 RID: 1172
	[SerializeField]
	private float flameSpeed = 5f;

	// Token: 0x04000495 RID: 1173
	[SerializeField]
	private float heatMultiplier = 100f;

	// Token: 0x04000496 RID: 1174
	[Space]
	[SerializeField]
	private bool vibrateController;

	// Token: 0x04000497 RID: 1175
	[SerializeField]
	private float vibrationStrengthMult = 1f;

	// Token: 0x04000498 RID: 1176
	[Space]
	[Tooltip("Cause the mitts shoot fire when pointed towards each other.")]
	[SerializeField]
	private bool enableInterference;

	// Token: 0x04000499 RID: 1177
	[SerializeField]
	private MarkOneMitts otherMitts;

	// Token: 0x0400049A RID: 1178
	[SerializeField]
	private AudioSource interferenceAudioSource;

	// Token: 0x0400049B RID: 1179
	[SerializeField]
	private float interferenceAudioPitchMin = 0.8f;

	// Token: 0x0400049C RID: 1180
	[SerializeField]
	private float interferenceAudioPitchMult = 1f;

	// Token: 0x0400049D RID: 1181
	[SerializeField]
	private float interferenceAudioVolumeMin = 0.2f;

	// Token: 0x0400049E RID: 1182
	[SerializeField]
	private float interferenceAudioVolumeMult = 1f;

	// Token: 0x0400049F RID: 1183
	[SerializeField]
	private float interferenceAudioReactionSpeed = 0.2f;

	// Token: 0x040004A0 RID: 1184
	[Space]
	[SerializeField]
	private AudioSource interferenceStartStopAudioSource;

	// Token: 0x040004A1 RID: 1185
	[SerializeField]
	private AudioClip interferenceAudioStartClip;

	// Token: 0x040004A2 RID: 1186
	[SerializeField]
	private float interferenceAudioStartVolume = 0.5f;

	// Token: 0x040004A3 RID: 1187
	[SerializeField]
	private AudioClip interferenceAudioStopClip;

	// Token: 0x040004A4 RID: 1188
	[SerializeField]
	private float interferenceAudioStopVolume = 0.5f;

	// Token: 0x040004A5 RID: 1189
	private float lastTapStrength;

	// Token: 0x040004A6 RID: 1190
	private float timer;

	// Token: 0x040004A7 RID: 1191
	private ParticleSystem.Burst[] bursts;

	// Token: 0x040004A8 RID: 1192
	private Transform burstTransform;

	// Token: 0x040004A9 RID: 1193
	private Transform flameTransform;

	// Token: 0x040004AA RID: 1194
	private VRRig rig;

	// Token: 0x040004AB RID: 1195
	private ParticleSystem.MinMaxCurve emptyParticleCurve = new ParticleSystem.MinMaxCurve(0f);
}
