using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C9C RID: 3228
	public class BuilderScaleAudioRadius : MonoBehaviour
	{
		// Token: 0x06005016 RID: 20502 RVA: 0x0018F1E1 File Offset: 0x0018D3E1
		private void OnEnable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x0018F1FD File Offset: 0x0018D3FD
		private void OnDisable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x0018F20D File Offset: 0x0018D40D
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScaleOnEnable)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06005019 RID: 20505 RVA: 0x0018F249 File Offset: 0x0018D449
		private void PlaySound()
		{
			if (this.autoPlaySoundBank != null)
			{
				this.autoPlaySoundBank.Play();
				return;
			}
			if (this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}

		// Token: 0x0600501A RID: 20506 RVA: 0x0018F284 File Offset: 0x0018D484
		public void SetScale(float inScale)
		{
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.maxDist = this.audioSource.maxDistance;
					this.audioSource.maxDistance *= this.scale;
				}
			}
			else
			{
				this.minDist = this.audioSource.minDistance;
				this.maxDist = this.audioSource.maxDistance;
				this.audioSource.maxDistance *= this.scale;
				this.audioSource.minDistance *= this.scale;
			}
			if (this.autoPlay)
			{
				this.PlaySound();
			}
			this.shouldRevert = true;
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x0018F384 File Offset: 0x0018D584
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.audioSource.maxDistance = this.maxDist;
				}
			}
			else
			{
				this.audioSource.minDistance = this.minDist;
				this.audioSource.maxDistance = this.maxDist;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x0400592A RID: 22826
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScaleOnEnable;

		// Token: 0x0400592B RID: 22827
		[Tooltip("Play sound after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x0400592C RID: 22828
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400592D RID: 22829
		[FormerlySerializedAs("soundBankToPlay")]
		[SerializeField]
		private SoundBankPlayer autoPlaySoundBank;

		// Token: 0x0400592E RID: 22830
		private float minDist;

		// Token: 0x0400592F RID: 22831
		private float maxDist = 1f;

		// Token: 0x04005930 RID: 22832
		private AnimationCurve customCurve;

		// Token: 0x04005931 RID: 22833
		private AnimationCurve scaledCurve = new AnimationCurve();

		// Token: 0x04005932 RID: 22834
		private float scale = 1f;

		// Token: 0x04005933 RID: 22835
		private bool shouldRevert;

		// Token: 0x04005934 RID: 22836
		private bool setScaleNextFrame;

		// Token: 0x04005935 RID: 22837
		private int enableFrame;
	}
}
