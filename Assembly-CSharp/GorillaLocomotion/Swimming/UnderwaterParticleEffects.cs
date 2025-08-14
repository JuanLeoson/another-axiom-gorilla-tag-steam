using System;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E16 RID: 3606
	public class UnderwaterParticleEffects : MonoBehaviour
	{
		// Token: 0x0600599D RID: 22941 RVA: 0x001C2B48 File Offset: 0x001C0D48
		public void UpdateParticleEffect(bool waterSurfaceDetected, ref WaterVolume.SurfaceQuery waterSurface)
		{
			GTPlayer instance = GTPlayer.Instance;
			Plane plane = new Plane(waterSurface.surfaceNormal, waterSurface.surfacePoint);
			if (waterSurfaceDetected && plane.GetDistanceToPoint(instance.headCollider.transform.position) < instance.headCollider.radius)
			{
				this.underwaterFloaterParticles.gameObject.SetActive(true);
				Vector3 averagedVelocity = instance.AveragedVelocity;
				float magnitude = averagedVelocity.magnitude;
				Vector3 a = (magnitude > 0.001f) ? (averagedVelocity / magnitude) : this.playerCamera.transform.forward;
				float d = this.floaterSpeedVsOffsetDist.Evaluate(Mathf.Clamp(magnitude, this.floaterSpeedVsOffsetDistMinMax.x, this.floaterSpeedVsOffsetDistMinMax.y));
				Quaternion rotation = this.playerCamera.transform.rotation;
				Vector3 vector = this.playerCamera.transform.position + this.playerCamera.transform.rotation * this.floaterParticleBaseOffset + a * d;
				Vector3 vector2 = vector + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
				Vector3 vector3 = vector + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
				float num = this.floaterParticleBoxExtents.z * 2f;
				float num2 = plane.GetDistanceToPoint(vector2);
				float num3 = plane.GetDistanceToPoint(vector3);
				Quaternion rotation2 = rotation;
				Vector3 vector4 = vector;
				if (num2 > 0f || num3 > 0f)
				{
					if (vector2.y < vector3.y)
					{
						if (num2 > 0f)
						{
							vector2 -= plane.normal * num2;
							num2 = 0f;
						}
						Vector3 rhs = (new Vector3(vector3.x, vector2.y, vector3.z) - vector2).normalized * num;
						Vector3 axis = Vector3.Cross(vector3 - vector2, rhs);
						rotation2 = Quaternion.AngleAxis((Mathf.Asin((vector3.y - vector2.y) / num) - Mathf.Asin(-num2 / num)) * 57.29578f, axis) * this.playerCamera.transform.rotation;
						vector4 = vector2 + rotation2 * new Vector3(0f, -this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					}
					else
					{
						if (num3 > 0f)
						{
							vector3 -= plane.normal * num3;
							num3 = 0f;
						}
						Vector3 rhs2 = (new Vector3(vector2.x, vector3.y, vector2.z) - vector3).normalized * num;
						Vector3 axis2 = Vector3.Cross(vector2 - vector3, rhs2);
						rotation2 = Quaternion.AngleAxis((Mathf.Asin((vector2.y - vector3.y) / num) - Mathf.Asin(-num3 / num)) * 57.29578f, axis2) * this.playerCamera.transform.rotation;
						vector4 = vector3 + rotation2 * new Vector3(0f, -this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					}
				}
				if (this.IsValid(vector4))
				{
					this.underwaterFloaterParticles.transform.rotation = rotation2;
					this.underwaterFloaterParticles.transform.position = vector4;
				}
				else
				{
					this.underwaterFloaterParticles.gameObject.SetActive(false);
				}
				if (this.debugDraw)
				{
					vector2 = vector + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					vector3 = vector + rotation * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					DebugUtil.DrawSphere(vector2, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawSphere(vector3, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawLine(vector2, vector3, Color.red, false);
					vector2 = vector4 + rotation2 * new Vector3(0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
					vector3 = vector4 + rotation2 * new Vector3(0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
					DebugUtil.DrawSphere(vector2, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawSphere(vector3, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
					DebugUtil.DrawLine(vector2, vector3, Color.green, false);
					return;
				}
			}
			else
			{
				this.underwaterFloaterParticles.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600599E RID: 22942 RVA: 0x001C307D File Offset: 0x001C127D
		private bool IsValid(Vector3 vector)
		{
			return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
		}

		// Token: 0x04006438 RID: 25656
		public ParticleSystem underwaterFloaterParticles;

		// Token: 0x04006439 RID: 25657
		public ParticleSystem underwaterBubbleParticles;

		// Token: 0x0400643A RID: 25658
		public Camera playerCamera;

		// Token: 0x0400643B RID: 25659
		public Vector3 floaterParticleBoxExtents = Vector3.one;

		// Token: 0x0400643C RID: 25660
		public Vector3 floaterParticleBaseOffset = Vector3.forward;

		// Token: 0x0400643D RID: 25661
		public AnimationCurve floaterSpeedVsOffsetDist = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400643E RID: 25662
		public Vector2 floaterSpeedVsOffsetDistMinMax = new Vector2(0f, 1f);

		// Token: 0x0400643F RID: 25663
		private bool debugDraw;
	}
}
