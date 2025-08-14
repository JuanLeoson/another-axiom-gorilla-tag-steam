using System;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02000BE6 RID: 3046
	[RequireComponent(typeof(SoundBankPlayer))]
	public class SoundBankPlayerCosmetic : MonoBehaviour
	{
		// Token: 0x060049E9 RID: 18921 RVA: 0x0016725C File Offset: 0x0016545C
		private void Awake()
		{
			this.playAudioLoop = false;
		}

		// Token: 0x060049EA RID: 18922 RVA: 0x00167268 File Offset: 0x00165468
		public void Update()
		{
			if (!this.playAudioLoop)
			{
				return;
			}
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null && !this.soundBankPlayer.audioSource.isPlaying)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x060049EB RID: 18923 RVA: 0x001672D0 File Offset: 0x001654D0
		public void PlayAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x060049EC RID: 18924 RVA: 0x0016731C File Offset: 0x0016551C
		public void PlayAudioLoop()
		{
			this.playAudioLoop = true;
		}

		// Token: 0x060049ED RID: 18925 RVA: 0x00167328 File Offset: 0x00165528
		public void PlayAudioNonInterrupting()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				if (this.soundBankPlayer.audioSource.isPlaying)
				{
					return;
				}
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x00167388 File Offset: 0x00165588
		public void PlayAudioWithTunableVolume(bool leftHand, float fingerValue)
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				float volume = Mathf.Clamp01(fingerValue);
				this.soundBankPlayer.audioSource.volume = volume;
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x001673EC File Offset: 0x001655EC
		public void StopAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.audioSource.Stop();
			}
			this.playAudioLoop = false;
		}

		// Token: 0x040052B3 RID: 21171
		[SerializeField]
		private SoundBankPlayer soundBankPlayer;

		// Token: 0x040052B4 RID: 21172
		private bool playAudioLoop;
	}
}
