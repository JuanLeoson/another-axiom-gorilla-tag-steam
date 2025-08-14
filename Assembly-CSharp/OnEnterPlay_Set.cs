using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD1 RID: 2769
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x060042D6 RID: 17110 RVA: 0x0014FC46 File Offset: 0x0014DE46
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x060042D7 RID: 17111 RVA: 0x0014FC55 File Offset: 0x0014DE55
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04004DC3 RID: 19907
	private object value;
}
