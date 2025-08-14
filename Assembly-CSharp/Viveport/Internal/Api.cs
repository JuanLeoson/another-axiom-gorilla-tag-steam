using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000BA0 RID: 2976
	internal class Api
	{
		// Token: 0x06004748 RID: 18248 RVA: 0x0015FAA7 File Offset: 0x0015DCA7
		static Api()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06004749 RID: 18249
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x0600474A RID: 18250
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense_64(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x0600474B RID: 18251
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init(StatusCallback initCallback, string appId);

		// Token: 0x0600474C RID: 18252
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init_64(StatusCallback initCallback, string appId);

		// Token: 0x0600474D RID: 18253
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown(StatusCallback initCallback);

		// Token: 0x0600474E RID: 18254
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown_64(StatusCallback initCallback);

		// Token: 0x0600474F RID: 18255
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version();

		// Token: 0x06004750 RID: 18256
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version_64();

		// Token: 0x06004751 RID: 18257
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06004752 RID: 18258
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode_64(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06004753 RID: 18259
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllToLoad);

		// Token: 0x06004754 RID: 18260 RVA: 0x0015FAB4 File Offset: 0x0015DCB4
		internal static void LoadLibraryManually(string dllName)
		{
			if (string.IsNullOrEmpty(dllName))
			{
				return;
			}
			if (IntPtr.Size == 8)
			{
				Api.LoadLibrary("x64/" + dllName + "64.dll");
				return;
			}
			Api.LoadLibrary("x86/" + dllName + ".dll");
		}
	}
}
