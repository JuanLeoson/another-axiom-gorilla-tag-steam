using System;

namespace LitJson
{
	// Token: 0x02000BC0 RID: 3008
	public enum JsonToken
	{
		// Token: 0x04005202 RID: 20994
		None,
		// Token: 0x04005203 RID: 20995
		ObjectStart,
		// Token: 0x04005204 RID: 20996
		PropertyName,
		// Token: 0x04005205 RID: 20997
		ObjectEnd,
		// Token: 0x04005206 RID: 20998
		ArrayStart,
		// Token: 0x04005207 RID: 20999
		ArrayEnd,
		// Token: 0x04005208 RID: 21000
		Int,
		// Token: 0x04005209 RID: 21001
		Long,
		// Token: 0x0400520A RID: 21002
		Double,
		// Token: 0x0400520B RID: 21003
		String,
		// Token: 0x0400520C RID: 21004
		Boolean,
		// Token: 0x0400520D RID: 21005
		Null
	}
}
