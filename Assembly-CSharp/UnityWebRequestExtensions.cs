using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000950 RID: 2384
public static class UnityWebRequestExtensions
{
	// Token: 0x06003AA5 RID: 15013 RVA: 0x0012F7A0 File Offset: 0x0012D9A0
	public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		TaskCompletionSource<UnityWebRequest> tcs = new TaskCompletionSource<UnityWebRequest>();
		asyncOp.completed += delegate(AsyncOperation operation)
		{
			tcs.TrySetResult(asyncOp.webRequest);
		};
		return tcs.Task.GetAwaiter();
	}
}
