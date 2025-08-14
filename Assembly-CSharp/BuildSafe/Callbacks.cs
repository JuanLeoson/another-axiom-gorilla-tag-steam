using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000CE2 RID: 3298
	public static class Callbacks
	{
		// Token: 0x02000CE3 RID: 3299
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x060051F5 RID: 20981 RVA: 0x00198441 File Offset: 0x00196641
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04005B92 RID: 23442
			public bool activeOnly;
		}
	}
}
