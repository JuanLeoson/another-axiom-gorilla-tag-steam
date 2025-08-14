using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000B18 RID: 2840
public class FireworksController : MonoBehaviour
{
	// Token: 0x06004473 RID: 17523 RVA: 0x001560F6 File Offset: 0x001542F6
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x06004474 RID: 17524 RVA: 0x0015611C File Offset: 0x0015431C
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float time = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", time);
		}
	}

	// Token: 0x06004475 RID: 17525 RVA: 0x00156180 File Offset: 0x00154380
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float time = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", time);
			num++;
		}
	}

	// Token: 0x06004476 RID: 17526 RVA: 0x001561C4 File Offset: 0x001543C4
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent ev = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(ev);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x06004477 RID: 17527 RVA: 0x001563F4 File Offset: 0x001545F4
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x06004478 RID: 17528 RVA: 0x00156435 File Offset: 0x00154635
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06004479 RID: 17529 RVA: 0x00156440 File Offset: 0x00154640
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x0600447A RID: 17530 RVA: 0x001564B4 File Offset: 0x001546B4
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient color = new ParticleSystem.MinMaxGradient(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = color;
		colorOverLifetime2.color = color;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x0600447B RID: 17531 RVA: 0x00156544 File Offset: 0x00154744
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x04004E8E RID: 20110
	public Firework[] fireworks;

	// Token: 0x04004E8F RID: 20111
	public AudioClip[] whistles;

	// Token: 0x04004E90 RID: 20112
	public AudioClip[] bursts;

	// Token: 0x04004E91 RID: 20113
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x04004E92 RID: 20114
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x04004E93 RID: 20115
	public float minWhistleDelay = 1f;

	// Token: 0x04004E94 RID: 20116
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x04004E95 RID: 20117
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x04004E96 RID: 20118
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x04004E97 RID: 20119
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x04004E98 RID: 20120
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x04004E99 RID: 20121
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x04004E9A RID: 20122
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x04004E9B RID: 20123
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x04004E9C RID: 20124
	public uint roundLength = 6U;

	// Token: 0x04004E9D RID: 20125
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeEvent _fireworksEvent;

	// Token: 0x02000B19 RID: 2841
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x04004E9E RID: 20126
		public TimeSince timeSince;

		// Token: 0x04004E9F RID: 20127
		public double delay;

		// Token: 0x04004EA0 RID: 20128
		public int explosionIndex;

		// Token: 0x04004EA1 RID: 20129
		public int burstIndex;

		// Token: 0x04004EA2 RID: 20130
		public bool active;

		// Token: 0x04004EA3 RID: 20131
		public Firework firework;
	}
}
