using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD8 RID: 2776
public class OnExitPlay_SetNull : OnExitPlay_Attribute
{
	// Token: 0x060042E4 RID: 17124 RVA: 0x0014FC10 File Offset: 0x0014DE10
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
