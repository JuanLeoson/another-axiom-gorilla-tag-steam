using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	// Token: 0x02000BA8 RID: 2984
	// (Invoke) Token: 0x060047C8 RID: 18376
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void SessionCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
