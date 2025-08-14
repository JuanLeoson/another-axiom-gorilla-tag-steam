using System;
using System.Collections.Generic;
using System.Text;

namespace GorillaTag.Scripts.Utilities
{
	// Token: 0x02000EA1 RID: 3745
	public static class GTStr
	{
		// Token: 0x06005DAE RID: 23982 RVA: 0x001D87F8 File Offset: 0x001D69F8
		public static void Bullet(StringBuilder builder, IList<string> strings, string bulletStr = "- ")
		{
			for (int i = 0; i < strings.Count; i++)
			{
				builder.Append(bulletStr).Append(strings[i]).Append("\n");
			}
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x001D8834 File Offset: 0x001D6A34
		public static string Bullet(IList<string> strings, string bulletStr = "- ")
		{
			if (strings == null || strings.Count == 0)
			{
				return string.Empty;
			}
			int num = strings.Count * (bulletStr.Length + 1);
			for (int i = 0; i < strings.Count; i++)
			{
				num += strings[i].Length;
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			GTStr.Bullet(stringBuilder, strings, bulletStr);
			return stringBuilder.ToString();
		}
	}
}
