using System;

namespace Utilities
{
	// Token: 0x02000BDD RID: 3037
	public static class PathUtils
	{
		// Token: 0x060049A8 RID: 18856 RVA: 0x001661D8 File Offset: 0x001643D8
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] value = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", value)).AbsolutePath);
		}

		// Token: 0x0400527F RID: 21119
		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		// Token: 0x04005280 RID: 21120
		private const string kFwdSlash = "/";
	}
}
