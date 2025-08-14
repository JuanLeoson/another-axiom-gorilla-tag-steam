using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B93 RID: 2963
	// (Invoke) Token: 0x06004731 RID: 18225
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
