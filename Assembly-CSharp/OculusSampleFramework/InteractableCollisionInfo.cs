using System;

namespace OculusSampleFramework
{
	// Token: 0x02000D1D RID: 3357
	public class InteractableCollisionInfo
	{
		// Token: 0x06005301 RID: 21249 RVA: 0x0019B805 File Offset: 0x00199A05
		public InteractableCollisionInfo(ColliderZone collider, InteractableCollisionDepth collisionDepth, InteractableTool collidingTool)
		{
			this.InteractableCollider = collider;
			this.CollisionDepth = collisionDepth;
			this.CollidingTool = collidingTool;
		}

		// Token: 0x04005C5E RID: 23646
		public ColliderZone InteractableCollider;

		// Token: 0x04005C5F RID: 23647
		public InteractableCollisionDepth CollisionDepth;

		// Token: 0x04005C60 RID: 23648
		public InteractableTool CollidingTool;
	}
}
