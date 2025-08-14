using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000D97 RID: 3479
	[Serializable]
	internal class FeatureFlagData
	{
		// Token: 0x0400601F RID: 24607
		public string name;

		// Token: 0x04006020 RID: 24608
		public int value;

		// Token: 0x04006021 RID: 24609
		public string valueType;

		// Token: 0x04006022 RID: 24610
		public List<string> alwaysOnForUsers;
	}
}
