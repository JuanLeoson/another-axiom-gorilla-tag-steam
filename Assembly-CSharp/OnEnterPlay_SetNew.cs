using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD2 RID: 2770
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x060042D8 RID: 17112 RVA: 0x0014FC88 File Offset: 0x0014DE88
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object value = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, value);
	}
}
