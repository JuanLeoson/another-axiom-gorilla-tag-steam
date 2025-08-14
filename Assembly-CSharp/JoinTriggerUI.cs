using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class JoinTriggerUI : MonoBehaviour
{
	// Token: 0x06000C5A RID: 3162 RVA: 0x00042A43 File Offset: 0x00040C43
	private void Awake()
	{
		this.joinTrigger_isRefResolved = (this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger) && this.joinTrigger != null);
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00042A6D File Offset: 0x00040C6D
	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x00042A7C File Offset: 0x00040C7C
	private void OnEnable()
	{
		if (this.didStart && this._IsValid())
		{
			this.joinTrigger.RegisterUI(this);
		}
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x00042A9A File Offset: 0x00040C9A
	private void OnDisable()
	{
		if (this._IsValid())
		{
			this.joinTrigger.UnregisterUI(this);
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x00042AB0 File Offset: 0x00040CB0
	public void SetState(JoinTriggerVisualState state, Func<string> oldZone, Func<string> newZone, Func<string> oldGameMode, Func<string> newGameMode)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.ChangingGameModeSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_ChangingGameModeSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_ChangingGameModeSoloJoin;
			this.screenText.text = this.template.ScreenText_ChangingGameModeSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x00042D64 File Offset: 0x00040F64
	private bool _IsValid()
	{
		if (!this.joinTrigger_isRefResolved)
		{
			if (this.joinTriggerRef.TargetID == 0)
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` is not assigned so could not resolve. Path=" + base.transform.GetPathQ(), this);
			}
			else
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` could not be resolved. Path=" + base.transform.GetPathQ(), this);
			}
		}
		return this.joinTrigger_isRefResolved;
	}

	// Token: 0x04000F36 RID: 3894
	[SerializeField]
	private XSceneRef joinTriggerRef;

	// Token: 0x04000F37 RID: 3895
	private GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04000F38 RID: 3896
	private bool joinTrigger_isRefResolved;

	// Token: 0x04000F39 RID: 3897
	[SerializeField]
	private MeshRenderer milestoneRenderer;

	// Token: 0x04000F3A RID: 3898
	[SerializeField]
	private MeshRenderer screenBGRenderer;

	// Token: 0x04000F3B RID: 3899
	[SerializeField]
	private TextMeshPro screenText;

	// Token: 0x04000F3C RID: 3900
	[SerializeField]
	private JoinTriggerUITemplate template;

	// Token: 0x04000F3D RID: 3901
	private bool didStart;
}
