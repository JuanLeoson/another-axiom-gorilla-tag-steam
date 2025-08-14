using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts.ModIO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000846 RID: 2118
public class CustomMapsTerminal : MonoBehaviour
{
	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x0600351B RID: 13595 RVA: 0x001161B6 File Offset: 0x001143B6
	public static int LocalPlayerID
	{
		get
		{
			return NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x0600351C RID: 13596 RVA: 0x001161C7 File Offset: 0x001143C7
	public static bool IsDriver
	{
		get
		{
			return CustomMapsTerminal.localDriverID == CustomMapsTerminal.LocalPlayerID;
		}
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x001161D5 File Offset: 0x001143D5
	private void Awake()
	{
		CustomMapsTerminal.instance = this;
		CustomMapsTerminal.hasInstance = true;
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x001161E4 File Offset: 0x001143E4
	private void Start()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.modsPerPage = CustomMapsTerminal.instance.modListScreen.ModsPerPage;
		CustomMapsTerminal.cachedNetStatus.modsPerPage = CustomMapsTerminal.localStatus.modsPerPage;
		CustomMapsTerminal.HideTerminalControlScreen();
		this.accessScreen.Show();
		this.modListScreen.Hide();
		this.modDetailsScreen.Hide();
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x001162D0 File Offset: 0x001144D0
	private void LateUpdate()
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (CustomMapsTerminal.useNametags == GorillaComputer.instance.NametagsEnabled)
		{
			return;
		}
		CustomMapsTerminal.useNametags = GorillaComputer.instance.NametagsEnabled;
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x00116324 File Offset: 0x00114524
	private void OnDestroy()
	{
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x001163A0 File Offset: 0x001145A0
	public static CustomMapsTerminal.TerminalStatus UpdateAndRetrieveLocalStatus()
	{
		CustomMapsTerminal.localStatus.sortType = CustomMapsTerminal.instance.modListScreen.SortType;
		if (CustomMapsTerminal.instance.modDetailsScreen.isActiveAndEnabled)
		{
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapsTerminal.instance.modDetailsScreen.GetModId();
		}
		if (CustomMapsTerminal.instance.modListScreen.isActiveAndEnabled)
		{
			CustomMapsTerminal.localStatus.modIndex = CustomMapsTerminal.instance.modListScreen.SelectedModIndex;
			CustomMapsTerminal.localStatus.pageIndex = CustomMapsTerminal.instance.modListScreen.CurrentModPage;
			if (CustomMapsTerminal.instance.modListScreen.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods)
			{
				CustomMapsTerminal.instance.modListScreen.GetModList(out CustomMapsTerminal.localStatus.modList);
				CustomMapsTerminal.localStatus.numModPages = CustomMapsTerminal.instance.modListScreen.GetNumPages();
			}
			else
			{
				CustomMapsTerminal.localStatus.modList = Array.Empty<long>();
				CustomMapsTerminal.localStatus.numModPages = -1;
			}
		}
		return CustomMapsTerminal.localStatus;
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x0011649B File Offset: 0x0011469B
	public static void UpdateListScreenState(CustomMapsListScreen.ListScreenState screenState)
	{
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		CustomMapsTerminal.localStatus.currentScreen = ((screenState == CustomMapsListScreen.ListScreenState.AvailableMods) ? CustomMapsTerminal.ScreenType.AvailableMods : CustomMapsTerminal.ScreenType.SubscribedMods);
		CustomMapsTerminal.UpdateSubscriptionButtonStatus(screenState);
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x001164C8 File Offset: 0x001146C8
	public static void ShowDetailsScreen(ModProfile profile)
	{
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.accessScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.SetModProfile(profile);
		CustomMapsTerminal.SendTerminalStatus(false, false);
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x00116538 File Offset: 0x00114738
	public static void ReturnFromDetailsScreen()
	{
		CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.localStatus.previousScreen;
		if (previousScreen == CustomMapsTerminal.ScreenType.ModDetails || previousScreen == CustomMapsTerminal.ScreenType.Invalid || previousScreen == CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.localStatus.pageIndex = 0;
			CustomMapsTerminal.localStatus.modIndex = 0;
		}
		else
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.localStatus.previousScreen;
		}
		switch (CustomMapsTerminal.localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.Access:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
			CustomMapsTerminal.instance.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Hide();
			break;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Hide();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus(CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.SubscribedMods, false);
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x00116690 File Offset: 0x00114890
	public static bool SetTagButtonStatus(short tagIndex, out string tagText)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			tagText = string.Empty;
			return false;
		}
		int num = (int)MathF.Max(0f, (float)(tagIndex - 1));
		short num2 = (short)MathF.Pow(2f, (float)num);
		int num3;
		bool flag;
		if ((CustomMapsTerminal.localStatus.tagFlags & num2) == 0)
		{
			num3 = (int)(CustomMapsTerminal.localStatus.tagFlags | num2);
			flag = true;
		}
		else
		{
			num3 = (int)(CustomMapsTerminal.localStatus.tagFlags & ~(int)num2);
			flag = false;
		}
		CustomMapsTerminal.localStatus.tagFlags = (short)num3;
		CustomMapsTerminal.instance.tagButtons[num].SetButtonStatus(flag);
		tagText = CustomMapsTerminal.instance.tagButtons[num].characterString;
		return flag;
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x00116736 File Offset: 0x00114936
	public static void SendTerminalStatus(bool sendFullModList = false, bool forceSearch = false)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.mapTerminalNetworkObject.SendTerminalStatus(sendFullModList, forceSearch);
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00116751 File Offset: 0x00114951
	public static void ResetTerminalControl()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
		CustomMapsTerminal.ShowTerminalControlScreen();
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x0011676E File Offset: 0x0011496E
	public static void HandleTerminalControlStatusChangeRequest(bool lockedStatus, int playerID)
	{
		if (lockedStatus && playerID == -2)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			if (!lockedStatus)
			{
				return;
			}
		}
		else if (CustomMapsTerminal.localDriverID != playerID)
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(lockedStatus, playerID, true);
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x00116798 File Offset: 0x00114998
	public static void SetTerminalControlStatus(bool isLocked, int driverID = -2, bool sendRPC = false)
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		if (isLocked)
		{
			CustomMapsTerminal.localDriverID = driverID;
			CustomMapsTerminal.instance.terminalControlButton.LockTerminalControl();
			CustomMapsTerminal.HideTerminalControlScreen();
		}
		else
		{
			CustomMapsTerminal.localDriverID = -2;
			CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
			CustomMapsTerminal.ShowTerminalControlScreen();
		}
		if (sendRPC && NetworkSystem.Instance.IsMasterClient)
		{
			CustomMapsTerminal.instance.mapTerminalNetworkObject.SetTerminalControlStatus(isLocked, CustomMapsTerminal.localDriverID);
		}
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x0011680C File Offset: 0x00114A0C
	public static void UpdateStatusFromDriver(long[] data, int driverID, bool forceSearch = false)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.localDriverID = driverID;
		CustomMapsTerminal.cachedNetStatus.UnpackData(data);
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		CustomMapsTerminal.localStatus.UnpackData(data);
		if (CustomMapsTerminal.localDriverID != -2)
		{
			CustomMapsTerminal.RefreshDriverNickName();
		}
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(forceSearch);
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x0011685E File Offset: 0x00114A5E
	public static void ClearTags()
	{
		CustomMapsTerminal.localTagStatus = 0;
		CustomMapsTerminal.instance.modListScreen.ClearTags(true);
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x00116878 File Offset: 0x00114A78
	private void UpdateTagsAndSortFromDriver()
	{
		CustomMapsTerminal.instance.modListScreen.SortType = CustomMapsTerminal.localStatus.sortType;
		if (CustomMapsTerminal.localTagStatus == CustomMapsTerminal.localStatus.tagFlags)
		{
			return;
		}
		CustomMapsTerminal.localTagStatus = CustomMapsTerminal.localStatus.tagFlags;
		List<string> list = new List<string>();
		for (int i = 0; i < 8; i++)
		{
			int num = (int)Mathf.Pow(2f, (float)i);
			if (((int)CustomMapsTerminal.localStatus.tagFlags & num) == 0)
			{
				CustomMapsTerminal.instance.tagButtons[i].SetButtonStatus(false);
			}
			else
			{
				CustomMapsTerminal.instance.tagButtons[i].SetButtonStatus(true);
				list.Add(CustomMapsTerminal.instance.tagButtons[i].characterString);
			}
		}
		CustomMapsTerminal.instance.modListScreen.UpdateTagsFromDriver(list);
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x00116948 File Offset: 0x00114B48
	private void UpdateScreenToMatchStatus(bool forceSearch = false)
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			this.terminalControlButton.UnlockTerminalControl();
		}
		else
		{
			this.terminalControlButton.LockTerminalControl();
		}
		this.ValidateLocalStatus();
		this.UpdateTagsAndSortFromDriver();
		CustomMapsTerminal.UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState.AvailableMods);
		switch (CustomMapsTerminal.localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.Access:
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.accessScreen.Show();
			return;
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.accessScreen.Show();
			this.accessScreen.ShowTerminalControlPrompt();
			return;
		case CustomMapsTerminal.ScreenType.AvailableMods:
			this.accessScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Show();
			if (forceSearch)
			{
				this.modListScreen.Refresh(null);
			}
			return;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			this.accessScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Show();
			CustomMapsTerminal.UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState.SubscribedMods);
			if (forceSearch)
			{
				this.modListScreen.Refresh(CustomMapsTerminal.localStatus.modList);
				return;
			}
			if (!CustomMapsTerminal.IsDriver && !this.modListScreen.DoesModListMatchDisplay(CustomMapsTerminal.localStatus.modList))
			{
				this.modListScreen.ShowCustomModList(CustomMapsTerminal.localStatus.modList);
			}
			return;
		case CustomMapsTerminal.ScreenType.ModDetails:
			this.accessScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Hide();
			this.modDetailsScreen.Show();
			this.modDetailsScreen.RetrieveProfileFromModIO(CustomMapsTerminal.localStatus.modDetailsID, null);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x00116B0C File Offset: 0x00114D0C
	private void ValidateLocalStatus()
	{
		if (!ModIOManager.IsLoggedIn() || CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (CustomMapLoader.IsModLoaded(0L))
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapLoader.LoadedMapModId;
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (CustomMapManager.IsLoading(0L))
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapManager.LoadingMapId;
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapManager.GetRoomMapId().id;
			CustomMapsTerminal.SendTerminalStatus(false, false);
		}
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x00116BB8 File Offset: 0x00114DB8
	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.Access)
		{
			if (!NetworkSystem.Instance.InRoom || CustomMapsTerminal.cachedNetStatus.currentScreen == CustomMapsTerminal.ScreenType.Invalid)
			{
				CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
				CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
				CustomMapsTerminal.localStatus.modIndex = 0;
			}
			else
			{
				CustomMapsTerminal.localStatus.Copy(CustomMapsTerminal.cachedNetStatus);
				this.UpdateScreenToMatchStatus(false);
			}
			if (CustomMapsTerminal.localDriverID == -2)
			{
				CustomMapsTerminal.ShowTerminalControlScreen();
			}
		}
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x00116C31 File Offset: 0x00114E31
	private void OnModIOLoggedOut()
	{
		CustomMapsTerminal.ResetTerminalControl();
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.Access;
		this.accessScreen.Reset();
		this.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x00116C60 File Offset: 0x00114E60
	public void HandleTerminalControlButtonPressed()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			this.accessScreen.DisplayError("User is logged out of mod.io, not allowing terminal check-out");
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			CustomMapsTerminal.SetTerminalControlStatus(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID, false);
			return;
		}
		if (CustomMapsTerminal.localDriverID != -2 && !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.mapTerminalNetworkObject.HasAuthority)
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID);
			return;
		}
		this.mapTerminalNetworkObject.RequestTerminalControlStatusChange(!this.terminalControlButton.IsLocked);
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x00116CFB File Offset: 0x00114EFB
	private static void UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState screenState)
	{
		if (CustomMapsTerminal.hasInstance && CustomMapsTerminal.instance.subscribedOnlyButton != null)
		{
			CustomMapsTerminal.instance.subscribedOnlyButton.SetButtonStatus(screenState == CustomMapsListScreen.ListScreenState.SubscribedMods);
		}
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x00116D2C File Offset: 0x00114F2C
	private static void ShowTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(false);
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(false);
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		if (CustomMapsTerminal.localStatus.currentScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		}
		else
		{
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		}
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x00116DB8 File Offset: 0x00114FB8
	private static void HideTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance || !ModIOManager.IsLoggedIn())
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		if (CustomMapsTerminal.localStatus.currentScreen != CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			return;
		}
		if (CustomMapsTerminal.localStatus.previousScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.localStatus.previousScreen;
			if (CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
			{
				CustomMapsTerminal.localStatus.modIndex = 0;
			}
		}
		else if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		}
		else
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? netPlayerByID.NickName : netPlayerByID.DefaultName);
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00116F15 File Offset: 0x00115115
	public static void RequestDriverNickNameRefresh()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		CustomMapsTerminal.instance.mapTerminalNetworkObject.RefreshDriverNickName();
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x00116F3C File Offset: 0x0011513C
	public static void RefreshDriverNickName()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.DefaultName;
			if (CustomMapsTerminal.useNametags && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					CustomMapsTerminal.instance.terminalControllerText.text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.modListScreen.RefreshDriverNickname(CustomMapsTerminal.instance.terminalControllerText.text);
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x0011705F File Offset: 0x0011525F
	private void OnReturnedToSinglePlayer()
	{
		if (CustomMapsTerminal.localDriverID != CustomMapsTerminal.cachedLocalPlayerID)
		{
			CustomMapsTerminal.ResetTerminalControl();
		}
		else
		{
			CustomMapsTerminal.localDriverID = CustomMapsTerminal.LocalPlayerID;
		}
		CustomMapsTerminal.cachedLocalPlayerID = -1;
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x00117084 File Offset: 0x00115284
	private void OnJoinedRoom()
	{
		CustomMapsTerminal.cachedLocalPlayerID = CustomMapsTerminal.LocalPlayerID;
		CustomMapsTerminal.ResetTerminalControl();
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x00117095 File Offset: 0x00115295
	public static bool IsLocked()
	{
		return CustomMapsTerminal.localDriverID != -2;
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x001170A3 File Offset: 0x001152A3
	public static int GetDriverID()
	{
		return CustomMapsTerminal.localDriverID;
	}

	// Token: 0x0600353B RID: 13627 RVA: 0x001170AA File Offset: 0x001152AA
	public static string GetDriverNickname()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return "";
		}
		return CustomMapsTerminal.instance.terminalControllerText.text;
	}

	// Token: 0x0400421B RID: 16923
	[SerializeField]
	private CustomMapsAccessScreen accessScreen;

	// Token: 0x0400421C RID: 16924
	[SerializeField]
	private CustomMapsListScreen modListScreen;

	// Token: 0x0400421D RID: 16925
	[SerializeField]
	private CustomMapsDetailsScreen modDetailsScreen;

	// Token: 0x0400421E RID: 16926
	[SerializeField]
	private VirtualStumpSerializer mapTerminalNetworkObject;

	// Token: 0x0400421F RID: 16927
	[SerializeField]
	private CustomMapsTerminalControlButton terminalControlButton;

	// Token: 0x04004220 RID: 16928
	[SerializeField]
	private TMP_Text terminalControllerLabelText;

	// Token: 0x04004221 RID: 16929
	[SerializeField]
	private TMP_Text terminalControllerText;

	// Token: 0x04004222 RID: 16930
	[SerializeField]
	private List<CustomMapsKeyToggleButton> tagButtons = new List<CustomMapsKeyToggleButton>();

	// Token: 0x04004223 RID: 16931
	[SerializeField]
	private CustomMapsKeyToggleButton subscribedOnlyButton;

	// Token: 0x04004224 RID: 16932
	public const int NO_DRIVER_ID = -2;

	// Token: 0x04004225 RID: 16933
	private const short NUM_OF_TAGS = 8;

	// Token: 0x04004226 RID: 16934
	private static CustomMapsTerminal instance;

	// Token: 0x04004227 RID: 16935
	private static bool hasInstance;

	// Token: 0x04004228 RID: 16936
	private static CustomMapsTerminal.TerminalStatus localStatus = new CustomMapsTerminal.TerminalStatus();

	// Token: 0x04004229 RID: 16937
	private static CustomMapsTerminal.TerminalStatus cachedNetStatus = new CustomMapsTerminal.TerminalStatus();

	// Token: 0x0400422A RID: 16938
	private static int localDriverID = -1;

	// Token: 0x0400422B RID: 16939
	private static int cachedLocalPlayerID = -1;

	// Token: 0x0400422C RID: 16940
	private static bool useNametags;

	// Token: 0x0400422D RID: 16941
	private static short localTagStatus = 0;

	// Token: 0x02000847 RID: 2119
	public enum ScreenType
	{
		// Token: 0x0400422F RID: 16943
		Invalid = -1,
		// Token: 0x04004230 RID: 16944
		Access,
		// Token: 0x04004231 RID: 16945
		TerminalControlPrompt,
		// Token: 0x04004232 RID: 16946
		AvailableMods,
		// Token: 0x04004233 RID: 16947
		SubscribedMods,
		// Token: 0x04004234 RID: 16948
		ModDetails
	}

	// Token: 0x02000848 RID: 2120
	public class TerminalStatus
	{
		// Token: 0x0600353E RID: 13630 RVA: 0x00117104 File Offset: 0x00115304
		public long[] PackData(bool packModList)
		{
			long[] array;
			if (packModList && !this.modList.IsNullOrEmpty<long>())
			{
				array = new long[3 + this.modList.Length];
				for (int i = 3; i < array.Length; i++)
				{
					array[i] = this.modList[i - 3];
				}
			}
			else
			{
				array = new long[3];
			}
			array[0] = (long)this.currentScreen;
			array[0] += (long)((long)this.previousScreen << 4);
			array[0] += (long)((long)(this.modIndex + 1) << 8);
			array[0] += (long)((long)(this.numModPages + 1) << 16);
			array[0] += (long)(this.pageIndex + 1) << 32;
			array[1] = this.modDetailsID;
			array[2] = (long)this.tagFlags;
			array[2] += (long)this.sortType << 32;
			return array;
		}

		// Token: 0x0600353F RID: 13631 RVA: 0x001171E0 File Offset: 0x001153E0
		public void UnpackData(long[] data)
		{
			if (data.Length < 3 || data.Length > 3 + this.modsPerPage)
			{
				return;
			}
			int num = (int)(data[0] & 15L);
			this.currentScreen = (CustomMapsTerminal.ScreenType)((num >= -1 && num <= 4) ? num : -1);
			num = (int)(data[0] >> 4 & 15L);
			this.previousScreen = (CustomMapsTerminal.ScreenType)((num >= -1 && num <= 4) ? num : -1);
			this.modIndex = (int)(data[0] >> 8 & 255L);
			this.modIndex = Mathf.Clamp(this.modIndex - 1, -1, this.modsPerPage);
			this.numModPages = (int)(data[0] >> 16 & 65535L);
			this.numModPages = Mathf.Clamp(this.numModPages - 1, -1, 65535);
			this.pageIndex = (int)(data[0] >> 32);
			this.pageIndex = Mathf.Max(this.pageIndex - 1, -1);
			this.modDetailsID = ((data[1] > 0L) ? data[1] : 0L);
			this.tagFlags = (short)Mathf.Clamp((float)(data[2] & 255L), 0f, 255f);
			num = (int)(data[2] >> 32);
			this.sortType = (SortModsBy)((num >= 0 && num <= 6) ? num : 3);
			if (data.Length <= 3)
			{
				return;
			}
			this.modList = new long[data.Length - 3];
			for (int i = 0; i < this.modList.Length; i++)
			{
				this.modList[i] = data[i + 3];
			}
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x0011733C File Offset: 0x0011553C
		public void Copy(CustomMapsTerminal.TerminalStatus other)
		{
			this.currentScreen = other.currentScreen;
			this.previousScreen = other.previousScreen;
			this.modIndex = other.modIndex;
			this.numModPages = other.numModPages;
			this.pageIndex = other.pageIndex;
			this.modDetailsID = other.modDetailsID;
			this.modList = other.modList;
			this.tagFlags = other.tagFlags;
			this.sortType = other.sortType;
		}

		// Token: 0x04004235 RID: 16949
		public CustomMapsTerminal.ScreenType currentScreen = CustomMapsTerminal.ScreenType.Invalid;

		// Token: 0x04004236 RID: 16950
		public CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.ScreenType.Invalid;

		// Token: 0x04004237 RID: 16951
		public int modIndex;

		// Token: 0x04004238 RID: 16952
		public int pageIndex;

		// Token: 0x04004239 RID: 16953
		public long modDetailsID;

		// Token: 0x0400423A RID: 16954
		public long[] modList;

		// Token: 0x0400423B RID: 16955
		public int numModPages = -1;

		// Token: 0x0400423C RID: 16956
		public short tagFlags = 128;

		// Token: 0x0400423D RID: 16957
		public SortModsBy sortType = SortModsBy.Popular;

		// Token: 0x0400423E RID: 16958
		private const int MINIMUM_ARRAY_LENGTH = 3;

		// Token: 0x0400423F RID: 16959
		public int modsPerPage;
	}
}
