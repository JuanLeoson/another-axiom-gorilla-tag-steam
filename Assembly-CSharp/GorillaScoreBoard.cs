using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class GorillaScoreBoard : MonoBehaviour
{
	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06002C95 RID: 11413 RVA: 0x000EBBE8 File Offset: 0x000E9DE8
	// (set) Token: 0x06002C96 RID: 11414 RVA: 0x000EBBFF File Offset: 0x000E9DFF
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
		set
		{
			this._isDirty = value;
		}
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x000EBC08 File Offset: 0x000E9E08
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDestroy()
	{
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x000EBC3C File Offset: 0x000E9E3C
	public string GetBeginningString()
	{
		return "ROOM ID: " + (NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (NetworkSystem.Instance.RoomName + "   GAME: ")) + this.RoomType() + "\n  PLAYER     COLOR  MUTE   REPORT";
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000EBC7C File Offset: 0x000E9E7C
	public string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		for (int i = 0; i < count; i++)
		{
			this.tempGmName = this.gmNames[i];
			if (this.initialGameMode.Contains(this.tempGmName))
			{
				this.gmName = this.tempGmName;
				break;
			}
		}
		return this.gmName;
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000EBCFC File Offset: 0x000E9EFC
	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				if (this.lines[i].gameObject.activeInHierarchy)
				{
					this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
					if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
						if (this.lines[i].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[i].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this._isDirty = false;
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000EBEE8 File Offset: 0x000EA0E8
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 10);
			}
			text = text.ToUpper();
		}
		return text;
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000EBF47 File Offset: 0x000EA147
	private void Start()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000EBF4F File Offset: 0x000EA14F
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this._isDirty = true;
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000EBF5E File Offset: 0x000EA15E
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
	}

	// Token: 0x040037FC RID: 14332
	public GameObject scoreBoardLinePrefab;

	// Token: 0x040037FD RID: 14333
	public int startingYValue;

	// Token: 0x040037FE RID: 14334
	public int lineHeight;

	// Token: 0x040037FF RID: 14335
	public bool includeMMR;

	// Token: 0x04003800 RID: 14336
	public bool isActive;

	// Token: 0x04003801 RID: 14337
	public GameObject linesParent;

	// Token: 0x04003802 RID: 14338
	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x04003803 RID: 14339
	public TextMeshPro boardText;

	// Token: 0x04003804 RID: 14340
	public TextMeshPro buttonText;

	// Token: 0x04003805 RID: 14341
	public bool needsUpdate;

	// Token: 0x04003806 RID: 14342
	public TextMeshPro notInRoomText;

	// Token: 0x04003807 RID: 14343
	public string initialGameMode;

	// Token: 0x04003808 RID: 14344
	private string tempGmName;

	// Token: 0x04003809 RID: 14345
	private string gmName;

	// Token: 0x0400380A RID: 14346
	private const string error = "ERROR";

	// Token: 0x0400380B RID: 14347
	private List<string> gmNames;

	// Token: 0x0400380C RID: 14348
	private bool _isDirty = true;

	// Token: 0x0400380D RID: 14349
	private StringBuilder stringBuilder = new StringBuilder(220);

	// Token: 0x0400380E RID: 14350
	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
