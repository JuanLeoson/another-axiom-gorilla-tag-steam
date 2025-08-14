using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000BA3 RID: 2979
	internal class IAPurchase
	{
		// Token: 0x06004784 RID: 18308
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06004785 RID: 18309
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady_64(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06004786 RID: 18310
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x06004787 RID: 18311
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x06004788 RID: 18312
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x06004789 RID: 18313
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x0600478A RID: 18314
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600478B RID: 18315
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600478C RID: 18316
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600478D RID: 18317
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600478E RID: 18318
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query(IAPurchaseCallback callback);

		// Token: 0x0600478F RID: 18319
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query_64(IAPurchaseCallback callback);

		// Token: 0x06004790 RID: 18320
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance(IAPurchaseCallback callback);

		// Token: 0x06004791 RID: 18321
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance_64(IAPurchaseCallback callback);

		// Token: 0x06004792 RID: 18322
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x06004793 RID: 18323
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription_64(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x06004794 RID: 18324
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x06004795 RID: 18325
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID_64(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x06004796 RID: 18326
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004797 RID: 18327
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004798 RID: 18328
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x06004799 RID: 18329
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x0600479A RID: 18330
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList(IAPurchaseCallback callback);

		// Token: 0x0600479B RID: 18331
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList_64(IAPurchaseCallback callback);

		// Token: 0x0600479C RID: 18332
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x0600479D RID: 18333
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);
	}
}
