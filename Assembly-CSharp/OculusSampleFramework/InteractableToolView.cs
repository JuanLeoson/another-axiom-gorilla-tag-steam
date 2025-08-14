using System;

namespace OculusSampleFramework
{
	// Token: 0x02000D1F RID: 3359
	public interface InteractableToolView
	{
		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06005318 RID: 21272
		InteractableTool InteractableTool { get; }

		// Token: 0x06005319 RID: 21273
		void SetFocusedInteractable(Interactable interactable);

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x0600531A RID: 21274
		// (set) Token: 0x0600531B RID: 21275
		bool EnableState { get; set; }

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x0600531C RID: 21276
		// (set) Token: 0x0600531D RID: 21277
		bool ToolActivateState { get; set; }
	}
}
