using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200096F RID: 2415
public class LightningManager : MonoBehaviour
{
	// Token: 0x06003B0C RID: 15116 RVA: 0x00131DD2 File Offset: 0x0012FFD2
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x06003B0D RID: 15117 RVA: 0x00131E08 File Offset: 0x00130008
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x06003B0E RID: 15118 RVA: 0x00131E38 File Offset: 0x00130038
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime d = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - d).TotalSeconds;
		seed = d.Ticks;
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x00131E98 File Offset: 0x00130098
	private void InitializeRng()
	{
		long seed;
		float num;
		this.GetHourStart(out seed, out num);
		this.currentHourlySeed = seed;
		this.rng = new SRand(seed);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num2 = num;
		float num3 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num3 < 3600f)
		{
			float num4 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num3 += num4;
			num2 += num4;
			if (this.nextLightningTimestampIndex == -1 && num2 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num2);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num + 3605f;
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x00131F5C File Offset: 0x0013015C
	internal void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.GTPlay();
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x00131FB9 File Offset: 0x001301B9
	private IEnumerator LightningEffectRunner()
	{
		for (;;)
		{
			if (this.lightningTimestampsRealtime.Count <= this.nextLightningTimestampIndex)
			{
				this.InitializeRng();
			}
			if (this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
			{
				yield return new WaitForSecondsRealtime(this.lightningTimestampsRealtime[this.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				float num = this.lightningTimestampsRealtime[this.nextLightningTimestampIndex];
				this.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num < 1f && this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
				{
					this.DoLightningStrike();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x040048A0 RID: 18592
	public int lightMapIndex;

	// Token: 0x040048A1 RID: 18593
	public float minTimeBetweenFlashes;

	// Token: 0x040048A2 RID: 18594
	public float maxTimeBetweenFlashes;

	// Token: 0x040048A3 RID: 18595
	public float flashFadeInDuration;

	// Token: 0x040048A4 RID: 18596
	public float flashHoldDuration;

	// Token: 0x040048A5 RID: 18597
	public float flashFadeOutDuration;

	// Token: 0x040048A6 RID: 18598
	private AudioSource lightningAudio;

	// Token: 0x040048A7 RID: 18599
	private SRand rng;

	// Token: 0x040048A8 RID: 18600
	private long currentHourlySeed;

	// Token: 0x040048A9 RID: 18601
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x040048AA RID: 18602
	private int nextLightningTimestampIndex;

	// Token: 0x040048AB RID: 18603
	public AudioClip regularLightning;

	// Token: 0x040048AC RID: 18604
	public AudioClip muffledLightning;

	// Token: 0x040048AD RID: 18605
	private Coroutine lightningRunner;
}
