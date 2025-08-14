using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F66 RID: 3942
	public class TransferrableObjectHoldablePart_Pin : TransferrableObjectHoldablePart
	{
		// Token: 0x0600619A RID: 24986 RVA: 0x001F0709 File Offset: 0x001EE909
		protected void OnEnable()
		{
			UnityEvent onEnableHoldable = this.OnEnableHoldable;
			if (onEnableHoldable == null)
			{
				return;
			}
			onEnableHoldable.Invoke();
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x001F071C File Offset: 0x001EE91C
		protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
		{
			if (rig.isOfflineVRRig)
			{
				Transform transform = isHeldLeftHand ? GTPlayer.Instance.leftControllerTransform : GTPlayer.Instance.rightControllerTransform;
				if ((isHeldLeftHand ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false)).magnitude > this.breakStrengthThreshold || (transform.position - this.pin.transform.position).IsLongerThan(this.maxHandSnapDistance))
				{
					this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
					UnityEvent onBreak = this.OnBreak;
					if (onBreak != null)
					{
						onBreak.Invoke();
					}
					if (this.transferrableParentObject && this.transferrableParentObject.IsMyItem())
					{
						UnityEvent onBreakLocal = this.OnBreakLocal;
						if (onBreakLocal == null)
						{
							return;
						}
						onBreakLocal.Invoke();
					}
					return;
				}
				transform.position = this.pin.position;
			}
		}

		// Token: 0x04006DCB RID: 28107
		[SerializeField]
		private float breakStrengthThreshold = 0.8f;

		// Token: 0x04006DCC RID: 28108
		[SerializeField]
		private float maxHandSnapDistance = 0.5f;

		// Token: 0x04006DCD RID: 28109
		[SerializeField]
		private Transform pin;

		// Token: 0x04006DCE RID: 28110
		public UnityEvent OnBreak;

		// Token: 0x04006DCF RID: 28111
		public UnityEvent OnBreakLocal;

		// Token: 0x04006DD0 RID: 28112
		public UnityEvent OnEnableHoldable;
	}
}
