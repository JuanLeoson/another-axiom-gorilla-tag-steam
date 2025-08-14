using System;

// Token: 0x020001E0 RID: 480
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000B9F RID: 2975 RVA: 0x000405A4 File Offset: 0x0003E7A4
	public string Color { get; }

	// Token: 0x06000BA0 RID: 2976 RVA: 0x000405AC File Offset: 0x0003E7AC
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
