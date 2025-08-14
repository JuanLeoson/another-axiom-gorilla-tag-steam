using System;
using CjLib;
using GorillaTag;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E14 RID: 3604
	[ExecuteAlways]
	public class UnderwaterCameraEffect : MonoBehaviour
	{
		// Token: 0x06005992 RID: 22930 RVA: 0x001C2270 File Offset: 0x001C0470
		private void SetOffScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 0f, 1f);
			base.transform.localPosition = new Vector3(0f, -(this.frustumPlaneExtents.y + 0.04f), this.distanceFromCamera);
		}

		// Token: 0x06005993 RID: 22931 RVA: 0x001C22DC File Offset: 0x001C04DC
		private void SetFullScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 2f * (this.frustumPlaneExtents.y + 0.04f), 1f);
			base.transform.localPosition = new Vector3(0f, 0f, this.distanceFromCamera);
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x001C234C File Offset: 0x001C054C
		private void OnEnable()
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = Camera.main;
			}
			this.hasTargetCamera = (this.targetCamera != null);
			this.InitializeShaderProperties();
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x001C2380 File Offset: 0x001C0580
		private void Start()
		{
			this.player = GTPlayer.Instance;
			this.cachedAspectRatio = this.targetCamera.aspect;
			this.cachedFov = this.targetCamera.fieldOfView;
			this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			this.SetOffScreenPosition();
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x001C23D4 File Offset: 0x001C05D4
		private void LateUpdate()
		{
			if (!this.hasTargetCamera)
			{
				return;
			}
			if (!this.player)
			{
				return;
			}
			if (this.player.HeadOverlappingWaterVolumes.Count < 1)
			{
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				if (this.planeRenderer.enabled)
				{
					this.planeRenderer.enabled = false;
					this.SetOffScreenPosition();
				}
				if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
				{
					this.underwaterParticleEffect.UpdateParticleEffect(false, ref this.waterSurface);
				}
				return;
			}
			if (this.targetCamera.aspect != this.cachedAspectRatio || this.targetCamera.fieldOfView != this.cachedFov)
			{
				this.cachedAspectRatio = this.targetCamera.aspect;
				this.cachedFov = this.targetCamera.fieldOfView;
				this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			}
			bool flag = false;
			float num = float.MinValue;
			Vector3 position = this.targetCamera.transform.position;
			for (int i = 0; i < this.player.HeadOverlappingWaterVolumes.Count; i++)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (this.player.HeadOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(position, out surfaceQuery, false))
				{
					float num2 = Vector3.Dot(surfaceQuery.surfacePoint - position, surfaceQuery.surfaceNormal);
					if (num2 > num)
					{
						flag = true;
						num = num2;
						this.waterSurface = surfaceQuery;
					}
				}
			}
			if (flag)
			{
				Vector3 inPoint = this.targetCamera.transform.InverseTransformPoint(this.waterSurface.surfacePoint);
				Vector3 inNormal = this.targetCamera.transform.InverseTransformDirection(this.waterSurface.surfaceNormal);
				Plane p = new Plane(inNormal, inPoint);
				Plane p2 = new Plane(Vector3.forward, -this.distanceFromCamera);
				Vector3 vector;
				Vector3 vector2;
				if (this.IntersectPlanes(p2, p, out vector, out vector2))
				{
					Vector3 normalized = Vector3.Cross(vector2, Vector3.forward).normalized;
					float num3 = Vector3.Dot(new Vector3(vector.x, vector.y, 0f), normalized);
					if (num3 > this.frustumPlaneExtents.y + 0.04f)
					{
						this.SetFullScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
					}
					else if (num3 < -(this.frustumPlaneExtents.y + 0.04f))
					{
						this.SetOffScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
					}
					else
					{
						float num4 = num3;
						num4 += this.GetFrustumCoverageDistance(-normalized) + 0.04f;
						float num5 = this.GetFrustumCoverageDistance(vector2) + 0.04f;
						num5 += this.GetFrustumCoverageDistance(-vector2) + 0.04f;
						base.transform.localScale = new Vector3(num5, num4, 1f);
						base.transform.localPosition = normalized * (num3 - num4 * 0.5f) + new Vector3(0f, 0f, this.distanceFromCamera);
						float angle = Vector3.SignedAngle(Vector3.up, normalized, Vector3.forward);
						base.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged);
					}
					if (this.debugDraw)
					{
						Vector3 a = this.targetCamera.transform.TransformPoint(vector);
						Vector3 a2 = this.targetCamera.transform.TransformDirection(vector2);
						DebugUtil.DrawLine(a - 2f * this.frustumPlaneExtents.x * a2, a + 2f * this.frustumPlaneExtents.x * a2, Color.white, false);
					}
				}
				else if (new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint).GetSide(this.targetCamera.transform.position))
				{
					this.SetFullScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
				}
				else
				{
					this.SetOffScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				}
			}
			else
			{
				this.SetOffScreenPosition();
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
			}
			if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
			{
				this.underwaterParticleEffect.UpdateParticleEffect(flag, ref this.waterSurface);
			}
		}

		// Token: 0x06005997 RID: 22935 RVA: 0x001C280C File Offset: 0x001C0A0C
		[DebugOption]
		private void InitializeShaderProperties()
		{
			Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
			Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
			float w = -Vector3.Dot(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
			Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(this.waterSurface.surfaceNormal.x, this.waterSurface.surfaceNormal.y, this.waterSurface.surfaceNormal.z, w));
		}

		// Token: 0x06005998 RID: 22936 RVA: 0x001C288C File Offset: 0x001C0A8C
		private void SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState state)
		{
			if (state != this.cameraOverlapWaterState || state == UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized)
			{
				this.cameraOverlapWaterState = state;
				switch (this.cameraOverlapWaterState)
				{
				case UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized:
				case UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater:
					Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.EnableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				}
			}
			if (this.cameraOverlapWaterState == UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged)
			{
				Plane plane = new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
				Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
			}
		}

		// Token: 0x06005999 RID: 22937 RVA: 0x001C296C File Offset: 0x001C0B6C
		private void CalculateFrustumPlaneBounds(float fieldOfView, float aspectRatio)
		{
			float num = Mathf.Tan(0.017453292f * fieldOfView * 0.5f) * this.distanceFromCamera;
			float num2 = aspectRatio * num + 0.04f;
			float num3 = 1f / aspectRatio * num + 0.04f;
			this.frustumPlaneExtents = new Vector2(num2, num3);
			this.frustumPlaneCornersLocal[0] = new Vector3(-num2, -num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[1] = new Vector3(-num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[2] = new Vector3(num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[3] = new Vector3(num2, -num3, this.distanceFromCamera);
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x001C2A24 File Offset: 0x001C0C24
		private bool IntersectPlanes(Plane p1, Plane p2, out Vector3 point, out Vector3 direction)
		{
			direction = Vector3.Cross(p1.normal, p2.normal);
			float num = Vector3.Dot(direction, direction);
			if (num < Mathf.Epsilon)
			{
				point = Vector3.zero;
				return false;
			}
			point = Vector3.Cross(direction, p1.distance * p2.normal - p2.distance * p1.normal) / num;
			return true;
		}

		// Token: 0x0600599B RID: 22939 RVA: 0x001C2AB8 File Offset: 0x001C0CB8
		private float GetFrustumCoverageDistance(Vector3 localDirection)
		{
			float num = float.MinValue;
			for (int i = 0; i < this.frustumPlaneCornersLocal.Length; i++)
			{
				float num2 = Vector3.Dot(this.frustumPlaneCornersLocal[i], localDirection);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x04006422 RID: 25634
		private const float edgeBuffer = 0.04f;

		// Token: 0x04006423 RID: 25635
		[SerializeField]
		private Camera targetCamera;

		// Token: 0x04006424 RID: 25636
		[SerializeField]
		private MeshRenderer planeRenderer;

		// Token: 0x04006425 RID: 25637
		[SerializeField]
		private UnderwaterParticleEffects underwaterParticleEffect;

		// Token: 0x04006426 RID: 25638
		[SerializeField]
		private float distanceFromCamera = 0.02f;

		// Token: 0x04006427 RID: 25639
		[SerializeField]
		[DebugOption]
		private bool debugDraw;

		// Token: 0x04006428 RID: 25640
		private float cachedAspectRatio = 1f;

		// Token: 0x04006429 RID: 25641
		private float cachedFov = 90f;

		// Token: 0x0400642A RID: 25642
		private readonly Vector3[] frustumPlaneCornersLocal = new Vector3[4];

		// Token: 0x0400642B RID: 25643
		private Vector2 frustumPlaneExtents;

		// Token: 0x0400642C RID: 25644
		private GTPlayer player;

		// Token: 0x0400642D RID: 25645
		private WaterVolume.SurfaceQuery waterSurface;

		// Token: 0x0400642E RID: 25646
		private const string kShaderKeyword_GlobalCameraTouchingWater = "_GLOBAL_CAMERA_TOUCHING_WATER";

		// Token: 0x0400642F RID: 25647
		private const string kShaderKeyword_GlobalCameraFullyUnderwater = "_GLOBAL_CAMERA_FULLY_UNDERWATER";

		// Token: 0x04006430 RID: 25648
		private int shaderParam_GlobalCameraOverlapWaterSurfacePlane = Shader.PropertyToID("_GlobalCameraOverlapWaterSurfacePlane");

		// Token: 0x04006431 RID: 25649
		private bool hasTargetCamera;

		// Token: 0x04006432 RID: 25650
		[DebugReadout]
		private UnderwaterCameraEffect.CameraOverlapWaterState cameraOverlapWaterState;

		// Token: 0x02000E15 RID: 3605
		private enum CameraOverlapWaterState
		{
			// Token: 0x04006434 RID: 25652
			Uninitialized,
			// Token: 0x04006435 RID: 25653
			OutOfWater,
			// Token: 0x04006436 RID: 25654
			PartiallySubmerged,
			// Token: 0x04006437 RID: 25655
			FullySubmerged
		}
	}
}
