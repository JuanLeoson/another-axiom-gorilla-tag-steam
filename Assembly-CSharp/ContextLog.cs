using System;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000861 RID: 2145
public static class ContextLog
{
	// Token: 0x060035F8 RID: 13816 RVA: 0x0011BC8F File Offset: 0x00119E8F
	public static void Log<T0, T1>(this T0 ctx, T1 arg1)
	{
		Debug.Log(ZString.Concat<string, T1>(ContextLog.GetPrefix<T0>(ref ctx), arg1));
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x0011BCA4 File Offset: 0x00119EA4
	public static void LogCall<T0, T1>(this T0 ctx, T1 arg1, [CallerMemberName] string call = null)
	{
		string prefix = ContextLog.GetPrefix<T0>(ref ctx);
		string arg2 = ZString.Concat<string, string, string>("{.", call, "()} ");
		Debug.Log(ZString.Concat<string, string, T1>(prefix, arg2, arg1));
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x0011BCD8 File Offset: 0x00119ED8
	private static string GetPrefix<T>(ref T ctx)
	{
		if (ctx == null)
		{
			return string.Empty;
		}
		Type type = ctx as Type;
		string arg;
		if (type != null)
		{
			arg = type.Name;
		}
		else
		{
			string text = ctx as string;
			if (text != null)
			{
				arg = text;
			}
			else
			{
				arg = ctx.GetType().Name;
			}
		}
		return ZString.Concat<string, string, string>("[", arg, "] ");
	}
}
