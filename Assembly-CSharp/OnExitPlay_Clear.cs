using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000ADB RID: 2779
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x060042EA RID: 17130 RVA: 0x0014FE54 File Offset: 0x0014E054
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
