using System;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public static class DeepLinkSender
{
	// Token: 0x06001DF9 RID: 7673 RVA: 0x000A0658 File Offset: 0x0009E858
	public static bool SendDeepLink(ulong deepLinkAppID, string deepLinkMessage, Action<string> onSent)
	{
		Debug.LogError("[DeepLinkSender::SendDeepLink] Called on non-oculus platform!");
		return false;
	}

	// Token: 0x0400269C RID: 9884
	private static Action<string> currentDeepLinkSentCallback;
}
