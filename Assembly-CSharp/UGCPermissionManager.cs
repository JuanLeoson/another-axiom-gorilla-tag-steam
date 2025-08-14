using System;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x0200090C RID: 2316
internal class UGCPermissionManager : MonoBehaviour
{
	// Token: 0x06003934 RID: 14644 RVA: 0x00128F0E File Offset: 0x0012710E
	public static void UsePlayFabSafety()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.PlayFabPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x06003935 RID: 14645 RVA: 0x00128F30 File Offset: 0x00127130
	public static void UseKID()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.KIDPermissions(new Action<bool>(UGCPermissionManager.SetUGCEnabled));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x1700058F RID: 1423
	// (get) Token: 0x06003936 RID: 14646 RVA: 0x00128F52 File Offset: 0x00127152
	public static bool IsUGCDisabled
	{
		get
		{
			return !UGCPermissionManager.isUGCEnabled.GetValueOrDefault();
		}
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x00128F61 File Offset: 0x00127161
	public static void CheckPermissions()
	{
		UGCPermissionManager.IUGCPermissions iugcpermissions = UGCPermissionManager.permissions;
		if (iugcpermissions == null)
		{
			return;
		}
		iugcpermissions.CheckPermissions();
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x00128F72 File Offset: 0x00127172
	public static void SubscribeToUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x00128F89 File Offset: 0x00127189
	public static void UnsubscribeFromUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x00128FA0 File Offset: 0x001271A0
	public static void SubscribeToUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x00128FB7 File Offset: 0x001271B7
	public static void UnsubscribeFromUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x00128FD0 File Offset: 0x001271D0
	private static void SetUGCEnabled(bool enabled)
	{
		bool? flag = UGCPermissionManager.isUGCEnabled;
		if (!(enabled == flag.GetValueOrDefault() & flag != null))
		{
			Debug.LogFormat("[UGCPermissionManager][KID] UGC state changed: [{0}]", new object[]
			{
				enabled ? "ENABLED" : "DISABLED"
			});
			UGCPermissionManager.isUGCEnabled = new bool?(enabled);
			if (enabled)
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCEnabled");
				Action action = UGCPermissionManager.onUGCEnabled;
				if (action == null)
				{
					return;
				}
				action();
				return;
			}
			else
			{
				Debug.Log("[UGCPermissionManager][KID] Invoking onUGCDisabled");
				Action action2 = UGCPermissionManager.onUGCDisabled;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}
	}

	// Token: 0x04004662 RID: 18018
	[OnEnterPlay_SetNull]
	private static UGCPermissionManager.IUGCPermissions permissions;

	// Token: 0x04004663 RID: 18019
	[OnEnterPlay_SetNull]
	private static Action onUGCEnabled;

	// Token: 0x04004664 RID: 18020
	[OnEnterPlay_SetNull]
	private static Action onUGCDisabled;

	// Token: 0x04004665 RID: 18021
	private static bool? isUGCEnabled;

	// Token: 0x0200090D RID: 2317
	private interface IUGCPermissions
	{
		// Token: 0x0600393E RID: 14654
		void Initialize();

		// Token: 0x0600393F RID: 14655
		void CheckPermissions();
	}

	// Token: 0x0200090E RID: 2318
	private class PlayFabPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06003940 RID: 14656 RVA: 0x0012905A File Offset: 0x0012725A
		public PlayFabPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x0012906C File Offset: 0x0012726C
		public void Initialize()
		{
			bool safety = PlayFabAuthenticator.instance.GetSafety();
			Debug.LogFormat("[UGCPermissionManager][KID] UGC initialized from Playfab: [{0}]", new object[]
			{
				safety ? "DISABLED" : "ENABLED"
			});
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(!safety);
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x000023F5 File Offset: 0x000005F5
		public void CheckPermissions()
		{
		}

		// Token: 0x04004666 RID: 18022
		private Action<bool> setUGCEnabled;
	}

	// Token: 0x0200090F RID: 2319
	private class KIDPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06003943 RID: 14659 RVA: 0x001290BC File Offset: 0x001272BC
		public KIDPermissions(Action<bool> setUGCEnabled)
		{
			this.setUGCEnabled = setUGCEnabled;
		}

		// Token: 0x06003944 RID: 14660 RVA: 0x001290CB File Offset: 0x001272CB
		private void SetUGCEnabled(bool enabled)
		{
			Action<bool> action = this.setUGCEnabled;
			if (action == null)
			{
				return;
			}
			action(enabled);
		}

		// Token: 0x06003945 RID: 14661 RVA: 0x001290DE File Offset: 0x001272DE
		public void Initialize()
		{
			Debug.Log("[UGCPermissionManager][KID] Initializing with KID");
			this.CheckPermissions();
			KIDManager.RegisterSessionUpdatedCallback_UGC(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdate));
		}

		// Token: 0x06003946 RID: 14662 RVA: 0x00129104 File Offset: 0x00127304
		public void CheckPermissions()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Mods);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, permissionDataByFeature.Enabled, permissionDataByFeature.ManagedBy);
		}

		// Token: 0x06003947 RID: 14663 RVA: 0x00129138 File Offset: 0x00127338
		private void OnKIDSessionUpdate(bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.Log("[UGCPermissionManager][KID] KID session update.");
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, isEnabled, managedBy);
		}

		// Token: 0x06003948 RID: 14664 RVA: 0x00129168 File Offset: 0x00127368
		private void ProcessPermissionKID(bool hasOptedIn, bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.LogFormat("[UGCPermissionManager][KID] Process KID permissions - opted in: [{0}], enabled: [{1}], managedBy: [{2}].", new object[]
			{
				hasOptedIn,
				isEnabled,
				managedBy
			});
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC prohibited.");
				this.SetUGCEnabled(false);
				return;
			}
			if (managedBy != Permission.ManagedByEnum.PLAYER)
			{
				if (managedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by guardian. (opted in: [{0}], enabled: [{1}])", new object[]
					{
						hasOptedIn,
						isEnabled
					});
					this.SetUGCEnabled(isEnabled);
				}
				return;
			}
			if (isEnabled)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC managed by player and enabled - opting in and enabling UGC.");
				if (!hasOptedIn)
				{
					KIDManager.SetFeatureOptIn(EKIDFeatures.Mods, true);
				}
				this.SetUGCEnabled(true);
				return;
			}
			Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by player and disabled by default - using opt in status. (opted in: [{0}])", new object[]
			{
				hasOptedIn
			});
			this.SetUGCEnabled(hasOptedIn);
		}

		// Token: 0x04004667 RID: 18023
		private Action<bool> setUGCEnabled;
	}
}
