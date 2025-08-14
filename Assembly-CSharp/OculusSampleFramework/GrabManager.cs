using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D03 RID: 3331
	public class GrabManager : MonoBehaviour
	{
		// Token: 0x0600526E RID: 21102 RVA: 0x00199CDC File Offset: 0x00197EDC
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		// Token: 0x0600526F RID: 21103 RVA: 0x00199D00 File Offset: 0x00197F00
		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		// Token: 0x04005BDF RID: 23519
		private Collider m_grabVolume;

		// Token: 0x04005BE0 RID: 23520
		public Color OutlineColorInRange;

		// Token: 0x04005BE1 RID: 23521
		public Color OutlineColorHighlighted;

		// Token: 0x04005BE2 RID: 23522
		public Color OutlineColorOutOfRange;
	}
}
