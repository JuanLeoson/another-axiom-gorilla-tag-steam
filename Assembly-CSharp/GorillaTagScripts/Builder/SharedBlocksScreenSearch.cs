using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CC7 RID: 3271
	public class SharedBlocksScreenSearch : SharedBlocksScreen, IGorillaSliceableSimple
	{
		// Token: 0x06005122 RID: 20770 RVA: 0x001947D0 File Offset: 0x001929D0
		public override void OnSelectPressed()
		{
			if (SharedBlocksManager.IsMapIDValid(this.currentMapCode))
			{
				this.savedMapCode = this.currentMapCode;
				this.terminal.SelectMapIDAndOpenInfo(this.savedMapCode);
				return;
			}
			if (this.currentMapCode.Length < 8)
			{
				this.terminal.SetStatusText("INVALID MAP ID LENGTH");
				return;
			}
			this.terminal.SetStatusText("INVALID MAP ID");
		}

		// Token: 0x06005123 RID: 20771 RVA: 0x00194837 File Offset: 0x00192A37
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x06005124 RID: 20772 RVA: 0x0019486C File Offset: 0x00192A6C
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x06005125 RID: 20773 RVA: 0x0019489A File Offset: 0x00192A9A
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x06005126 RID: 20774 RVA: 0x001948C4 File Offset: 0x00192AC4
		public override void Show()
		{
			SharedBlocksManager.OnRecentMapIdsUpdated += this.DrawScreen;
			this.currentMapCode = string.Empty;
			this.DrawScreen();
			base.Show();
			this.RefreshPlayerCounter();
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnMapLoaded(table.GetCurrentMapID());
			}
		}

		// Token: 0x06005127 RID: 20775 RVA: 0x00194950 File Offset: 0x00192B50
		public override void Hide()
		{
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			SharedBlocksManager.OnRecentMapIdsUpdated -= this.DrawScreen;
			base.Hide();
		}

		// Token: 0x06005128 RID: 20776 RVA: 0x001949D8 File Offset: 0x00192BD8
		private void OnMapLoaded(string mapID)
		{
			this.loadedMap.text = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
		}

		// Token: 0x06005129 RID: 20777 RVA: 0x00194A04 File Offset: 0x00192C04
		private void OnMapCleared()
		{
			this.loadedMap.text = "LOADED MAP : NONE";
		}

		// Token: 0x0600512A RID: 20778 RVA: 0x00194A16 File Offset: 0x00192C16
		private void UpdateInput()
		{
			this.inputText.text = "MAP SEARCH : " + SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
		}

		// Token: 0x0600512B RID: 20779 RVA: 0x00194A38 File Offset: 0x00192C38
		public void SetMapCode(string mapCode)
		{
			if (mapCode == null)
			{
				this.currentMapCode = string.Empty;
			}
			else
			{
				this.currentMapCode = mapCode;
			}
			this.UpdateInput();
		}

		// Token: 0x0600512C RID: 20780 RVA: 0x00194A57 File Offset: 0x00192C57
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x0600512D RID: 20781 RVA: 0x00194A80 File Offset: 0x00192C80
		private void DrawScreen()
		{
			this.UpdateInput();
			this.sb.Clear();
			this.sb.Append("RECENT VOTES\n");
			foreach (string mapID in SharedBlocksManager.GetRecentUpVotes())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID));
					this.sb.Append("\n");
				}
			}
			this.recentList.text = this.sb.ToString();
			this.sb.Clear();
			this.sb.Append("MY MAPS\n");
			foreach (string mapID2 in SharedBlocksManager.GetLocalMapIDs())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID2))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID2));
					this.sb.Append("\n");
				}
			}
			this.myScanList.text = this.sb.ToString();
		}

		// Token: 0x0600512E RID: 20782 RVA: 0x00194BC8 File Offset: 0x00192DC8
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x0600512F RID: 20783 RVA: 0x00194C14 File Offset: 0x00192E14
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06005130 RID: 20784 RVA: 0x00194C1C File Offset: 0x00192E1C
		public void OnEnable()
		{
			if (!this.updating)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = true;
			}
			this.RefreshPlayerCounter();
			RoomSystem.PlayersChangedEvent += new Action(this.PlayersChangedEvent);
		}

		// Token: 0x06005131 RID: 20785 RVA: 0x00194C14 File Offset: 0x00192E14
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06005132 RID: 20786 RVA: 0x00194C55 File Offset: 0x00192E55
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent -= new Action(this.PlayersChangedEvent);
		}

		// Token: 0x04005AA8 RID: 23208
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x04005AA9 RID: 23209
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x04005AAA RID: 23210
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04005AAB RID: 23211
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x04005AAC RID: 23212
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x04005AAD RID: 23213
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x04005AAE RID: 23214
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x04005AAF RID: 23215
		private string currentMapCode;

		// Token: 0x04005AB0 RID: 23216
		private string savedMapCode;

		// Token: 0x04005AB1 RID: 23217
		private StringBuilder sb = new StringBuilder();

		// Token: 0x04005AB2 RID: 23218
		private bool updating;
	}
}
