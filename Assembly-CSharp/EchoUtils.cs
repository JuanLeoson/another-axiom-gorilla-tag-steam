using System;
using UnityEngine;

// Token: 0x02000AB4 RID: 2740
public static class EchoUtils
{
	// Token: 0x0600423E RID: 16958 RVA: 0x0014D9B0 File Offset: 0x0014BBB0
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x0600423F RID: 16959 RVA: 0x0014D9BE File Offset: 0x0014BBBE
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
