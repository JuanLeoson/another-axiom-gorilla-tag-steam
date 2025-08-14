using System;

namespace GorillaTag
{
	// Token: 0x02000E4B RID: 3659
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06005BE3 RID: 23523 RVA: 0x001CF3E7 File Offset: 0x001CD5E7
		public string Condition { get; }

		// Token: 0x06005BE4 RID: 23524 RVA: 0x001CF3EF File Offset: 0x001CD5EF
		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}
	}
}
