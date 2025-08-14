using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000E1A RID: 3610
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x04006476 RID: 25718
		public bool suppressWaterEffects;

		// Token: 0x04006477 RID: 25719
		public bool playBigSplash;

		// Token: 0x04006478 RID: 25720
		public bool playDrippingEffect = true;

		// Token: 0x04006479 RID: 25721
		public bool scaleByPlayersScale;

		// Token: 0x0400647A RID: 25722
		public bool overrideBoundingRadius;

		// Token: 0x0400647B RID: 25723
		public float boundingRadiusOverride = 1f;
	}
}
