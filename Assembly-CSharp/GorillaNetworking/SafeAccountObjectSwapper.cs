using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D61 RID: 3425
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		// Token: 0x06005504 RID: 21764 RVA: 0x001A5CD6 File Offset: 0x001A3ED6
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		// Token: 0x06005505 RID: 21765 RVA: 0x001A5D14 File Offset: 0x001A3F14
		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		// Token: 0x06005506 RID: 21766 RVA: 0x001A5D20 File Offset: 0x001A3F20
		public void SwitchToSafeMode()
		{
			foreach (GameObject gameObject in this.UnSafeGameObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.UnSafeTexts)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			foreach (GameObject gameObject3 in this.SafeTexts)
			{
				if (gameObject3 != null)
				{
					gameObject3.SetActive(true);
				}
			}
			foreach (GameObject gameObject4 in this.SafeModeObjects)
			{
				if (gameObject4 != null)
				{
					gameObject4.SetActive(true);
				}
			}
		}

		// Token: 0x04005E98 RID: 24216
		public GameObject[] UnSafeGameObjects;

		// Token: 0x04005E99 RID: 24217
		public GameObject[] UnSafeTexts;

		// Token: 0x04005E9A RID: 24218
		public GameObject[] SafeTexts;

		// Token: 0x04005E9B RID: 24219
		public GameObject[] SafeModeObjects;
	}
}
