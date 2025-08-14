using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F3D RID: 3901
	public class Dreidel : MonoBehaviour
	{
		// Token: 0x060060AD RID: 24749 RVA: 0x001EBB5C File Offset: 0x001E9D5C
		public bool TrySetIdle()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface || this.state == Dreidel.State.Fallen)
			{
				this.StartIdle();
				return true;
			}
			return false;
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x001EBB81 File Offset: 0x001E9D81
		public bool TryCheckForSurfaces()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface)
			{
				this.StartFindingSurfaces();
				return true;
			}
			return false;
		}

		// Token: 0x060060AF RID: 24751 RVA: 0x001EBB9D File Offset: 0x001E9D9D
		public void Spin()
		{
			this.StartSpin();
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x001EBBA8 File Offset: 0x001E9DA8
		public bool TryGetSpinStartData(out Vector3 surfacePoint, out Vector3 surfaceNormal, out float randomDuration, out Dreidel.Side randomSide, out Dreidel.Variation randomVariation, out double startTime)
		{
			if (this.canStartSpin)
			{
				surfacePoint = this.surfacePlanePoint;
				surfaceNormal = this.surfacePlaneNormal;
				randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
				randomSide = (Dreidel.Side)Random.Range(0, 4);
				randomVariation = (Dreidel.Variation)Random.Range(0, 5);
				startTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0);
				return true;
			}
			surfacePoint = Vector3.zero;
			surfaceNormal = Vector3.zero;
			randomDuration = 0f;
			randomSide = Dreidel.Side.Shin;
			randomVariation = Dreidel.Variation.Tumble;
			startTime = -1.0;
			return false;
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x001EBC54 File Offset: 0x001E9E54
		public void SetSpinStartData(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			this.surfacePlanePoint = surfacePoint;
			this.surfacePlaneNormal = surfaceNormal;
			this.spinTime = duration;
			this.spinStartTime = startTime;
			this.spinCounterClockwise = counterClockwise;
			this.landingSide = side;
			this.landingVariation = variation;
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x001EBC8C File Offset: 0x001E9E8C
		private void LateUpdate()
		{
			float deltaTime = Time.deltaTime;
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.canStartSpin = false;
			switch (this.state)
			{
			default:
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			case Dreidel.State.FindingSurface:
			{
				float num2 = (GTPlayer.Instance != null) ? GTPlayer.Instance.scale : 1f;
				Vector3 down = Vector3.down;
				Vector3 origin = base.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
				float maxDistance = (3f * this.surfaceCheckDistance + -this.bottomPointOffset.y) * num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(origin, down, out raycastHit, maxDistance, this.surfaceLayers.value, QueryTriggerInteraction.Ignore) && Vector3.Dot(raycastHit.normal, Vector3.up) > this.surfaceUprightThreshold && Vector3.Dot(raycastHit.normal, this.spinTransform.up) > this.surfaceDreidelAngleThreshold)
				{
					this.canStartSpin = true;
					this.surfacePlanePoint = raycastHit.point;
					this.surfacePlaneNormal = raycastHit.normal;
					this.AlignToSurfacePlane();
					this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
					this.UpdateSpinTransform();
					return;
				}
				this.canStartSpin = false;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			}
			case Dreidel.State.Spinning:
			{
				float num3 = Mathf.Clamp01((float)(num - this.stateStartTime) / this.spinTime);
				this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, num3);
				float num4 = this.spinCounterClockwise ? -1f : 1f;
				this.spinAngle += num4 * this.spinSpeed * 360f * deltaTime;
				float num5 = this.tiltWobble;
				float num6 = Mathf.Sin(this.spinWobbleFrequency * 2f * 3.1415927f * (float)(num - this.stateStartTime));
				float t = 0.5f * num6 + 0.5f;
				this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * num3, this.spinWobbleAmplitude * num3, t);
				if (this.landingTiltTarget.y == 0f)
				{
					if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble;
					}
					else
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
					}
				}
				else if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble;
				}
				else
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
				}
				float num7 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, num3) + num6 * this.pathTurnRateSinOffset;
				if (this.spinCounterClockwise)
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num7 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				else
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num7 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				if (num3 - Mathf.Epsilon >= 1f && this.tiltWobble > 0.9f * this.spinWobbleAmplitude && num5 < this.tiltWobble)
				{
					this.StartFall();
					return;
				}
				break;
			}
			case Dreidel.State.Falling:
			{
				float num8 = this.fallTimeTumble;
				Dreidel.Variation variation = this.landingVariation;
				if (variation <= Dreidel.Variation.Smooth || variation - Dreidel.Variation.Bounce > 2)
				{
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num9 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num9 * this.spinSpeed * 360f * deltaTime;
					float angularFrequency = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency;
					float dampingRatio = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio;
					float angularFrequency2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrequency;
					float dampingRatio2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio;
					this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, angularFrequency, dampingRatio, deltaTime);
					this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, angularFrequency2, dampingRatio2, deltaTime);
				}
				else
				{
					bool flag = this.landingVariation != Dreidel.Variation.Bounce;
					bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
					float num10 = flag ? this.slowTurnSwitchTime : this.bounceFallSwitchTime;
					if (flag)
					{
						num8 = this.fallTimeSlowTurn;
					}
					if (num - this.stateStartTime < (double)num10)
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
					}
					else
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						if (flag2)
						{
							if (!this.falseTargetReached && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49f)
							{
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
							}
							else
							{
								this.falseTargetReached = true;
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
							}
						}
						else if (flag && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.45f)
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
						}
						else
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
						}
					}
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num11 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num11 * this.spinSpeed * 360f * deltaTime;
				}
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				float num12 = (float)(num - this.stateStartTime);
				if (num12 > num8)
				{
					if (!this.hasLanded)
					{
						this.hasLanded = true;
						if (this.landingSide == Dreidel.Side.Gimel)
						{
							this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
							this.gimelConfetti.gameObject.SetActive(true);
							this.audioSource.GTPlayOneShot(this.gimelConfettiSound, 1f);
						}
					}
					if (num12 > num8 + this.respawnTimeAfterLanding)
					{
						this.StartIdle();
					}
				}
				break;
			}
			case Dreidel.State.Fallen:
				break;
			}
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x001EC4B8 File Offset: 0x001EA6B8
		private void StartIdle()
		{
			this.state = Dreidel.State.Idle;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x001EC594 File Offset: 0x001EA794
		private void StartFindingSurfaces()
		{
			this.state = Dreidel.State.FindingSurface;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x060060B5 RID: 24757 RVA: 0x001EC670 File Offset: 0x001EA870
		private void StartSpin()
		{
			this.state = Dreidel.State.Spinning;
			this.stateStartTime = ((this.spinStartTime > 0.0) ? this.spinStartTime : ((double)Time.time));
			this.canStartSpin = false;
			this.spinSpeed = this.spinSpeedStart;
			this.tiltWobble = 0f;
			this.audioSource.loop = true;
			this.audioSource.clip = this.spinLoopAudio;
			this.audioSource.GTPlay();
			this.gimelConfetti.gameObject.SetActive(false);
			this.AlignToSurfacePlane();
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
		}

		// Token: 0x060060B6 RID: 24758 RVA: 0x001EC738 File Offset: 0x001EA938
		private void StartFall()
		{
			this.state = Dreidel.State.Falling;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.falseTargetReached = false;
			this.hasLanded = false;
			if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
			{
				if (this.spinCounterClockwise)
				{
					this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
				else
				{
					this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
			}
			else if (this.spinCounterClockwise)
			{
				this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			else
			{
				this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			this.spinSpeedSpring.Reset(this.spinSpeed, 0f);
			this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0f);
			this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0f);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.audioSource.loop = false;
			this.audioSource.GTPlayOneShot(this.fallSound, 1f);
			this.gimelConfetti.gameObject.SetActive(false);
		}

		// Token: 0x060060B7 RID: 24759 RVA: 0x001EC888 File Offset: 0x001EAA88
		private Vector3 GetGroundContactPoint()
		{
			Vector3 position = this.spinTransform.position;
			this.dreidelCollider.enabled = true;
			Vector3 vector = this.dreidelCollider.ClosestPoint(position - base.transform.up);
			this.dreidelCollider.enabled = false;
			float num = Vector3.Dot(vector - position, this.spinTransform.up);
			if (num > 0f)
			{
				vector -= num * this.spinTransform.up;
			}
			return this.spinTransform.InverseTransformPoint(vector);
		}

		// Token: 0x060060B8 RID: 24760 RVA: 0x001EC91C File Offset: 0x001EAB1C
		private void GetTiltVectorsForSideWithPrev(Dreidel.Side side, out Vector2 sideTilt, out Vector2 prevSideTilt)
		{
			int num = (side <= Dreidel.Side.Shin) ? 3 : (side - Dreidel.Side.Hey);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				prevSideTilt = this.landingTiltValues[num];
				prevSideTilt.x = sideTilt.x;
				return;
			}
			prevSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = prevSideTilt.x;
		}

		// Token: 0x060060B9 RID: 24761 RVA: 0x001EC9A0 File Offset: 0x001EABA0
		private void GetTiltVectorsForSideWithNext(Dreidel.Side side, out Vector2 sideTilt, out Vector2 nextSideTilt)
		{
			int num = (int)((side + 1) % Dreidel.Side.Count);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				nextSideTilt = this.landingTiltValues[num];
				nextSideTilt.x = sideTilt.x;
				return;
			}
			nextSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = nextSideTilt.x;
		}

		// Token: 0x060060BA RID: 24762 RVA: 0x001ECA1C File Offset: 0x001EAC1C
		private void AlignToSurfacePlane()
		{
			Vector3 forward = Vector3.forward;
			if (Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.9999f)
			{
				Vector3 axis = Vector3.Cross(this.surfacePlaneNormal, Vector3.up);
				forward = Quaternion.AngleAxis(90f, axis) * this.surfacePlaneNormal;
			}
			Quaternion rotation = Quaternion.LookRotation(forward, this.surfacePlaneNormal);
			base.transform.position = this.surfacePlanePoint;
			base.transform.rotation = rotation;
		}

		// Token: 0x060060BB RID: 24763 RVA: 0x001ECA98 File Offset: 0x001EAC98
		private void UpdateSpinTransform()
		{
			Vector3 position = this.spinTransform.position;
			Vector3 groundContactPoint = this.GetGroundContactPoint();
			Vector3 position2 = this.groundPointSpring.TrackDampingRatio(groundContactPoint, this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime);
			Vector3 b = this.spinTransform.TransformPoint(position2);
			Quaternion rhs = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
			this.spinAxis = base.transform.InverseTransformDirection(base.transform.up);
			Quaternion lhs = Quaternion.AngleAxis(this.spinAngle, this.spinAxis);
			this.spinTransform.localRotation = lhs * rhs;
			Vector3 a = base.transform.InverseTransformVector(Vector3.Dot(position - b, base.transform.up) * base.transform.up);
			this.spinTransform.localPosition = a + this.pathOffset;
			this.spinTransform.TransformPoint(this.bottomPointOffset);
		}

		// Token: 0x04006C50 RID: 27728
		[Header("References")]
		[SerializeField]
		private Transform spinTransform;

		// Token: 0x04006C51 RID: 27729
		[SerializeField]
		private MeshCollider dreidelCollider;

		// Token: 0x04006C52 RID: 27730
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006C53 RID: 27731
		[SerializeField]
		private AudioClip spinLoopAudio;

		// Token: 0x04006C54 RID: 27732
		[SerializeField]
		private AudioClip fallSound;

		// Token: 0x04006C55 RID: 27733
		[SerializeField]
		private AudioClip gimelConfettiSound;

		// Token: 0x04006C56 RID: 27734
		[SerializeField]
		private ParticleSystem gimelConfetti;

		// Token: 0x04006C57 RID: 27735
		[Header("Offsets")]
		[SerializeField]
		private Vector3 centerOfMassOffset = Vector3.zero;

		// Token: 0x04006C58 RID: 27736
		[SerializeField]
		private Vector3 bottomPointOffset = Vector3.zero;

		// Token: 0x04006C59 RID: 27737
		[SerializeField]
		private Vector2 bodyRect = Vector2.one;

		// Token: 0x04006C5A RID: 27738
		[SerializeField]
		private float confettiHeight = 0.125f;

		// Token: 0x04006C5B RID: 27739
		[Header("Surface Detection")]
		[SerializeField]
		private float surfaceCheckDistance = 0.15f;

		// Token: 0x04006C5C RID: 27740
		[SerializeField]
		private float surfaceUprightThreshold = 0.5f;

		// Token: 0x04006C5D RID: 27741
		[SerializeField]
		private float surfaceDreidelAngleThreshold = 0.9f;

		// Token: 0x04006C5E RID: 27742
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04006C5F RID: 27743
		[Header("Spin Paramss")]
		[SerializeField]
		private float spinSpeedStart = 2f;

		// Token: 0x04006C60 RID: 27744
		[SerializeField]
		private float spinSpeedEnd = 1f;

		// Token: 0x04006C61 RID: 27745
		[SerializeField]
		private float spinTime = 10f;

		// Token: 0x04006C62 RID: 27746
		[SerializeField]
		private Vector2 spinTimeRange = new Vector2(7f, 12f);

		// Token: 0x04006C63 RID: 27747
		[SerializeField]
		private float spinWobbleFrequency = 0.1f;

		// Token: 0x04006C64 RID: 27748
		[SerializeField]
		private float spinWobbleAmplitude = 0.01f;

		// Token: 0x04006C65 RID: 27749
		[SerializeField]
		private float spinWobbleAmplitudeEndMin = 0.01f;

		// Token: 0x04006C66 RID: 27750
		[SerializeField]
		private float tiltFrontBack;

		// Token: 0x04006C67 RID: 27751
		[SerializeField]
		private float tiltLeftRight;

		// Token: 0x04006C68 RID: 27752
		[SerializeField]
		private float groundTrackingDampingRatio = 0.9f;

		// Token: 0x04006C69 RID: 27753
		[SerializeField]
		private float groundTrackingFrequency = 1f;

		// Token: 0x04006C6A RID: 27754
		[Header("Motion Path")]
		[SerializeField]
		private float pathMoveSpeed = 0.1f;

		// Token: 0x04006C6B RID: 27755
		[SerializeField]
		private float pathStartTurnRate = 360f;

		// Token: 0x04006C6C RID: 27756
		[SerializeField]
		private float pathEndTurnRate = 90f;

		// Token: 0x04006C6D RID: 27757
		[SerializeField]
		private float pathTurnRateSinOffset = 180f;

		// Token: 0x04006C6E RID: 27758
		[Header("Falling Params")]
		[SerializeField]
		private float spinSpeedStopRate = 1f;

		// Token: 0x04006C6F RID: 27759
		[SerializeField]
		private float tumbleFallDampingRatio = 0.4f;

		// Token: 0x04006C70 RID: 27760
		[SerializeField]
		private float tumbleFallFrequency = 6f;

		// Token: 0x04006C71 RID: 27761
		[SerializeField]
		private float tumbleFallFrontBackDampingRatio = 0.4f;

		// Token: 0x04006C72 RID: 27762
		[SerializeField]
		private float tumbleFallFrontBackFrequency = 6f;

		// Token: 0x04006C73 RID: 27763
		[SerializeField]
		private float smoothFallDampingRatio = 0.9f;

		// Token: 0x04006C74 RID: 27764
		[SerializeField]
		private float smoothFallFrequency = 2f;

		// Token: 0x04006C75 RID: 27765
		[SerializeField]
		private float slowTurnDampingRatio = 0.9f;

		// Token: 0x04006C76 RID: 27766
		[SerializeField]
		private float slowTurnFrequency = 2f;

		// Token: 0x04006C77 RID: 27767
		[SerializeField]
		private float bounceFallSwitchTime = 0.5f;

		// Token: 0x04006C78 RID: 27768
		[SerializeField]
		private float slowTurnSwitchTime = 0.5f;

		// Token: 0x04006C79 RID: 27769
		[SerializeField]
		private float respawnTimeAfterLanding = 3f;

		// Token: 0x04006C7A RID: 27770
		[SerializeField]
		private float fallTimeTumble = 3f;

		// Token: 0x04006C7B RID: 27771
		[SerializeField]
		private float fallTimeSlowTurn = 5f;

		// Token: 0x04006C7C RID: 27772
		private Dreidel.State state;

		// Token: 0x04006C7D RID: 27773
		private double stateStartTime;

		// Token: 0x04006C7E RID: 27774
		private float spinSpeed;

		// Token: 0x04006C7F RID: 27775
		private float spinAngle;

		// Token: 0x04006C80 RID: 27776
		private Vector3 spinAxis = Vector3.up;

		// Token: 0x04006C81 RID: 27777
		private bool canStartSpin;

		// Token: 0x04006C82 RID: 27778
		private double spinStartTime = -1.0;

		// Token: 0x04006C83 RID: 27779
		private float tiltWobble;

		// Token: 0x04006C84 RID: 27780
		private bool falseTargetReached;

		// Token: 0x04006C85 RID: 27781
		private bool hasLanded;

		// Token: 0x04006C86 RID: 27782
		private Vector3 pathOffset = Vector3.zero;

		// Token: 0x04006C87 RID: 27783
		private Vector3 pathDir = Vector3.forward;

		// Token: 0x04006C88 RID: 27784
		private Vector3 surfacePlanePoint;

		// Token: 0x04006C89 RID: 27785
		private Vector3 surfacePlaneNormal;

		// Token: 0x04006C8A RID: 27786
		private FloatSpring tiltFrontBackSpring;

		// Token: 0x04006C8B RID: 27787
		private FloatSpring tiltLeftRightSpring;

		// Token: 0x04006C8C RID: 27788
		private FloatSpring spinSpeedSpring;

		// Token: 0x04006C8D RID: 27789
		private Vector3Spring groundPointSpring;

		// Token: 0x04006C8E RID: 27790
		private Vector2[] landingTiltValues = new Vector2[]
		{
			new Vector2(1f, -1f),
			new Vector2(1f, 0f),
			new Vector2(-1f, 1f),
			new Vector2(-1f, 0f)
		};

		// Token: 0x04006C8F RID: 27791
		private Vector2 landingTiltLeadingTarget = Vector2.zero;

		// Token: 0x04006C90 RID: 27792
		private Vector2 landingTiltTarget = Vector2.zero;

		// Token: 0x04006C91 RID: 27793
		[Header("Debug Params")]
		[SerializeField]
		private Dreidel.Side landingSide;

		// Token: 0x04006C92 RID: 27794
		[SerializeField]
		private Dreidel.Variation landingVariation;

		// Token: 0x04006C93 RID: 27795
		[SerializeField]
		private bool spinCounterClockwise;

		// Token: 0x04006C94 RID: 27796
		[SerializeField]
		private bool debugDraw;

		// Token: 0x02000F3E RID: 3902
		private enum State
		{
			// Token: 0x04006C96 RID: 27798
			Idle,
			// Token: 0x04006C97 RID: 27799
			FindingSurface,
			// Token: 0x04006C98 RID: 27800
			Spinning,
			// Token: 0x04006C99 RID: 27801
			Falling,
			// Token: 0x04006C9A RID: 27802
			Fallen
		}

		// Token: 0x02000F3F RID: 3903
		public enum Side
		{
			// Token: 0x04006C9C RID: 27804
			Shin,
			// Token: 0x04006C9D RID: 27805
			Hey,
			// Token: 0x04006C9E RID: 27806
			Gimel,
			// Token: 0x04006C9F RID: 27807
			Nun,
			// Token: 0x04006CA0 RID: 27808
			Count
		}

		// Token: 0x02000F40 RID: 3904
		public enum Variation
		{
			// Token: 0x04006CA2 RID: 27810
			Tumble,
			// Token: 0x04006CA3 RID: 27811
			Smooth,
			// Token: 0x04006CA4 RID: 27812
			Bounce,
			// Token: 0x04006CA5 RID: 27813
			SlowTurn,
			// Token: 0x04006CA6 RID: 27814
			FalseSlowTurn,
			// Token: 0x04006CA7 RID: 27815
			Count
		}
	}
}
