using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F21 RID: 3873
	public class MedusaEyeLantern : MonoBehaviour
	{
		// Token: 0x06005FFB RID: 24571 RVA: 0x001E79A8 File Offset: 0x001E5BA8
		private void Awake()
		{
			foreach (MedusaEyeLantern.EyeState eyeState in this.allStates)
			{
				this.allStatesDict.Add(eyeState.eyeState, eyeState);
			}
		}

		// Token: 0x06005FFC RID: 24572 RVA: 0x001E79E0 File Offset: 0x001E5BE0
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x06005FFD RID: 24573 RVA: 0x001E79ED File Offset: 0x001E5BED
		private void Start()
		{
			if (this.rotatingObjectTransform == null)
			{
				this.rotatingObjectTransform = base.transform;
			}
			this.initialRotation = this.rotatingObjectTransform.localRotation;
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x001E7A24 File Offset: 0x001E5C24
		private void Update()
		{
			if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
			if (!this.transferableParent.InHand())
			{
				return;
			}
			this.UpdateState();
			if (this.velocityTracker == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			float x = Mathf.Clamp(-normalized.z, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			float z = Mathf.Clamp(normalized.x, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			this.targetRotation = this.initialRotation * Quaternion.Euler(x, 0f, z);
			if (magnitude > this.sloshVelocityThreshold)
			{
				this.SwitchState(MedusaEyeLantern.State.SLOSHING);
			}
			if ((double)magnitude < 0.01)
			{
				this.targetRotation = this.initialRotation;
			}
			if (!this.EyeIsLockedOn())
			{
				this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
			}
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x001E7B82 File Offset: 0x001E5D82
		public void HandleOnNoOneInRange()
		{
			this.SwitchState(MedusaEyeLantern.State.RESET);
			this.resetTargetTime = Time.time;
			this.rotatingObjectTransform.localRotation = this.initialRotation;
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x001E7BA7 File Offset: 0x001E5DA7
		public void HandleOnNewPlayerDetected(VRRig target, float distance)
		{
			this.targetRig = target;
			if (this.currentState != MedusaEyeLantern.State.SLOSHING)
			{
				this.SwitchState(MedusaEyeLantern.State.TRACKING);
			}
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x001E7BC0 File Offset: 0x001E5DC0
		private void Sloshing()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			if ((double)vector.magnitude < 0.01)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
		}

		// Token: 0x06006002 RID: 24578 RVA: 0x001E7C14 File Offset: 0x001E5E14
		private void FaceTarget()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 normalized = (this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.z).normalized;
			Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward * 0.3f, Color.blue);
			Debug.DrawRay(this.rotatingObjectTransform.position, normalized2 * 0.3f, Color.green);
			if (normalized2.sqrMagnitude > 0.001f)
			{
				float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(this.rotatingObjectTransform.forward.normalized, normalized2), -1f, 1f)) * 57.29578f;
				if (180f - num < this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
				{
					this.SwitchState(MedusaEyeLantern.State.WARMUP);
					return;
				}
				Quaternion to = Quaternion.LookRotation(-normalized2, Vector3.up);
				this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, to, this.lookAtTargetSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x001E7D78 File Offset: 0x001E5F78
		private bool IsTargetLookingAtEye()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return false;
			}
			Transform transform = this.targetRig.tagSound.transform;
			Vector3 normalized = (this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position).normalized;
			float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up.normalized, normalized), -1f, 1f)) * 57.29578f;
			Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
			Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
			return num < this.lookAtEyeAngleThreshold;
		}

		// Token: 0x06006004 RID: 24580 RVA: 0x001E7E60 File Offset: 0x001E6060
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case MedusaEyeLantern.State.SLOSHING:
				this.Sloshing();
				break;
			case MedusaEyeLantern.State.DORMANT:
				this.warmupCounter = 0f;
				this.petrificationStarted = float.PositiveInfinity;
				if (this.targetRig != null && (this.targetRig.transform.position - base.transform.position).IsShorterThan(this.distanceChecker.distanceThreshold))
				{
					this.SwitchState(MedusaEyeLantern.State.TRACKING);
				}
				break;
			case MedusaEyeLantern.State.TRACKING:
				this.FaceTarget();
				break;
			case MedusaEyeLantern.State.WARMUP:
				this.warmupCounter += Time.deltaTime;
				this.FaceTarget();
				if (this.warmupCounter > this.warmUpProgressTime)
				{
					this.SwitchState(MedusaEyeLantern.State.PRIMING);
					this.warmupCounter = 0f;
				}
				break;
			case MedusaEyeLantern.State.PRIMING:
				this.FaceTarget();
				if (this.IsTargetLookingAtEye())
				{
					UnityEvent<VRRig> onPetrification = this.OnPetrification;
					if (onPetrification != null)
					{
						onPetrification.Invoke(this.targetRig);
					}
					this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
					this.petrificationStarted = Time.time;
				}
				break;
			case MedusaEyeLantern.State.PETRIFICATION:
				if (Time.time - this.petrificationStarted > this.petrificationDuration)
				{
					this.SwitchState(MedusaEyeLantern.State.COOLDOWN);
				}
				break;
			case MedusaEyeLantern.State.COOLDOWN:
				if (Time.time - this.petrificationStarted > this.resetCooldown)
				{
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
					this.petrificationStarted = float.PositiveInfinity;
				}
				break;
			case MedusaEyeLantern.State.RESET:
				if (Time.time - this.resetTargetTime > this.resetTargetTimer)
				{
					this.resetTargetTime = float.PositiveInfinity;
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
				}
				break;
			}
			this.PlayHaptic(this.currentState);
		}

		// Token: 0x06006005 RID: 24581 RVA: 0x001E8010 File Offset: 0x001E6210
		private void SwitchState(MedusaEyeLantern.State newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			MedusaEyeLantern.EyeState eyeState;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, out eyeState))
			{
				UnityEvent onEnterState = eyeState.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
			}
			MedusaEyeLantern.EyeState eyeState2;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(this.lastState, out eyeState2))
			{
				UnityEvent onExitState = eyeState2.onExitState;
				if (onExitState == null)
				{
					return;
				}
				onExitState.Invoke();
			}
		}

		// Token: 0x06006006 RID: 24582 RVA: 0x001E8094 File Offset: 0x001E6294
		private void PlayHaptic(MedusaEyeLantern.State state)
		{
			if (!this.transferableParent.IsMyItem())
			{
				return;
			}
			MedusaEyeLantern.EyeState eyeState;
			this.allStatesDict.TryGetValue(state, out eyeState);
			if (this.currentState == MedusaEyeLantern.State.WARMUP)
			{
				float time = Mathf.Clamp01(this.warmupCounter / this.warmUpProgressTime);
				if (eyeState != null && eyeState.hapticStrength != null)
				{
					float amplitude = eyeState.hapticStrength.Evaluate(time);
					bool forLeftController = this.transferableParent.InLeftHand();
					GorillaTagger.Instance.StartVibration(forLeftController, amplitude, Time.deltaTime);
					return;
				}
			}
			else if (eyeState != null && eyeState.hapticStrength != null)
			{
				float amplitude2 = eyeState.hapticStrength.Evaluate(0.5f);
				bool forLeftController2 = this.transferableParent.InLeftHand();
				GorillaTagger.Instance.StartVibration(forLeftController2, amplitude2, Time.deltaTime);
			}
		}

		// Token: 0x06006007 RID: 24583 RVA: 0x001E814D File Offset: 0x001E634D
		private bool EyeIsLockedOn()
		{
			return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
		}

		// Token: 0x04006B3A RID: 27450
		[SerializeField]
		private DistanceCheckerCosmetic distanceChecker;

		// Token: 0x04006B3B RID: 27451
		[SerializeField]
		private TransferrableObject transferableParent;

		// Token: 0x04006B3C RID: 27452
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04006B3D RID: 27453
		[SerializeField]
		private Transform rotatingObjectTransform;

		// Token: 0x04006B3E RID: 27454
		[Space]
		[Header("Rotation Settings")]
		[SerializeField]
		private float maxRotationAngle = 50f;

		// Token: 0x04006B3F RID: 27455
		[SerializeField]
		private float sloshVelocityThreshold = 1f;

		// Token: 0x04006B40 RID: 27456
		[SerializeField]
		private float rotationSmoothing = 10f;

		// Token: 0x04006B41 RID: 27457
		[SerializeField]
		private float rotationSpeedMultiplier = 5f;

		// Token: 0x04006B42 RID: 27458
		[Space]
		[Header("Target Tracking Settings")]
		[SerializeField]
		private float lookAtEyeAngleThreshold = 90f;

		// Token: 0x04006B43 RID: 27459
		[SerializeField]
		private float targetHeadAngleThreshold = 5f;

		// Token: 0x04006B44 RID: 27460
		[SerializeField]
		private float lookAtTargetSpeed = 5f;

		// Token: 0x04006B45 RID: 27461
		[SerializeField]
		private float warmUpProgressTime = 3f;

		// Token: 0x04006B46 RID: 27462
		[SerializeField]
		private float resetCooldown = 5f;

		// Token: 0x04006B47 RID: 27463
		[SerializeField]
		private float faceDistanceOffset = 0.2f;

		// Token: 0x04006B48 RID: 27464
		[SerializeField]
		private float petrificationDuration = 0.2f;

		// Token: 0x04006B49 RID: 27465
		[Space]
		[Header("Eye State Settings")]
		public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];

		// Token: 0x04006B4A RID: 27466
		public UnityEvent<VRRig> OnPetrification;

		// Token: 0x04006B4B RID: 27467
		private Quaternion initialRotation;

		// Token: 0x04006B4C RID: 27468
		private Quaternion targetRotation;

		// Token: 0x04006B4D RID: 27469
		private MedusaEyeLantern.State currentState;

		// Token: 0x04006B4E RID: 27470
		private MedusaEyeLantern.State lastState;

		// Token: 0x04006B4F RID: 27471
		private float petrificationStarted = float.PositiveInfinity;

		// Token: 0x04006B50 RID: 27472
		private float warmupCounter;

		// Token: 0x04006B51 RID: 27473
		private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();

		// Token: 0x04006B52 RID: 27474
		private VRRig targetRig;

		// Token: 0x04006B53 RID: 27475
		private float resetTargetTimer = 1f;

		// Token: 0x04006B54 RID: 27476
		private float resetTargetTime = float.PositiveInfinity;

		// Token: 0x02000F22 RID: 3874
		[Serializable]
		public class EyeState
		{
			// Token: 0x04006B55 RID: 27477
			public MedusaEyeLantern.State eyeState;

			// Token: 0x04006B56 RID: 27478
			public AnimationCurve hapticStrength;

			// Token: 0x04006B57 RID: 27479
			public UnityEvent onEnterState;

			// Token: 0x04006B58 RID: 27480
			public UnityEvent onExitState;
		}

		// Token: 0x02000F23 RID: 3875
		public enum State
		{
			// Token: 0x04006B5A RID: 27482
			SLOSHING,
			// Token: 0x04006B5B RID: 27483
			DORMANT,
			// Token: 0x04006B5C RID: 27484
			TRACKING,
			// Token: 0x04006B5D RID: 27485
			WARMUP,
			// Token: 0x04006B5E RID: 27486
			PRIMING,
			// Token: 0x04006B5F RID: 27487
			PETRIFICATION,
			// Token: 0x04006B60 RID: 27488
			COOLDOWN,
			// Token: 0x04006B61 RID: 27489
			RESET
		}
	}
}
