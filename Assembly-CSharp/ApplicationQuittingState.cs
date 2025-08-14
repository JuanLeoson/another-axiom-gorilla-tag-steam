using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public static class ApplicationQuittingState
{
	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000999 RID: 2457 RVA: 0x00034A77 File Offset: 0x00032C77
	// (set) Token: 0x0600099A RID: 2458 RVA: 0x00034A7E File Offset: 0x00032C7E
	public static bool IsQuitting { get; private set; }

	// Token: 0x0600099B RID: 2459 RVA: 0x00034A86 File Offset: 0x00032C86
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += ApplicationQuittingState.HandleApplicationQuitting;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00034A99 File Offset: 0x00032C99
	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}
}
