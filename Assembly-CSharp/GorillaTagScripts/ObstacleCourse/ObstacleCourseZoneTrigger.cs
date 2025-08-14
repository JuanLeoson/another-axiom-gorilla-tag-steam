using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C7A RID: 3194
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x14000085 RID: 133
		// (add) Token: 0x06004F0D RID: 20237 RVA: 0x00189490 File Offset: 0x00187690
		// (remove) Token: 0x06004F0E RID: 20238 RVA: 0x001894C8 File Offset: 0x001876C8
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x06004F0F RID: 20239 RVA: 0x00189500 File Offset: 0x00187700
		// (remove) Token: 0x06004F10 RID: 20240 RVA: 0x00189538 File Offset: 0x00187738
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06004F11 RID: 20241 RVA: 0x0018956D File Offset: 0x0018776D
		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		// Token: 0x06004F12 RID: 20242 RVA: 0x001895A5 File Offset: 0x001877A5
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		// Token: 0x0400580A RID: 22538
		public LayerMask bodyLayer;

		// Token: 0x02000C7B RID: 3195
		// (Invoke) Token: 0x06004F15 RID: 20245
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
