using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E5A RID: 3674
	public static class GTAppState
	{
		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06005C2A RID: 23594 RVA: 0x001D0330 File Offset: 0x001CE530
		// (set) Token: 0x06005C2B RID: 23595 RVA: 0x001D0337 File Offset: 0x001CE537
		[OnEnterPlay_Set(false)]
		public static bool isQuitting { get; private set; }

		// Token: 0x06005C2C RID: 23596 RVA: 0x001D0340 File Offset: 0x001CE540
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate()
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		// Token: 0x06005C2D RID: 23597 RVA: 0x000023F5 File Offset: 0x000005F5
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
