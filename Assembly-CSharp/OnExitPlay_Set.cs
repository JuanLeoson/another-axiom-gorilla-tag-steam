using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD9 RID: 2777
public class OnExitPlay_Set : OnExitPlay_Attribute
{
	// Token: 0x060042E6 RID: 17126 RVA: 0x0014FDB9 File Offset: 0x0014DFB9
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x0014FDC8 File Offset: 0x0014DFC8
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04004DC6 RID: 19910
	private object value;
}
