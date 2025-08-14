using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000E36 RID: 3638
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x06005A7B RID: 23163 RVA: 0x001C94DB File Offset: 0x001C76DB
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x0400654C RID: 25932
		public bool snapX;

		// Token: 0x0400654D RID: 25933
		public bool snapY;

		// Token: 0x0400654E RID: 25934
		public bool snapZ;

		// Token: 0x0400654F RID: 25935
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04006550 RID: 25936
		public AudioClip clip;

		// Token: 0x04006551 RID: 25937
		public AudioClip clipOnFullRelease;

		// Token: 0x04006552 RID: 25938
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x04006553 RID: 25939
		public bool climbOnlyWhileSmall;

		// Token: 0x04006554 RID: 25940
		public bool IsPlayerAttached;

		// Token: 0x04006555 RID: 25941
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x04006556 RID: 25942
		[NonSerialized]
		public Collider colliderCache;
	}
}
