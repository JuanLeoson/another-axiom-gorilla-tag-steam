using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C47 RID: 3143
	public class PlayerTimerBoardLine : MonoBehaviour
	{
		// Token: 0x06004DC4 RID: 19908 RVA: 0x001826A3 File Offset: 0x001808A3
		public void ResetData()
		{
			this.linePlayer = null;
			this.currentNickname = string.Empty;
			this.playerTimeStr = string.Empty;
			this.playerTimeSeconds = 0f;
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x001826D0 File Offset: 0x001808D0
		public void SetLineData(NetPlayer netPlayer)
		{
			if (!netPlayer.InRoom || netPlayer == this.linePlayer)
			{
				return;
			}
			this.linePlayer = netPlayer;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.rigContainer = rigContainer;
				this.playerVRRig = rigContainer.Rig;
			}
			this.InitializeLine();
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x0018271E File Offset: 0x0018091E
		public void InitializeLine()
		{
			this.currentNickname = string.Empty;
			this.UpdatePlayerText();
			this.UpdateTimeText();
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x00182738 File Offset: 0x00180938
		public void UpdateLine()
		{
			if (this.linePlayer != null)
			{
				if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
				{
					this.UpdatePlayerText();
					this.parentBoard.IsDirty = true;
				}
				string value = this.playerTimeStr;
				this.UpdateTimeText();
				if (!this.playerTimeStr.Equals(value))
				{
					this.parentBoard.IsDirty = true;
				}
			}
		}

		// Token: 0x06004DC8 RID: 19912 RVA: 0x001827A0 File Offset: 0x001809A0
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
			}
			catch (Exception)
			{
				this.playerNameVisible = this.linePlayer.DefaultName;
				GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}

		// Token: 0x06004DC9 RID: 19913 RVA: 0x001828D4 File Offset: 0x00180AD4
		private void UpdateTimeText()
		{
			if (this.linePlayer == null || !(PlayerTimerManager.instance != null))
			{
				this.playerTimeStr = "--:--:--";
				return;
			}
			this.playerTimeSeconds = PlayerTimerManager.instance.GetLastDurationForPlayer(this.linePlayer.ActorNumber);
			if (this.playerTimeSeconds > 0f)
			{
				this.playerTimeStr = TimeSpan.FromSeconds((double)this.playerTimeSeconds).ToString("mm\\:ss\\:ff");
				return;
			}
			this.playerTimeStr = "--:--:--";
		}

		// Token: 0x06004DCA RID: 19914 RVA: 0x00182958 File Offset: 0x00180B58
		public string NormalizeName(bool doIt, string text)
		{
			if (doIt)
			{
				if (GorillaComputer.instance.CheckAutoBanListForName(text))
				{
					text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
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

		// Token: 0x06004DCB RID: 19915 RVA: 0x001829FC File Offset: 0x00180BFC
		public static int CompareByTotalTime(PlayerTimerBoardLine lineA, PlayerTimerBoardLine lineB)
		{
			if (lineA.playerTimeSeconds > 0f && lineB.playerTimeSeconds > 0f)
			{
				return lineA.playerTimeSeconds.CompareTo(lineB.playerTimeSeconds);
			}
			if (lineA.playerTimeSeconds <= 0f)
			{
				return 1;
			}
			if (lineB.playerTimeSeconds <= 0f)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x040056C7 RID: 22215
		public string playerNameVisible;

		// Token: 0x040056C8 RID: 22216
		public string playerTimeStr;

		// Token: 0x040056C9 RID: 22217
		private float playerTimeSeconds;

		// Token: 0x040056CA RID: 22218
		public NetPlayer linePlayer;

		// Token: 0x040056CB RID: 22219
		public VRRig playerVRRig;

		// Token: 0x040056CC RID: 22220
		public PlayerTimerBoard parentBoard;

		// Token: 0x040056CD RID: 22221
		internal RigContainer rigContainer;

		// Token: 0x040056CE RID: 22222
		private string currentNickname;
	}
}
