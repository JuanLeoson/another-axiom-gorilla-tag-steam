using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F28 RID: 3880
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06006027 RID: 24615 RVA: 0x001E8A66 File Offset: 0x001E6C66
		// (set) Token: 0x06006028 RID: 24616 RVA: 0x001E8A6E File Offset: 0x001E6C6E
		public VRRig MyRig { get; private set; }

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06006029 RID: 24617 RVA: 0x001E8A77 File Offset: 0x001E6C77
		// (set) Token: 0x0600602A RID: 24618 RVA: 0x001E8A7F File Offset: 0x001E6C7F
		public bool IsSpawned { get; set; }

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x0600602B RID: 24619 RVA: 0x001E8A88 File Offset: 0x001E6C88
		// (set) Token: 0x0600602C RID: 24620 RVA: 0x001E8A90 File Offset: 0x001E6C90
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600602D RID: 24621 RVA: 0x001E8A99 File Offset: 0x001E6C99
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x0600602E RID: 24622 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDespawn()
		{
		}

		// Token: 0x0600602F RID: 24623 RVA: 0x001E8AA2 File Offset: 0x001E6CA2
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x06006030 RID: 24624 RVA: 0x001E8AC8 File Offset: 0x001E6CC8
		private void OnPlayEffectLocal()
		{
			if (this.particlesToPlay != null)
			{
				this.particlesToPlay.Play();
			}
			if (this.objectToEnable != null)
			{
				this.objectToEnable.SetActive(true);
			}
			if (this.audioSource != null && this.audioToPlay != null)
			{
				this.audioSource.GTPlayOneShot(this.audioToPlay, 1f);
			}
			this.restartTimer = false;
			this.triggeredTime = Time.time;
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x001E8B4C File Offset: 0x001E6D4C
		public void OnButtonPressed(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			if (!this.restartTimer || !this.buttonReleased)
			{
				return;
			}
			this.OnPlayEffectLocal();
			this.buttonReleased = false;
		}

		// Token: 0x06006032 RID: 24626 RVA: 0x001E8B76 File Offset: 0x001E6D76
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x06006033 RID: 24627 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x06006034 RID: 24628 RVA: 0x001E8B89 File Offset: 0x001E6D89
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x04006B89 RID: 27529
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x04006B8A RID: 27530
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04006B8B RID: 27531
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04006B8C RID: 27532
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006B8D RID: 27533
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04006B8E RID: 27534
		[SerializeField]
		private bool leftHand;

		// Token: 0x04006B8F RID: 27535
		private float triggeredTime;

		// Token: 0x04006B90 RID: 27536
		private bool restartTimer;

		// Token: 0x04006B91 RID: 27537
		private bool buttonReleased = true;
	}
}
