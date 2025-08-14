using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD0 RID: 2768
public class OnEnterPlay_SetNull : OnEnterPlay_Attribute
{
	// Token: 0x060042D4 RID: 17108 RVA: 0x0014FC10 File Offset: 0x0014DE10
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNull non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, null);
	}
}
