using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000BA6 RID: 2982
	internal class Token
	{
		// Token: 0x060047B3 RID: 18355 RVA: 0x0015FAA7 File Offset: 0x0015DCA7
		static Token()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060047B4 RID: 18356
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x060047B5 RID: 18357
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x060047B6 RID: 18358
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken(StatusCallback2 GetSessionTokenCallback);

		// Token: 0x060047B7 RID: 18359
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken_64(StatusCallback2 GetSessionTokenCallback);
	}
}
