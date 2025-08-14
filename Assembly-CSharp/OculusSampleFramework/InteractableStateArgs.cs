using System;

namespace OculusSampleFramework
{
	// Token: 0x02000D13 RID: 3347
	public class InteractableStateArgs : EventArgs
	{
		// Token: 0x060052C9 RID: 21193 RVA: 0x0019AC27 File Offset: 0x00198E27
		public InteractableStateArgs(Interactable interactable, InteractableTool tool, InteractableState newInteractableState, InteractableState oldState, ColliderZoneArgs colliderArgs)
		{
			this.Interactable = interactable;
			this.Tool = tool;
			this.NewInteractableState = newInteractableState;
			this.OldInteractableState = oldState;
			this.ColliderArgs = colliderArgs;
		}

		// Token: 0x04005C2A RID: 23594
		public readonly Interactable Interactable;

		// Token: 0x04005C2B RID: 23595
		public readonly InteractableTool Tool;

		// Token: 0x04005C2C RID: 23596
		public readonly InteractableState OldInteractableState;

		// Token: 0x04005C2D RID: 23597
		public readonly InteractableState NewInteractableState;

		// Token: 0x04005C2E RID: 23598
		public readonly ColliderZoneArgs ColliderArgs;
	}
}
