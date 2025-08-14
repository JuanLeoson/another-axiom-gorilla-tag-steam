using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E24 RID: 3620
	public class GorillaRopeSwingUpdateManager : MonoBehaviour
	{
		// Token: 0x06005A02 RID: 23042 RVA: 0x001C629E File Offset: 0x001C449E
		protected void Awake()
		{
			if (GorillaRopeSwingUpdateManager.hasInstance && GorillaRopeSwingUpdateManager.instance != null && GorillaRopeSwingUpdateManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaRopeSwingUpdateManager.SetInstance(this);
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x001C62CE File Offset: 0x001C44CE
		public static void CreateManager()
		{
			GorillaRopeSwingUpdateManager.SetInstance(new GameObject("GorillaRopeSwingUpdateManager").AddComponent<GorillaRopeSwingUpdateManager>());
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x001C62E4 File Offset: 0x001C44E4
		private static void SetInstance(GorillaRopeSwingUpdateManager manager)
		{
			GorillaRopeSwingUpdateManager.instance = manager;
			GorillaRopeSwingUpdateManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x001C62FF File Offset: 0x001C44FF
		public static void RegisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (!GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Add(ropeSwing);
			}
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x001C6325 File Offset: 0x001C4525
		public static void UnregisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Remove(ropeSwing);
			}
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x001C634C File Offset: 0x001C454C
		public void Update()
		{
			for (int i = 0; i < GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Count; i++)
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings[i].InvokeUpdate();
			}
		}

		// Token: 0x040064C8 RID: 25800
		public static GorillaRopeSwingUpdateManager instance;

		// Token: 0x040064C9 RID: 25801
		public static bool hasInstance = false;

		// Token: 0x040064CA RID: 25802
		public static List<GorillaRopeSwing> allGorillaRopeSwings = new List<GorillaRopeSwing>();
	}
}
