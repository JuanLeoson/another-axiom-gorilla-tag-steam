using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E28 RID: 3624
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x040064DD RID: 25821
		public float minSlidePitch = 0.5f;

		// Token: 0x040064DE RID: 25822
		public float maxSlidePitch = 1f;

		// Token: 0x040064DF RID: 25823
		public float minSlideVolume;

		// Token: 0x040064E0 RID: 25824
		public float maxSlideVolume = 0.2f;

		// Token: 0x040064E1 RID: 25825
		public float maxSpeed = 10f;

		// Token: 0x040064E2 RID: 25826
		public float gravityMulti = 1.1f;

		// Token: 0x040064E3 RID: 25827
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x040064E4 RID: 25828
		public float maxFriction = 1f;

		// Token: 0x040064E5 RID: 25829
		public float maxFrictionSpeed = 15f;
	}
}
