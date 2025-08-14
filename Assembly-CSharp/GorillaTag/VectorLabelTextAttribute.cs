using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E4C RID: 3660
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x06005BE5 RID: 23525 RVA: 0x001CF3FE File Offset: 0x001CD5FE
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x000121C7 File Offset: 0x000103C7
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
