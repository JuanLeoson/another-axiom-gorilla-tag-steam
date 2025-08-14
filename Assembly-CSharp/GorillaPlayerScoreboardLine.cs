using System;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E3 RID: 2019
public class GorillaPlayerScoreboardLine : MonoBehaviour
{
	// Token: 0x06003278 RID: 12920 RVA: 0x00106AB6 File Offset: 0x00104CB6
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00106AD4 File Offset: 0x00104CD4
	public void InitializeLine()
	{
		this.currentNickname = string.Empty;
		this.UpdatePlayerText();
		if (this.linePlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.muteButton.gameObject.SetActive(false);
			this.reportButton.gameObject.SetActive(false);
			this.hateSpeechButton.SetActive(false);
			this.toxicityButton.SetActive(false);
			this.cheatingButton.SetActive(false);
			this.cancelButton.SetActive(false);
			return;
		}
		this.muteButton.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null && GorillaScoreboardTotalUpdater.instance.reportDict.ContainsKey(this.playerActorNumber))
		{
			GorillaScoreboardTotalUpdater.PlayerReports playerReports = GorillaScoreboardTotalUpdater.instance.reportDict[this.playerActorNumber];
			this.reportedCheating = playerReports.cheating;
			this.reportedHateSpeech = playerReports.hateSpeech;
			this.reportedToxicity = playerReports.toxicity;
			this.reportInProgress = playerReports.pressedReport;
		}
		else
		{
			this.reportedCheating = false;
			this.reportedHateSpeech = false;
			this.reportedToxicity = false;
			this.reportInProgress = false;
		}
		this.reportButton.isOn = (this.reportedCheating || this.reportedHateSpeech || this.reportedToxicity);
		this.reportButton.UpdateColor();
		this.SwapToReportState(this.reportInProgress);
		this.muteButton.gameObject.SetActive(true);
		this.isMuteManual = PlayerPrefs.HasKey(this.linePlayer.UserId);
		this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
		this.muteButton.isOn = (this.mute != 0);
		this.muteButton.isAutoOn = false;
		this.muteButton.UpdateColor();
		if (this.rigContainer != null)
		{
			this.rigContainer.hasManualMute = this.isMuteManual;
			this.rigContainer.Muted = (this.mute != 0);
		}
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x00106CD0 File Offset: 0x00104ED0
	public void SetLineData(NetPlayer netPlayer)
	{
		if (!netPlayer.InRoom || netPlayer == this.linePlayer)
		{
			return;
		}
		if (this.playerActorNumber != netPlayer.ActorNumber)
		{
			this.initTime = Time.time;
		}
		this.playerActorNumber = netPlayer.ActorNumber;
		this.linePlayer = netPlayer;
		this.playerNameValue = (netPlayer.NickName ?? "");
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			this.rigContainer = rigContainer;
			this.playerVRRig = rigContainer.Rig;
		}
		this.InitializeLine();
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x00106D58 File Offset: 0x00104F58
	public void UpdateLine()
	{
		if (this.linePlayer != null)
		{
			if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
			{
				this.UpdatePlayerText();
				this.parentScoreboard.IsDirty = true;
			}
			if (this.rigContainer != null)
			{
				if (Time.time > this.initTime + this.emptyRigCooldown)
				{
					if (this.playerVRRig.netView != null)
					{
						this.emptyRigCount = 0;
					}
					else
					{
						this.emptyRigCount++;
						if (this.emptyRigCount > 30)
						{
							GorillaNot.instance.SendReport("empty rig", this.linePlayer.UserId, this.linePlayer.NickName);
						}
					}
				}
				Material material;
				if (this.playerVRRig.setMatIndex == 0)
				{
					material = this.playerVRRig.scoreboardMaterial;
				}
				else
				{
					material = this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex];
				}
				if (this.playerSwatch.material != material)
				{
					this.playerSwatch.material = material;
				}
				if (this.playerSwatch.color != this.playerVRRig.materialsToChangeTo[0].color)
				{
					this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
				}
				if (this.myRecorder == null)
				{
					this.myRecorder = NetworkSystem.Instance.LocalRecorder;
				}
				if (this.playerVRRig != null)
				{
					if (this.playerVRRig.remoteUseReplacementVoice || this.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE")
					{
						if (this.playerVRRig.SpeakingLoudness > this.playerVRRig.replacementVoiceLoudnessThreshold && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
						{
							this.speakerIcon.enabled = true;
						}
						else
						{
							this.speakerIcon.enabled = false;
						}
					}
					else if ((this.rigContainer.Voice != null && this.rigContainer.Voice.IsSpeaking) || (this.playerVRRig.rigSerializer != null && this.playerVRRig.rigSerializer.IsLocallyOwned && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
					{
						this.speakerIcon.enabled = true;
					}
					else
					{
						this.speakerIcon.enabled = false;
					}
				}
				else
				{
					this.speakerIcon.enabled = false;
				}
				if (!this.isMuteManual)
				{
					bool isPlayerAutoMuted = this.rigContainer.GetIsPlayerAutoMuted();
					if (this.muteButton.isAutoOn != isPlayerAutoMuted)
					{
						this.muteButton.isAutoOn = isPlayerAutoMuted;
						this.muteButton.UpdateColor();
					}
				}
			}
		}
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x00107028 File Offset: 0x00105228
	private void UpdatePlayerText()
	{
		try
		{
			if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				this.currentNickname = this.linePlayer.NickName;
			}
			else if (this.rigContainer.Initialized)
			{
				this.playerNameVisible = this.playerVRRig.playerNameVisible;
			}
			else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			this.currentNickname = this.linePlayer.NickName;
			this.playerName.text = (flag ? this.playerNameVisible : this.linePlayer.DefaultName);
		}
		catch (Exception)
		{
			this.playerNameVisible = this.linePlayer.DefaultName;
			GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
		}
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x001071A0 File Offset: 0x001053A0
	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		if (buttonType != GorillaPlayerLineButton.ButtonType.Mute)
		{
			if (buttonType == GorillaPlayerLineButton.ButtonType.Report)
			{
				this.SetReportState(true, buttonType);
				return;
			}
			this.SetReportState(false, buttonType);
		}
		else if (this.linePlayer != null && this.playerVRRig != null)
		{
			this.isMuteManual = true;
			this.muteButton.isAutoOn = false;
			this.mute = (isOn ? 1 : 0);
			PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
			if (this.rigContainer != null)
			{
				this.rigContainer.hasManualMute = this.isMuteManual;
				this.rigContainer.Muted = (this.mute != 0);
			}
			PlayerPrefs.Save();
			this.muteButton.UpdateColor();
			GorillaScoreboardTotalUpdater.ReportMute(this.linePlayer, this.mute);
			return;
		}
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x00107278 File Offset: 0x00105478
	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = (buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report);
		this.reportInProgress = reportState;
		if (reportState)
		{
			this.SwapToReportState(true);
		}
		else
		{
			this.SwapToReportState(false);
			if (this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerNameVisible);
					this.doneReporting = true;
				}
				this.reportedCheating = (this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating);
				this.reportedToxicity = (this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity);
				this.reportedHateSpeech = (this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech);
				this.reportButton.isOn = true;
				this.reportButton.UpdateColor();
			}
		}
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateLineState(this);
		}
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x00107384 File Offset: 0x00105584
	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags flags = new WebFlags(1);
		NetEventOptions options = new NetEventOptions
		{
			Flags = flags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte code = 50;
		object[] data = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(code, data, options, true);
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x0010741C File Offset: 0x0010561C
	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags flags = new WebFlags(1);
		NetEventOptions options = new NetEventOptions
		{
			Flags = flags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte code = 51;
		object[] data = new object[]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(code, data, options, true);
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x001074B4 File Offset: 0x001056B4
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
				GorillaNot.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}
		return text;
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x0010756B File Offset: 0x0010576B
	public void ResetData()
	{
		this.emptyRigCount = 0;
		this.playerActorNumber = -1;
		this.linePlayer = null;
		this.playerNameValue = string.Empty;
		this.currentNickname = string.Empty;
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x00107598 File Offset: 0x00105798
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterSL(this);
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x001075A0 File Offset: 0x001057A0
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterSL(this);
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x001075A8 File Offset: 0x001057A8
	private void SwapToReportState(bool reportInProgress)
	{
		this.reportButton.gameObject.SetActive(!reportInProgress);
		this.hateSpeechButton.SetActive(reportInProgress);
		this.toxicityButton.SetActive(reportInProgress);
		this.cheatingButton.SetActive(reportInProgress);
		this.cancelButton.SetActive(reportInProgress);
	}

	// Token: 0x04003F4B RID: 16203
	private static int[] targetActors = new int[]
	{
		-1
	};

	// Token: 0x04003F4C RID: 16204
	public Text playerName;

	// Token: 0x04003F4D RID: 16205
	public Text playerLevel;

	// Token: 0x04003F4E RID: 16206
	public Text playerMMR;

	// Token: 0x04003F4F RID: 16207
	public Image playerSwatch;

	// Token: 0x04003F50 RID: 16208
	public Texture infectedTexture;

	// Token: 0x04003F51 RID: 16209
	public NetPlayer linePlayer;

	// Token: 0x04003F52 RID: 16210
	public VRRig playerVRRig;

	// Token: 0x04003F53 RID: 16211
	public string playerLevelValue;

	// Token: 0x04003F54 RID: 16212
	public string playerMMRValue;

	// Token: 0x04003F55 RID: 16213
	public string playerNameValue;

	// Token: 0x04003F56 RID: 16214
	public string playerNameVisible;

	// Token: 0x04003F57 RID: 16215
	public int playerActorNumber;

	// Token: 0x04003F58 RID: 16216
	public GorillaPlayerLineButton muteButton;

	// Token: 0x04003F59 RID: 16217
	public GorillaPlayerLineButton reportButton;

	// Token: 0x04003F5A RID: 16218
	public GameObject hateSpeechButton;

	// Token: 0x04003F5B RID: 16219
	public GameObject toxicityButton;

	// Token: 0x04003F5C RID: 16220
	public GameObject cheatingButton;

	// Token: 0x04003F5D RID: 16221
	public GameObject cancelButton;

	// Token: 0x04003F5E RID: 16222
	public SpriteRenderer speakerIcon;

	// Token: 0x04003F5F RID: 16223
	public bool canPressNextReportButton = true;

	// Token: 0x04003F60 RID: 16224
	public Text[] texts;

	// Token: 0x04003F61 RID: 16225
	public SpriteRenderer[] sprites;

	// Token: 0x04003F62 RID: 16226
	public MeshRenderer[] meshes;

	// Token: 0x04003F63 RID: 16227
	public Image[] images;

	// Token: 0x04003F64 RID: 16228
	private Recorder myRecorder;

	// Token: 0x04003F65 RID: 16229
	private bool isMuteManual;

	// Token: 0x04003F66 RID: 16230
	private int mute;

	// Token: 0x04003F67 RID: 16231
	private int emptyRigCount;

	// Token: 0x04003F68 RID: 16232
	public GameObject myRig;

	// Token: 0x04003F69 RID: 16233
	public bool reportedCheating;

	// Token: 0x04003F6A RID: 16234
	public bool reportedToxicity;

	// Token: 0x04003F6B RID: 16235
	public bool reportedHateSpeech;

	// Token: 0x04003F6C RID: 16236
	public bool reportInProgress;

	// Token: 0x04003F6D RID: 16237
	private string currentNickname;

	// Token: 0x04003F6E RID: 16238
	public bool doneReporting;

	// Token: 0x04003F6F RID: 16239
	public bool lastVisible = true;

	// Token: 0x04003F70 RID: 16240
	public GorillaScoreBoard parentScoreboard;

	// Token: 0x04003F71 RID: 16241
	public float initTime;

	// Token: 0x04003F72 RID: 16242
	public float emptyRigCooldown = 10f;

	// Token: 0x04003F73 RID: 16243
	internal RigContainer rigContainer;
}
