using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000CE6 RID: 3302
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Class)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
