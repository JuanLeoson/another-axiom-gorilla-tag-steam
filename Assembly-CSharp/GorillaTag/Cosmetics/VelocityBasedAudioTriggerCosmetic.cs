using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F68 RID: 3944
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		// Token: 0x060061A5 RID: 24997 RVA: 0x001F0934 File Offset: 0x001EEB34
		private void Awake()
		{
			if (this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
			}
			if (this.soundBank != null && this.audioSource != null)
			{
				this.soundBank.audioSource = this.audioSource;
			}
		}

		// Token: 0x060061A6 RID: 24998 RVA: 0x001F0990 File Offset: 0x001EEB90
		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float t = Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude);
			float num = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, t);
			this.audioSource.volume = num;
			if (this.audioSource != null && !this.audioSource.isPlaying && this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (this.soundBank != null && this.soundBank.soundBank != null && !this.soundBank.isPlaying)
			{
				this.soundBank.Play(new float?(num), null);
			}
		}

		// Token: 0x04006DD9 RID: 28121
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04006DDA RID: 28122
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006DDB RID: 28123
		[SerializeField]
		private AudioClip audioClip;

		// Token: 0x04006DDC RID: 28124
		[SerializeField]
		private SoundBankPlayer soundBank;

		// Token: 0x04006DDD RID: 28125
		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold = 0.5f;

		// Token: 0x04006DDE RID: 28126
		[SerializeField]
		private float maxVelocity = 2f;

		// Token: 0x04006DDF RID: 28127
		[SerializeField]
		private float minOutputVolume;

		// Token: 0x04006DE0 RID: 28128
		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
