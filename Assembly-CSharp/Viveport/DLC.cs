using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B8C RID: 2956
	public class DLC
	{
		// Token: 0x06004709 RID: 18185 RVA: 0x0015F464 File Offset: 0x0015D664
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsDlcReadyIl2cppCallback(int errorCode)
		{
			DLC.isDlcReadyIl2cppCallback(errorCode);
		}

		// Token: 0x0600470A RID: 18186 RVA: 0x0015F474 File Offset: 0x0015D674
		public static int IsDlcReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			DLC.isDlcReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return DLC.IsReady_64(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
			}
			return DLC.IsReady(new StatusCallback(DLC.IsDlcReadyIl2cppCallback));
		}

		// Token: 0x0600470B RID: 18187 RVA: 0x0015F4E1 File Offset: 0x0015D6E1
		public static int GetCount()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.GetCount_64();
			}
			return DLC.GetCount();
		}

		// Token: 0x0600470C RID: 18188 RVA: 0x0015F4F8 File Offset: 0x0015D6F8
		public static bool GetIsAvailable(int index, out string appId, out bool isAvailable)
		{
			StringBuilder stringBuilder = new StringBuilder(37);
			bool result;
			if (IntPtr.Size == 8)
			{
				result = DLC.GetIsAvailable_64(index, stringBuilder, out isAvailable);
			}
			else
			{
				result = DLC.GetIsAvailable(index, stringBuilder, out isAvailable);
			}
			appId = stringBuilder.ToString();
			return result;
		}

		// Token: 0x0600470D RID: 18189 RVA: 0x0015F534 File Offset: 0x0015D734
		public static int IsSubscribed()
		{
			if (IntPtr.Size == 8)
			{
				return DLC.IsSubscribed_64();
			}
			return DLC.IsSubscribed();
		}

		// Token: 0x04005176 RID: 20854
		private static StatusCallback isDlcReadyIl2cppCallback;

		// Token: 0x04005177 RID: 20855
		private const int AppIdLength = 37;
	}
}
