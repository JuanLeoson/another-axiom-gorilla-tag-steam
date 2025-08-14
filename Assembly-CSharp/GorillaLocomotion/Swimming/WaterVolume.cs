using System;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E1B RID: 3611
	[RequireComponent(typeof(Collider))]
	public class WaterVolume : BaseGuidedRefTargetMono
	{
		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x060059B0 RID: 22960 RVA: 0x001C3A88 File Offset: 0x001C1C88
		// (remove) Token: 0x060059B1 RID: 22961 RVA: 0x001C3AC0 File Offset: 0x001C1CC0
		public event WaterVolume.WaterVolumeEvent ColliderEnteredVolume;

		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x060059B2 RID: 22962 RVA: 0x001C3AF8 File Offset: 0x001C1CF8
		// (remove) Token: 0x060059B3 RID: 22963 RVA: 0x001C3B30 File Offset: 0x001C1D30
		public event WaterVolume.WaterVolumeEvent ColliderExitedVolume;

		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x060059B4 RID: 22964 RVA: 0x001C3B68 File Offset: 0x001C1D68
		// (remove) Token: 0x060059B5 RID: 22965 RVA: 0x001C3BA0 File Offset: 0x001C1DA0
		public event WaterVolume.WaterVolumeEvent ColliderEnteredWater;

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x060059B6 RID: 22966 RVA: 0x001C3BD8 File Offset: 0x001C1DD8
		// (remove) Token: 0x060059B7 RID: 22967 RVA: 0x001C3C10 File Offset: 0x001C1E10
		public event WaterVolume.WaterVolumeEvent ColliderExitedWater;

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x060059B8 RID: 22968 RVA: 0x001C3C45 File Offset: 0x001C1E45
		public GTPlayer.LiquidType LiquidType
		{
			get
			{
				return this.liquidType;
			}
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060059B9 RID: 22969 RVA: 0x001C3C4D File Offset: 0x001C1E4D
		public WaterCurrent Current
		{
			get
			{
				return this.waterCurrent;
			}
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x060059BA RID: 22970 RVA: 0x001C3C55 File Offset: 0x001C1E55
		public WaterParameters Parameters
		{
			get
			{
				return this.waterParams;
			}
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x060059BB RID: 22971 RVA: 0x001C3C60 File Offset: 0x001C1E60
		private VRRig PlayerVRRig
		{
			get
			{
				if (this.playerVRRig == null)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (instance != null)
					{
						this.playerVRRig = instance.offlineVRRig;
					}
				}
				return this.playerVRRig;
			}
		}

		// Token: 0x060059BC RID: 22972 RVA: 0x001C3C9C File Offset: 0x001C1E9C
		public bool GetSurfaceQueryForPoint(Vector3 point, out WaterVolume.SurfaceQuery result, bool debugDraw = false)
		{
			result = default(WaterVolume.SurfaceQuery);
			if (!this.isStationary)
			{
				float num = float.MinValue;
				float num2 = float.MaxValue;
				for (int i = 0; i < this.volumeColliders.Count; i++)
				{
					float y = this.volumeColliders[i].bounds.max.y;
					float y2 = this.volumeColliders[i].bounds.min.y;
					if (y > num)
					{
						num = y;
					}
					if (y2 < num2)
					{
						num2 = y2;
					}
				}
				this.volumeMaxHeight = num;
				this.volumeMinHeight = num2;
			}
			Ray ray = new Ray(new Vector3(point.x, this.volumeMaxHeight, point.z), Vector3.down);
			Ray ray2 = new Ray(new Vector3(point.x, this.volumeMinHeight, point.z), Vector3.up);
			float num3 = this.volumeMaxHeight - this.volumeMinHeight;
			float num4 = float.MinValue;
			float num5 = float.MaxValue;
			bool flag = false;
			bool flag2 = false;
			float num6 = 0f;
			for (int j = 0; j < this.surfaceColliders.Count; j++)
			{
				bool enabled = this.surfaceColliders[j].enabled;
				this.surfaceColliders[j].enabled = true;
				RaycastHit hit;
				if (this.surfaceColliders[j].Raycast(ray, out hit, num3) && hit.point.y > num4 && this.HitOutsideSurfaceOfMesh(ray.direction, this.surfaceColliders[j], hit))
				{
					num4 = hit.point.y;
					flag = true;
					result.surfacePoint = hit.point;
					result.surfaceNormal = hit.normal;
				}
				RaycastHit hit2;
				if (this.surfaceColliders[j].Raycast(ray2, out hit2, num3) && hit2.point.y < num5 && this.HitOutsideSurfaceOfMesh(ray2.direction, this.surfaceColliders[j], hit2))
				{
					num5 = hit2.point.y;
					flag2 = true;
					num6 = hit2.point.y;
				}
				this.surfaceColliders[j].enabled = enabled;
			}
			if (!flag && this.surfacePlane != null)
			{
				flag = true;
				result.surfacePoint = point - Vector3.Dot(point - this.surfacePlane.position, this.surfacePlane.up) * this.surfacePlane.up;
				result.surfaceNormal = this.surfacePlane.up;
			}
			if (flag && flag2)
			{
				result.maxDepth = result.surfacePoint.y - num6;
			}
			else if (flag)
			{
				result.maxDepth = result.surfacePoint.y - this.volumeMinHeight;
			}
			else
			{
				result.maxDepth = this.volumeMaxHeight - this.volumeMinHeight;
			}
			if (debugDraw)
			{
				if (flag)
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.green, false);
					DebugUtil.DrawSphere(result.surfacePoint, 0.001f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
				}
				else
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.red, false);
				}
				if (flag2)
				{
					DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num3, Color.yellow, false);
					DebugUtil.DrawSphere(new Vector3(result.surfacePoint.x, num6, result.surfacePoint.z), 0.001f, 12, 12, Color.yellow, false, DebugUtil.Style.SolidColor);
				}
			}
			return flag;
		}

		// Token: 0x060059BD RID: 22973 RVA: 0x001C407C File Offset: 0x001C227C
		private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
		{
			if (!WaterVolume.meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshTris))
			{
				this.sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
				WaterVolume.meshTrianglesDict.Add(meshCollider.sharedMesh, this.sharedMeshTris);
			}
			if (!WaterVolume.meshVertsDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshVerts))
			{
				this.sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
				WaterVolume.meshVertsDict.Add(meshCollider.sharedMesh, this.sharedMeshVerts);
			}
			Vector3 b = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3]];
			Vector3 a = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 1]];
			Vector3 a2 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 2]];
			Vector3 vector = meshCollider.transform.TransformDirection(Vector3.Cross(a - b, a2 - b).normalized);
			bool flag = Vector3.Dot(castDir, vector) < 0f;
			if (this.debugDrawSurfaceCast)
			{
				Color color = flag ? Color.blue : Color.red;
				DebugUtil.DrawLine(hit.point, hit.point + vector * 0.3f, color, false);
			}
			return flag;
		}

		// Token: 0x060059BE RID: 22974 RVA: 0x001C41F0 File Offset: 0x001C23F0
		private void DebugDrawMeshColliderHitTriangle(RaycastHit hit)
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider != null)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				int[] triangles = sharedMesh.triangles;
				Vector3[] vertices = sharedMesh.vertices;
				Vector3 vector = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
				Vector3 vector2 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
				Vector3 vector3 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				float d = 0.2f;
				DebugUtil.DrawLine(vector, vector + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector2 + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector3, vector3 + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector, vector2, Color.blue, false);
				DebugUtil.DrawLine(vector, vector3, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector3, Color.blue, false);
			}
		}

		// Token: 0x060059BF RID: 22975 RVA: 0x001C433C File Offset: 0x001C253C
		public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
		{
			if (this.triggerCollider != null)
			{
				return Physics.Raycast(new Ray(origin, direction), out hit, distance, layerMask, QueryTriggerInteraction.Collide);
			}
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x060059C0 RID: 22976 RVA: 0x001C4368 File Offset: 0x001C2568
		public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
		{
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == collider)
				{
					inWater = this.persistentColliders[i].inWater;
					surfaceDetected = this.persistentColliders[i].surfaceDetected;
					return true;
				}
			}
			inWater = false;
			surfaceDetected = false;
			return false;
		}

		// Token: 0x060059C1 RID: 22977 RVA: 0x001C43D3 File Offset: 0x001C25D3
		protected override void Awake()
		{
			base.Awake();
			this.RefreshColliders();
		}

		// Token: 0x060059C2 RID: 22978 RVA: 0x001C43E4 File Offset: 0x001C25E4
		public void RefreshColliders()
		{
			this.triggerCollider = base.GetComponent<Collider>();
			if (this.volumeColliders == null || this.volumeColliders.Count < 1)
			{
				this.volumeColliders = new List<Collider>();
				this.volumeColliders.Add(base.gameObject.GetComponent<Collider>());
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.volumeColliders.Count; i++)
			{
				float y = this.volumeColliders[i].bounds.max.y;
				float y2 = this.volumeColliders[i].bounds.min.y;
				if (y > num)
				{
					num = y;
				}
				if (y2 < num2)
				{
					num2 = y2;
				}
			}
			this.volumeMaxHeight = num;
			this.volumeMinHeight = num2;
		}

		// Token: 0x060059C3 RID: 22979 RVA: 0x001C44B4 File Offset: 0x001C26B4
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				waterOverlappingCollider.inVolume = false;
				waterOverlappingCollider.playDripEffect = false;
				WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
				if (colliderExitedVolume != null)
				{
					colliderExitedVolume(this, waterOverlappingCollider.collider);
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
			this.RemoveCollidersOutsideVolume(Time.time);
		}

		// Token: 0x060059C4 RID: 22980 RVA: 0x001C452C File Offset: 0x001C272C
		private void Update()
		{
			if (this.persistentColliders.Count < 1)
			{
				return;
			}
			float time = Time.time;
			this.RemoveCollidersOutsideVolume(time);
			if (!this.CanPlayerSwim())
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				bool inWater = waterOverlappingCollider.inWater;
				if (waterOverlappingCollider.inVolume)
				{
					this.CheckColliderAgainstWater(ref waterOverlappingCollider, time);
				}
				else
				{
					waterOverlappingCollider.inWater = false;
				}
				this.TryRegisterOwnershipOfCollider(waterOverlappingCollider.collider, waterOverlappingCollider.inWater, waterOverlappingCollider.surfaceDetected);
				if (waterOverlappingCollider.inWater && !inWater)
				{
					this.OnWaterSurfaceEnter(ref waterOverlappingCollider);
				}
				else if (!waterOverlappingCollider.inWater && inWater)
				{
					this.OnWaterSurfaceExit(ref waterOverlappingCollider, time);
				}
				if (this.HasOwnershipOfCollider(waterOverlappingCollider.collider) && waterOverlappingCollider.surfaceDetected)
				{
					if (!waterOverlappingCollider.inWater)
					{
						this.ColliderOutOfWaterUpdate(ref waterOverlappingCollider, time);
					}
					else
					{
						this.ColliderInWaterUpdate(ref waterOverlappingCollider, time);
					}
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
		}

		// Token: 0x060059C5 RID: 22981 RVA: 0x001C462C File Offset: 0x001C282C
		private void RemoveCollidersOutsideVolume(float currentTime)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = this.persistentColliders.Count - 1; i >= 0; i--)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > this.waterParams.postExitDripDuration)) || !this.CanPlayerSwim())
				{
					this.UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
					GTPlayer instance = GTPlayer.Instance;
					if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
					{
						instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
					}
					this.persistentColliders.RemoveAt(i);
				}
			}
		}

		// Token: 0x060059C6 RID: 22982 RVA: 0x001C470C File Offset: 0x001C290C
		private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 position = persistentCollider.collider.transform.position;
			bool flag = true;
			if (persistentCollider.surfaceDetected && persistentCollider.scaleMultiplier > 0.99f && this.isStationary)
			{
				flag = ((position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > this.waterParams.recomputeSurfaceForColliderDist * this.waterParams.recomputeSurfaceForColliderDist);
			}
			if (flag)
			{
				WaterVolume.SurfaceQuery lastSurfaceQuery;
				if (this.GetSurfaceQueryForPoint(position, out lastSurfaceQuery, this.debugDrawSurfaceCast))
				{
					persistentCollider.surfaceDetected = true;
					persistentCollider.lastSurfaceQuery = lastSurfaceQuery;
				}
				else
				{
					persistentCollider.surfaceDetected = false;
					persistentCollider.lastSurfaceQuery = default(WaterVolume.SurfaceQuery);
				}
			}
			if (persistentCollider.surfaceDetected)
			{
				bool flag2 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.down * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f)).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
				bool flag3 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.up * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f)).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
				persistentCollider.inWater = (flag2 && flag3);
			}
			else
			{
				persistentCollider.inWater = false;
			}
			if (persistentCollider.inWater)
			{
				persistentCollider.lastInWaterTime = currentTime;
			}
		}

		// Token: 0x060059C7 RID: 22983 RVA: 0x001C48F4 File Offset: 0x001C2AF4
		private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 result = Vector3.one * (this.waterParams.splashSpeedRequirement + 0.1f);
			if (persistentCollider.velocityTracker != null)
			{
				result = persistentCollider.velocityTracker.GetAverageVelocity(true, 0.1f, false);
			}
			else if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				result = instance.AveragedVelocity;
			}
			else if (persistentCollider.collider.attachedRigidbody != null && !persistentCollider.collider.attachedRigidbody.isKinematic)
			{
				result = persistentCollider.collider.attachedRigidbody.velocity;
			}
			return result;
		}

		// Token: 0x060059C8 RID: 22984 RVA: 0x001C49AC File Offset: 0x001C2BAC
		private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredWater = this.ColliderEnteredWater;
			if (colliderEnteredWater != null)
			{
				colliderEnteredWater(this, persistentCollider.collider);
			}
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnEnterWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				Vector3 colliderVelocity = this.GetColliderVelocity(ref persistentCollider);
				bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, true, this);
				}
			}
		}

		// Token: 0x060059C9 RID: 22985 RVA: 0x001C4AF8 File Offset: 0x001C2CF8
		private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			WaterVolume.WaterVolumeEvent colliderExitedWater = this.ColliderExitedWater;
			if (colliderExitedWater != null)
			{
				colliderExitedWater(this, persistentCollider.collider);
			}
			persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnExitWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				float num = Vector3.Dot(this.GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
				bool flag = num > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = num > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, false, this);
				}
			}
		}

		// Token: 0x060059CA RID: 22986 RVA: 0x001C4C64 File Offset: 0x001C2E64
		private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			if (currentTime < persistentCollider.lastInWaterTime + this.waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
			{
				persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
				float dripScale = this.waterParams.rippleEffectScale * 2f * (this.waterParams.perDripDefaultRadius + Random.Range(-this.waterParams.perDripRadiusRandRange * 0.5f, this.waterParams.perDripRadiusRandRange * 0.5f));
				persistentCollider.PlayDripEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, dripScale);
			}
		}

		// Token: 0x060059CB RID: 22987 RVA: 0x001C4D4C File Offset: 0x001C2F4C
		private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
			bool flag;
			if (persistentCollider.overrideBoundingRadius)
			{
				flag = ((persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride);
			}
			else
			{
				flag = ((persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f);
			}
			if (flag)
			{
				float num = Mathf.Max(this.waterParams.minDistanceBetweenRipples, this.waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / this.waterParams.rippleEffectScale));
				bool flag2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
				bool flag3 = currentTime - persistentCollider.lastRippleTime > this.waterParams.minTimeBetweenRipples;
				if (flag2 || flag3)
				{
					persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, currentTime, this);
					return;
				}
			}
			else
			{
				persistentCollider.lastRippleTime = currentTime;
			}
		}

		// Token: 0x060059CC RID: 22988 RVA: 0x001C4E9C File Offset: 0x001C309C
		private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
		{
			WaterVolume waterVolume;
			if (WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume))
			{
				if (waterVolume != this)
				{
					bool flag;
					bool flag2;
					waterVolume.CheckColliderInVolume(collider, out flag, out flag2);
					if ((isSurfaceDetected && !flag2) || (isInWater && !flag))
					{
						WaterVolume.sharedColliderRegistry.Remove(collider);
						WaterVolume.sharedColliderRegistry.Add(collider, this);
						return;
					}
				}
			}
			else
			{
				WaterVolume.sharedColliderRegistry.Add(collider, this);
			}
		}

		// Token: 0x060059CD RID: 22989 RVA: 0x001C4EFE File Offset: 0x001C30FE
		private void UnregisterOwnershipOfCollider(Collider collider)
		{
			if (WaterVolume.sharedColliderRegistry.ContainsKey(collider))
			{
				WaterVolume.sharedColliderRegistry.Remove(collider);
			}
		}

		// Token: 0x060059CE RID: 22990 RVA: 0x001C4F1C File Offset: 0x001C311C
		private bool HasOwnershipOfCollider(Collider collider)
		{
			WaterVolume x;
			return WaterVolume.sharedColliderRegistry.TryGetValue(collider, out x) && x == this;
		}

		// Token: 0x060059CF RID: 22991 RVA: 0x001C4F44 File Offset: 0x001C3144
		protected virtual bool CanPlayerSwim()
		{
			if (this.isMonkeblock && this.PlayerVRRig != null)
			{
				if (this.PlayerVRRig.scaleFactor < 0.5f)
				{
					return true;
				}
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(this.PlayerVRRig.zoneEntity.currentZone, out builderTable))
				{
					return !builderTable.isTableMutable;
				}
			}
			return true;
		}

		// Token: 0x060059D0 RID: 22992 RVA: 0x001C4FA0 File Offset: 0x001C31A0
		public void OnTriggerEnter(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderEnteredVolume = this.ColliderEnteredVolume;
			if (colliderEnteredVolume != null)
			{
				colliderEnteredVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider value = this.persistentColliders[i];
					value.inVolume = true;
					this.persistentColliders[i] = value;
					return;
				}
			}
			WaterOverlappingCollider waterOverlappingCollider = new WaterOverlappingCollider
			{
				collider = other
			};
			waterOverlappingCollider.inVolume = true;
			waterOverlappingCollider.lastInWaterTime = Time.time - this.waterParams.postExitDripDuration - 10f;
			WaterSplashOverride component2 = other.GetComponent<WaterSplashOverride>();
			if (component2 != null)
			{
				if (component2.suppressWaterEffects)
				{
					return;
				}
				waterOverlappingCollider.playBigSplash = component2.playBigSplash;
				waterOverlappingCollider.playDripEffect = component2.playDrippingEffect;
				waterOverlappingCollider.overrideBoundingRadius = component2.overrideBoundingRadius;
				waterOverlappingCollider.boundingRadiusOverride = component2.boundingRadiusOverride;
				waterOverlappingCollider.scaleMultiplier = (component2.scaleByPlayersScale ? GTPlayer.Instance.scale : 1f);
			}
			else
			{
				if (other.GetComponent<BuilderPieceCollider>() != null)
				{
					return;
				}
				waterOverlappingCollider.playDripEffect = true;
				waterOverlappingCollider.overrideBoundingRadius = false;
				waterOverlappingCollider.scaleMultiplier = 1f;
				waterOverlappingCollider.playBigSplash = false;
			}
			GTPlayer instance = GTPlayer.Instance;
			if (component != null)
			{
				waterOverlappingCollider.velocityTracker = (component.isLeftHand ? instance.leftHandCenterVelocityTracker : instance.rightHandCenterVelocityTracker);
				waterOverlappingCollider.scaleMultiplier = instance.scale;
			}
			else
			{
				waterOverlappingCollider.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
			}
			if (this.PlayerVRRig != null && this.waterParams.sendSplashEffectRPCs && (component != null || waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider))
			{
				waterOverlappingCollider.photonViewForRPC = this.PlayerVRRig.netView;
			}
			this.persistentColliders.Add(waterOverlappingCollider);
		}

		// Token: 0x060059D1 RID: 22993 RVA: 0x001C51C8 File Offset: 0x001C33C8
		private void OnTriggerExit(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
			if (colliderExitedVolume != null)
			{
				colliderExitedVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider value = this.persistentColliders[i];
					value.inVolume = false;
					this.persistentColliders[i] = value;
				}
			}
		}

		// Token: 0x060059D2 RID: 22994 RVA: 0x001C525B File Offset: 0x001C345B
		public void SetPropertiesFromPlaceholder(WaterVolumeProperties properties, List<Collider> waterVolumeColliders, WaterParameters parameters)
		{
			this.surfacePlane = properties.surfacePlane;
			this.surfaceColliders = properties.surfaceColliders;
			this.volumeColliders = waterVolumeColliders;
			this.liquidType = (GTPlayer.LiquidType)Math.Clamp(properties.liquidType - CMSZoneShaderSettings.EZoneLiquidType.Water, 0, 1);
			this.waterParams = parameters;
		}

		// Token: 0x0400647C RID: 25724
		[SerializeField]
		public Transform surfacePlane;

		// Token: 0x0400647D RID: 25725
		[SerializeField]
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x0400647E RID: 25726
		[SerializeField]
		public List<Collider> volumeColliders = new List<Collider>();

		// Token: 0x0400647F RID: 25727
		[SerializeField]
		private GTPlayer.LiquidType liquidType;

		// Token: 0x04006480 RID: 25728
		[SerializeField]
		private WaterCurrent waterCurrent;

		// Token: 0x04006481 RID: 25729
		[SerializeField]
		private WaterParameters waterParams;

		// Token: 0x04006482 RID: 25730
		[SerializeField]
		[Tooltip("The water volume be placed in the scene (not spawned) and not moved for this to be true")]
		public bool isStationary = true;

		// Token: 0x04006483 RID: 25731
		[SerializeField]
		[Tooltip("Check scale of monke entering")]
		public bool isMonkeblock;

		// Token: 0x04006484 RID: 25732
		public const string WaterSplashRPC = "RPC_PlaySplashEffect";

		// Token: 0x04006485 RID: 25733
		public static float[] splashRPCSendTimes = new float[4];

		// Token: 0x04006486 RID: 25734
		private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

		// Token: 0x04006487 RID: 25735
		private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

		// Token: 0x04006488 RID: 25736
		private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

		// Token: 0x04006489 RID: 25737
		private int[] sharedMeshTris;

		// Token: 0x0400648A RID: 25738
		private Vector3[] sharedMeshVerts;

		// Token: 0x0400648F RID: 25743
		private VRRig playerVRRig;

		// Token: 0x04006490 RID: 25744
		private float volumeMaxHeight;

		// Token: 0x04006491 RID: 25745
		private float volumeMinHeight;

		// Token: 0x04006492 RID: 25746
		private bool debugDrawSurfaceCast;

		// Token: 0x04006493 RID: 25747
		private Collider triggerCollider;

		// Token: 0x04006494 RID: 25748
		private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

		// Token: 0x04006495 RID: 25749
		private GuidedRefTargetIdSO _guidedRefTargetId;

		// Token: 0x04006496 RID: 25750
		private Object _guidedRefTargetObject;

		// Token: 0x02000E1C RID: 3612
		public struct SurfaceQuery
		{
			// Token: 0x170008CE RID: 2254
			// (get) Token: 0x060059D5 RID: 22997 RVA: 0x001C52FB File Offset: 0x001C34FB
			public Plane surfacePlane
			{
				get
				{
					return new Plane(this.surfaceNormal, this.surfacePoint);
				}
			}

			// Token: 0x04006497 RID: 25751
			public Vector3 surfacePoint;

			// Token: 0x04006498 RID: 25752
			public Vector3 surfaceNormal;

			// Token: 0x04006499 RID: 25753
			public float maxDepth;
		}

		// Token: 0x02000E1D RID: 3613
		// (Invoke) Token: 0x060059D7 RID: 22999
		public delegate void WaterVolumeEvent(WaterVolume volume, Collider collider);
	}
}
