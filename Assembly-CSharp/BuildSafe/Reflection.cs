using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000CEA RID: 3306
	public static class Reflection<T>
	{
		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06005202 RID: 20994 RVA: 0x00198450 File Offset: 0x00196650
		public static Type Type { get; } = typeof(T);

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06005203 RID: 20995 RVA: 0x00198457 File Offset: 0x00196657
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06005204 RID: 20996 RVA: 0x0019845E File Offset: 0x0019665E
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06005205 RID: 20997 RVA: 0x00198465 File Offset: 0x00196665
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06005206 RID: 20998 RVA: 0x0019846C File Offset: 0x0019666C
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06005207 RID: 20999 RVA: 0x00198473 File Offset: 0x00196673
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x06005208 RID: 21000 RVA: 0x00198497 File Offset: 0x00196697
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x06005209 RID: 21001 RVA: 0x001984BB File Offset: 0x001966BB
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x0600520A RID: 21002 RVA: 0x001984DF File Offset: 0x001966DF
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x04005B93 RID: 23443
		private static Type gCachedType;

		// Token: 0x04005B94 RID: 23444
		private static MethodInfo[] gMethodsCache;

		// Token: 0x04005B95 RID: 23445
		private static FieldInfo[] gFieldsCache;

		// Token: 0x04005B96 RID: 23446
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04005B97 RID: 23447
		private static EventInfo[] gEventsCache;
	}
}
