using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F58 RID: 3928
	public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x0600614B RID: 24907 RVA: 0x001EEEAC File Offset: 0x001ED0AC
		private bool IsMovementShoot()
		{
			return this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold;
		}

		// Token: 0x0600614C RID: 24908 RVA: 0x001EEEB7 File Offset: 0x001ED0B7
		private bool IsRigDirection()
		{
			return this.shootDirectionType == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform;
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x0600614D RID: 24909 RVA: 0x001EEEC2 File Offset: 0x001ED0C2
		// (set) Token: 0x0600614E RID: 24910 RVA: 0x001EEECA File Offset: 0x001ED0CA
		public bool shootingAllowed { get; set; } = true;

		// Token: 0x0600614F RID: 24911 RVA: 0x001EEED4 File Offset: 0x001ED0D4
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.rig = ((this.transferrableObject == null) ? base.GetComponentInParent<VRRig>() : this.transferrableObject.ownerRig);
			UnityEvent<int> unityEvent = this.onMovedToNextStep;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentStep);
			}
			this.isLocal = ((this.transferrableObject != null && this.transferrableObject.IsMyItem()) || (this.rig != null && this.rig == GorillaTagger.Instance.offlineVRRig));
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06006150 RID: 24912 RVA: 0x001EEF75 File Offset: 0x001ED175
		// (set) Token: 0x06006151 RID: 24913 RVA: 0x001EEF7D File Offset: 0x001ED17D
		public bool TickRunning { get; set; }

		// Token: 0x06006152 RID: 24914 RVA: 0x001EEF88 File Offset: 0x001ED188
		public void Tick()
		{
			if (this.isCoolingDown && Time.time - this.lastShotTime >= this.cooldown)
			{
				this.isCoolingDown = false;
				UnityEvent unityEvent = this.onReadyToShoot;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				if (this.isPressed)
				{
					this.pressStartedTime = Time.time;
				}
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.isPressed && !this.isCoolingDown && this.allowCharging)
			{
				float chargeFrac = this.GetChargeFrac();
				UnityEvent<float> unityEvent2 = this.whileCharging;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(chargeFrac);
				}
				if (this.chargeHapticsIntensity > 0f)
				{
					this.RunHaptics(chargeFrac * this.chargeHapticsIntensity, Time.deltaTime);
				}
				this.lastStep = this.currentStep;
				this.currentStep = Mathf.Clamp(Mathf.FloorToInt(chargeFrac * (float)this.numberOfProgressSteps), 0, this.numberOfProgressSteps - 1);
				if (this.currentStep >= 0 && this.currentStep != this.lastStep)
				{
					UnityEvent<int> unityEvent3 = this.onMovedToNextStep;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.currentStep);
					}
					if (this.currentStep == this.numberOfProgressSteps - 1)
					{
						UnityEvent<int> unityEvent4 = this.onReachedLastProgressStep;
						if (unityEvent4 != null)
						{
							unityEvent4.Invoke(this.currentStep);
						}
					}
				}
				if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
				{
					Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
					float num = linearVelocity.magnitude;
					float num2 = Vector3.Dot(linearVelocity / num, this.GetVectorFromBodyToLaunchPosition().normalized);
					num *= Mathf.Ceil(num2 - this.velocityEstimatorMinRigDotProduct);
					if (num >= this.velocityEstimatorStartGestureSpeed)
					{
						this.velocityEstimatorThresholdMet = true;
						return;
					}
					if (this.velocityEstimatorThresholdMet && num < this.velocityEstimatorStopGestureSpeed)
					{
						this.TryShoot();
					}
				}
			}
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x001EF131 File Offset: 0x001ED331
		private Vector3 GetVectorFromBodyToLaunchPosition()
		{
			return this.shootFromTransform.position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition);
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x001EF15C File Offset: 0x001ED35C
		private void GetShootPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			ProjectileShooterCosmetic.ShootDirection shootDirection = this.shootDirectionType;
			if (shootDirection != ProjectileShooterCosmetic.ShootDirection.LaunchTransformRotation && shootDirection == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform)
			{
				position = this.shootFromTransform.position;
				rotation = Quaternion.LookRotation(position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition));
				return;
			}
			this.shootFromTransform.GetPositionAndRotation(out position, out rotation);
		}

		// Token: 0x06006155 RID: 24917 RVA: 0x001EF1C4 File Offset: 0x001ED3C4
		private void Shoot()
		{
			float chargeFrac = this.GetChargeFrac();
			float d = Mathf.Lerp(this.shootMinSpeed, this.shootMaxSpeed, this.chargeToShotSpeedCurve.Evaluate(chargeFrac));
			GameObject gameObject = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
			gameObject.transform.localScale = Vector3.one * this.rig.scaleFactor;
			IProjectile component = gameObject.GetComponent<IProjectile>();
			if (component != null)
			{
				Vector3 startPosition;
				Quaternion quaternion;
				this.GetShootPositionAndRotation(out startPosition, out quaternion);
				Vector3 velocity = quaternion * Vector3.forward * d * this.rig.scaleFactor;
				component.Launch(startPosition, quaternion, velocity, chargeFrac, this.rig, this.currentStep);
			}
			UnityEvent<float> unityEvent = this.onShoot;
			if (unityEvent != null)
			{
				unityEvent.Invoke(chargeFrac);
			}
			if (this.isLocal)
			{
				UnityEvent<float> unityEvent2 = this.onShootLocal;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(chargeFrac);
				}
			}
			if (this.allowCharging && this.runChargeCancelledEventOnShoot)
			{
				UnityEvent unityEvent3 = this.onChargeCancelled;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke();
				}
			}
			this.RunHaptics(chargeFrac * this.hapticsIntensity, this.hapticsDuration);
			this.SetPressState(false);
			this.isCoolingDown = true;
			this.lastShotTime = Time.time;
			this.currentStep = -1;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x001EF302 File Offset: 0x001ED502
		private bool TryShoot()
		{
			if (!this.isCoolingDown && this.shootingAllowed)
			{
				this.Shoot();
				return true;
			}
			return false;
		}

		// Token: 0x06006157 RID: 24919 RVA: 0x001EF320 File Offset: 0x001ED520
		private void RunHaptics(float intensity, float duration)
		{
			if (!this.enableHaptics || !this.isLocal)
			{
				return;
			}
			bool flag = this.transferrableObject != null && this.transferrableObject.InLeftHand();
			GorillaTagger.Instance.StartVibration(flag, intensity, duration);
			if (this.hapticsBothHands)
			{
				GorillaTagger.Instance.StartVibration(!flag, intensity, duration);
			}
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x001EF380 File Offset: 0x001ED580
		private float GetChargeFrac()
		{
			if (!this.isPressed)
			{
				return 0f;
			}
			if (!this.allowCharging || this.isAtMaxCharge)
			{
				return 1f;
			}
			float num = Time.time - this.pressStartedTime;
			if (num >= this.snapToMaxChargeAt || num >= this.maxChargeSeconds)
			{
				this.isAtMaxCharge = true;
				UnityEvent unityEvent = this.onMaxCharge;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				return 1f;
			}
			return this.chargeRateCurve.Evaluate(num / this.maxChargeSeconds);
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x001EF401 File Offset: 0x001ED601
		private void SetPressState(bool pressed)
		{
			this.isPressed = pressed;
			if (pressed)
			{
				this.pressStartedTime = Time.time;
			}
			else
			{
				this.pressStoppedTime = Time.time;
			}
			this.isAtMaxCharge = false;
			this.velocityEstimatorThresholdMet = false;
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x001EF433 File Offset: 0x001ED633
		public void OnButtonPressed()
		{
			this.SetPressState(true);
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonPressed)
			{
				this.TryShoot();
				return;
			}
			if (this.allowCharging || this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x001EF464 File Offset: 0x001ED664
		public void OnButtonReleased()
		{
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold && this.velocityEstimatorThresholdMet)
			{
				return;
			}
			if (this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.ButtonReleased || !this.TryShoot())
			{
				this.SetPressState(false);
				if (this.allowCharging)
				{
					UnityEvent unityEvent = this.onChargeCancelled;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
			}
		}

		// Token: 0x0600615C RID: 24924 RVA: 0x001EF4B5 File Offset: 0x001ED6B5
		public void ResetShoot()
		{
			this.isPressed = false;
			this.isAtMaxCharge = false;
			this.pressStoppedTime = Time.time;
			this.velocityEstimatorThresholdMet = false;
			this.currentStep = -1;
			this.lastStep = -1;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x04006D2F RID: 27951
		private const string CHARGE_STR = "allowCharging";

		// Token: 0x04006D30 RID: 27952
		private const string CHARGE_MSG = "only enabled when allowCharging is true.";

		// Token: 0x04006D31 RID: 27953
		private const string HAPTICS_STR = "enableHaptics";

		// Token: 0x04006D32 RID: 27954
		private const string MOVE_STR = "IsMovementShoot";

		// Token: 0x04006D33 RID: 27955
		[SerializeField]
		private HashWrapper projectilePrefab;

		// Token: 0x04006D34 RID: 27956
		[FormerlySerializedAs("launchActivatorType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootActivator shootActivatorType;

		// Token: 0x04006D35 RID: 27957
		[FormerlySerializedAs("launchDirectionType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootDirection shootDirectionType;

		// Token: 0x04006D36 RID: 27958
		[SerializeField]
		private Vector3 offsetRigPosition;

		// Token: 0x04006D37 RID: 27959
		[FormerlySerializedAs("launchTransform")]
		[SerializeField]
		private Transform shootFromTransform;

		// Token: 0x04006D38 RID: 27960
		[SerializeField]
		private bool drawShootVector;

		// Token: 0x04006D39 RID: 27961
		[SerializeField]
		private float cooldown;

		// Token: 0x04006D3A RID: 27962
		[Space]
		[SerializeField]
		private bool enableHaptics = true;

		// Token: 0x04006D3B RID: 27963
		[SerializeField]
		private float hapticsIntensity = 0.5f;

		// Token: 0x04006D3C RID: 27964
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float chargeHapticsIntensity = 0.3f;

		// Token: 0x04006D3D RID: 27965
		[SerializeField]
		private float hapticsDuration = 0.2f;

		// Token: 0x04006D3E RID: 27966
		[SerializeField]
		private bool hapticsBothHands;

		// Token: 0x04006D3F RID: 27967
		[Space]
		[SerializeField]
		private GorillaVelocityEstimator velocityEstimator;

		// Token: 0x04006D40 RID: 27968
		[SerializeField]
		private float velocityEstimatorStartGestureSpeed = 0.5f;

		// Token: 0x04006D41 RID: 27969
		[SerializeField]
		private float velocityEstimatorStopGestureSpeed = 0.2f;

		// Token: 0x04006D42 RID: 27970
		[SerializeField]
		private float velocityEstimatorMinRigDotProduct = 0.5f;

		// Token: 0x04006D43 RID: 27971
		[SerializeField]
		private bool logVelocityEstimatorSpeed;

		// Token: 0x04006D44 RID: 27972
		[FormerlySerializedAs("launchMinSpeed")]
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float shootMinSpeed;

		// Token: 0x04006D45 RID: 27973
		[FormerlySerializedAs("launchMaxSpeed")]
		[SerializeField]
		private float shootMaxSpeed;

		// Token: 0x04006D46 RID: 27974
		[SerializeField]
		private bool allowCharging;

		// Token: 0x04006D47 RID: 27975
		[SerializeField]
		private float maxChargeSeconds = 2f;

		// Token: 0x04006D48 RID: 27976
		[SerializeField]
		private float snapToMaxChargeAt = 9999999f;

		// Token: 0x04006D49 RID: 27977
		[SerializeField]
		private bool runChargeCancelledEventOnShoot;

		// Token: 0x04006D4A RID: 27978
		[SerializeField]
		private AnimationCurve chargeRateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006D4B RID: 27979
		[SerializeField]
		private AnimationCurve chargeToShotSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006D4C RID: 27980
		public UnityEvent onReadyToShoot;

		// Token: 0x04006D4D RID: 27981
		public UnityEvent<float> whileCharging;

		// Token: 0x04006D4E RID: 27982
		public UnityEvent onMaxCharge;

		// Token: 0x04006D4F RID: 27983
		public UnityEvent onChargeCancelled;

		// Token: 0x04006D50 RID: 27984
		[FormerlySerializedAs("onLaunchProjectileShared")]
		public UnityEvent<float> onShoot;

		// Token: 0x04006D51 RID: 27985
		[FormerlySerializedAs("onOwnerLaunchProjectile")]
		public UnityEvent<float> onShootLocal;

		// Token: 0x04006D52 RID: 27986
		[SerializeField]
		private int numberOfProgressSteps;

		// Token: 0x04006D53 RID: 27987
		public UnityEvent<int> onMovedToNextStep;

		// Token: 0x04006D54 RID: 27988
		public UnityEvent<int> onReachedLastProgressStep;

		// Token: 0x04006D55 RID: 27989
		private int currentStep = -1;

		// Token: 0x04006D56 RID: 27990
		private int lastStep = -1;

		// Token: 0x04006D58 RID: 27992
		private bool isCoolingDown;

		// Token: 0x04006D59 RID: 27993
		private bool isPressed;

		// Token: 0x04006D5A RID: 27994
		private bool isAtMaxCharge;

		// Token: 0x04006D5B RID: 27995
		private float lastShotTime;

		// Token: 0x04006D5C RID: 27996
		private bool velocityEstimatorThresholdMet;

		// Token: 0x04006D5D RID: 27997
		private float pressStartedTime;

		// Token: 0x04006D5E RID: 27998
		private float pressStoppedTime;

		// Token: 0x04006D5F RID: 27999
		private TransferrableObject transferrableObject;

		// Token: 0x04006D60 RID: 28000
		private VRRig rig;

		// Token: 0x04006D61 RID: 28001
		private bool isLocal;

		// Token: 0x04006D62 RID: 28002
		private Transform debugShootDirection;

		// Token: 0x02000F59 RID: 3929
		private enum ShootActivator
		{
			// Token: 0x04006D65 RID: 28005
			ButtonReleased,
			// Token: 0x04006D66 RID: 28006
			ButtonPressed,
			// Token: 0x04006D67 RID: 28007
			ButtonStayed,
			// Token: 0x04006D68 RID: 28008
			VelocityEstimatorThreshold
		}

		// Token: 0x02000F5A RID: 3930
		private enum ShootDirection
		{
			// Token: 0x04006D6A RID: 28010
			LaunchTransformRotation,
			// Token: 0x04006D6B RID: 28011
			LineFromRigToLaunchTransform
		}
	}
}
