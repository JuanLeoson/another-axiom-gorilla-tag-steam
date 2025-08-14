using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004CE RID: 1230
public class GameModePages : BasePageHandler
{
	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001E39 RID: 7737 RVA: 0x000A0D7D File Offset: 0x0009EF7D
	protected override int pageSize
	{
		get
		{
			return this.buttons.Length;
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001E3A RID: 7738 RVA: 0x000A0D87 File Offset: 0x0009EF87
	protected override int entriesCount
	{
		get
		{
			return GameMode.gameModeNames.Count;
		}
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x000A0D94 File Offset: 0x0009EF94
	private void Awake()
	{
		GameModePages.gameModeSelectorInstances.Add(this);
		this.buttons = base.GetComponentsInChildren<GameModeSelectButton>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].buttonIndex = i;
			this.buttons[i].selector = this;
		}
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000A0DE7 File Offset: 0x0009EFE7
	protected override void Start()
	{
		base.Start();
		base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		this.initialized = true;
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000A0E01 File Offset: 0x0009F001
	private void OnEnable()
	{
		if (this.initialized)
		{
			base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000A0E16 File Offset: 0x0009F016
	private void OnDestroy()
	{
		GameModePages.gameModeSelectorInstances.Remove(this);
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000A0E24 File Offset: 0x0009F024
	protected override void ShowPage(int selectedPage, int startIndex, int endIndex)
	{
		GameModePages.textBuilder.Clear();
		for (int i = startIndex; i < endIndex; i++)
		{
			GameModePages.textBuilder.AppendLine(GameMode.gameModeNames[i]);
		}
		this.gameModeText.text = GameModePages.textBuilder.ToString();
		if (base.selectedIndex >= startIndex && base.selectedIndex <= endIndex)
		{
			this.UpdateAllButtons(this.currentButtonIndex);
		}
		else
		{
			this.UpdateAllButtons(-1);
		}
		int buttonsMissing = (selectedPage == base.pages - 1 && base.maxEntires > endIndex) ? (base.maxEntires - endIndex) : 0;
		this.EnableEntryButtons(buttonsMissing);
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000A0EC1 File Offset: 0x0009F0C1
	protected override void PageEntrySelected(int pageEntry, int selectionIndex)
	{
		if (selectionIndex >= this.entriesCount)
		{
			return;
		}
		GameModePages.sharedSelectedIndex = selectionIndex;
		this.UpdateAllButtons(pageEntry);
		this.currentButtonIndex = pageEntry;
		GorillaComputer.instance.OnModeSelectButtonPress(GameMode.gameModeNames[selectionIndex], false);
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000A0EFC File Offset: 0x0009F0FC
	private void UpdateAllButtons(int onButton)
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i == onButton)
			{
				this.buttons[onButton].isOn = true;
				this.buttons[onButton].UpdateColor();
			}
			else if (this.buttons[i].isOn)
			{
				this.buttons[i].isOn = false;
				this.buttons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000A0F68 File Offset: 0x0009F168
	private void EnableEntryButtons(int buttonsMissing)
	{
		int num = this.buttons.Length - buttonsMissing;
		int i;
		for (i = 0; i < num; i++)
		{
			this.buttons[i].gameObject.SetActive(true);
		}
		while (i < this.buttons.Length)
		{
			this.buttons[i].gameObject.SetActive(false);
			i++;
		}
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000A0FC4 File Offset: 0x0009F1C4
	public static void SetSelectedGameModeShared(string gameMode)
	{
		GameModePages.sharedSelectedIndex = GameMode.gameModeNames.IndexOf(gameMode);
		if (GameModePages.sharedSelectedIndex < 0)
		{
			return;
		}
		for (int i = 0; i < GameModePages.gameModeSelectorInstances.Count; i++)
		{
			GameModePages.gameModeSelectorInstances[i].SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x040026BD RID: 9917
	private int currentButtonIndex;

	// Token: 0x040026BE RID: 9918
	[SerializeField]
	private Text gameModeText;

	// Token: 0x040026BF RID: 9919
	[SerializeField]
	private GameModeSelectButton[] buttons;

	// Token: 0x040026C0 RID: 9920
	private bool initialized;

	// Token: 0x040026C1 RID: 9921
	private static int sharedSelectedIndex = 0;

	// Token: 0x040026C2 RID: 9922
	private static StringBuilder textBuilder = new StringBuilder(50);

	// Token: 0x040026C3 RID: 9923
	[OnEnterPlay_Clear]
	private static List<GameModePages> gameModeSelectorInstances = new List<GameModePages>(7);
}
