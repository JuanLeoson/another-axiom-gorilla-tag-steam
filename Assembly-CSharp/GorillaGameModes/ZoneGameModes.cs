using System;

namespace GorillaGameModes
{
	// Token: 0x02000BE3 RID: 3043
	[Serializable]
	public struct ZoneGameModes
	{
		// Token: 0x040052AC RID: 21164
		public GTZone[] zone;

		// Token: 0x040052AD RID: 21165
		public GameModeType[] modes;

		// Token: 0x040052AE RID: 21166
		public GameModeType[] privateModes;
	}
}
