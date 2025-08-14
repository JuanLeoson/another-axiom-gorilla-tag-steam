using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000ADC RID: 2780
public class OnExitPlay_Run : OnExitPlay_Attribute
{
	// Token: 0x060042EC RID: 17132 RVA: 0x0014FD34 File Offset: 0x0014DF34
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
