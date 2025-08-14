using System;
using System.Diagnostics;

// Token: 0x02000175 RID: 373
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.All)]
public class InlineAttribute : Attribute
{
	// Token: 0x060009AD RID: 2477 RVA: 0x0003506C File Offset: 0x0003326C
	public InlineAttribute(bool keepLabel = false, bool asGroup = false)
	{
		this.keepLabel = keepLabel;
		this.asGroup = asGroup;
	}

	// Token: 0x04000B7C RID: 2940
	public readonly bool keepLabel;

	// Token: 0x04000B7D RID: 2941
	public readonly bool asGroup;
}
