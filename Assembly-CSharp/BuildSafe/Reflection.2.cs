using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000CEB RID: 3307
	public static class Reflection
	{
		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x0600520C RID: 21004 RVA: 0x00198514 File Offset: 0x00196714
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x0600520D RID: 21005 RVA: 0x0019851B File Offset: 0x0019671B
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x0600520E RID: 21006 RVA: 0x00198522 File Offset: 0x00196722
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x0600520F RID: 21007 RVA: 0x00198530 File Offset: 0x00196730
		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = (from a in AppDomain.CurrentDomain.GetAssemblies()
			where a != null
			select a).ToArray<Assembly>();
		}

		// Token: 0x06005210 RID: 21008 RVA: 0x00198584 File Offset: 0x00196784
		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = (from t in Reflection.PreFetchAllAssemblies().SelectMany((Assembly a) => a.GetTypes())
			where t != null
			select t).ToArray<Type>();
		}

		// Token: 0x06005211 RID: 21009 RVA: 0x001985F8 File Offset: 0x001967F8
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
			where m.GetCustomAttributes(typeof(T), false).Length != 0
			select m).ToArray<MethodInfo>();
		}

		// Token: 0x04005B99 RID: 23449
		private static Assembly[] gAssemblyCache;

		// Token: 0x04005B9A RID: 23450
		private static Type[] gTypeCache;
	}
}
