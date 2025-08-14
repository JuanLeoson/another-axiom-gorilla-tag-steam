using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F49 RID: 3913
	public interface IProjectile
	{
		// Token: 0x060060E4 RID: 24804
		void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep = -1);
	}
}
