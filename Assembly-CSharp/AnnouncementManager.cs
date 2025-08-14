using System;
using GorillaNetworking;
using LitJson;
using PlayFab;
using UnityEngine;

// Token: 0x0200089E RID: 2206
public class AnnouncementManager : MonoBehaviour
{
	// Token: 0x06003793 RID: 14227 RVA: 0x00120442 File Offset: 0x0011E642
	public bool ShowAnnouncement()
	{
		return this._showAnnouncement;
	}

	// Token: 0x1700054F RID: 1359
	// (get) Token: 0x06003794 RID: 14228 RVA: 0x0012044A File Offset: 0x0011E64A
	// (set) Token: 0x06003795 RID: 14229 RVA: 0x00120452 File Offset: 0x0011E652
	public bool _completedSetup { get; private set; }

	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x06003796 RID: 14230 RVA: 0x0012045B File Offset: 0x0011E65B
	// (set) Token: 0x06003797 RID: 14231 RVA: 0x00120463 File Offset: 0x0011E663
	public bool _announcementActive { get; private set; }

	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x06003798 RID: 14232 RVA: 0x0012046C File Offset: 0x0011E66C
	public static AnnouncementManager Instance
	{
		get
		{
			if (AnnouncementManager._instance == null)
			{
				Debug.LogError("[KID::ANNOUNCEMENT] [_instance] is NULL, does it exist in the scene?");
			}
			return AnnouncementManager._instance;
		}
	}

	// Token: 0x17000552 RID: 1362
	// (get) Token: 0x06003799 RID: 14233 RVA: 0x0012048A File Offset: 0x0011E68A
	private static string AnnouncementDPlayerPref
	{
		get
		{
			if (string.IsNullOrEmpty(AnnouncementManager._announcementIDPref))
			{
				AnnouncementManager._announcementIDPref = "announcement-id-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return AnnouncementManager._announcementIDPref;
		}
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x001204B8 File Offset: 0x0011E6B8
	private void Awake()
	{
		if (AnnouncementManager._instance != null)
		{
			Debug.LogError("[KID::ANNOUNCEMENT] [AnnouncementManager] has already been setup, does another already exist in the scene?");
			return;
		}
		AnnouncementManager._instance = this;
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Announcement Message Box has not been set. Announcement system will not work without it");
		}
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x001204F0 File Offset: 0x0011E6F0
	private void Start()
	{
		if (this._announcementMessageBox == null)
		{
			return;
		}
		this._announcementMessageBox.RightButton = "";
		this._announcementMessageBox.LeftButton = "Continue";
		PlayFabTitleDataCache.Instance.GetTitleData("AnnouncementData", new Action<string>(this.ConfigureAnnouncement), new Action<PlayFabError>(this.OnError));
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x00120554 File Offset: 0x0011E754
	public void OnContinuePressed()
	{
		HandRayController.Instance.DisableHandRays();
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Message Box is null, Continue Button cannot work");
			return;
		}
		PrivateUIRoom.RemoveUI(this._announcementMessageBox.transform);
		this._announcementActive = false;
		PlayerPrefs.SetString(AnnouncementManager.AnnouncementDPlayerPref, this._announcementData.AnnouncementID);
		PlayerPrefs.Save();
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x001205B5 File Offset: 0x0011E7B5
	private void OnError(PlayFabError error)
	{
		Debug.LogError("[ANNOUNCEMENT] Failed to Get Title Data for key [AnnouncementData]. Error:\n[" + error.ErrorMessage);
		this._completedSetup = true;
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x001205D4 File Offset: 0x0011E7D4
	private void ConfigureAnnouncement(string data)
	{
		this._announcementString = data;
		this._announcementData = JsonMapper.ToObject<SAnnouncementData>(this._announcementString);
		if (!bool.TryParse(this._announcementData.ShowAnnouncement, out this._showAnnouncement))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Failed to parse [ShowAnnouncement] with value [" + this._announcementData.ShowAnnouncement + "] to a bool, assuming false");
			return;
		}
		if (!this.ShowAnnouncement())
		{
			this._completedSetup = true;
			return;
		}
		if (string.IsNullOrEmpty(this._announcementData.AnnouncementID))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Announcement Version is empty or null. Will not show announcement");
			return;
		}
		string @string = PlayerPrefs.GetString(AnnouncementManager.AnnouncementDPlayerPref, "");
		if (this._announcementData.AnnouncementID == @string)
		{
			this._completedSetup = true;
			return;
		}
		PrivateUIRoom.ForceStartOverlay();
		HandRayController.Instance.EnableHandRays();
		this._announcementMessageBox.Header = this._announcementData.AnnouncementTitle;
		this._announcementMessageBox.Body = this._announcementData.Message;
		this._announcementActive = true;
		PrivateUIRoom.AddUI(this._announcementMessageBox.transform);
		this._completedSetup = true;
	}

	// Token: 0x0400442A RID: 17450
	private const string ANNOUNCEMENT_ID_PLAYERPREF_PREFIX = "announcement-id-";

	// Token: 0x0400442B RID: 17451
	private const string ANNOUNCEMENT_TITLE_DATA_KEY = "AnnouncementData";

	// Token: 0x0400442C RID: 17452
	private const string ANNOUNCEMENT_HEADING = "Announcement!";

	// Token: 0x0400442D RID: 17453
	private const string ANNOUNCEMENT_BUTTON_TEXT = "Continue";

	// Token: 0x0400442E RID: 17454
	[SerializeField]
	private MessageBox _announcementMessageBox;

	// Token: 0x0400442F RID: 17455
	private string _announcementString = string.Empty;

	// Token: 0x04004430 RID: 17456
	private SAnnouncementData _announcementData;

	// Token: 0x04004431 RID: 17457
	private bool _showAnnouncement;

	// Token: 0x04004434 RID: 17460
	private static AnnouncementManager _instance;

	// Token: 0x04004435 RID: 17461
	private static string _announcementIDPref = "";
}
