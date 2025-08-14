using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	// Token: 0x02000BA9 RID: 2985
	internal class Session
	{
		// Token: 0x060047CB RID: 18379
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady(SessionCallback callback);

		// Token: 0x060047CC RID: 18380
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady_64(SessionCallback callback);

		// Token: 0x060047CD RID: 18381
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start(SessionCallback callback);

		// Token: 0x060047CE RID: 18382
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start_64(SessionCallback callback);

		// Token: 0x060047CF RID: 18383
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop(SessionCallback callback);

		// Token: 0x060047D0 RID: 18384
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop_64(SessionCallback callback);
	}
}
