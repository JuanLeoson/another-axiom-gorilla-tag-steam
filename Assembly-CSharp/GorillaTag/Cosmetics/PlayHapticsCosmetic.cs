using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F56 RID: 3926
	public class PlayHapticsCosmetic : MonoBehaviour
	{
		// Token: 0x06006138 RID: 24888 RVA: 0x001EEC7C File Offset: 0x001ECE7C
		private void Awake()
		{
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x06006139 RID: 24889 RVA: 0x001EEC8A File Offset: 0x001ECE8A
		public void PlayHaptics()
		{
			GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600613A RID: 24890 RVA: 0x001EECA8 File Offset: 0x001ECEA8
		public void PlayHapticsTransferableObject()
		{
			if (this.parentTransferable != null)
			{
				bool forLeftController = this.parentTransferable.InLeftHand();
				GorillaTagger.Instance.StartVibration(forLeftController, this.hapticStrength, this.hapticDuration);
			}
		}

		// Token: 0x0600613B RID: 24891 RVA: 0x001EECE6 File Offset: 0x001ECEE6
		public void PlayHaptics(bool isLeftHand)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600613C RID: 24892 RVA: 0x001EECFF File Offset: 0x001ECEFF
		public void PlayHapticsBothHands(bool isLeftHand)
		{
			this.PlayHaptics(false);
			this.PlayHaptics(true);
		}

		// Token: 0x0600613D RID: 24893 RVA: 0x001EECE6 File Offset: 0x001ECEE6
		public void PlayHaptics(bool isLeftHand, float value)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x0600613E RID: 24894 RVA: 0x001EED0F File Offset: 0x001ECF0F
		public void PlayHapticsBothHands(bool isLeftHand, float value)
		{
			this.PlayHaptics(false, value);
			this.PlayHaptics(true, value);
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x001EECE6 File Offset: 0x001ECEE6
		public void PlayHaptics(bool isLeftHand, Collider other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x001EED21 File Offset: 0x001ECF21
		public void PlayHapticsBothHands(bool isLeftHand, Collider other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06006141 RID: 24897 RVA: 0x001EECE6 File Offset: 0x001ECEE6
		public void PlayHaptics(bool isLeftHand, Collision other)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x001EED33 File Offset: 0x001ECF33
		public void PlayHapticsBothHands(bool isLeftHand, Collision other)
		{
			this.PlayHaptics(false, other);
			this.PlayHaptics(true, other);
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x001EED48 File Offset: 0x001ECF48
		public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
		{
			float amplitude = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
			GorillaTagger.Instance.StartVibration(isLeftHand, amplitude, this.hapticDuration);
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x001EED7A File Offset: 0x001ECF7A
		public void PlayHapticsByButtonValueBothHands(bool isLeftHand, float strength)
		{
			this.PlayHapticsByButtonValue(false, strength);
			this.PlayHapticsByButtonValue(true, strength);
		}

		// Token: 0x06006145 RID: 24901 RVA: 0x001EED8C File Offset: 0x001ECF8C
		public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
		{
			float num = (isLeftHand ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false)).magnitude;
			num = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, num);
			GorillaTagger.Instance.StartVibration(isLeftHand, num, this.hapticDuration);
		}

		// Token: 0x06006146 RID: 24902 RVA: 0x001EEDF8 File Offset: 0x001ECFF8
		public void PlayHapticsByVelocityBothHands(bool isLeftHand, float velocity)
		{
			this.PlayHapticsByVelocity(false, velocity);
			this.PlayHapticsByVelocity(true, velocity);
		}

		// Token: 0x04006D26 RID: 27942
		[SerializeField]
		private float hapticDuration;

		// Token: 0x04006D27 RID: 27943
		[SerializeField]
		private float hapticStrength;

		// Token: 0x04006D28 RID: 27944
		[SerializeField]
		private float minHapticStrengthThreshold;

		// Token: 0x04006D29 RID: 27945
		[SerializeField]
		private float maxHapticStrengthThreshold;

		// Token: 0x04006D2A RID: 27946
		[Tooltip("Only check this box if you are not setting the left/hand right from the subscriber")]
		[SerializeField]
		private bool leftHand;

		// Token: 0x04006D2B RID: 27947
		private TransferrableObject parentTransferable;
	}
}
