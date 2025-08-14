using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CC8 RID: 3272
	public class SharedBlocksTerminal : MonoBehaviour
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06005134 RID: 20788 RVA: 0x00194C9B File Offset: 0x00192E9B
		public SharedBlocksManager.SharedBlocksMap SelectedMap
		{
			get
			{
				return this.selectedMap;
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06005135 RID: 20789 RVA: 0x00194CA3 File Offset: 0x00192EA3
		public bool IsTerminalLocked
		{
			get
			{
				return this.isTerminalLocked;
			}
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06005136 RID: 20790 RVA: 0x00194CAB File Offset: 0x00192EAB
		private int playersInLobby
		{
			get
			{
				return this.lobbyTrigger.playerIDsCurrentlyTouching.Count;
			}
		}

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06005137 RID: 20791 RVA: 0x00194CBD File Offset: 0x00192EBD
		public bool IsDriver
		{
			get
			{
				return this.localState.driverID == NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x06005138 RID: 20792 RVA: 0x00194CDB File Offset: 0x00192EDB
		public BuilderTable GetTable()
		{
			return this.linkedTable;
		}

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06005139 RID: 20793 RVA: 0x00194CE3 File Offset: 0x00192EE3
		public int GetDriverID
		{
			get
			{
				return this.localState.driverID;
			}
		}

		// Token: 0x0600513A RID: 20794 RVA: 0x00194CF0 File Offset: 0x00192EF0
		public static string MapIDToDisplayedString(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return "____-____";
			}
			int num = 4;
			SharedBlocksTerminal.sb.Clear();
			if (mapID.Length > num)
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0, num));
				SharedBlocksTerminal.sb.Append("-");
				SharedBlocksTerminal.sb.Append(mapID.Substring(num));
				int repeatCount = 9 - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', repeatCount);
			}
			else
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0));
				int repeatCount2 = num - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', repeatCount2);
				SharedBlocksTerminal.sb.Append("-____");
			}
			return SharedBlocksTerminal.sb.ToString();
		}

		// Token: 0x0600513B RID: 20795 RVA: 0x00194DBC File Offset: 0x00192FBC
		public void Init(BuilderTable table)
		{
			if (this.hasInitialized)
			{
				return;
			}
			this.localState = new SharedBlocksTerminal.SharedBlocksTerminalState
			{
				state = SharedBlocksTerminal.TerminalState.NoStatus,
				driverID = -2
			};
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.AddListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			this.terminalControlButton.onPressButton.AddListener(new UnityAction(this.OnTerminalControlPressed));
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.linkedTable = table;
			table.linkedTerminal = this;
			this.linkedTable.OnMapLoaded.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
			this.linkedTable.OnMapLoadFailed.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
			this.linkedTable.OnMapCleared.AddListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
			this.hasInitialized = true;
		}

		// Token: 0x0600513C RID: 20796 RVA: 0x00194ED8 File Offset: 0x001930D8
		private void Start()
		{
			BuilderTable table;
			if (!this.hasInitialized && BuilderTable.TryGetBuilderTableForZone(this.tableZone, out table))
			{
				this.Init(table);
				return;
			}
			Debug.LogWarning("Could not find builder table for zone " + this.tableZone.ToString());
		}

		// Token: 0x0600513D RID: 20797 RVA: 0x00194F24 File Offset: 0x00193124
		private void LateUpdate()
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
			{
				return;
			}
			this.useNametags = GorillaComputer.instance.NametagsEnabled;
			this.RefreshDriverNickname();
		}

		// Token: 0x0600513E RID: 20798 RVA: 0x00194F80 File Offset: 0x00193180
		private void OnDestroy()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.RemoveListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			if (this.terminalControlButton != null)
			{
				this.terminalControlButton.onPressButton.RemoveListener(new UnityAction(this.OnTerminalControlPressed));
			}
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
			}
			if (this.linkedTable != null)
			{
				this.linkedTable.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
				this.linkedTable.OnMapLoadFailed.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
				this.linkedTable.OnMapCleared.RemoveListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			}
		}

		// Token: 0x0600513F RID: 20799 RVA: 0x00195080 File Offset: 0x00193280
		private void RefreshActiveScreen()
		{
			if (this.localState.driverID == -2)
			{
				if (this.currentScreen != this.noDriverScreen)
				{
					if (this.currentScreen != null)
					{
						this.currentScreen.Hide();
					}
					this.currentScreen = this.noDriverScreen;
					this.currentScreen.Show();
				}
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			if (this.currentScreen != this.searchScreen)
			{
				if (this.currentScreen != null)
				{
					this.currentScreen.Hide();
				}
				this.currentScreen = this.searchScreen;
				this.currentScreen.Show();
			}
		}

		// Token: 0x06005140 RID: 20800 RVA: 0x00195134 File Offset: 0x00193334
		private void SetTerminalState(SharedBlocksTerminal.TerminalState state)
		{
			this.localState.state = state;
			if (this.localState.driverID == -2)
			{
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			switch (state)
			{
			case SharedBlocksTerminal.TerminalState.NoStatus:
				this.statusMessageText.gameObject.SetActive(false);
				return;
			case SharedBlocksTerminal.TerminalState.Searching:
				this.SetStatusText("SEARCHING...");
				return;
			case SharedBlocksTerminal.TerminalState.NotFound:
				this.SetStatusText("MAP NOT FOUND");
				return;
			case SharedBlocksTerminal.TerminalState.Found:
				this.SetStatusText("MAP FOUND. PRESS 'ENTER' TO LOAD");
				return;
			case SharedBlocksTerminal.TerminalState.Loading:
				this.SetStatusText("LOADING...");
				return;
			case SharedBlocksTerminal.TerminalState.LoadSuccess:
				this.SetStatusText("LOAD SUCCESS");
				return;
			case SharedBlocksTerminal.TerminalState.LoadFail:
				this.SetStatusText("LOAD FAILED");
				return;
			default:
				return;
			}
		}

		// Token: 0x06005141 RID: 20801 RVA: 0x001951EA File Offset: 0x001933EA
		public void SelectMapIDAndOpenInfo(string mapID)
		{
			if (this.awaitingWebRequest)
			{
				return;
			}
			this.selectedMap = null;
			this.awaitingWebRequest = true;
			this.requestedMapID = mapID;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.OnPlayerMapRequestComplete));
		}

		// Token: 0x06005142 RID: 20802 RVA: 0x00195228 File Offset: 0x00193428
		private void OnPlayerMapRequestComplete(SharedBlocksManager.SharedBlocksMap response)
		{
			if (this.awaitingWebRequest)
			{
				this.awaitingWebRequest = false;
				this.requestedMapID = null;
				if (this.IsDriver)
				{
					if (response == null || response.MapID == null)
					{
						this.SetTerminalState(SharedBlocksTerminal.TerminalState.NotFound);
						return;
					}
					this.selectedMap = response;
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Found);
				}
			}
		}

		// Token: 0x06005143 RID: 20803 RVA: 0x00195274 File Offset: 0x00193474
		private bool CanChangeMapState(bool load, out string disallowedReason)
		{
			disallowedReason = "";
			if (!NetworkSystem.Instance.InRoom)
			{
				disallowedReason = "MUST BE IN A ROOM BEFORE  " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			this.RefreshLobbyCount();
			if (!this.AreAllPlayersInLobby())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE LOBBY BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}

		// Token: 0x06005144 RID: 20804 RVA: 0x001952E7 File Offset: 0x001934E7
		public void SetStatusText(string text)
		{
			this.statusMessageText.text = text;
			this.statusMessageText.gameObject.SetActive(true);
		}

		// Token: 0x06005145 RID: 20805 RVA: 0x00195306 File Offset: 0x00193506
		private bool IsLocalPlayerInLobby()
		{
			return base.isActiveAndEnabled && this.lobbyTrigger.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06005146 RID: 20806 RVA: 0x00195336 File Offset: 0x00193536
		public bool AreAllPlayersInLobby()
		{
			return base.isActiveAndEnabled && this.playersInLobby == this.playersInRoom;
		}

		// Token: 0x06005147 RID: 20807 RVA: 0x00195350 File Offset: 0x00193550
		public string GetLobbyText()
		{
			return string.Format("PLAYERS IN ROOM {0}\nPLAYERS IN LOBBY {1}", this.playersInRoom, this.playersInLobby);
		}

		// Token: 0x06005148 RID: 20808 RVA: 0x00195372 File Offset: 0x00193572
		public void RefreshLobbyCount()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.playersInRoom = NetworkSystem.Instance.RoomPlayerCount;
				return;
			}
			this.playersInRoom = 0;
		}

		// Token: 0x06005149 RID: 20809 RVA: 0x001953A8 File Offset: 0x001935A8
		public void PressButton(SharedBlocksKeyboardBindings buttonPressed)
		{
			if (!this.IsDriver)
			{
				this.SetStatusText("NOT TERMINAL CONTROLLER");
				return;
			}
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.up)
			{
				this.OnUpButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.down)
			{
				this.OnDownButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.delete)
			{
				this.OnDeleteButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.enter)
			{
				this.OnSelectButtonPressed();
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.zero && buttonPressed <= SharedBlocksKeyboardBindings.nine)
			{
				this.OnNumberPressed((int)buttonPressed);
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.A && buttonPressed <= SharedBlocksKeyboardBindings.Z)
			{
				this.OnLetterPressed(buttonPressed.ToString());
			}
		}

		// Token: 0x0600514A RID: 20810 RVA: 0x00195444 File Offset: 0x00193644
		private void OnUpButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnUpPressed();
			}
		}

		// Token: 0x0600514B RID: 20811 RVA: 0x0019545F File Offset: 0x0019365F
		private void OnDownButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDownPressed();
			}
		}

		// Token: 0x0600514C RID: 20812 RVA: 0x0019547A File Offset: 0x0019367A
		private void OnSelectButtonPressed()
		{
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Found)
			{
				this.OnLoadMapPressed();
				return;
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnSelectPressed();
			}
		}

		// Token: 0x0600514D RID: 20813 RVA: 0x001954AA File Offset: 0x001936AA
		private void OnDeleteButtonPressed()
		{
			if (this.localState.state != SharedBlocksTerminal.TerminalState.Loading && this.localState.state != SharedBlocksTerminal.TerminalState.Searching)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDeletePressed();
			}
		}

		// Token: 0x0600514E RID: 20814 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnBackButtonPressed()
		{
		}

		// Token: 0x0600514F RID: 20815 RVA: 0x001954E8 File Offset: 0x001936E8
		private void OnNumberPressed(int number)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnNumberPressed(number);
			}
		}

		// Token: 0x06005150 RID: 20816 RVA: 0x00195504 File Offset: 0x00193704
		private void OnLetterPressed(string letter)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnLetterPressed(letter);
			}
		}

		// Token: 0x06005151 RID: 20817 RVA: 0x00195520 File Offset: 0x00193720
		private void OnTerminalControlPressed()
		{
			if (this.isTerminalLocked)
			{
				if (this.IsDriver)
				{
					if (NetworkSystem.Instance.InRoom)
					{
						this.linkedTable.builderNetworking.RequestBlocksTerminalControl(false);
						return;
					}
					this.SetTerminalDriver(-2);
					return;
				}
			}
			else
			{
				if (NetworkSystem.Instance.InRoom)
				{
					this.linkedTable.builderNetworking.RequestBlocksTerminalControl(true);
					return;
				}
				this.SetTerminalDriver(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}

		// Token: 0x06005152 RID: 20818 RVA: 0x00195598 File Offset: 0x00193798
		public void OnLoadMapPressed()
		{
			if (!this.IsDriver)
			{
				this.SetStatusText("NOT TERMINAL CONTROLLER");
				return;
			}
			if (this.currentScreen == null || this.selectedMap == null)
			{
				this.SetStatusText("NO MAP SELECTED");
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap)
			{
				this.SetStatusText("BLOCKS LOAD ALREADY IN PROGRESS");
				return;
			}
			string statusText;
			if (!this.CanChangeMapState(true, out statusText))
			{
				this.SetStatusText(statusText);
				return;
			}
			if (this.linkedTable != null)
			{
				if (Time.time > this.lastLoadTime + this.loadMapCooldown)
				{
					this.SetStatusText("LOADING BLOCKS ...");
					this.isLoadingMap = true;
					this.lastLoadTime = Time.time;
					this.linkedTable.LoadSharedMap(this.selectedMap);
					return;
				}
				int num = Mathf.RoundToInt(this.lastLoadTime + this.loadMapCooldown - Time.time);
				this.SetStatusText(string.Format("PLEASE WAIT {0} SECONDS BEFORE LOADING ANOTHER MAP", num));
			}
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x0019568A File Offset: 0x0019388A
		public bool IsPlayerDriver(Player player)
		{
			return player.ActorNumber == this.localState.driverID;
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x0019569F File Offset: 0x0019389F
		public bool ValidateTerminalControlRequest(bool locked, int playerNumber)
		{
			if (locked && playerNumber == -2)
			{
				return false;
			}
			if (this.localState.driverID == -2)
			{
				return locked;
			}
			return this.localState.driverID == playerNumber;
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x001956CA File Offset: 0x001938CA
		private void OnDriverNameChanged()
		{
			this.RefreshDriverNickname();
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x001956D4 File Offset: 0x001938D4
		public void SetTerminalDriver(int playerNum)
		{
			if (playerNum != -2)
			{
				if (this.localState.driverID != -2 && this.localState.driverID != playerNum)
				{
					GTDev.LogWarning<string>(string.Format("Shared BlocksTerminal SetTerminalDriver cannot set {0} as driver while {1} is driver", playerNum, this.localState.driverID), null);
					return;
				}
				this.localState.driverID = playerNum;
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerNum);
				RigContainer rigContainer;
				if (netPlayerByID != null && VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					this.driverRig = rigContainer.Rig;
					this.driverRig.OnPlayerNameVisibleChanged += this.OnDriverNameChanged;
				}
				this.isTerminalLocked = true;
				this.UpdateTerminalButton();
				this.RefreshActiveScreen();
				this.searchScreen.SetInputTextEnabled(this.IsDriver);
				if (this.IsDriver && this.awaitingWebRequest)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
					this.searchScreen.SetMapCode(this.requestedMapID);
				}
				else if (this.isLoadingMap)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
					this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				}
				else
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				}
			}
			else
			{
				if (this.driverRig != null)
				{
					this.driverRig.OnPlayerNameVisibleChanged -= this.OnDriverNameChanged;
					this.driverRig = null;
				}
				this.localState.driverID = -2;
				this.isTerminalLocked = false;
				this.UpdateTerminalButton();
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				this.RefreshActiveScreen();
			}
			this.RefreshDriverNickname();
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x00195854 File Offset: 0x00193A54
		private void RefreshDriverNickname()
		{
			if (this.localState.driverID == -2)
			{
				this.currentDriverLabel.gameObject.SetActive(false);
				this.currentDriverText.text = "";
				this.currentDriverText.gameObject.SetActive(false);
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			if (NetworkSystem.Instance.InRoom)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(this.localState.driverID);
				if (player != null && this.useNametags && flag)
				{
					RigContainer rigContainer;
					if (player.IsLocal)
					{
						this.currentDriverText.text = player.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						this.currentDriverText.text = rigContainer.Rig.playerNameVisible;
					}
					else
					{
						this.currentDriverText.text = player.DefaultName;
					}
				}
				else
				{
					this.currentDriverText.text = "";
				}
			}
			else
			{
				this.currentDriverText.text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
			}
			this.currentDriverLabel.gameObject.SetActive(true);
			this.currentDriverText.gameObject.SetActive(true);
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x001959A0 File Offset: 0x00193BA0
		public bool ValidateLoadMapRequest(string mapID, int playerNum)
		{
			return playerNum == this.localState.driverID && this.AreAllPlayersInLobby() && SharedBlocksManager.IsMapIDValid(mapID);
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x001959C2 File Offset: 0x00193BC2
		private void OnJoinedRoom()
		{
			GTDev.Log<string>("[SharedBlocksTerminal::OnJoinedRoom] Joined a multiplayer room, resetting terminal control", null);
			this.cachedLocalPlayerID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.ResetTerminalControl();
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x001959EA File Offset: 0x00193BEA
		private void OnReturnedToSinglePlayer()
		{
			if (this.localState.driverID != this.cachedLocalPlayerID)
			{
				this.ResetTerminalControl();
			}
			else
			{
				this.localState.driverID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
			this.cachedLocalPlayerID = -1;
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x00195A28 File Offset: 0x00193C28
		public void ResetTerminalControl()
		{
			this.localState.driverID = -2;
			this.isTerminalLocked = false;
			this.selectedMap = null;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.UpdateTerminalButton();
		}

		// Token: 0x0600515C RID: 20828 RVA: 0x00195A58 File Offset: 0x00193C58
		private void UpdateTerminalButton()
		{
			this.terminalControlButton.isOn = this.isTerminalLocked;
			this.terminalControlButton.UpdateColor();
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x00195A78 File Offset: 0x00193C78
		private void OnSharedBlocksMapLoaded(string mapID)
		{
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(mapID);
			}
			if (SharedBlocksManager.IsMapIDValid(mapID))
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadSuccess);
			}
			else if (this.localState.state != SharedBlocksTerminal.TerminalState.LoadFail)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			}
			this.isLoadingMap = false;
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x00195AC6 File Offset: 0x00193CC6
		private void OnSharedBlocksMapLoadFailed(string message)
		{
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			this.SetStatusText(message);
			this.isLoadingMap = false;
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x00195AE0 File Offset: 0x00193CE0
		private void OnSharedBlocksMapLoadStart()
		{
			if (this.linkedTable == null)
			{
				return;
			}
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
				this.isLoadingMap = true;
				this.lastLoadTime = Time.time;
			}
		}

		// Token: 0x04005AB3 RID: 23219
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04005AB4 RID: 23220
		[SerializeField]
		private TMP_Text currentMapSelectionText;

		// Token: 0x04005AB5 RID: 23221
		[SerializeField]
		private TMP_Text statusMessageText;

		// Token: 0x04005AB6 RID: 23222
		[SerializeField]
		private TMP_Text currentDriverText;

		// Token: 0x04005AB7 RID: 23223
		[SerializeField]
		private TMP_Text currentDriverLabel;

		// Token: 0x04005AB8 RID: 23224
		[SerializeField]
		private SharedBlocksScreen noDriverScreen;

		// Token: 0x04005AB9 RID: 23225
		[SerializeField]
		private SharedBlocksScreenSearch searchScreen;

		// Token: 0x04005ABA RID: 23226
		[SerializeField]
		private GorillaPressableButton terminalControlButton;

		// Token: 0x04005ABB RID: 23227
		[SerializeField]
		private float loadMapCooldown = 30f;

		// Token: 0x04005ABC RID: 23228
		[SerializeField]
		private GorillaFriendCollider lobbyTrigger;

		// Token: 0x04005ABD RID: 23229
		private SharedBlocksManager.SharedBlocksMap selectedMap;

		// Token: 0x04005ABE RID: 23230
		private SharedBlocksScreen currentScreen;

		// Token: 0x04005ABF RID: 23231
		private BuilderTable linkedTable;

		// Token: 0x04005AC0 RID: 23232
		public const int NO_DRIVER_ID = -2;

		// Token: 0x04005AC1 RID: 23233
		private bool awaitingWebRequest;

		// Token: 0x04005AC2 RID: 23234
		private string requestedMapID;

		// Token: 0x04005AC3 RID: 23235
		public const string POINTER = "> ";

		// Token: 0x04005AC4 RID: 23236
		public Action<bool> OnMapLoadComplete;

		// Token: 0x04005AC5 RID: 23237
		private bool isTerminalLocked;

		// Token: 0x04005AC6 RID: 23238
		private SharedBlocksTerminal.SharedBlocksTerminalState localState;

		// Token: 0x04005AC7 RID: 23239
		private int cachedLocalPlayerID = -1;

		// Token: 0x04005AC8 RID: 23240
		private bool isLoadingMap;

		// Token: 0x04005AC9 RID: 23241
		private float lastLoadTime;

		// Token: 0x04005ACA RID: 23242
		private bool useNametags;

		// Token: 0x04005ACB RID: 23243
		private bool hasInitialized;

		// Token: 0x04005ACC RID: 23244
		private static StringBuilder sb = new StringBuilder();

		// Token: 0x04005ACD RID: 23245
		private VRRig driverRig;

		// Token: 0x04005ACE RID: 23246
		private static List<VRRig> tempRigs = new List<VRRig>(16);

		// Token: 0x04005ACF RID: 23247
		private int playersInRoom;

		// Token: 0x02000CC9 RID: 3273
		public enum ScreenType
		{
			// Token: 0x04005AD1 RID: 23249
			NO_DRIVER,
			// Token: 0x04005AD2 RID: 23250
			SEARCH,
			// Token: 0x04005AD3 RID: 23251
			LOADING,
			// Token: 0x04005AD4 RID: 23252
			ERROR,
			// Token: 0x04005AD5 RID: 23253
			SCAN_INFO,
			// Token: 0x04005AD6 RID: 23254
			OTHER_DRIVER
		}

		// Token: 0x02000CCA RID: 3274
		public enum TerminalState
		{
			// Token: 0x04005AD8 RID: 23256
			NoStatus,
			// Token: 0x04005AD9 RID: 23257
			Searching,
			// Token: 0x04005ADA RID: 23258
			NotFound,
			// Token: 0x04005ADB RID: 23259
			Found,
			// Token: 0x04005ADC RID: 23260
			Loading,
			// Token: 0x04005ADD RID: 23261
			LoadSuccess,
			// Token: 0x04005ADE RID: 23262
			LoadFail
		}

		// Token: 0x02000CCB RID: 3275
		public class SharedBlocksTerminalState
		{
			// Token: 0x04005ADF RID: 23263
			public SharedBlocksTerminal.ScreenType currentScreen;

			// Token: 0x04005AE0 RID: 23264
			public SharedBlocksTerminal.TerminalState state;

			// Token: 0x04005AE1 RID: 23265
			public int driverID;
		}
	}
}
