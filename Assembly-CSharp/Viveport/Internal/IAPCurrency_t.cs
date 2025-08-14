using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B9E RID: 2974
	internal struct IAPCurrency_t
	{
		// Token: 0x040051AD RID: 20909
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pName;

		// Token: 0x040051AE RID: 20910
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
