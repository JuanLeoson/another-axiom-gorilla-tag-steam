using System;
using System.Diagnostics;

// Token: 0x02000173 RID: 371
[Conditional("UNITY_EDITOR")]
public class DarkBoxAttribute : Attribute
{
	// Token: 0x060009AA RID: 2474 RVA: 0x0000224E File Offset: 0x0000044E
	public DarkBoxAttribute()
	{
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0003505D File Offset: 0x0003325D
	public DarkBoxAttribute(bool withBorders)
	{
		this.withBorders = withBorders;
	}

	// Token: 0x04000B7B RID: 2939
	public readonly bool withBorders;
}
