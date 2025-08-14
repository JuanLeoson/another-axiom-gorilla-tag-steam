using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class CrittersNoiseMaker : CrittersToolThrowable
{
	// Token: 0x06000223 RID: 547 RVA: 0x0000D872 File Offset: 0x0000BA72
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.destroyOnImpact || this.playOnce)
			{
				this.PlaySingleNoise();
				return;
			}
			this.StartPlayingRepeatNoise();
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000D89F File Offset: 0x0000BA9F
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000D8BD File Offset: 0x0000BABD
	protected override void OnPickedUp()
	{
		this.StopPlayRepeatNoise();
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000D8C8 File Offset: 0x0000BAC8
	private void PlaySingleNoise()
	{
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, this.actorId, base.transform.position);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000D945 File Offset: 0x0000BB45
	private void StartPlayingRepeatNoise()
	{
		this.StopPlayRepeatNoise();
		this.repeatPlayNoise = base.StartCoroutine(this.PlayRepeatNoise());
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000D95F File Offset: 0x0000BB5F
	private void StopPlayRepeatNoise()
	{
		if (this.repeatPlayNoise != null)
		{
			base.StopCoroutine(this.repeatPlayNoise);
			this.repeatPlayNoise = null;
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000D97C File Offset: 0x0000BB7C
	private IEnumerator PlayRepeatNoise()
	{
		int num = Mathf.FloorToInt(this.repeatNoiseDuration / this.repeatNoiseRate);
		int num2;
		for (int i = num; i > 0; i = num2 - 1)
		{
			this.PlaySingleNoise();
			yield return new WaitForSeconds(this.repeatNoiseRate);
			num2 = i;
		}
		if (this.destroyAfterPlayingRepeatNoise)
		{
			this.shouldDisable = true;
		}
		yield break;
	}

	// Token: 0x04000282 RID: 642
	[Header("Noise Maker")]
	public int soundSubIndex = 3;

	// Token: 0x04000283 RID: 643
	public bool playOnce = true;

	// Token: 0x04000284 RID: 644
	public float repeatNoiseDuration;

	// Token: 0x04000285 RID: 645
	public float repeatNoiseRate;

	// Token: 0x04000286 RID: 646
	public bool destroyAfterPlayingRepeatNoise = true;

	// Token: 0x04000287 RID: 647
	private Coroutine repeatPlayNoise;
}
