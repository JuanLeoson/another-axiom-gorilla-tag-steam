using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000BA7 RID: 2983
	internal class Deeplink
	{
		// Token: 0x060047B9 RID: 18361 RVA: 0x0015FAA7 File Offset: 0x0015DCA7
		static Deeplink()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060047BA RID: 18362
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_IsReady")]
		internal static extern void IsReady(StatusCallback IsReadyCallback);

		// Token: 0x060047BB RID: 18363
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_IsReady")]
		internal static extern void IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x060047BC RID: 18364
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToApp")]
		internal static extern void GoToApp(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060047BD RID: 18365
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToApp")]
		internal static extern void GoToApp_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060047BE RID: 18366
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToAppWithBranchName")]
		internal static extern void GoToApp(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData, string branchName);

		// Token: 0x060047BF RID: 18367
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToAppWithBranchName")]
		internal static extern void GoToApp_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData, string branchName);

		// Token: 0x060047C0 RID: 18368
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToStore")]
		internal static extern void GoToStore(StatusCallback2 GetSessionTokenCallback, string ViveportId);

		// Token: 0x060047C1 RID: 18369
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToStore")]
		internal static extern void GoToStore_64(StatusCallback2 GetSessionTokenCallback, string ViveportId);

		// Token: 0x060047C2 RID: 18370
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToAppOrGoToStore")]
		internal static extern void GoToAppOrGoToStore(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060047C3 RID: 18371
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GoToAppOrGoToStore")]
		internal static extern void GoToAppOrGoToStore_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060047C4 RID: 18372
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GetAppLaunchData")]
		internal static extern int GetAppLaunchData(StringBuilder userId, int size);

		// Token: 0x060047C5 RID: 18373
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportDeeplink_GetAppLaunchData")]
		internal static extern int GetAppLaunchData_64(StringBuilder userId, int size);
	}
}
