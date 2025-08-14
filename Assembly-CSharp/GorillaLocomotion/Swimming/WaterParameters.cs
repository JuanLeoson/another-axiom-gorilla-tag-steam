using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E19 RID: 3609
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04006460 RID: 25696
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04006461 RID: 25697
		public GameObject splashEffect;

		// Token: 0x04006462 RID: 25698
		public float splashEffectScale = 1f;

		// Token: 0x04006463 RID: 25699
		public bool sendSplashEffectRPCs;

		// Token: 0x04006464 RID: 25700
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x04006465 RID: 25701
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x04006466 RID: 25702
		public Gradient splashColorBySpeedGradient;

		// Token: 0x04006467 RID: 25703
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x04006468 RID: 25704
		public GameObject rippleEffect;

		// Token: 0x04006469 RID: 25705
		public float rippleEffectScale = 1f;

		// Token: 0x0400646A RID: 25706
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x0400646B RID: 25707
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x0400646C RID: 25708
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x0400646D RID: 25709
		public Color rippleSpriteColor = Color.white;

		// Token: 0x0400646E RID: 25710
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x0400646F RID: 25711
		public float postExitDripDuration = 1.5f;

		// Token: 0x04006470 RID: 25712
		public float perDripTimeDelay = 0.2f;

		// Token: 0x04006471 RID: 25713
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x04006472 RID: 25714
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x04006473 RID: 25715
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x04006474 RID: 25716
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x04006475 RID: 25717
		public bool allowBubblesInVolume;
	}
}
