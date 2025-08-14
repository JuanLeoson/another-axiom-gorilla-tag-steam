using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C7C RID: 3196
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x14000087 RID: 135
		// (add) Token: 0x06004F18 RID: 20248 RVA: 0x001895E0 File Offset: 0x001877E0
		// (remove) Token: 0x06004F19 RID: 20249 RVA: 0x00189618 File Offset: 0x00187818
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x06004F1A RID: 20250 RVA: 0x00189650 File Offset: 0x00187850
		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		// Token: 0x02000C7D RID: 3197
		// (Invoke) Token: 0x06004F1D RID: 20253
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
