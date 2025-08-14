using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	// Token: 0x04000F3E RID: 3902
	public Material Milestone_Error;

	// Token: 0x04000F3F RID: 3903
	public Material Milestone_AlreadyInRoom;

	// Token: 0x04000F40 RID: 3904
	public Material Milestone_InPrivateRoom;

	// Token: 0x04000F41 RID: 3905
	public Material Milestone_NotConnectedSoloJoin;

	// Token: 0x04000F42 RID: 3906
	public Material Milestone_LeaveRoomAndSoloJoin;

	// Token: 0x04000F43 RID: 3907
	public Material Milestone_LeaveRoomAndGroupJoin;

	// Token: 0x04000F44 RID: 3908
	public Material Milestone_AbandonPartyAndSoloJoin;

	// Token: 0x04000F45 RID: 3909
	public Material Milestone_ChangingGameModeSoloJoin;

	// Token: 0x04000F46 RID: 3910
	public Material ScreenBG_Error;

	// Token: 0x04000F47 RID: 3911
	public Material ScreenBG_AlreadyInRoom;

	// Token: 0x04000F48 RID: 3912
	public Material ScreenBG_InPrivateRoom;

	// Token: 0x04000F49 RID: 3913
	public Material ScreenBG_NotConnectedSoloJoin;

	// Token: 0x04000F4A RID: 3914
	public Material ScreenBG_LeaveRoomAndSoloJoin;

	// Token: 0x04000F4B RID: 3915
	public Material ScreenBG_LeaveRoomAndGroupJoin;

	// Token: 0x04000F4C RID: 3916
	public Material ScreenBG_AbandonPartyAndSoloJoin;

	// Token: 0x04000F4D RID: 3917
	public Material ScreenBG_ChangingGameModeSoloJoin;

	// Token: 0x04000F4E RID: 3918
	public string ScreenText_Error;

	// Token: 0x04000F4F RID: 3919
	public bool showFullErrorMessages;

	// Token: 0x04000F50 RID: 3920
	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	// Token: 0x04000F51 RID: 3921
	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	// Token: 0x04000F52 RID: 3922
	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	// Token: 0x04000F53 RID: 3923
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	// Token: 0x04000F54 RID: 3924
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	// Token: 0x04000F55 RID: 3925
	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	// Token: 0x04000F56 RID: 3926
	public JoinTriggerUITemplate.FormattedString ScreenText_ChangingGameModeSoloJoin;

	// Token: 0x0200020F RID: 527
	[Serializable]
	public struct FormattedString
	{
		// Token: 0x06000C62 RID: 3170 RVA: 0x00042DC4 File Offset: 0x00040FC4
		public string GetText(string oldZone, string newZone, string oldGameType, string newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(new string[]
			{
				oldZone,
				newZone,
				oldGameType,
				newGameType
			});
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00042E01 File Offset: 0x00041001
		public string GetText(Func<string> oldZone, Func<string> newZone, Func<string> oldGameType, Func<string> newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldZone, newZone, oldGameType, newGameType);
		}

		// Token: 0x04000F57 RID: 3927
		[TextArea]
		[SerializeField]
		private string formatText;

		// Token: 0x04000F58 RID: 3928
		[NonSerialized]
		private StringFormatter formatter;
	}
}
