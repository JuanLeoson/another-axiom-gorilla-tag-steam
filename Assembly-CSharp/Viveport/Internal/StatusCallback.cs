using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B91 RID: 2961
	// (Invoke) Token: 0x06004729 RID: 18217
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback(int nResult);
}
