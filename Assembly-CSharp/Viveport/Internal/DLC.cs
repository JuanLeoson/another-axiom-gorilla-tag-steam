using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000BA4 RID: 2980
	internal class DLC
	{
		// Token: 0x0600479F RID: 18335 RVA: 0x0015FAA7 File Offset: 0x0015DCA7
		static DLC()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060047A0 RID: 18336
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady(StatusCallback callback);

		// Token: 0x060047A1 RID: 18337
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady_64(StatusCallback callback);

		// Token: 0x060047A2 RID: 18338
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount();

		// Token: 0x060047A3 RID: 18339
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount_64();

		// Token: 0x060047A4 RID: 18340
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x060047A5 RID: 18341
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable_64(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x060047A6 RID: 18342
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed();

		// Token: 0x060047A7 RID: 18343
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed_64();
	}
}
