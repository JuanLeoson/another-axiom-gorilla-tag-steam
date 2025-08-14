using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E25 RID: 3621
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x040064CB RID: 25803
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x040064CC RID: 25804
		public float frictionWhenNotHeld = 0.25f;
	}
}
