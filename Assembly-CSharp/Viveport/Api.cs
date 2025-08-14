using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using AOT;
using LitJson;
using PublicKeyConvert;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B74 RID: 2932
	public class Api
	{
		// Token: 0x06004628 RID: 17960 RVA: 0x0015CD60 File Offset: 0x0015AF60
		public static void GetLicense(Api.LicenseChecker checker, string appId, string appKey)
		{
			if (checker == null || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey))
			{
				throw new InvalidOperationException("checker == null || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appKey)");
			}
			Api._appId = appId;
			Api._appKey = appKey;
			Api.InternalLicenseCheckers.Add(checker);
			if (IntPtr.Size == 8)
			{
				Api.GetLicense_64(new GetLicenseCallback(Api.GetLicenseHandler), Api._appId, Api._appKey);
				return;
			}
			Api.GetLicense(new GetLicenseCallback(Api.GetLicenseHandler), Api._appId, Api._appKey);
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x0015CDE1 File Offset: 0x0015AFE1
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void InitIl2cppCallback(int errorCode)
		{
			Api.initIl2cppCallback(errorCode);
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x0015CDF0 File Offset: 0x0015AFF0
		public static int Init(StatusCallback callback, string appId)
		{
			if (callback == null || string.IsNullOrEmpty(appId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(appId)");
			}
			Api.initIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Api.InitIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return Api.Init_64(new StatusCallback(Api.InitIl2cppCallback), appId);
			}
			return Api.Init(new StatusCallback(Api.InitIl2cppCallback), appId);
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x0015CE67 File Offset: 0x0015B067
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void ShutdownIl2cppCallback(int errorCode)
		{
			Api.shutdownIl2cppCallback(errorCode);
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x0015CE74 File Offset: 0x0015B074
		public static int Shutdown(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Api.shutdownIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Api.ShutdownIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return Api.Shutdown_64(new StatusCallback(Api.ShutdownIl2cppCallback));
			}
			return Api.Shutdown(new StatusCallback(Api.ShutdownIl2cppCallback));
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x0015CEE4 File Offset: 0x0015B0E4
		public static string Version()
		{
			string text = "";
			try
			{
				if (IntPtr.Size == 8)
				{
					text += Marshal.PtrToStringAnsi(Api.Version_64());
				}
				else
				{
					text += Marshal.PtrToStringAnsi(Api.Version());
				}
			}
			catch (Exception)
			{
				Logger.Log("Can not load version from native library");
			}
			return "C# version: " + Api.VERSION + ", Native version: " + text;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x0015CF58 File Offset: 0x0015B158
		[MonoPInvokeCallback(typeof(QueryRuntimeModeCallback))]
		private static void QueryRuntimeModeIl2cppCallback(int errorCode, int mode)
		{
			Api.queryRuntimeModeIl2cppCallback(errorCode, mode);
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x0015CF68 File Offset: 0x0015B168
		public static void QueryRuntimeMode(QueryRuntimeModeCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Api.queryRuntimeModeIl2cppCallback = new QueryRuntimeModeCallback(callback.Invoke);
			Api.InternalQueryRunTimeCallbacks.Add(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Api.QueryRuntimeMode_64(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
				return;
			}
			Api.QueryRuntimeMode(new QueryRuntimeModeCallback(Api.QueryRuntimeModeIl2cppCallback));
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x0015CFD8 File Offset: 0x0015B1D8
		[MonoPInvokeCallback(typeof(GetLicenseCallback))]
		private static void GetLicenseHandler([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature)
		{
			if (string.IsNullOrEmpty(message))
			{
				for (int i = Api.InternalLicenseCheckers.Count - 1; i >= 0; i--)
				{
					Api.LicenseChecker licenseChecker = Api.InternalLicenseCheckers[i];
					licenseChecker.OnFailure(90003, "License message is empty");
					Api.InternalLicenseCheckers.Remove(licenseChecker);
				}
				return;
			}
			if (string.IsNullOrEmpty(signature))
			{
				JsonData jsonData = JsonMapper.ToObject(message);
				int errorCode = 99999;
				string errorMessage = "";
				try
				{
					errorCode = int.Parse((string)jsonData["code"]);
				}
				catch
				{
				}
				try
				{
					errorMessage = (string)jsonData["message"];
				}
				catch
				{
				}
				for (int j = Api.InternalLicenseCheckers.Count - 1; j >= 0; j--)
				{
					Api.LicenseChecker licenseChecker2 = Api.InternalLicenseCheckers[j];
					licenseChecker2.OnFailure(errorCode, errorMessage);
					Api.InternalLicenseCheckers.Remove(licenseChecker2);
				}
				return;
			}
			if (!Api.VerifyMessage(Api._appId, Api._appKey, message, signature))
			{
				for (int k = Api.InternalLicenseCheckers.Count - 1; k >= 0; k--)
				{
					Api.LicenseChecker licenseChecker3 = Api.InternalLicenseCheckers[k];
					licenseChecker3.OnFailure(90001, "License verification failed");
					Api.InternalLicenseCheckers.Remove(licenseChecker3);
				}
				return;
			}
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(message.Substring(message.IndexOf("\n", StringComparison.Ordinal) + 1)));
			JsonData jsonData2 = JsonMapper.ToObject(@string);
			Logger.Log("License: " + @string);
			long issueTime = -1L;
			long expirationTime = -1L;
			int latestVersion = -1;
			bool updateRequired = false;
			try
			{
				issueTime = (long)jsonData2["issueTime"];
			}
			catch
			{
			}
			try
			{
				expirationTime = (long)jsonData2["expirationTime"];
			}
			catch
			{
			}
			try
			{
				latestVersion = (int)jsonData2["latestVersion"];
			}
			catch
			{
			}
			try
			{
				updateRequired = (bool)jsonData2["updateRequired"];
			}
			catch
			{
			}
			for (int l = Api.InternalLicenseCheckers.Count - 1; l >= 0; l--)
			{
				Api.LicenseChecker licenseChecker4 = Api.InternalLicenseCheckers[l];
				licenseChecker4.OnSuccess(issueTime, expirationTime, latestVersion, updateRequired);
				Api.InternalLicenseCheckers.Remove(licenseChecker4);
			}
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x0015D264 File Offset: 0x0015B464
		private static bool VerifyMessage(string appId, string appKey, string message, string signature)
		{
			try
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = PEMKeyLoader.CryptoServiceProviderFromPublicKeyInfo(appKey);
				byte[] signature2 = Convert.FromBase64String(signature);
				SHA1Managed halg = new SHA1Managed();
				byte[] bytes = Encoding.UTF8.GetBytes(appId + "\n" + message);
				return rsacryptoServiceProvider.VerifyData(bytes, halg, signature2);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
			return false;
		}

		// Token: 0x0400510B RID: 20747
		internal static readonly List<GetLicenseCallback> InternalGetLicenseCallbacks = new List<GetLicenseCallback>();

		// Token: 0x0400510C RID: 20748
		internal static readonly List<StatusCallback> InternalStatusCallbacks = new List<StatusCallback>();

		// Token: 0x0400510D RID: 20749
		internal static readonly List<QueryRuntimeModeCallback> InternalQueryRunTimeCallbacks = new List<QueryRuntimeModeCallback>();

		// Token: 0x0400510E RID: 20750
		internal static readonly List<StatusCallback2> InternalStatusCallback2s = new List<StatusCallback2>();

		// Token: 0x0400510F RID: 20751
		internal static readonly List<Api.LicenseChecker> InternalLicenseCheckers = new List<Api.LicenseChecker>();

		// Token: 0x04005110 RID: 20752
		private static StatusCallback initIl2cppCallback;

		// Token: 0x04005111 RID: 20753
		private static StatusCallback shutdownIl2cppCallback;

		// Token: 0x04005112 RID: 20754
		private static QueryRuntimeModeCallback queryRuntimeModeIl2cppCallback;

		// Token: 0x04005113 RID: 20755
		private static readonly string VERSION = "1.7.2.30";

		// Token: 0x04005114 RID: 20756
		private static string _appId = "";

		// Token: 0x04005115 RID: 20757
		private static string _appKey = "";

		// Token: 0x02000B75 RID: 2933
		public abstract class LicenseChecker
		{
			// Token: 0x06004634 RID: 17972
			public abstract void OnSuccess(long issueTime, long expirationTime, int latestVersion, bool updateRequired);

			// Token: 0x06004635 RID: 17973
			public abstract void OnFailure(int errorCode, string errorMessage);
		}
	}
}
