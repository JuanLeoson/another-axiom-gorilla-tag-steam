using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D20 RID: 3360
	public class PinchStateModule
	{
		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x0600531E RID: 21278 RVA: 0x0019BBA1 File Offset: 0x00199DA1
		public bool PinchUpAndDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchUp && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x0600531F RID: 21279 RVA: 0x0019BBBA File Offset: 0x00199DBA
		public bool PinchSteadyOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchStay && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06005320 RID: 21280 RVA: 0x0019BBD3 File Offset: 0x00199DD3
		public bool PinchDownOnFocusedObject
		{
			get
			{
				return this._currPinchState == PinchStateModule.PinchState.PinchDown && this._firstFocusedInteractable != null;
			}
		}

		// Token: 0x06005321 RID: 21281 RVA: 0x0019BBEC File Offset: 0x00199DEC
		public PinchStateModule()
		{
			this._currPinchState = PinchStateModule.PinchState.None;
			this._firstFocusedInteractable = null;
		}

		// Token: 0x06005322 RID: 21282 RVA: 0x0019BC04 File Offset: 0x00199E04
		public void UpdateState(OVRHand hand, Interactable currFocusedInteractable)
		{
			float fingerPinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
			bool flag = Mathf.Abs(1f - fingerPinchStrength) < Mathf.Epsilon;
			switch (this._currPinchState)
			{
			case PinchStateModule.PinchState.PinchDown:
				this._currPinchState = (flag ? PinchStateModule.PinchState.PinchStay : PinchStateModule.PinchState.PinchUp);
				if (this._firstFocusedInteractable != currFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchStay:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchUp;
				}
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			case PinchStateModule.PinchState.PinchUp:
				if (!flag)
				{
					this._currPinchState = PinchStateModule.PinchState.None;
					this._firstFocusedInteractable = null;
					return;
				}
				this._currPinchState = PinchStateModule.PinchState.PinchDown;
				if (currFocusedInteractable != this._firstFocusedInteractable)
				{
					this._firstFocusedInteractable = null;
					return;
				}
				break;
			default:
				if (flag)
				{
					this._currPinchState = PinchStateModule.PinchState.PinchDown;
					this._firstFocusedInteractable = currFocusedInteractable;
				}
				break;
			}
		}

		// Token: 0x04005C6A RID: 23658
		private const float PINCH_STRENGTH_THRESHOLD = 1f;

		// Token: 0x04005C6B RID: 23659
		private PinchStateModule.PinchState _currPinchState;

		// Token: 0x04005C6C RID: 23660
		private Interactable _firstFocusedInteractable;

		// Token: 0x02000D21 RID: 3361
		private enum PinchState
		{
			// Token: 0x04005C6E RID: 23662
			None,
			// Token: 0x04005C6F RID: 23663
			PinchDown,
			// Token: 0x04005C70 RID: 23664
			PinchStay,
			// Token: 0x04005C71 RID: 23665
			PinchUp
		}
	}
}
