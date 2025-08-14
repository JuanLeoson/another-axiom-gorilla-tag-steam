using System;

// Token: 0x0200075D RID: 1885
public interface IVariable<T> : IVariable
{
	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002F3E RID: 12094 RVA: 0x000F9CF9 File Offset: 0x000F7EF9
	// (set) Token: 0x06002F3F RID: 12095 RVA: 0x000F9D01 File Offset: 0x000F7F01
	T Value
	{
		get
		{
			return this.Get();
		}
		set
		{
			this.Set(value);
		}
	}

	// Token: 0x06002F40 RID: 12096
	T Get();

	// Token: 0x06002F41 RID: 12097
	void Set(T value);

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x06002F42 RID: 12098 RVA: 0x000F9D0A File Offset: 0x000F7F0A
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
