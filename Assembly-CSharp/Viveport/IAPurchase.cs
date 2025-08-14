using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B80 RID: 2944
	public class IAPurchase
	{
		// Token: 0x06004662 RID: 18018 RVA: 0x0015DAE3 File Offset: 0x0015BCE3
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004663 RID: 18019 RVA: 0x0015DAF1 File Offset: 0x0015BCF1
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x06004664 RID: 18020 RVA: 0x0015DB30 File Offset: 0x0015BD30
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06004665 RID: 18021 RVA: 0x0015DB3E File Offset: 0x0015BD3E
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x06004666 RID: 18022 RVA: 0x0015DB7D File Offset: 0x0015BD7D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06004667 RID: 18023 RVA: 0x0015DB8C File Offset: 0x0015BD8C
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x06004668 RID: 18024 RVA: 0x0015DBD8 File Offset: 0x0015BDD8
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004669 RID: 18025 RVA: 0x0015DBE6 File Offset: 0x0015BDE6
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x0600466A RID: 18026 RVA: 0x0015DC25 File Offset: 0x0015BE25
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x0600466B RID: 18027 RVA: 0x0015DC33 File Offset: 0x0015BE33
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x0600466C RID: 18028 RVA: 0x0015DC72 File Offset: 0x0015BE72
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x0600466D RID: 18029 RVA: 0x0015DC80 File Offset: 0x0015BE80
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x0600466E RID: 18030 RVA: 0x0015DCBD File Offset: 0x0015BEBD
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600466F RID: 18031 RVA: 0x0015DCCB File Offset: 0x0015BECB
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x06004670 RID: 18032 RVA: 0x0015DD08 File Offset: 0x0015BF08
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004671 RID: 18033 RVA: 0x0015DD18 File Offset: 0x0015BF18
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x06004672 RID: 18034 RVA: 0x0015DD76 File Offset: 0x0015BF76
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004673 RID: 18035 RVA: 0x0015DD84 File Offset: 0x0015BF84
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x06004674 RID: 18036 RVA: 0x0015DDC3 File Offset: 0x0015BFC3
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004675 RID: 18037 RVA: 0x0015DDD1 File Offset: 0x0015BFD1
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06004676 RID: 18038 RVA: 0x0015DE10 File Offset: 0x0015C010
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004677 RID: 18039 RVA: 0x0015DE1E File Offset: 0x0015C01E
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06004678 RID: 18040 RVA: 0x0015DE5D File Offset: 0x0015C05D
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x06004679 RID: 18041 RVA: 0x0015DE6B File Offset: 0x0015C06B
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x0600467A RID: 18042 RVA: 0x0015DEA8 File Offset: 0x0015C0A8
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600467B RID: 18043 RVA: 0x0015DEB6 File Offset: 0x0015C0B6
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x0400513F RID: 20799
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x04005140 RID: 20800
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04005141 RID: 20801
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04005142 RID: 20802
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04005143 RID: 20803
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04005144 RID: 20804
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04005145 RID: 20805
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x04005146 RID: 20806
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x04005147 RID: 20807
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x04005148 RID: 20808
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x04005149 RID: 20809
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x0400514A RID: 20810
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x0400514B RID: 20811
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000B81 RID: 2945
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x0600467D RID: 18045 RVA: 0x0015DEF5 File Offset: 0x0015C0F5
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x0600467E RID: 18046 RVA: 0x0015DF03 File Offset: 0x0015C103
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x0600467F RID: 18047 RVA: 0x0015DF14 File Offset: 0x0015C114
			protected override void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string str2 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004680 RID: 18048 RVA: 0x0015E044 File Offset: 0x0015C244
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06004681 RID: 18049 RVA: 0x0015E054 File Offset: 0x0015C254
			protected override void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string str2 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004682 RID: 18050 RVA: 0x0015E184 File Offset: 0x0015C384
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06004683 RID: 18051 RVA: 0x0015E194 File Offset: 0x0015C394
			protected override void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004684 RID: 18052 RVA: 0x0015E2E4 File Offset: 0x0015C4E4
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06004685 RID: 18053 RVA: 0x0015E2F4 File Offset: 0x0015C4F4
			protected override void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long paid_timestamp = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							paid_timestamp = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] status =",
							text4,
							",price=",
							text5,
							",currency=",
							text6
						}));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							paid_timestamp.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = paid_timestamp;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004686 RID: 18054 RVA: 0x0015E540 File Offset: 0x0015C740
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06004687 RID: 18055 RVA: 0x0015E550 File Offset: 0x0015C750
			protected override void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int total = 0;
				int from = 0;
				int to = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							total = (int)jsonData2["total"];
							from = (int)jsonData2["from"];
							to = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in ((IEnumerable)jsonData3))
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)((int)jsonData4["paid_timestamp"]);
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string str2 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = total;
							queryListResponse.from = from;
							queryListResponse.to = to;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004688 RID: 18056 RVA: 0x0015E8D8 File Offset: 0x0015CAD8
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x06004689 RID: 18057 RVA: 0x0015E8E8 File Offset: 0x0015CAE8
			protected override void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string str = "";
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str2 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							str = (string)jsonData["currencyName"];
							text = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string str3 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(str3 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + str + ",balance=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600468A RID: 18058 RVA: 0x0015EA44 File Offset: 0x0015CC44
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x0600468B RID: 18059 RVA: 0x0015EA54 File Offset: 0x0015CC54
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600468C RID: 18060 RVA: 0x0015EB7C File Offset: 0x0015CD7C
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x0600468D RID: 18061 RVA: 0x0015EB8C File Offset: 0x0015CD8C
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string str2 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600468E RID: 18062 RVA: 0x0015ECB4 File Offset: 0x0015CEB4
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x0600468F RID: 18063 RVA: 0x0015ECC4 File Offset: 0x0015CEC4
			protected override void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string str = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string str2 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004690 RID: 18064 RVA: 0x0015EE4C File Offset: 0x0015D04C
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x06004691 RID: 18065 RVA: 0x0015EE5C File Offset: 0x0015D05C
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004692 RID: 18066 RVA: 0x0015EF84 File Offset: 0x0015D184
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x06004693 RID: 18067 RVA: 0x0015EF94 File Offset: 0x0015D194
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string str2 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06004694 RID: 18068 RVA: 0x0015F0BC File Offset: 0x0015D2BC
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x06004695 RID: 18069 RVA: 0x0015F0CC File Offset: 0x0015D2CC
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool bCanceled = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string str = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						bCanceled = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + bCanceled.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(bCanceled);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0400514C RID: 20812
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000B82 RID: 2946
		private abstract class BaseHandler
		{
			// Token: 0x06004696 RID: 18070
			protected abstract void IsReadyHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06004697 RID: 18071
			protected abstract void RequestHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06004698 RID: 18072
			protected abstract void PurchaseHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x06004699 RID: 18073
			protected abstract void QueryHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469A RID: 18074
			protected abstract void QueryListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469B RID: 18075
			protected abstract void BalanceHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469C RID: 18076
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469D RID: 18077
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469E RID: 18078
			protected abstract void SubscribeHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0600469F RID: 18079
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060046A0 RID: 18080
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x060046A1 RID: 18081
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
		}

		// Token: 0x02000B83 RID: 2947
		public class IAPurchaseListener
		{
			// Token: 0x060046A3 RID: 18083 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x060046A4 RID: 18084 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060046A5 RID: 18085 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060046A6 RID: 18086 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x060046A7 RID: 18087 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x060046A8 RID: 18088 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x060046A9 RID: 18089 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x060046AA RID: 18090 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060046AB RID: 18091 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060046AC RID: 18092 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060046AD RID: 18093 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x060046AE RID: 18094 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x060046AF RID: 18095 RVA: 0x000023F5 File Offset: 0x000005F5
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000B84 RID: 2948
		public class QueryResponse
		{
			// Token: 0x170006AE RID: 1710
			// (get) Token: 0x060046B1 RID: 18097 RVA: 0x0015F1BC File Offset: 0x0015D3BC
			// (set) Token: 0x060046B2 RID: 18098 RVA: 0x0015F1C4 File Offset: 0x0015D3C4
			public string order_id { get; set; }

			// Token: 0x170006AF RID: 1711
			// (get) Token: 0x060046B3 RID: 18099 RVA: 0x0015F1CD File Offset: 0x0015D3CD
			// (set) Token: 0x060046B4 RID: 18100 RVA: 0x0015F1D5 File Offset: 0x0015D3D5
			public string purchase_id { get; set; }

			// Token: 0x170006B0 RID: 1712
			// (get) Token: 0x060046B5 RID: 18101 RVA: 0x0015F1DE File Offset: 0x0015D3DE
			// (set) Token: 0x060046B6 RID: 18102 RVA: 0x0015F1E6 File Offset: 0x0015D3E6
			public string status { get; set; }

			// Token: 0x170006B1 RID: 1713
			// (get) Token: 0x060046B7 RID: 18103 RVA: 0x0015F1EF File Offset: 0x0015D3EF
			// (set) Token: 0x060046B8 RID: 18104 RVA: 0x0015F1F7 File Offset: 0x0015D3F7
			public string price { get; set; }

			// Token: 0x170006B2 RID: 1714
			// (get) Token: 0x060046B9 RID: 18105 RVA: 0x0015F200 File Offset: 0x0015D400
			// (set) Token: 0x060046BA RID: 18106 RVA: 0x0015F208 File Offset: 0x0015D408
			public string currency { get; set; }

			// Token: 0x170006B3 RID: 1715
			// (get) Token: 0x060046BB RID: 18107 RVA: 0x0015F211 File Offset: 0x0015D411
			// (set) Token: 0x060046BC RID: 18108 RVA: 0x0015F219 File Offset: 0x0015D419
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000B85 RID: 2949
		public class QueryResponse2
		{
			// Token: 0x170006B4 RID: 1716
			// (get) Token: 0x060046BE RID: 18110 RVA: 0x0015F222 File Offset: 0x0015D422
			// (set) Token: 0x060046BF RID: 18111 RVA: 0x0015F22A File Offset: 0x0015D42A
			public string order_id { get; set; }

			// Token: 0x170006B5 RID: 1717
			// (get) Token: 0x060046C0 RID: 18112 RVA: 0x0015F233 File Offset: 0x0015D433
			// (set) Token: 0x060046C1 RID: 18113 RVA: 0x0015F23B File Offset: 0x0015D43B
			public string app_id { get; set; }

			// Token: 0x170006B6 RID: 1718
			// (get) Token: 0x060046C2 RID: 18114 RVA: 0x0015F244 File Offset: 0x0015D444
			// (set) Token: 0x060046C3 RID: 18115 RVA: 0x0015F24C File Offset: 0x0015D44C
			public string purchase_id { get; set; }

			// Token: 0x170006B7 RID: 1719
			// (get) Token: 0x060046C4 RID: 18116 RVA: 0x0015F255 File Offset: 0x0015D455
			// (set) Token: 0x060046C5 RID: 18117 RVA: 0x0015F25D File Offset: 0x0015D45D
			public string user_data { get; set; }

			// Token: 0x170006B8 RID: 1720
			// (get) Token: 0x060046C6 RID: 18118 RVA: 0x0015F266 File Offset: 0x0015D466
			// (set) Token: 0x060046C7 RID: 18119 RVA: 0x0015F26E File Offset: 0x0015D46E
			public string price { get; set; }

			// Token: 0x170006B9 RID: 1721
			// (get) Token: 0x060046C8 RID: 18120 RVA: 0x0015F277 File Offset: 0x0015D477
			// (set) Token: 0x060046C9 RID: 18121 RVA: 0x0015F27F File Offset: 0x0015D47F
			public string currency { get; set; }

			// Token: 0x170006BA RID: 1722
			// (get) Token: 0x060046CA RID: 18122 RVA: 0x0015F288 File Offset: 0x0015D488
			// (set) Token: 0x060046CB RID: 18123 RVA: 0x0015F290 File Offset: 0x0015D490
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000B86 RID: 2950
		public class QueryListResponse
		{
			// Token: 0x170006BB RID: 1723
			// (get) Token: 0x060046CD RID: 18125 RVA: 0x0015F299 File Offset: 0x0015D499
			// (set) Token: 0x060046CE RID: 18126 RVA: 0x0015F2A1 File Offset: 0x0015D4A1
			public int total { get; set; }

			// Token: 0x170006BC RID: 1724
			// (get) Token: 0x060046CF RID: 18127 RVA: 0x0015F2AA File Offset: 0x0015D4AA
			// (set) Token: 0x060046D0 RID: 18128 RVA: 0x0015F2B2 File Offset: 0x0015D4B2
			public int from { get; set; }

			// Token: 0x170006BD RID: 1725
			// (get) Token: 0x060046D1 RID: 18129 RVA: 0x0015F2BB File Offset: 0x0015D4BB
			// (set) Token: 0x060046D2 RID: 18130 RVA: 0x0015F2C3 File Offset: 0x0015D4C3
			public int to { get; set; }

			// Token: 0x0400515D RID: 20829
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000B87 RID: 2951
		public class StatusDetailTransaction
		{
			// Token: 0x170006BE RID: 1726
			// (get) Token: 0x060046D4 RID: 18132 RVA: 0x0015F2CC File Offset: 0x0015D4CC
			// (set) Token: 0x060046D5 RID: 18133 RVA: 0x0015F2D4 File Offset: 0x0015D4D4
			public long create_time { get; set; }

			// Token: 0x170006BF RID: 1727
			// (get) Token: 0x060046D6 RID: 18134 RVA: 0x0015F2DD File Offset: 0x0015D4DD
			// (set) Token: 0x060046D7 RID: 18135 RVA: 0x0015F2E5 File Offset: 0x0015D4E5
			public string payment_method { get; set; }

			// Token: 0x170006C0 RID: 1728
			// (get) Token: 0x060046D8 RID: 18136 RVA: 0x0015F2EE File Offset: 0x0015D4EE
			// (set) Token: 0x060046D9 RID: 18137 RVA: 0x0015F2F6 File Offset: 0x0015D4F6
			public string status { get; set; }
		}

		// Token: 0x02000B88 RID: 2952
		public class StatusDetail
		{
			// Token: 0x170006C1 RID: 1729
			// (get) Token: 0x060046DB RID: 18139 RVA: 0x0015F2FF File Offset: 0x0015D4FF
			// (set) Token: 0x060046DC RID: 18140 RVA: 0x0015F307 File Offset: 0x0015D507
			public long date_next_charge { get; set; }

			// Token: 0x170006C2 RID: 1730
			// (get) Token: 0x060046DD RID: 18141 RVA: 0x0015F310 File Offset: 0x0015D510
			// (set) Token: 0x060046DE RID: 18142 RVA: 0x0015F318 File Offset: 0x0015D518
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x170006C3 RID: 1731
			// (get) Token: 0x060046DF RID: 18143 RVA: 0x0015F321 File Offset: 0x0015D521
			// (set) Token: 0x060046E0 RID: 18144 RVA: 0x0015F329 File Offset: 0x0015D529
			public string cancel_reason { get; set; }
		}

		// Token: 0x02000B89 RID: 2953
		public class TimePeriod
		{
			// Token: 0x170006C4 RID: 1732
			// (get) Token: 0x060046E2 RID: 18146 RVA: 0x0015F332 File Offset: 0x0015D532
			// (set) Token: 0x060046E3 RID: 18147 RVA: 0x0015F33A File Offset: 0x0015D53A
			public string time_type { get; set; }

			// Token: 0x170006C5 RID: 1733
			// (get) Token: 0x060046E4 RID: 18148 RVA: 0x0015F343 File Offset: 0x0015D543
			// (set) Token: 0x060046E5 RID: 18149 RVA: 0x0015F34B File Offset: 0x0015D54B
			public int value { get; set; }
		}

		// Token: 0x02000B8A RID: 2954
		public class Subscription
		{
			// Token: 0x170006C6 RID: 1734
			// (get) Token: 0x060046E7 RID: 18151 RVA: 0x0015F354 File Offset: 0x0015D554
			// (set) Token: 0x060046E8 RID: 18152 RVA: 0x0015F35C File Offset: 0x0015D55C
			public string app_id { get; set; }

			// Token: 0x170006C7 RID: 1735
			// (get) Token: 0x060046E9 RID: 18153 RVA: 0x0015F365 File Offset: 0x0015D565
			// (set) Token: 0x060046EA RID: 18154 RVA: 0x0015F36D File Offset: 0x0015D56D
			public string order_id { get; set; }

			// Token: 0x170006C8 RID: 1736
			// (get) Token: 0x060046EB RID: 18155 RVA: 0x0015F376 File Offset: 0x0015D576
			// (set) Token: 0x060046EC RID: 18156 RVA: 0x0015F37E File Offset: 0x0015D57E
			public string subscription_id { get; set; }

			// Token: 0x170006C9 RID: 1737
			// (get) Token: 0x060046ED RID: 18157 RVA: 0x0015F387 File Offset: 0x0015D587
			// (set) Token: 0x060046EE RID: 18158 RVA: 0x0015F38F File Offset: 0x0015D58F
			public string price { get; set; }

			// Token: 0x170006CA RID: 1738
			// (get) Token: 0x060046EF RID: 18159 RVA: 0x0015F398 File Offset: 0x0015D598
			// (set) Token: 0x060046F0 RID: 18160 RVA: 0x0015F3A0 File Offset: 0x0015D5A0
			public string currency { get; set; }

			// Token: 0x170006CB RID: 1739
			// (get) Token: 0x060046F1 RID: 18161 RVA: 0x0015F3A9 File Offset: 0x0015D5A9
			// (set) Token: 0x060046F2 RID: 18162 RVA: 0x0015F3B1 File Offset: 0x0015D5B1
			public long subscribed_timestamp { get; set; }

			// Token: 0x170006CC RID: 1740
			// (get) Token: 0x060046F3 RID: 18163 RVA: 0x0015F3BA File Offset: 0x0015D5BA
			// (set) Token: 0x060046F4 RID: 18164 RVA: 0x0015F3C2 File Offset: 0x0015D5C2
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x170006CD RID: 1741
			// (get) Token: 0x060046F5 RID: 18165 RVA: 0x0015F3CB File Offset: 0x0015D5CB
			// (set) Token: 0x060046F6 RID: 18166 RVA: 0x0015F3D3 File Offset: 0x0015D5D3
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x170006CE RID: 1742
			// (get) Token: 0x060046F7 RID: 18167 RVA: 0x0015F3DC File Offset: 0x0015D5DC
			// (set) Token: 0x060046F8 RID: 18168 RVA: 0x0015F3E4 File Offset: 0x0015D5E4
			public int number_of_charge_period { get; set; }

			// Token: 0x170006CF RID: 1743
			// (get) Token: 0x060046F9 RID: 18169 RVA: 0x0015F3ED File Offset: 0x0015D5ED
			// (set) Token: 0x060046FA RID: 18170 RVA: 0x0015F3F5 File Offset: 0x0015D5F5
			public string plan_id { get; set; }

			// Token: 0x170006D0 RID: 1744
			// (get) Token: 0x060046FB RID: 18171 RVA: 0x0015F3FE File Offset: 0x0015D5FE
			// (set) Token: 0x060046FC RID: 18172 RVA: 0x0015F406 File Offset: 0x0015D606
			public string plan_name { get; set; }

			// Token: 0x170006D1 RID: 1745
			// (get) Token: 0x060046FD RID: 18173 RVA: 0x0015F40F File Offset: 0x0015D60F
			// (set) Token: 0x060046FE RID: 18174 RVA: 0x0015F417 File Offset: 0x0015D617
			public string status { get; set; }

			// Token: 0x170006D2 RID: 1746
			// (get) Token: 0x060046FF RID: 18175 RVA: 0x0015F420 File Offset: 0x0015D620
			// (set) Token: 0x06004700 RID: 18176 RVA: 0x0015F428 File Offset: 0x0015D628
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x02000B8B RID: 2955
		public class QuerySubscritionResponse
		{
			// Token: 0x170006D3 RID: 1747
			// (get) Token: 0x06004702 RID: 18178 RVA: 0x0015F431 File Offset: 0x0015D631
			// (set) Token: 0x06004703 RID: 18179 RVA: 0x0015F439 File Offset: 0x0015D639
			public int statusCode { get; set; }

			// Token: 0x170006D4 RID: 1748
			// (get) Token: 0x06004704 RID: 18180 RVA: 0x0015F442 File Offset: 0x0015D642
			// (set) Token: 0x06004705 RID: 18181 RVA: 0x0015F44A File Offset: 0x0015D64A
			public string message { get; set; }

			// Token: 0x170006D5 RID: 1749
			// (get) Token: 0x06004706 RID: 18182 RVA: 0x0015F453 File Offset: 0x0015D653
			// (set) Token: 0x06004707 RID: 18183 RVA: 0x0015F45B File Offset: 0x0015D65B
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
