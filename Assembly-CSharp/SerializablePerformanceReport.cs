using System;
using System.Collections.Generic;

// Token: 0x02000274 RID: 628
[Serializable]
public class SerializablePerformanceReport<T>
{
	// Token: 0x04001759 RID: 5977
	public string reportDate;

	// Token: 0x0400175A RID: 5978
	public string version;

	// Token: 0x0400175B RID: 5979
	public List<T> results;
}
