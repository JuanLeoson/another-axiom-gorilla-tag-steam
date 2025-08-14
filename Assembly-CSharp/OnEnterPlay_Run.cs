using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000AD4 RID: 2772
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x060042DC RID: 17116 RVA: 0x0014FD34 File Offset: 0x0014DF34
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
