using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B8F RID: 2959
	internal class Token
	{
		// Token: 0x0600471F RID: 18207 RVA: 0x0015F9A8 File Offset: 0x0015DBA8
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Token.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06004720 RID: 18208 RVA: 0x0015F9B8 File Offset: 0x0015DBB8
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Token.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.IsReady_64(new StatusCallback(Token.IsReadyIl2cppCallback));
				return;
			}
			Token.IsReady(new StatusCallback(Token.IsReadyIl2cppCallback));
		}

		// Token: 0x06004721 RID: 18209 RVA: 0x0015FA27 File Offset: 0x0015DC27
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GetSessionTokenIl2cppCallback(int errorCode, string message)
		{
			Token.getSessionTokenIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004722 RID: 18210 RVA: 0x0015FA38 File Offset: 0x0015DC38
		public static void GetSessionToken(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.getSessionTokenIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.GetSessionToken_64(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
				return;
			}
			Token.GetSessionToken(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
		}

		// Token: 0x0400517F RID: 20863
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04005180 RID: 20864
		private static StatusCallback2 getSessionTokenIl2cppCallback;
	}
}
