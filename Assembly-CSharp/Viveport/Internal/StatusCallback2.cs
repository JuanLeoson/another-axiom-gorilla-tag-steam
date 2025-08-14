using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B92 RID: 2962
	// (Invoke) Token: 0x0600472D RID: 18221
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
