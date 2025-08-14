using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F0E RID: 3854
	public class DicePhysics : MonoBehaviour
	{
		// Token: 0x06005F6B RID: 24427 RVA: 0x001E1F7C File Offset: 0x001E017C
		public int GetRandomSide()
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 20);
				}
				int value;
				if (this.CheckCosmeticRollOverride(out value))
				{
					return Mathf.Clamp(value, 1, 20);
				}
				return Random.Range(1, 21);
			}
			else
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 6);
				}
				int value2;
				if (this.CheckCosmeticRollOverride(out value2))
				{
					return Mathf.Clamp(value2, 1, 6);
				}
				return Random.Range(1, 7);
			}
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x001E1FFC File Offset: 0x001E01FC
		public Vector3 GetSideDirection(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				int num = Mathf.Clamp(side - 1, 0, 19);
				return this.d20SideDirections[num];
			}
			int num2 = Mathf.Clamp(side - 1, 0, 5);
			return this.d6SideDirections[num2];
		}

		// Token: 0x06005F6D RID: 24429 RVA: 0x001E2048 File Offset: 0x001E0248
		public void StartThrow(DiceHoldable holdable, Vector3 startPosition, Vector3 velocity, float playerScale, int side, double startTime)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.velocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.throwStartTime = ((startTime > 0.0) ? startTime : ((double)Time.time));
			this.throwSettledTime = -1.0;
			this.scale = playerScale;
			this.landingSide = Mathf.Clamp(side, 1, 20);
			this.prevVelocity = Vector3.zero;
			velocity = Vector3.zero;
			base.enabled = true;
		}

		// Token: 0x06005F6E RID: 24430 RVA: 0x001E2130 File Offset: 0x001E0330
		public void EndThrow()
		{
			this.rb.isKinematic = true;
			this.rb.velocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.throwStartTime = -1.0;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			this.onRollFinished.Invoke();
			base.enabled = false;
		}

		// Token: 0x06005F6F RID: 24431 RVA: 0x001E21F4 File Offset: 0x001E03F4
		private void FixedUpdate()
		{
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			float num2 = (float)(num - this.throwStartTime);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f * this.scale, this.surfaceLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 normal = raycastHit.normal;
				Vector3 sideDirection = this.GetSideDirection(this.landingSide);
				Vector3 vector = base.transform.rotation * sideDirection;
				Vector3 normalized = Vector3.Cross(vector, normal).normalized;
				float num3 = Vector3.SignedAngle(vector, normal, normalized);
				float num4 = Mathf.Sign(num3) * this.angleDeltaVsStrengthCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(num3) / 180f));
				float num5 = this.landingTimeVsStrengthCurve.Evaluate(Mathf.Clamp01(num2 / this.landingTime));
				float magnitude = this.rb.velocity.magnitude;
				float num6 = Mathf.Clamp01(1f - Mathf.Min(magnitude, 1f));
				float num7 = Mathf.Max(num5, num6);
				Vector3 torque = this.strength * (num7 * num4 * normalized) - this.damping * this.rb.angularVelocity;
				this.rb.AddTorque(torque, ForceMode.Acceleration);
				if (!this.rb.isKinematic && magnitude < 0.01f && num3 < 2f)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					this.InvokeLandingEffects(this.landingSide);
				}
				else if (!this.rb.isKinematic && num2 > this.landingTime)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					base.transform.rotation = Quaternion.FromToRotation(vector, normal) * base.transform.rotation;
					this.InvokeLandingEffects(this.landingSide);
				}
			}
			if (num2 > this.landingTime + this.postLandingTime || (this.rb.isKinematic && (float)(num - this.throwSettledTime) > this.postLandingTime))
			{
				this.EndThrow();
			}
			this.prevVelocity = this.velocity;
			this.velocity = this.rb.velocity;
		}

		// Token: 0x06005F70 RID: 24432 RVA: 0x001E2440 File Offset: 0x001E0640
		private void OnCollisionEnter(Collision collision)
		{
			float magnitude = collision.impulse.magnitude;
			if (magnitude > 0.001f)
			{
				Vector3 vector = Vector3.Reflect(this.prevVelocity, collision.impulse / magnitude);
				this.rb.velocity = vector * this.bounceAmplification;
			}
		}

		// Token: 0x06005F71 RID: 24433 RVA: 0x001E2494 File Offset: 0x001E0694
		private void InvokeLandingEffects(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (side == 20)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
					return;
				}
			}
			else
			{
				if (side == 6)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
				}
			}
		}

		// Token: 0x06005F72 RID: 24434 RVA: 0x001E24F0 File Offset: 0x001E06F0
		private bool CheckCosmeticRollOverride(out int rollSide)
		{
			if (this.cosmeticRollOverrides.Length != 0)
			{
				if (this.cachedLocalRig == null)
				{
					RigContainer rigContainer;
					if (PhotonNetwork.InRoom && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						this.cachedLocalRig = rigContainer.Rig;
					}
					else
					{
						this.cachedLocalRig = GorillaTagger.Instance.offlineVRRig;
					}
				}
				if (this.cachedLocalRig != null)
				{
					int num = -1;
					for (int i = 0; i < this.cosmeticRollOverrides.Length; i++)
					{
						if (this.cosmeticRollOverrides[i].cosmeticName != null && this.cachedLocalRig.cosmeticSet != null && this.cachedLocalRig.cosmeticSet.HasItem(this.cosmeticRollOverrides[i].cosmeticName) && (!this.cosmeticRollOverrides[i].requireHolding || (EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName)) && this.cosmeticRollOverrides[i].landingSide > num)
						{
							num = this.cosmeticRollOverrides[i].landingSide;
						}
					}
					if (num > 0)
					{
						rollSide = num;
						return true;
					}
				}
			}
			rollSide = 0;
			return false;
		}

		// Token: 0x04006A19 RID: 27161
		[SerializeField]
		private DicePhysics.DiceType diceType = DicePhysics.DiceType.D20;

		// Token: 0x04006A1A RID: 27162
		[SerializeField]
		private float landingTime = 5f;

		// Token: 0x04006A1B RID: 27163
		[SerializeField]
		private float postLandingTime = 2f;

		// Token: 0x04006A1C RID: 27164
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04006A1D RID: 27165
		[SerializeField]
		private AnimationCurve angleDeltaVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006A1E RID: 27166
		[SerializeField]
		private AnimationCurve landingTimeVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006A1F RID: 27167
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04006A20 RID: 27168
		[SerializeField]
		private float damping = 0.5f;

		// Token: 0x04006A21 RID: 27169
		[SerializeField]
		private bool forceLandingSide;

		// Token: 0x04006A22 RID: 27170
		[SerializeField]
		private int forcedLandingSide = 20;

		// Token: 0x04006A23 RID: 27171
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04006A24 RID: 27172
		[SerializeField]
		private float bounceAmplification = 1f;

		// Token: 0x04006A25 RID: 27173
		[SerializeField]
		private DicePhysics.CosmeticRollOverride[] cosmeticRollOverrides;

		// Token: 0x04006A26 RID: 27174
		[SerializeField]
		private UnityEvent onBestRoll;

		// Token: 0x04006A27 RID: 27175
		[SerializeField]
		private UnityEvent onWorstRoll;

		// Token: 0x04006A28 RID: 27176
		[SerializeField]
		private UnityEvent onRollFinished;

		// Token: 0x04006A29 RID: 27177
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04006A2A RID: 27178
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04006A2B RID: 27179
		private VRRig cachedLocalRig;

		// Token: 0x04006A2C RID: 27180
		private DiceHoldable holdableParent;

		// Token: 0x04006A2D RID: 27181
		private double throwStartTime = -1.0;

		// Token: 0x04006A2E RID: 27182
		private double throwSettledTime = -1.0;

		// Token: 0x04006A2F RID: 27183
		private int landingSide;

		// Token: 0x04006A30 RID: 27184
		private float scale;

		// Token: 0x04006A31 RID: 27185
		private Vector3 prevVelocity = Vector3.zero;

		// Token: 0x04006A32 RID: 27186
		private Vector3 velocity = Vector3.zero;

		// Token: 0x04006A33 RID: 27187
		private const float a = 38.833332f;

		// Token: 0x04006A34 RID: 27188
		private const float b = 77.66666f;

		// Token: 0x04006A35 RID: 27189
		private Vector3[] d20SideDirections = new Vector3[]
		{
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up
		};

		// Token: 0x04006A36 RID: 27190
		private Vector3[] d6SideDirections = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		// Token: 0x02000F0F RID: 3855
		private enum DiceType
		{
			// Token: 0x04006A38 RID: 27192
			D6,
			// Token: 0x04006A39 RID: 27193
			D20
		}

		// Token: 0x02000F10 RID: 3856
		[Serializable]
		private struct CosmeticRollOverride
		{
			// Token: 0x04006A3A RID: 27194
			public string cosmeticName;

			// Token: 0x04006A3B RID: 27195
			public int landingSide;

			// Token: 0x04006A3C RID: 27196
			public bool requireHolding;
		}
	}
}
