using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E11 RID: 3601
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x040063E6 RID: 25574
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x040063E7 RID: 25575
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x040063E8 RID: 25576
		public bool extendBouyancyFromSpeed;

		// Token: 0x040063E9 RID: 25577
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x040063EA RID: 25578
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x040063EB RID: 25579
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x040063EC RID: 25580
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040063ED RID: 25581
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x040063EE RID: 25582
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x040063EF RID: 25583
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x040063F0 RID: 25584
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x040063F1 RID: 25585
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x040063F2 RID: 25586
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x040063F3 RID: 25587
		public float waterSurfaceJumpAmount;

		// Token: 0x040063F4 RID: 25588
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x040063F5 RID: 25589
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040063F6 RID: 25590
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040063F7 RID: 25591
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x040063F8 RID: 25592
		public bool applyDiveDampingMultiplier;

		// Token: 0x040063F9 RID: 25593
		public float diveDampingMultiplier = 1f;

		// Token: 0x040063FA RID: 25594
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x040063FB RID: 25595
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x040063FC RID: 25596
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x040063FD RID: 25597
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x040063FE RID: 25598
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x040063FF RID: 25599
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x04006400 RID: 25600
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x04006401 RID: 25601
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x04006402 RID: 25602
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006403 RID: 25603
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04006404 RID: 25604
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006405 RID: 25605
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04006406 RID: 25606
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006407 RID: 25607
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x04006408 RID: 25608
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04006409 RID: 25609
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400640A RID: 25610
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400640B RID: 25611
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400640C RID: 25612
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400640D RID: 25613
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
