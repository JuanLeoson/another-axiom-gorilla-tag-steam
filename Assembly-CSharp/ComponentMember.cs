using System;

// Token: 0x020001E5 RID: 485
public class ComponentMember
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x000405D5 File Offset: 0x0003E7D5
	public string Name { get; }

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000BA6 RID: 2982 RVA: 0x000405DD File Offset: 0x0003E7DD
	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x000405EA File Offset: 0x0003E7EA
	public bool IsStarred { get; }

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000BA8 RID: 2984 RVA: 0x000405F2 File Offset: 0x0003E7F2
	public string Color { get; }

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000405FA File Offset: 0x0003E7FA
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x04000E92 RID: 3730
	private Func<string> getValue;

	// Token: 0x04000E93 RID: 3731
	public string computedPrefix;

	// Token: 0x04000E94 RID: 3732
	public string computedSuffix;
}
