using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C4F RID: 3151
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x06004DFA RID: 19962 RVA: 0x001838F5 File Offset: 0x00181AF5
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x040056F8 RID: 22264
		public GTZone zone;
	}
}
