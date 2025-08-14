using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F5B RID: 3931
	public class ShakeDetectorCosmetic : MonoBehaviour
	{
		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x0600615E RID: 24926 RVA: 0x001EF5B1 File Offset: 0x001ED7B1
		// (set) Token: 0x0600615F RID: 24927 RVA: 0x001EF5B9 File Offset: 0x001ED7B9
		public Vector3 HandVelocity { get; private set; }

		// Token: 0x06006160 RID: 24928 RVA: 0x001EF5C2 File Offset: 0x001ED7C2
		private void Awake()
		{
			this.HandVelocity = Vector3.zero;
			this.shakeEndTime = 0f;
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x001EF5DC File Offset: 0x001ED7DC
		private void UpdateShakeVelocity()
		{
			if (!this.parentTransferrable)
			{
				return;
			}
			if (!this.parentTransferrable.InHand())
			{
				this.HandVelocity = Vector3.zero;
				return;
			}
			if (!this.parentTransferrable.IsMyItem())
			{
				return;
			}
			this.isLeftHand = this.parentTransferrable.InLeftHand();
			this.HandVelocity = (this.isLeftHand ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false));
			this.HandVelocity = Vector3.ClampMagnitude(this.HandVelocity, this.maxHandVelocity);
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x001EF684 File Offset: 0x001ED884
		public void Update()
		{
			this.UpdateShakeVelocity();
			if (Time.time - this.shakeEndTime > this.cooldown && !this.isShaking && this.HandVelocity.magnitude >= this.shakeStartVelocityThreshold)
			{
				UnityEvent<bool, float> unityEvent = this.onShakeStartLocal;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.isLeftHand, this.HandVelocity.magnitude);
				}
				this.isShaking = true;
			}
			if (this.isShaking && this.HandVelocity.magnitude < this.shakeEndVelocityThreshold)
			{
				UnityEvent<bool, float> unityEvent2 = this.onShakeEndLocal;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this.isLeftHand, this.HandVelocity.magnitude);
				}
				this.isShaking = false;
				this.shakeEndTime = Time.time;
			}
		}

		// Token: 0x04006D6C RID: 28012
		[SerializeField]
		private TransferrableObject parentTransferrable;

		// Token: 0x04006D6D RID: 28013
		[Tooltip("for velocity equal or above this, we fire a Shake Start event")]
		[SerializeField]
		private float shakeStartVelocityThreshold;

		// Token: 0x04006D6E RID: 28014
		[Tooltip("for velocity under this, we fire a Shake End event")]
		[SerializeField]
		private float shakeEndVelocityThreshold;

		// Token: 0x04006D6F RID: 28015
		[Tooltip("cooldown starts when shaking ends")]
		[SerializeField]
		private float cooldown;

		// Token: 0x04006D70 RID: 28016
		[Tooltip("Use for clamping hand velocity value")]
		[SerializeField]
		private float maxHandVelocity = 20f;

		// Token: 0x04006D71 RID: 28017
		[FormerlySerializedAs("onShakeStart")]
		public UnityEvent<bool, float> onShakeStartLocal;

		// Token: 0x04006D72 RID: 28018
		[FormerlySerializedAs("onShakeEnd")]
		public UnityEvent<bool, float> onShakeEndLocal;

		// Token: 0x04006D74 RID: 28020
		private bool isShaking;

		// Token: 0x04006D75 RID: 28021
		private float shakeEndTime;

		// Token: 0x04006D76 RID: 28022
		private bool isLeftHand;
	}
}
