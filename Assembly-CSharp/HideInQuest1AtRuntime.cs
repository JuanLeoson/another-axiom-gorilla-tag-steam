using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000ABB RID: 2747
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x06004247 RID: 16967 RVA: 0x0014DB26 File Offset: 0x0014BD26
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
