using System;

// Token: 0x02000A54 RID: 2644
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x0600408F RID: 16527 RVA: 0x00146C5F File Offset: 0x00144E5F
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04004C35 RID: 19509
	public FXType Key;

	// Token: 0x04004C36 RID: 19510
	public bool UseNetWorkTime;

	// Token: 0x04004C37 RID: 19511
	public T CallLimitSettings;
}
