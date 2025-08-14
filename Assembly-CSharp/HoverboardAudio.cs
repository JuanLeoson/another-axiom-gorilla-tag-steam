using System;
using UnityEngine;

// Token: 0x02000756 RID: 1878
public class HoverboardAudio : MonoBehaviour
{
	// Token: 0x06002F10 RID: 12048 RVA: 0x000F9437 File Offset: 0x000F7637
	private void Start()
	{
		this.Stop();
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x000F943F File Offset: 0x000F763F
	public void PlayTurnSound(float angle)
	{
		if (Time.time > this.turnSoundCooldownUntilTimestamp && angle > this.minAngleDeltaForTurnSound)
		{
			this.turnSoundCooldownUntilTimestamp = Time.time + this.turnSoundCooldownDuration;
			this.turnSounds.Play();
		}
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x000F9474 File Offset: 0x000F7674
	public void UpdateAudioLoop(float speed, float airspeed, float strainLevel, float grindLevel)
	{
		this.motorAnimator.UpdateValue(speed, false);
		this.windRushAnimator.UpdateValue(airspeed, false);
		if (grindLevel > 0f)
		{
			this.grindAnimator.UpdatePitchAndVolume(speed, grindLevel + 0.5f, false);
		}
		else
		{
			this.grindAnimator.UpdatePitchAndVolume(0f, 0f, false);
		}
		strainLevel = Mathf.Clamp01(strainLevel * 10f);
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = Mathf.MoveTowards(this.hum1.volume, this.hum1BaseVolume * strainLevel, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x000F9530 File Offset: 0x000F7730
	public void Stop()
	{
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = 0f;
		this.windRushAnimator.UpdateValue(0f, true);
		this.motorAnimator.UpdateValue(0f, true);
		this.grindAnimator.UpdateValue(0f, true);
	}

	// Token: 0x04003B10 RID: 15120
	[SerializeField]
	private AudioSource hum1;

	// Token: 0x04003B11 RID: 15121
	[SerializeField]
	private SoundBankPlayer turnSounds;

	// Token: 0x04003B12 RID: 15122
	private bool didInitHum1BaseVolume;

	// Token: 0x04003B13 RID: 15123
	private float hum1BaseVolume;

	// Token: 0x04003B14 RID: 15124
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x04003B15 RID: 15125
	[SerializeField]
	private AudioAnimator windRushAnimator;

	// Token: 0x04003B16 RID: 15126
	[SerializeField]
	private AudioAnimator motorAnimator;

	// Token: 0x04003B17 RID: 15127
	[SerializeField]
	private AudioAnimator grindAnimator;

	// Token: 0x04003B18 RID: 15128
	[SerializeField]
	private float turnSoundCooldownDuration;

	// Token: 0x04003B19 RID: 15129
	[SerializeField]
	private float minAngleDeltaForTurnSound;

	// Token: 0x04003B1A RID: 15130
	private float turnSoundCooldownUntilTimestamp;
}
