using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000551 RID: 1361
public class BuilderScanKiosk : MonoBehaviour
{
	// Token: 0x0600212C RID: 8492 RVA: 0x000B3A26 File Offset: 0x000B1C26
	public static bool IsSaveSlotValid(int slot)
	{
		return slot >= 0 && slot < BuilderScanKiosk.NUM_SAVE_SLOTS;
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x000B3A38 File Offset: 0x000B1C38
	private void Start()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.AddListener(new UnityAction(this.OnSavePressed));
		}
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.AddListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveSuccess.AddListener(new UnityAction(this.OnSaveSuccess));
			this.targetTable.OnSaveFailure.AddListener(new UnityAction<string>(this.OnSaveFail));
			SharedBlocksManager.OnSaveTimeUpdated += this.OnSaveTimeUpdated;
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.AddListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			gorillaPressableButton.onPressed += this.OnScanButtonPressed;
		}
		this.scanTriangle = this.scanAnimation.GetComponent<MeshRenderer>();
		this.scanTriangle.enabled = false;
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.LoadPlayerPrefs();
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x000B3B88 File Offset: 0x000B1D88
	private void OnDestroy()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.RemoveListener(new UnityAction(this.OnSavePressed));
		}
		SharedBlocksManager.OnSaveTimeUpdated -= this.OnSaveTimeUpdated;
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.RemoveListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveFailure.RemoveListener(new UnityAction<string>(this.OnSaveFail));
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.RemoveListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnScanButtonPressed;
			}
		}
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000B3C9C File Offset: 0x000B1E9C
	private void OnNoneButtonPressed()
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		if (this.targetTable.CurrentSaveSlot != -1)
		{
			this.targetTable.CurrentSaveSlot = -1;
			this.SavePlayerPrefs();
			this.UpdateUI();
		}
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000B3CF0 File Offset: 0x000B1EF0
	private void OnScanButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		int i = 0;
		while (i < this.scanButtons.Count)
		{
			if (button.Equals(this.scanButtons[i]))
			{
				if (i != this.targetTable.CurrentSaveSlot)
				{
					this.targetTable.CurrentSaveSlot = i;
					this.SavePlayerPrefs();
					this.UpdateUI();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDevScanPressed()
	{
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000B3D70 File Offset: 0x000B1F70
	private void LoadPlayerPrefs()
	{
		int @int = PlayerPrefs.GetInt(BuilderScanKiosk.playerPrefKey, -1);
		this.targetTable.CurrentSaveSlot = @int;
		this.UpdateUI();
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000B3D9B File Offset: 0x000B1F9B
	private void SavePlayerPrefs()
	{
		PlayerPrefs.SetInt(BuilderScanKiosk.playerPrefKey, this.targetTable.CurrentSaveSlot);
		PlayerPrefs.Save();
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000B3DB8 File Offset: 0x000B1FB8
	private void ToggleSaveButton(bool enabled)
	{
		if (enabled)
		{
			this.saveButton.enabled = true;
			this.saveButton.buttonRenderer.material = this.saveButton.unpressedMaterial;
			return;
		}
		this.saveButton.enabled = false;
		this.saveButton.buttonRenderer.material = this.saveButton.pressedMaterial;
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000B3E18 File Offset: 0x000B2018
	private void Update()
	{
		if (this.isAnimating)
		{
			if (this.scanAnimation == null)
			{
				this.isAnimating = false;
			}
			else if ((double)Time.time > this.scanCompleteTime)
			{
				this.scanTriangle.enabled = false;
				this.isAnimating = false;
			}
		}
		if (this.coolingDown && (double)Time.time > this.coolDownCompleteTime)
		{
			this.coolingDown = false;
			this.UpdateUI();
		}
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000B3E88 File Offset: 0x000B2088
	private void OnSavePressed()
	{
		if (this.targetTable == null || !this.isDirty || this.coolingDown)
		{
			return;
		}
		BuilderScanKiosk.ScannerState scannerState = this.scannerState;
		if (scannerState == BuilderScanKiosk.ScannerState.IDLE)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.CONFIRMATION;
			this.UpdateUI();
			return;
		}
		if (scannerState != BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			return;
		}
		this.scannerState = BuilderScanKiosk.ScannerState.SAVING;
		if (this.scanAnimation != null)
		{
			this.scanCompleteTime = (double)(Time.time + this.scanAnimation.clip.length);
			this.scanTriangle.enabled = true;
			this.scanAnimation.Rewind();
			this.scanAnimation.Play();
		}
		if (this.soundBank != null)
		{
			this.soundBank.Play();
		}
		this.isAnimating = true;
		this.saveError = false;
		this.errorMsg = string.Empty;
		this.coolDownCompleteTime = (double)(Time.time + this.saveCooldownSeconds);
		this.coolingDown = true;
		this.UpdateUI();
		this.targetTable.SaveTableForPlayer();
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000B3F84 File Offset: 0x000B2184
	private string GetSavePath()
	{
		return string.Concat(new string[]
		{
			this.GetSaveFolder(),
			Path.DirectorySeparatorChar.ToString(),
			BuilderScanKiosk.SAVE_FILE,
			"_",
			this.targetTable.CurrentSaveSlot.ToString(),
			".png"
		});
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000B3FE0 File Offset: 0x000B21E0
	private string GetSaveFolder()
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + BuilderScanKiosk.SAVE_FOLDER;
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000B3FFB File Offset: 0x000B21FB
	private void OnSaveDirtyChanged(bool dirty)
	{
		this.isDirty = dirty;
		this.UpdateUI();
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000B400A File Offset: 0x000B220A
	private void OnSaveTimeUpdated()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000B400A File Offset: 0x000B220A
	private void OnSaveSuccess()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000B4020 File Offset: 0x000B2220
	private void OnSaveFail(string errorMsg)
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = true;
		this.errorMsg = errorMsg;
		this.UpdateUI();
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000B4040 File Offset: 0x000B2240
	private void UpdateUI()
	{
		this.screenText.text = this.GetTextForScreen();
		this.ToggleSaveButton(BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot) && !this.coolingDown);
		this.noneButton.buttonRenderer.material = ((!BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot)) ? this.noneButton.pressedMaterial : this.noneButton.unpressedMaterial);
		for (int i = 0; i < this.scanButtons.Count; i++)
		{
			this.scanButtons[i].buttonRenderer.material = ((this.targetTable.CurrentSaveSlot == i) ? this.scanButtons[i].pressedMaterial : this.scanButtons[i].unpressedMaterial);
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.saveButton.myTmpText.text = "YES UPDATE SCAN";
			return;
		}
		this.saveButton.myTmpText.text = "UPDATE SCAN";
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000B4150 File Offset: 0x000B2350
	private string GetTextForScreen()
	{
		if (this.targetTable == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		int currentSaveSlot = this.targetTable.CurrentSaveSlot;
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			stringBuilder.Append("<b><color=red>NONE</color></b>");
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append("SCAN ");
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(currentSaveSlot);
			DateTime t = DateTime.FromBinary(publishInfoForSlot.publishTime);
			if (t > DateTime.MinValue)
			{
				stringBuilder.Append(": ");
				stringBuilder.Append("UPDATED ");
				stringBuilder.Append(t.ToString());
				stringBuilder.Append("\n");
			}
			if (SharedBlocksManager.IsMapIDValid(publishInfoForSlot.mapID))
			{
				stringBuilder.Append("MAP ID: ");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(0, 4));
				stringBuilder.Append("-");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(4));
				stringBuilder.Append("\nUSE THIS CODE IN THE SHARE MY BLOCKS ROOM");
			}
		}
		stringBuilder.Append("\n");
		switch (this.scannerState)
		{
		case BuilderScanKiosk.ScannerState.IDLE:
			if (this.saveError)
			{
				stringBuilder.Append("ERROR WHILE SCANNING: ");
				stringBuilder.Append(this.errorMsg);
			}
			else if (this.coolingDown)
			{
				stringBuilder.Append("COOLING DOWN...");
			}
			else if (!this.isDirty)
			{
				stringBuilder.Append("NO UNSAVED CHANGES");
			}
			break;
		case BuilderScanKiosk.ScannerState.CONFIRMATION:
			stringBuilder.Append("YOU ARE ABOUT TO REPLACE ");
			if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
			{
				stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
			}
			else
			{
				stringBuilder.Append("<b><color=red>SCAN ");
				stringBuilder.Append(currentSaveSlot + 1);
				stringBuilder.Append("</color></b>");
			}
			stringBuilder.Append(" ARE YOU SURE YOU WANT TO SCAN?");
			break;
		case BuilderScanKiosk.ScannerState.SAVING:
			stringBuilder.Append("SCANNING BUILD...");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		stringBuilder.Append("\n\n\n");
		stringBuilder.Append("CREATE A <b><color=red>NEW</color></b> PRIVATE ROOM TO LOAD ");
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			stringBuilder.Append("<b><color=red>AN EMPTY TABLE</color></b>");
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append("SCAN ");
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002A7B RID: 10875
	[SerializeField]
	private GorillaPressableButton saveButton;

	// Token: 0x04002A7C RID: 10876
	[SerializeField]
	private GorillaPressableButton noneButton;

	// Token: 0x04002A7D RID: 10877
	[SerializeField]
	private List<GorillaPressableButton> scanButtons;

	// Token: 0x04002A7E RID: 10878
	[SerializeField]
	private BuilderTable targetTable;

	// Token: 0x04002A7F RID: 10879
	[SerializeField]
	private float saveCooldownSeconds = 5f;

	// Token: 0x04002A80 RID: 10880
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x04002A81 RID: 10881
	[SerializeField]
	private SoundBankPlayer soundBank;

	// Token: 0x04002A82 RID: 10882
	[SerializeField]
	private Animation scanAnimation;

	// Token: 0x04002A83 RID: 10883
	private MeshRenderer scanTriangle;

	// Token: 0x04002A84 RID: 10884
	private bool isAnimating;

	// Token: 0x04002A85 RID: 10885
	private static string playerPrefKey = "BuilderSaveSlot";

	// Token: 0x04002A86 RID: 10886
	private static string SAVE_FOLDER = "MonkeBlocks";

	// Token: 0x04002A87 RID: 10887
	private static string SAVE_FILE = "MyBuild";

	// Token: 0x04002A88 RID: 10888
	public static int NUM_SAVE_SLOTS = 3;

	// Token: 0x04002A89 RID: 10889
	public static int DEV_SAVE_SLOT = -2;

	// Token: 0x04002A8A RID: 10890
	private Texture2D buildCaptureTexture;

	// Token: 0x04002A8B RID: 10891
	private bool isDirty;

	// Token: 0x04002A8C RID: 10892
	private bool saveError;

	// Token: 0x04002A8D RID: 10893
	private string errorMsg = string.Empty;

	// Token: 0x04002A8E RID: 10894
	private bool coolingDown;

	// Token: 0x04002A8F RID: 10895
	private double coolDownCompleteTime;

	// Token: 0x04002A90 RID: 10896
	private double scanCompleteTime;

	// Token: 0x04002A91 RID: 10897
	private BuilderScanKiosk.ScannerState scannerState;

	// Token: 0x02000552 RID: 1362
	private enum ScannerState
	{
		// Token: 0x04002A93 RID: 10899
		IDLE,
		// Token: 0x04002A94 RID: 10900
		CONFIRMATION,
		// Token: 0x04002A95 RID: 10901
		SAVING
	}
}
