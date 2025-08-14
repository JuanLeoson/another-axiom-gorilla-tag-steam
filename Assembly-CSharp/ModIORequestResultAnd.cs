using System;

// Token: 0x02000825 RID: 2085
public struct ModIORequestResultAnd<T>
{
	// Token: 0x0600343D RID: 13373 RVA: 0x00110148 File Offset: 0x0010E348
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x0011016C File Offset: 0x0010E36C
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x04004120 RID: 16672
	public ModIORequestResult result;

	// Token: 0x04004121 RID: 16673
	public T data;
}
