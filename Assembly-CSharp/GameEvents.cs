using System;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using ModIO;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004C1 RID: 1217
public class GameEvents
{
	// Token: 0x0400269D RID: 9885
	public static UnityEvent<GorillaKeyboardBindings> OnGorrillaKeyboardButtonPressedEvent = new UnityEvent<GorillaKeyboardBindings>();

	// Token: 0x0400269E RID: 9886
	public static UnityEvent<GorillaATMKeyBindings> OnGorrillaATMKeyButtonPressedEvent = new UnityEvent<GorillaATMKeyBindings>();

	// Token: 0x0400269F RID: 9887
	internal static UnityEvent<string> ScreenTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040026A0 RID: 9888
	internal static UnityEvent<Material[]> ScreenTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040026A1 RID: 9889
	internal static UnityEvent<string> FunctionSelectTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040026A2 RID: 9890
	internal static UnityEvent<Material[]> FunctionTextMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040026A3 RID: 9891
	internal static UnityEvent<string> ScoreboardTextChangedEvent = new UnityEvent<string>();

	// Token: 0x040026A4 RID: 9892
	internal static UnityEvent<Material[]> ScoreboardMaterialsEvent = new UnityEvent<Material[]>();

	// Token: 0x040026A5 RID: 9893
	public static UnityEvent<ModManagementEventType, ModId, Result> ModIOModManagementEvent = new UnityEvent<ModManagementEventType, ModId, Result>();

	// Token: 0x040026A6 RID: 9894
	public static UnityEvent<SharedBlocksKeyboardBindings> OnSharedBlocksKeyboardButtonPressedEvent = new UnityEvent<SharedBlocksKeyboardBindings>();
}
