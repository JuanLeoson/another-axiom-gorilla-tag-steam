using System;

// Token: 0x02000824 RID: 2084
public struct ModIORequestResult
{
	// Token: 0x0600343B RID: 13371 RVA: 0x00110104 File Offset: 0x0010E304
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult result;
		result.success = false;
		result.message = inMessage;
		return result;
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x00110124 File Offset: 0x0010E324
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult result;
		result.success = true;
		result.message = "";
		return result;
	}

	// Token: 0x0400411E RID: 16670
	public bool success;

	// Token: 0x0400411F RID: 16671
	public string message;
}
