using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020009ED RID: 2541
public class FriendCard : MonoBehaviour
{
	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003DFB RID: 15867 RVA: 0x0013A804 File Offset: 0x00138A04
	public TextMeshProUGUI NameText
	{
		get
		{
			return this.nameText;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003DFC RID: 15868 RVA: 0x0013A80C File Offset: 0x00138A0C
	public TextMeshProUGUI RoomText
	{
		get
		{
			return this.roomText;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003DFD RID: 15869 RVA: 0x0013A814 File Offset: 0x00138A14
	public TextMeshProUGUI ZoneText
	{
		get
		{
			return this.zoneText;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003DFE RID: 15870 RVA: 0x0013A81C File Offset: 0x00138A1C
	public float Width
	{
		get
		{
			return this.width;
		}
	}

	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x06003DFF RID: 15871 RVA: 0x0013A824 File Offset: 0x00138A24
	// (set) Token: 0x06003E00 RID: 15872 RVA: 0x0013A82C File Offset: 0x00138A2C
	public float Height { get; private set; } = 0.25f;

	// Token: 0x06003E01 RID: 15873 RVA: 0x0013A835 File Offset: 0x00138A35
	private void Awake()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003E02 RID: 15874 RVA: 0x0013A855 File Offset: 0x00138A55
	private void OnDestroy()
	{
		if (this._button)
		{
			this._button.onPressed -= this.OnButtonPressed;
		}
	}

	// Token: 0x06003E03 RID: 15875 RVA: 0x0013A87B File Offset: 0x00138A7B
	public void Init(FriendDisplay owner)
	{
		this.friendDisplay = owner;
	}

	// Token: 0x06003E04 RID: 15876 RVA: 0x0013A884 File Offset: 0x00138A84
	private void UpdateComponentStates()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(this.canRemove);
		}
		if (this.canRemove)
		{
			this.SetButtonState((this.currentFriend != null) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
			return;
		}
		if (this.joinable)
		{
			this.SetButtonState(FriendDisplay.ButtonState.Active);
			return;
		}
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06003E05 RID: 15877 RVA: 0x0013A8E8 File Offset: 0x00138AE8
	private void SetButtonState(FriendDisplay.ButtonState newState)
	{
		if (this._button == null)
		{
			return;
		}
		if (this._buttonState == newState)
		{
			return;
		}
		this._buttonState = newState;
		MeshRenderer buttonRenderer = this._button.buttonRenderer;
		FriendDisplay.ButtonState buttonState = this._buttonState;
		Material[] sharedMaterials;
		switch (buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			sharedMaterials = this._buttonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			sharedMaterials = this._buttonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			sharedMaterials = this._buttonAlertMaterials;
			break;
		default:
			throw new SwitchExpressionException(buttonState);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
		this._button.delayTime = (float)((this._buttonState == FriendDisplay.ButtonState.Alert) ? 3 : 0);
	}

	// Token: 0x06003E06 RID: 15878 RVA: 0x0013A984 File Offset: 0x00138B84
	public void Populate(FriendBackendController.Friend friend)
	{
		this.SetEmpty();
		if (friend != null && friend.Presence != null)
		{
			if (friend.Presence.UserName != null)
			{
				this.SetName(friend.Presence.UserName.ToUpper());
			}
			if (!string.IsNullOrEmpty(friend.Presence.RoomId) && friend.Presence.RoomId.Length > 0)
			{
				bool? isPublic = friend.Presence.IsPublic;
				bool flag = true;
				bool flag2 = isPublic.GetValueOrDefault() == flag & isPublic != null;
				bool flag3 = friend.Presence.RoomId[0] == '@';
				bool flag4 = friend.Presence.RoomId.Equals(NetworkSystem.Instance.RoomName);
				bool flag5 = false;
				if (!flag4 && flag2 && !friend.Presence.Zone.IsNullOrEmpty())
				{
					string text = friend.Presence.Zone.ToLower();
					foreach (GTZone e in ZoneManagement.instance.activeZones)
					{
						if (text.Contains(e.GetName<GTZone>().ToLower()))
						{
							flag5 = true;
						}
					}
				}
				this.joinable = (!flag3 && !flag4 && (!flag2 || flag5) && this.HasKIDPermissionToJoinPrivateRooms());
				if (flag3)
				{
					this.SetRoom(friend.Presence.RoomId.Substring(1).ToUpper());
					this.SetZone("CUSTOM");
				}
				else if (!flag2)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone("PRIVATE");
				}
				else if (friend.Presence.Zone != null)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone(friend.Presence.Zone.ToUpper());
				}
			}
			else
			{
				this.joinable = false;
				this.SetRoom("OFFLINE");
			}
			this.currentFriend = friend;
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06003E07 RID: 15879 RVA: 0x0013ABA0 File Offset: 0x00138DA0
	public void SetName(string friendName)
	{
		TMP_Text tmp_Text = this.nameText;
		this._friendName = friendName;
		tmp_Text.text = friendName;
	}

	// Token: 0x06003E08 RID: 15880 RVA: 0x0013ABC4 File Offset: 0x00138DC4
	public void SetRoom(string friendRoom)
	{
		TMP_Text tmp_Text = this.roomText;
		this._friendRoom = friendRoom;
		tmp_Text.text = friendRoom;
	}

	// Token: 0x06003E09 RID: 15881 RVA: 0x0013ABE8 File Offset: 0x00138DE8
	public void SetZone(string friendZone)
	{
		TMP_Text tmp_Text = this.zoneText;
		this._friendZone = friendZone;
		tmp_Text.text = friendZone;
	}

	// Token: 0x06003E0A RID: 15882 RVA: 0x0013AC0C File Offset: 0x00138E0C
	public void Randomize()
	{
		this.SetEmpty();
		int num = Random.Range(0, this.randomNames.Length);
		this.SetName(this.randomNames[num].ToUpper());
		this.SetRoom(string.Format("{0}{1}{2}{3}", new object[]
		{
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91)
		}));
		bool flag = Random.Range(0f, 1f) > 0.5f;
		this.joinable = (flag && Random.Range(0f, 1f) > 0.5f);
		if (flag)
		{
			int num2 = Random.Range(0, 17);
			GTZone gtzone = (GTZone)num2;
			this.SetZone(gtzone.ToString().ToUpper());
		}
		else
		{
			this.SetZone(this.privateString);
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06003E0B RID: 15883 RVA: 0x0013AD12 File Offset: 0x00138F12
	public void SetEmpty()
	{
		this.SetName(this.emptyString);
		this.SetRoom(this.emptyString);
		this.SetZone(this.emptyString);
		this.joinable = false;
		this.currentFriend = null;
		this.UpdateComponentStates();
	}

	// Token: 0x06003E0C RID: 15884 RVA: 0x0013AD4C File Offset: 0x00138F4C
	public void SetRemoveEnabled(bool enabled)
	{
		this.canRemove = enabled;
		this.UpdateComponentStates();
	}

	// Token: 0x06003E0D RID: 15885 RVA: 0x0013AD5C File Offset: 0x00138F5C
	private void JoinButtonPressed()
	{
		if (this.joinable && this.currentFriend != null && this.currentFriend.Presence != null)
		{
			bool? isPublic = this.currentFriend.Presence.IsPublic;
			bool flag = true;
			JoinType roomJoinType = (isPublic.GetValueOrDefault() == flag & isPublic != null) ? JoinType.FriendStationPublic : JoinType.FriendStationPrivate;
			GorillaComputer.instance.roomToJoin = this._friendRoom;
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this._friendRoom, roomJoinType);
			this.joinable = false;
			this.UpdateComponentStates();
		}
	}

	// Token: 0x06003E0E RID: 15886 RVA: 0x0013ADE4 File Offset: 0x00138FE4
	private void RemoveFriendButtonPressed()
	{
		if (this.friendDisplay.InRemoveMode)
		{
			FriendSystem.Instance.RemoveFriend(this.currentFriend, null);
			this.SetEmpty();
		}
	}

	// Token: 0x06003E0F RID: 15887 RVA: 0x0013AE0C File Offset: 0x0013900C
	private void OnDrawGizmosSelected()
	{
		float num = this.width * 0.5f * base.transform.lossyScale.x;
		float num2 = this.Height * 0.5f * base.transform.lossyScale.y;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = base.transform.position + base.transform.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = base.transform.position + base.transform.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = base.transform.position + base.transform.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = base.transform.position + base.transform.rotation * new Vector3(num3, -num4, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	// Token: 0x06003E10 RID: 15888 RVA: 0x0013AF40 File Offset: 0x00139140
	public void SetButton(GorillaPressableDelayButton friendCardButton, Material[] normalMaterials, Material[] activeMaterials, Material[] alertMaterials, TextMeshProUGUI buttonText)
	{
		this._button = friendCardButton;
		this._button.SetFillBar(this.removeProgressBar);
		this._button.onPressBegin += this.OnButtonPressBegin;
		this._button.onPressAbort += this.OnButtonPressAbort;
		this._button.onPressed += this.OnButtonPressed;
		this._buttonDefaultMaterials = normalMaterials;
		this._buttonActiveMaterials = activeMaterials;
		this._buttonAlertMaterials = alertMaterials;
		this._buttonText = buttonText;
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06003E11 RID: 15889 RVA: 0x0013AFCF File Offset: 0x001391CF
	private void OnRemoveFriendBegin()
	{
		this.nameText.text = "REMOVING";
		this.roomText.text = "FRIEND";
		this.zoneText.text = this.emptyString;
	}

	// Token: 0x06003E12 RID: 15890 RVA: 0x0013B002 File Offset: 0x00139202
	private void OnRemoveFriendEnd()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x06003E13 RID: 15891 RVA: 0x0013B038 File Offset: 0x00139238
	private void OnButtonPressBegin()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendBegin();
			break;
		default:
			return;
		}
	}

	// Token: 0x06003E14 RID: 15892 RVA: 0x0013B068 File Offset: 0x00139268
	private void OnButtonPressAbort()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendEnd();
			break;
		default:
			return;
		}
	}

	// Token: 0x06003E15 RID: 15893 RVA: 0x0013B098 File Offset: 0x00139298
	private void OnButtonPressed(GorillaPressableButton button, bool isLeftHand)
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			break;
		case FriendDisplay.ButtonState.Active:
			this.JoinButtonPressed();
			return;
		case FriendDisplay.ButtonState.Alert:
			this.RemoveFriendButtonPressed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06003E16 RID: 15894 RVA: 0x0013B0CC File Offset: 0x001392CC
	private bool HasKIDPermissionToJoinPrivateRooms()
	{
		return !KIDManager.KidEnabled || (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer));
	}

	// Token: 0x04004A09 RID: 18953
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x04004A0A RID: 18954
	[SerializeField]
	private TextMeshProUGUI roomText;

	// Token: 0x04004A0B RID: 18955
	[SerializeField]
	private TextMeshProUGUI zoneText;

	// Token: 0x04004A0C RID: 18956
	[SerializeField]
	private Transform removeProgressBar;

	// Token: 0x04004A0D RID: 18957
	[SerializeField]
	private float width = 0.25f;

	// Token: 0x04004A0F RID: 18959
	private string emptyString = "";

	// Token: 0x04004A10 RID: 18960
	private string privateString = "PRIVATE";

	// Token: 0x04004A11 RID: 18961
	private bool joinable;

	// Token: 0x04004A12 RID: 18962
	private bool canRemove;

	// Token: 0x04004A13 RID: 18963
	private GorillaPressableDelayButton _button;

	// Token: 0x04004A14 RID: 18964
	private TextMeshProUGUI _buttonText;

	// Token: 0x04004A15 RID: 18965
	private string _friendName;

	// Token: 0x04004A16 RID: 18966
	private string _friendRoom;

	// Token: 0x04004A17 RID: 18967
	private string _friendZone;

	// Token: 0x04004A18 RID: 18968
	private FriendBackendController.Friend currentFriend;

	// Token: 0x04004A19 RID: 18969
	private FriendDisplay friendDisplay;

	// Token: 0x04004A1A RID: 18970
	private string[] randomNames = new string[]
	{
		"Veronica",
		"Roman",
		"Janiyah",
		"Dalton",
		"Bellamy",
		"Eithan",
		"Celeste",
		"Isaac",
		"Astrid",
		"Azariah",
		"Keilani",
		"Zeke",
		"Jayleen",
		"Yosef",
		"Jaylee",
		"Bodie",
		"Greta",
		"Cain",
		"Ella",
		"Everly",
		"Finnley",
		"Paisley",
		"Kaison",
		"Luna",
		"Nina",
		"Maison",
		"Monroe",
		"Ricardo",
		"Zariyah",
		"Travis",
		"Lacey",
		"Elian",
		"Frankie",
		"Otis",
		"Adele",
		"Edison",
		"Amira",
		"Ivan",
		"Raelynn",
		"Eliel",
		"Aliana",
		"Beckett",
		"Mylah",
		"Melvin",
		"Magdalena",
		"Leroy",
		"Madeleine"
	};

	// Token: 0x04004A1B RID: 18971
	private FriendDisplay.ButtonState _buttonState = (FriendDisplay.ButtonState)(-1);

	// Token: 0x04004A1C RID: 18972
	private Material[] _buttonDefaultMaterials;

	// Token: 0x04004A1D RID: 18973
	private Material[] _buttonActiveMaterials;

	// Token: 0x04004A1E RID: 18974
	private Material[] _buttonAlertMaterials;
}
