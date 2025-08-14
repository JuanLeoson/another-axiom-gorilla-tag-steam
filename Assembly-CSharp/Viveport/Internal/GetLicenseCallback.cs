using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B90 RID: 2960
	// (Invoke) Token: 0x06004725 RID: 18213
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetLicenseCallback([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature);
}
