using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD3 RID: 2771
public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	// Token: 0x060042DA RID: 17114 RVA: 0x0014FCE0 File Offset: 0x0014DEE0
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
