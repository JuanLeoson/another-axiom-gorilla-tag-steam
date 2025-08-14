using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F33 RID: 3891
	public class CloserCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x0600607C RID: 24700 RVA: 0x001EAC68 File Offset: 0x001E8E68
		// (set) Token: 0x0600607D RID: 24701 RVA: 0x001EAC70 File Offset: 0x001E8E70
		public bool TickRunning { get; set; }

		// Token: 0x0600607E RID: 24702 RVA: 0x001EAC7C File Offset: 0x001E8E7C
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.localRotA = this.sideA.transform.localRotation;
			this.localRotB = this.sideB.transform.localRotation;
			this.fingerValue = 0f;
			this.UpdateState(CloserCosmetic.State.Opening);
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x0001D44F File Offset: 0x0001B64F
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06006080 RID: 24704 RVA: 0x001EACD0 File Offset: 0x001E8ED0
		public void Tick()
		{
			switch (this.currentState)
			{
			case CloserCosmetic.State.Closing:
				this.Closing();
				return;
			case CloserCosmetic.State.Opening:
				this.Opening();
				break;
			case CloserCosmetic.State.None:
				break;
			default:
				return;
			}
		}

		// Token: 0x06006081 RID: 24705 RVA: 0x001EAD04 File Offset: 0x001E8F04
		public void Close(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Closing);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06006082 RID: 24706 RVA: 0x001EAD14 File Offset: 0x001E8F14
		public void Open(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Opening);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x001EAD24 File Offset: 0x001E8F24
		private void Closing()
		{
			float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion b = Quaternion.Euler(this.maxRotationB);
			Quaternion quaternion = Quaternion.Slerp(this.localRotB, b, t);
			this.sideB.transform.localRotation = quaternion;
			Quaternion b2 = Quaternion.Euler(this.maxRotationA);
			Quaternion quaternion2 = Quaternion.Slerp(this.localRotA, b2, t);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06006084 RID: 24708 RVA: 0x001EADE8 File Offset: 0x001E8FE8
		private void Opening()
		{
			float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion quaternion = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, t);
			this.sideB.transform.localRotation = quaternion;
			Quaternion quaternion2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, t);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06006085 RID: 24709 RVA: 0x001EAEAD File Offset: 0x001E90AD
		private void UpdateState(CloserCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04006C06 RID: 27654
		[SerializeField]
		private GameObject sideA;

		// Token: 0x04006C07 RID: 27655
		[SerializeField]
		private GameObject sideB;

		// Token: 0x04006C08 RID: 27656
		[SerializeField]
		private Vector3 maxRotationA;

		// Token: 0x04006C09 RID: 27657
		[SerializeField]
		private Vector3 maxRotationB;

		// Token: 0x04006C0A RID: 27658
		[SerializeField]
		private bool useFingerFlexValueAsStrength;

		// Token: 0x04006C0B RID: 27659
		private Quaternion localRotA;

		// Token: 0x04006C0C RID: 27660
		private Quaternion localRotB;

		// Token: 0x04006C0D RID: 27661
		private CloserCosmetic.State currentState;

		// Token: 0x04006C0E RID: 27662
		private float fingerValue;

		// Token: 0x02000F34 RID: 3892
		private enum State
		{
			// Token: 0x04006C11 RID: 27665
			Closing,
			// Token: 0x04006C12 RID: 27666
			Opening,
			// Token: 0x04006C13 RID: 27667
			None
		}
	}
}
