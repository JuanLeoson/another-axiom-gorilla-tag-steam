using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Oculus.Platform;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// Token: 0x020003C4 RID: 964
public class GorillaMetaReport : MonoBehaviour
{
	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001647 RID: 5703 RVA: 0x000790C9 File Offset: 0x000772C9
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x000790D0 File Offset: 0x000772D0
	private void Start()
	{
		this.localPlayer.inOverlay = false;
		MothershipClientApiUnity.OnMessageNotificationSocket += this.OnNotification;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x000790FB File Offset: 0x000772FB
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00079118 File Offset: 0x00077318
	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Unhandled);
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.ReportText.SetActive(true);
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
			return;
		}
		if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
		}
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00079170 File Offset: 0x00077370
	private void OnNotification(NotificationsMessageResponse notification, [NativeInteger] IntPtr _)
	{
		string title = notification.Title;
		if (title == "Warning")
		{
			this.OnWarning(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Warning");
			return;
		}
		if (title == "Mute")
		{
			this.OnMuteSanction(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Mute");
			return;
		}
		if (!(title == "Unmute"))
		{
			return;
		}
		if (GorillaTagger.hasInstance)
		{
			GorillaTagger.moderationMutedTime = -1f;
		}
		GorillaTelemetry.PostNotificationEvent("Unmute");
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x000791F8 File Offset: 0x000773F8
	private void OnWarning(string warningNotification)
	{
		string[] array = warningNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 2)
		{
			Debug.LogError("Invalid warning notification");
			return;
		}
		string text = array[0];
		string[] array2 = array[1].Split(',', StringSplitOptions.None);
		if (array2.Length == 0)
		{
			Debug.LogError("Missing warning notification reasons");
			return;
		}
		string text2 = GorillaMetaReport.FormatListToString(array2);
		this.ReportText.GetComponent<Text>().text = text.ToUpper() + " WARNING FOR " + text2.ToUpper() + "\nNEXT COMES MUTE";
		this.StartOverlay(true);
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x0007927C File Offset: 0x0007747C
	private void OnMuteSanction(string muteNotification)
	{
		string[] array = muteNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 3)
		{
			Debug.LogError("Invalid mute notification");
			return;
		}
		if (!array[0].Equals("voice", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		int num;
		if (array[2].Length > 0 && int.TryParse(array[2], out num))
		{
			int num2 = num / 60;
			this.ReportText.GetComponent<Text>().text = string.Format("MUTED FOR {0} MINUTES\nBAD MONKE", num2);
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = (float)num;
			}
		}
		else
		{
			this.ReportText.GetComponent<Text>().text = "MUTED FOREVER";
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = float.PositiveInfinity;
			}
		}
		this.StartOverlay(true);
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x00079330 File Offset: 0x00077530
	private static string FormatListToString(in string[] list)
	{
		int num = list.Length;
		string result;
		if (num != 1)
		{
			if (num != 2)
			{
				string str = RuntimeHelpers.GetSubArray<string>(list, Range.EndAt(new Index(1, true))).Join(", ");
				string str2 = ", AND ";
				string[] array = list;
				result = str + str2 + array[array.Length - 1];
			}
			else
			{
				result = list[0] + " AND " + list[1];
			}
		}
		else
		{
			result = list[0];
		}
		return result;
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x00079399 File Offset: 0x00077599
	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x000793A8 File Offset: 0x000775A8
	private void DuplicateScoreboard()
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		Vector3 position;
		Quaternion rotation;
		Vector3 vector;
		this.GetIdealScreenPositionRotation(out position, out rotation, out vector);
		this.currentScoreboard.transform.SetPositionAndRotation(position, rotation);
		this.reportScoreboard.transform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x00079414 File Offset: 0x00077614
	private void ToggleLevelVisibility(bool state)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (state)
		{
			component.cullingMask = this.savedCullingLayers;
			return;
		}
		this.savedCullingLayers = component.cullingMask;
		component.cullingMask = this.visibleLayers;
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00079460 File Offset: 0x00077660
	private void Teardown()
	{
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		base.gameObject.SetActive(false);
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			gorillaPlayerScoreboardLine.doneReporting = false;
		}
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x0007953C File Offset: 0x0007773C
	private void CheckReportSubmit()
	{
		if (this.currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			if (gorillaPlayerScoreboardLine.doneReporting)
			{
				this.ReportText.SetActive(true);
				this.ReportText.GetComponent<Text>().text = "REPORTED " + gorillaPlayerScoreboardLine.playerNameVisible;
				this.currentScoreboard.gameObject.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x000795F0 File Offset: 0x000777F0
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * this.playerLocalScreenPosition * scale.x;
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x0007967C File Offset: 0x0007787C
	private void StartOverlay(bool isSanction = false)
	{
		Vector3 position;
		Quaternion rotation;
		Vector3 vector;
		this.GetIdealScreenPositionRotation(out position, out rotation, out vector);
		this.currentScoreboard.transform.localScale = vector * 2f;
		this.reportScoreboard.transform.localScale = vector;
		this.leftHandObject.transform.localScale = vector;
		this.rightHandObject.transform.localScale = vector;
		this.occluder.transform.localScale = vector;
		if (this.localPlayer.InReportMenu && !PhotonNetwork.InRoom)
		{
			return;
		}
		this.localPlayer.InReportMenu = true;
		this.localPlayer.disableMovement = true;
		this.localPlayer.inOverlay = true;
		base.gameObject.SetActive(true);
		if (PhotonNetwork.InRoom && !isSanction)
		{
			this.DuplicateScoreboard();
		}
		else
		{
			this.ReportText.SetActive(true);
			this.reportScoreboard.transform.SetPositionAndRotation(position, rotation);
			this.currentScoreboard.transform.SetPositionAndRotation(position, rotation);
		}
		this.ToggleLevelVisibility(false);
		this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
		if (isSanction)
		{
			this.currentScoreboard.gameObject.SetActive(false);
			return;
		}
		this.currentScoreboard.gameObject.SetActive(true);
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00079808 File Offset: 0x00077A08
	private void CheckDistance()
	{
		Vector3 b;
		Quaternion b2;
		Vector3 vector;
		this.GetIdealScreenPositionRotation(out b, out b2, out vector);
		float num = Vector3.Distance(this.reportScoreboard.transform.position, b);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = this.movementTime;
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, b, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, b2, num3));
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, b, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, b2, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x00079928 File Offset: 0x00077B28
	private void Update()
	{
		if (this.blockButtonsUntilTimestamp > Time.time)
		{
			return;
		}
		if (SteamVR_Actions.gorillaTag_System.GetState(SteamVR_Input_Sources.LeftHand) && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			this.blockButtonsUntilTimestamp = Time.time + 0.75f;
		}
		if (this.localPlayer.InReportMenu)
		{
			this.localPlayer.inOverlay = true;
			this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
			this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
			this.CheckDistance();
			this.CheckReportSubmit();
		}
		if (this.closeButton.selected)
		{
			this.Teardown();
		}
		if (this.testPress)
		{
			this.testPress = false;
			this.StartOverlay(false);
		}
	}

	// Token: 0x04001E03 RID: 7683
	[SerializeField]
	private GameObject occluder;

	// Token: 0x04001E04 RID: 7684
	[SerializeField]
	private GameObject reportScoreboard;

	// Token: 0x04001E05 RID: 7685
	[SerializeField]
	private GameObject ReportText;

	// Token: 0x04001E06 RID: 7686
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x04001E07 RID: 7687
	[SerializeField]
	private GorillaReportButton closeButton;

	// Token: 0x04001E08 RID: 7688
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04001E09 RID: 7689
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04001E0A RID: 7690
	[SerializeField]
	private Vector3 playerLocalScreenPosition;

	// Token: 0x04001E0B RID: 7691
	private float blockButtonsUntilTimestamp;

	// Token: 0x04001E0C RID: 7692
	[SerializeField]
	private GorillaScoreBoard currentScoreboard;

	// Token: 0x04001E0D RID: 7693
	private int savedCullingLayers;

	// Token: 0x04001E0E RID: 7694
	public bool testPress;

	// Token: 0x04001E0F RID: 7695
	public bool isMoving;

	// Token: 0x04001E10 RID: 7696
	private float movementTime;
}
