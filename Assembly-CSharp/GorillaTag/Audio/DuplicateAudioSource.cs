using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EE9 RID: 3817
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x06005EB1 RID: 24241 RVA: 0x001DD4DA File Offset: 0x001DB6DA
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x06005EB2 RID: 24242 RVA: 0x001DD4EC File Offset: 0x001DB6EC
		[ContextMenu("Start Duplicating")]
		public void StartDuplicating()
		{
			this._isDuplicating = true;
			this._audioSource.loop = this.TargetAudioSource.loop;
			this._audioSource.clip = this.TargetAudioSource.clip;
			if (this.TargetAudioSource.isPlaying)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x06005EB3 RID: 24243 RVA: 0x001DD544 File Offset: 0x001DB744
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x06005EB4 RID: 24244 RVA: 0x001DD558 File Offset: 0x001DB758
		public void LateUpdate()
		{
			if (this._isDuplicating)
			{
				if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
				{
					this._audioSource.Play();
					return;
				}
				if (!this.TargetAudioSource.isPlaying && this._audioSource.isPlaying)
				{
					this._audioSource.Stop();
				}
			}
		}

		// Token: 0x0400691E RID: 26910
		public AudioSource TargetAudioSource;

		// Token: 0x0400691F RID: 26911
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04006920 RID: 26912
		[SerializeField]
		private bool _isDuplicating;
	}
}
