using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000DE8 RID: 3560
	public static class ListMethodsExtensions
	{
		// Token: 0x0600585B RID: 22619 RVA: 0x001B6664 File Offset: 0x001B4864
		public static void RemoveAllNullItems<T>(this List<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
				}
			}
		}
	}
}
