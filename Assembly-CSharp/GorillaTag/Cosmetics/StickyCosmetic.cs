using System;
using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F61 RID: 3937
	public class StickyCosmetic : MonoBehaviour
	{
		// Token: 0x06006183 RID: 24963 RVA: 0x001F00CF File Offset: 0x001EE2CF
		private void Start()
		{
			this.endRigidbody.isKinematic = false;
			this.endRigidbody.useGravity = false;
			this.UpdateState(StickyCosmetic.ObjectState.Idle);
		}

		// Token: 0x06006184 RID: 24964 RVA: 0x001F00F0 File Offset: 0x001EE2F0
		public void Extend()
		{
			if (this.currentState == StickyCosmetic.ObjectState.Idle || this.currentState == StickyCosmetic.ObjectState.Extending)
			{
				this.UpdateState(StickyCosmetic.ObjectState.Extending);
			}
		}

		// Token: 0x06006185 RID: 24965 RVA: 0x001F010A File Offset: 0x001EE30A
		public void Retract()
		{
			this.UpdateState(StickyCosmetic.ObjectState.Retracting);
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x001F0114 File Offset: 0x001EE314
		private void Extend_Internal()
		{
			if (this.endRigidbody.isKinematic)
			{
				return;
			}
			this.rayLength = Mathf.Lerp(0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
			this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x001F0190 File Offset: 0x001EE390
		private void Retract_Internal()
		{
			this.endRigidbody.isKinematic = false;
			Vector3 position = Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime);
			this.endRigidbody.MovePosition(position);
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x001F01E0 File Offset: 0x001EE3E0
		private void FixedUpdate()
		{
			switch (this.currentState)
			{
			case StickyCosmetic.ObjectState.Extending:
			{
				if (Time.time - this.extendingStartedTime > this.retractAfterSecond)
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
				}
				this.Extend_Internal();
				RaycastHit raycastHit;
				if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, out raycastHit, this.rayLength, this.collisionLayers))
				{
					this.endRigidbody.isKinematic = true;
					this.endRigidbody.transform.parent = null;
					UnityEvent unityEvent = this.onStick;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.UpdateState(StickyCosmetic.ObjectState.Stuck);
				}
				break;
			}
			case StickyCosmetic.ObjectState.Retracting:
				if (Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.01f)
				{
					this.endRigidbody.position = this.startPosition.position;
					Transform transform = this.endRigidbody.transform;
					transform.parent = this.endPositionParent;
					transform.localRotation = quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
					{
						this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
					}
					else
					{
						this.UpdateState(StickyCosmetic.ObjectState.Idle);
					}
				}
				else
				{
					this.Retract_Internal();
				}
				break;
			case StickyCosmetic.ObjectState.Stuck:
				if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
				}
				break;
			case StickyCosmetic.ObjectState.AutoUnstuck:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			case StickyCosmetic.ObjectState.AutoRetract:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			}
			Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x001F03C0 File Offset: 0x001EE5C0
		private void UpdateState(StickyCosmetic.ObjectState newState)
		{
			this.lastState = this.currentState;
			if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
			{
				this.onUnstick.Invoke();
			}
			if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
			{
				this.extendingStartedTime = Time.time;
			}
			this.currentState = newState;
		}

		// Token: 0x04006D9A RID: 28058
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04006D9B RID: 28059
		[SerializeField]
		private LayerMask collisionLayers;

		// Token: 0x04006D9C RID: 28060
		[SerializeField]
		private Transform rayOrigin;

		// Token: 0x04006D9D RID: 28061
		[SerializeField]
		private float maxObjectLength = 0.7f;

		// Token: 0x04006D9E RID: 28062
		[SerializeField]
		private float autoRetractThreshold = 1f;

		// Token: 0x04006D9F RID: 28063
		[SerializeField]
		private float retractSpeed = 5f;

		// Token: 0x04006DA0 RID: 28064
		[Tooltip("If extended but not stuck, retract automatically after X seconds")]
		[SerializeField]
		private float retractAfterSecond = 2f;

		// Token: 0x04006DA1 RID: 28065
		[SerializeField]
		private Transform startPosition;

		// Token: 0x04006DA2 RID: 28066
		[SerializeField]
		private Rigidbody endRigidbody;

		// Token: 0x04006DA3 RID: 28067
		[SerializeField]
		private Transform endPositionParent;

		// Token: 0x04006DA4 RID: 28068
		public UnityEvent onStick;

		// Token: 0x04006DA5 RID: 28069
		public UnityEvent onUnstick;

		// Token: 0x04006DA6 RID: 28070
		private StickyCosmetic.ObjectState currentState;

		// Token: 0x04006DA7 RID: 28071
		private float rayLength;

		// Token: 0x04006DA8 RID: 28072
		private bool stick;

		// Token: 0x04006DA9 RID: 28073
		private StickyCosmetic.ObjectState lastState;

		// Token: 0x04006DAA RID: 28074
		private float extendingStartedTime;

		// Token: 0x02000F62 RID: 3938
		private enum ObjectState
		{
			// Token: 0x04006DAC RID: 28076
			Extending,
			// Token: 0x04006DAD RID: 28077
			Retracting,
			// Token: 0x04006DAE RID: 28078
			Stuck,
			// Token: 0x04006DAF RID: 28079
			JustRetracted,
			// Token: 0x04006DB0 RID: 28080
			Idle,
			// Token: 0x04006DB1 RID: 28081
			AutoUnstuck,
			// Token: 0x04006DB2 RID: 28082
			AutoRetract
		}
	}
}
