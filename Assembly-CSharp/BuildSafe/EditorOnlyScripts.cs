using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000CE7 RID: 3303
	internal static class EditorOnlyScripts
	{
		// Token: 0x060051FE RID: 20990 RVA: 0x000023F5 File Offset: 0x000005F5
		[Conditional("UNITY_EDITOR")]
		public static void Cleanup(GameObject[] rootObjects, bool force = false)
		{
		}
	}
}
