using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F5C RID: 3932
	[RequireComponent(typeof(TransferrableObject))]
	public class SmoothScaleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06006164 RID: 24932 RVA: 0x001EF75D File Offset: 0x001ED95D
		private void Awake()
		{
			this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
			this.initialScale = this.objectPrefab.transform.localScale;
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x001EF781 File Offset: 0x001ED981
		private void OnEnable()
		{
			this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x001EF78C File Offset: 0x001ED98C
		private void Update()
		{
			if (this.transferrableObject && !this.transferrableObject.InHand())
			{
				if (this.audioSource && this.audioSource.isPlaying)
				{
					this.audioSource.GTStop();
				}
				return;
			}
			switch (this.currentState)
			{
			case SmoothScaleModifierCosmetic.State.None:
				if (this.audioSource && this.normalSizeAudio && !this.audioSource.isPlaying)
				{
					this.audioSource.clip = this.normalSizeAudio;
					this.audioSource.volume = this.normalSizeAudioVolume;
					this.audioSource.GTPlay();
				}
				break;
			case SmoothScaleModifierCosmetic.State.Reset:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.initialScale;
					this.UpdateState(SmoothScaleModifierCosmetic.State.None);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaling:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.targetScale;
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaled:
				if (this.audioSource && this.scaledAudio && !this.audioSource.isPlaying)
				{
					this.audioSource.clip = this.scaledAudio;
					this.audioSource.volume = this.scaleAudioVolume;
					this.audioSource.GTPlay();
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x001EF967 File Offset: 0x001EDB67
		private void SmoothScale(Vector3 initial, Vector3 target)
		{
			this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
		}

		// Token: 0x06006168 RID: 24936 RVA: 0x001EF98C File Offset: 0x001EDB8C
		private void ApplyScaling(IFingerFlexListener.ComponentActivator activator)
		{
			if (this.audioSource)
			{
				this.audioSource.GTStop();
			}
			if (this.scaleOn == activator)
			{
				if (this.currentState != SmoothScaleModifierCosmetic.State.Scaled)
				{
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
					return;
				}
			}
			else if (this.resetOn == activator && this.currentState != SmoothScaleModifierCosmetic.State.Reset)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
			}
		}

		// Token: 0x06006169 RID: 24937 RVA: 0x001EF9E4 File Offset: 0x001EDBE4
		private void UpdateState(SmoothScaleModifierCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x001EF9ED File Offset: 0x001EDBED
		public void OnButtonPressed()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerFlexed);
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x001EF9F6 File Offset: 0x001EDBF6
		public void OnButtonReleased()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerReleased);
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x001EF9FF File Offset: 0x001EDBFF
		public void OnButtonPressStayed()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerStayed);
		}

		// Token: 0x04006D77 RID: 28023
		[SerializeField]
		private GameObject objectPrefab;

		// Token: 0x04006D78 RID: 28024
		[SerializeField]
		private Vector3 targetScale = new Vector3(2f, 2f, 2f);

		// Token: 0x04006D79 RID: 28025
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04006D7A RID: 28026
		[SerializeField]
		private IFingerFlexListener.ComponentActivator scaleOn;

		// Token: 0x04006D7B RID: 28027
		[SerializeField]
		private IFingerFlexListener.ComponentActivator resetOn;

		// Token: 0x04006D7C RID: 28028
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006D7D RID: 28029
		[SerializeField]
		private AudioClip scaledAudio;

		// Token: 0x04006D7E RID: 28030
		[SerializeField]
		private float scaleAudioVolume = 0.1f;

		// Token: 0x04006D7F RID: 28031
		[SerializeField]
		private AudioClip normalSizeAudio;

		// Token: 0x04006D80 RID: 28032
		[SerializeField]
		private float normalSizeAudioVolume = 0.1f;

		// Token: 0x04006D81 RID: 28033
		private SmoothScaleModifierCosmetic.State currentState;

		// Token: 0x04006D82 RID: 28034
		private Vector3 initialScale;

		// Token: 0x04006D83 RID: 28035
		private TransferrableObject transferrableObject;

		// Token: 0x02000F5D RID: 3933
		private enum State
		{
			// Token: 0x04006D85 RID: 28037
			None,
			// Token: 0x04006D86 RID: 28038
			Reset,
			// Token: 0x04006D87 RID: 28039
			Scaling,
			// Token: 0x04006D88 RID: 28040
			Scaled
		}
	}
}
