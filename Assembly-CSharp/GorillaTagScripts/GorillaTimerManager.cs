using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C3E RID: 3134
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x06004D79 RID: 19833 RVA: 0x00180F3B File Offset: 0x0017F13B
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x06004D7A RID: 19834 RVA: 0x00180F6B File Offset: 0x0017F16B
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x06004D7B RID: 19835 RVA: 0x00180F81 File Offset: 0x0017F181
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06004D7C RID: 19836 RVA: 0x00180F9C File Offset: 0x0017F19C
		public static void RegisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (!GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x00180FC2 File Offset: 0x0017F1C2
		public static void UnregisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x06004D7E RID: 19838 RVA: 0x00180FEC File Offset: 0x0017F1EC
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x04005669 RID: 22121
		public static GorillaTimerManager instance;

		// Token: 0x0400566A RID: 22122
		public static bool hasInstance = false;

		// Token: 0x0400566B RID: 22123
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
