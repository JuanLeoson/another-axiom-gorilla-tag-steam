using System;

namespace OculusSampleFramework
{
	// Token: 0x02000D0A RID: 3338
	public class ColliderZoneArgs : EventArgs
	{
		// Token: 0x06005292 RID: 21138 RVA: 0x0019A44F File Offset: 0x0019864F
		public ColliderZoneArgs(ColliderZone collider, float frameTime, InteractableTool collidingTool, InteractionType interactionType)
		{
			this.Collider = collider;
			this.FrameTime = frameTime;
			this.CollidingTool = collidingTool;
			this.InteractionT = interactionType;
		}

		// Token: 0x04005BFA RID: 23546
		public readonly ColliderZone Collider;

		// Token: 0x04005BFB RID: 23547
		public readonly float FrameTime;

		// Token: 0x04005BFC RID: 23548
		public readonly InteractableTool CollidingTool;

		// Token: 0x04005BFD RID: 23549
		public readonly InteractionType InteractionT;
	}
}
