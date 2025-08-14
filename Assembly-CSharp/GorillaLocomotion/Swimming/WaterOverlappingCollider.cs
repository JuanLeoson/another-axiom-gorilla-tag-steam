using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E18 RID: 3608
	public struct WaterOverlappingCollider
	{
		// Token: 0x060059A9 RID: 22953 RVA: 0x001C364C File Offset: 0x001C184C
		public void PlayRippleEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float defaultRippleScale, float currentTime, WaterVolume volume)
		{
			this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
			this.lastRippleScale = defaultRippleScale * this.lastBoundingRadius * 2f * this.scaleMultiplier;
			this.lastRippleTime = currentTime;
			ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale, true).GetComponent<WaterRippleEffect>().PlayEffect(volume);
		}

		// Token: 0x060059AA RID: 22954 RVA: 0x001C36E8 File Offset: 0x001C18E8
		public void PlaySplashEffect(GameObject splashEffectPrefab, Vector3 splashPosition, float splashScale, bool bigSplash, bool enteringWater, WaterVolume volume)
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
			ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, quaternion, splashScale * this.scaleMultiplier, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
			if (this.photonViewForRPC != null)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < WaterVolume.splashRPCSendTimes.Length; i++)
				{
					if (WaterVolume.splashRPCSendTimes[i] < num2)
					{
						num2 = WaterVolume.splashRPCSendTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					WaterVolume.splashRPCSendTimes[num] = time;
					this.photonViewForRPC.SendRPC("RPC_PlaySplashEffect", RpcTarget.Others, new object[]
					{
						splashPosition,
						quaternion,
						splashScale * this.scaleMultiplier,
						this.lastBoundingRadius,
						bigSplash,
						enteringWater
					});
				}
			}
		}

		// Token: 0x060059AB RID: 22955 RVA: 0x001C3808 File Offset: 0x001C1A08
		public void PlayDripEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float dripScale)
		{
			Vector3 closestPositionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			float d = this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius;
			Vector3 b = Vector3.ProjectOnPlane(Random.onUnitSphere * d * 0.5f, surfaceNormal);
			ObjectPools.instance.Instantiate(rippleEffectPrefab, closestPositionOnSurface + b, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier, true);
		}

		// Token: 0x060059AC RID: 22956 RVA: 0x001C3897 File Offset: 0x001C1A97
		public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
		{
			return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
		}

		// Token: 0x060059AD RID: 22957 RVA: 0x001C38BC File Offset: 0x001C1ABC
		private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
		{
			if (this.overrideBoundingRadius)
			{
				this.lastBoundingRadius = this.boundingRadiusOverride;
				return this.boundingRadiusOverride;
			}
			Vector3 extents = this.collider.bounds.extents;
			Vector3 vector = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
			Vector3 vector2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
			Vector3 vector3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
			float sqrMagnitude = vector.sqrMagnitude;
			float sqrMagnitude2 = vector2.sqrMagnitude;
			float sqrMagnitude3 = vector3.sqrMagnitude;
			if (sqrMagnitude >= sqrMagnitude2 && sqrMagnitude >= sqrMagnitude3)
			{
				return vector.magnitude;
			}
			if (sqrMagnitude2 >= sqrMagnitude && sqrMagnitude2 >= sqrMagnitude3)
			{
				return vector2.magnitude;
			}
			return vector3.magnitude;
		}

		// Token: 0x0400644E RID: 25678
		public bool playBigSplash;

		// Token: 0x0400644F RID: 25679
		public bool playDripEffect;

		// Token: 0x04006450 RID: 25680
		public bool overrideBoundingRadius;

		// Token: 0x04006451 RID: 25681
		public float boundingRadiusOverride;

		// Token: 0x04006452 RID: 25682
		public float scaleMultiplier;

		// Token: 0x04006453 RID: 25683
		public Collider collider;

		// Token: 0x04006454 RID: 25684
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x04006455 RID: 25685
		public WaterVolume.SurfaceQuery lastSurfaceQuery;

		// Token: 0x04006456 RID: 25686
		public NetworkView photonViewForRPC;

		// Token: 0x04006457 RID: 25687
		public bool surfaceDetected;

		// Token: 0x04006458 RID: 25688
		public bool inWater;

		// Token: 0x04006459 RID: 25689
		public bool inVolume;

		// Token: 0x0400645A RID: 25690
		public float lastBoundingRadius;

		// Token: 0x0400645B RID: 25691
		public Vector3 lastRipplePosition;

		// Token: 0x0400645C RID: 25692
		public float lastRippleScale;

		// Token: 0x0400645D RID: 25693
		public float lastRippleTime;

		// Token: 0x0400645E RID: 25694
		public float lastInWaterTime;

		// Token: 0x0400645F RID: 25695
		public float nextDripTime;
	}
}
