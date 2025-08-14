using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B76 RID: 2934
	public class User
	{
		// Token: 0x06004637 RID: 17975 RVA: 0x0015D325 File Offset: 0x0015B525
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			User.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06004638 RID: 17976 RVA: 0x0015D334 File Offset: 0x0015B534
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			User.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(User.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return User.IsReady_64(new StatusCallback(User.IsReadyIl2cppCallback));
			}
			return User.IsReady(new StatusCallback(User.IsReadyIl2cppCallback));
		}

		// Token: 0x06004639 RID: 17977 RVA: 0x0015D3A4 File Offset: 0x0015B5A4
		public static string GetUserId()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserID_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserID(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x0015D3E4 File Offset: 0x0015B5E4
		public static string GetUserName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserName_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserName(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x0015D424 File Offset: 0x0015B624
		public static string GetUserAvatarUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (IntPtr.Size == 8)
			{
				User.GetUserAvatarUrl_64(stringBuilder, 512);
			}
			else
			{
				User.GetUserAvatarUrl(stringBuilder, 512);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04005116 RID: 20758
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04005117 RID: 20759
		private const int MaxIdLength = 256;

		// Token: 0x04005118 RID: 20760
		private const int MaxNameLength = 256;

		// Token: 0x04005119 RID: 20761
		private const int MaxUrlLength = 512;
	}
}
