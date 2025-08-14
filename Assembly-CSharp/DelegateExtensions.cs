using System;
using System.Collections.Generic;

// Token: 0x020001EA RID: 490
public static class DelegateExtensions
{
	// Token: 0x06000BB3 RID: 2995 RVA: 0x000406FC File Offset: 0x0003E8FC
	public static List<string> ToStringList(this Delegate[] invocationList)
	{
		List<string> list = new List<string>();
		if (invocationList != null)
		{
			foreach (Delegate @delegate in invocationList)
			{
				string name = @delegate.Method.Name;
				string str = (@delegate.Target != null) ? @delegate.Target.GetType().FullName : "Static Method";
				list.Add(str + "." + name);
			}
		}
		return list;
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00040769 File Offset: 0x0003E969
	public static string ToText(this Delegate[] invocationList)
	{
		return string.Join(", ", invocationList.ToStringList());
	}
}
