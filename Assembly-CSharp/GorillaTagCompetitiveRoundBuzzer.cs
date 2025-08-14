using System;
using UnityEngine;

// Token: 0x02000706 RID: 1798
public class GorillaTagCompetitiveRoundBuzzer : MonoBehaviour
{
	// Token: 0x06002D1C RID: 11548 RVA: 0x000EDC1B File Offset: 0x000EBE1B
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.OnUpdateRemainingTime;
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000EDC3F File Offset: 0x000EBE3F
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.OnUpdateRemainingTime;
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000EDC64 File Offset: 0x000EBE64
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.PlaySFX(this.needMorePlayerClip);
			break;
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.PlaySFX(this.roundStartClip);
			break;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.PlaySFX(this.roundEndClip);
			break;
		}
		this.lastState = newState;
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000EDCBC File Offset: 0x000EBEBC
	private void OnUpdateRemainingTime(float remainingTime)
	{
		int num = Mathf.CeilToInt(remainingTime);
		int num2 = Mathf.CeilToInt(this.lastStateRemainingTime);
		if (num != num2)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.lastState;
			if (gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown)
			{
				if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
				{
					if (num > 0 && num <= this.roundEndCountdownDuration)
					{
						this.PlaySFX(this.roundEndingCountdownClip);
					}
				}
			}
			else if (num > 0)
			{
				this.PlaySFX(this.roundCountdownClip);
			}
		}
		this.lastStateRemainingTime = remainingTime;
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000EDD23 File Offset: 0x000EBF23
	private void PlaySFX(AudioClip clip)
	{
		this.PlaySFX(clip, 1f);
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x000EDD31 File Offset: 0x000EBF31
	private void PlaySFX(AudioClip clip, float volume)
	{
		this.audioSource.PlayOneShot(clip, volume);
	}

	// Token: 0x04003864 RID: 14436
	public AudioSource audioSource;

	// Token: 0x04003865 RID: 14437
	public AudioClip roundCountdownClip;

	// Token: 0x04003866 RID: 14438
	public AudioClip roundStartClip;

	// Token: 0x04003867 RID: 14439
	public AudioClip roundEndingCountdownClip;

	// Token: 0x04003868 RID: 14440
	public int roundEndCountdownDuration = 5;

	// Token: 0x04003869 RID: 14441
	public AudioClip roundEndClip;

	// Token: 0x0400386A RID: 14442
	public AudioClip needMorePlayerClip;

	// Token: 0x0400386B RID: 14443
	private GorillaTagCompetitiveManager.GameState lastState;

	// Token: 0x0400386C RID: 14444
	private float lastStateRemainingTime = -1f;
}
