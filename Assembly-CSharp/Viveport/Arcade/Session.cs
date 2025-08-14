using System;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal.Arcade;

namespace Viveport.Arcade
{
	// Token: 0x02000BAA RID: 2986
	internal class Session
	{
		// Token: 0x060047D2 RID: 18386 RVA: 0x0015FB01 File Offset: 0x0015DD01
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Session.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x060047D3 RID: 18387 RVA: 0x0015FB0F File Offset: 0x0015DD0F
		public static void IsReady(Session.SessionListener listener)
		{
			Session.isReadyIl2cppCallback = new Session.SessionHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				Session.IsReady_64(new SessionCallback(Session.IsReadyIl2cppCallback));
				return;
			}
			Session.IsReady(new SessionCallback(Session.IsReadyIl2cppCallback));
		}

		// Token: 0x060047D4 RID: 18388 RVA: 0x0015FB4C File Offset: 0x0015DD4C
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StartIl2cppCallback(int errorCode, string message)
		{
			Session.startIl2cppCallback(errorCode, message);
		}

		// Token: 0x060047D5 RID: 18389 RVA: 0x0015FB5A File Offset: 0x0015DD5A
		public static void Start(Session.SessionListener listener)
		{
			Session.startIl2cppCallback = new Session.SessionHandler(listener).getStartHandler();
			if (IntPtr.Size == 8)
			{
				Session.Start_64(new SessionCallback(Session.StartIl2cppCallback));
				return;
			}
			Session.Start(new SessionCallback(Session.StartIl2cppCallback));
		}

		// Token: 0x060047D6 RID: 18390 RVA: 0x0015FB97 File Offset: 0x0015DD97
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StopIl2cppCallback(int errorCode, string message)
		{
			Session.stopIl2cppCallback(errorCode, message);
		}

		// Token: 0x060047D7 RID: 18391 RVA: 0x0015FBA5 File Offset: 0x0015DDA5
		public static void Stop(Session.SessionListener listener)
		{
			Session.stopIl2cppCallback = new Session.SessionHandler(listener).getStopHandler();
			if (IntPtr.Size == 8)
			{
				Session.Stop_64(new SessionCallback(Session.StopIl2cppCallback));
				return;
			}
			Session.Stop(new SessionCallback(Session.StopIl2cppCallback));
		}

		// Token: 0x040051AF RID: 20911
		private static SessionCallback isReadyIl2cppCallback;

		// Token: 0x040051B0 RID: 20912
		private static SessionCallback startIl2cppCallback;

		// Token: 0x040051B1 RID: 20913
		private static SessionCallback stopIl2cppCallback;

		// Token: 0x02000BAB RID: 2987
		private class SessionHandler : Session.BaseHandler
		{
			// Token: 0x060047D9 RID: 18393 RVA: 0x0015FBE2 File Offset: 0x0015DDE2
			public SessionHandler(Session.SessionListener cb)
			{
				Session.SessionHandler.listener = cb;
			}

			// Token: 0x060047DA RID: 18394 RVA: 0x0015FBF0 File Offset: 0x0015DDF0
			public SessionCallback getIsReadyHandler()
			{
				return new SessionCallback(this.IsReadyHandler);
			}

			// Token: 0x060047DB RID: 18395 RVA: 0x0015FC00 File Offset: 0x0015DE00
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session IsReadyHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session IsReadyHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
						}
						catch (Exception ex5)
						{
							string str3 = "[IsReadyHandler] appID ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] appID=" + text2);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnSuccess(text2);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060047DC RID: 18396 RVA: 0x0015FD74 File Offset: 0x0015DF74
			public SessionCallback getStartHandler()
			{
				return new SessionCallback(this.StartHandler);
			}

			// Token: 0x060047DD RID: 18397 RVA: 0x0015FD84 File Offset: 0x0015DF84
			protected override void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StartHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session StartHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[StartHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StartHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
							text3 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string str3 = "[StartHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StartHandler] appID=" + text2 + ",Guid=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStartSuccess(text2, text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060047DE RID: 18398 RVA: 0x0015FF18 File Offset: 0x0015E118
			public SessionCallback getStopHandler()
			{
				return new SessionCallback(this.StopHandler);
			}

			// Token: 0x060047DF RID: 18399 RVA: 0x0015FF28 File Offset: 0x0015E128
			protected override void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[Session StopHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string str = "[Session StopHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string str2 = "[StopHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StopHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							text2 = (string)jsonData["appID"];
							text3 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string str3 = "[StopHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StopHandler] appID=" + text2 + ",Guid=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStopSuccess(text2, text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x040051B2 RID: 20914
			private static Session.SessionListener listener;
		}

		// Token: 0x02000BAC RID: 2988
		private abstract class BaseHandler
		{
			// Token: 0x060047E0 RID: 18400
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060047E1 RID: 18401
			protected abstract void StartHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060047E2 RID: 18402
			protected abstract void StopHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000BAD RID: 2989
		public class SessionListener
		{
			// Token: 0x060047E4 RID: 18404 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnSuccess(string pchAppID)
			{
			}

			// Token: 0x060047E5 RID: 18405 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnStartSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x060047E6 RID: 18406 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnStopSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x060047E7 RID: 18407 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}
		}
	}
}
