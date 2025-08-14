using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B9C RID: 2972
	internal struct LeaderboardEntry_t
	{
		// Token: 0x040051AA RID: 20906
		internal int m_nGlobalRank;

		// Token: 0x040051AB RID: 20907
		internal int m_nScore;

		// Token: 0x040051AC RID: 20908
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
