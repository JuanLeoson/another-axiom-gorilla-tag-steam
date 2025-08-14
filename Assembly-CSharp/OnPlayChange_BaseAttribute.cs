using System;
using System.Reflection;

// Token: 0x02000ACF RID: 2767
public class OnPlayChange_BaseAttribute : Attribute
{
	// Token: 0x060042D1 RID: 17105 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnEnterPlay(FieldInfo field)
	{
	}

	// Token: 0x060042D2 RID: 17106 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnEnterPlay(MethodInfo method)
	{
	}
}
