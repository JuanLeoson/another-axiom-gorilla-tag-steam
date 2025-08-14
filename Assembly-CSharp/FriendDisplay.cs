using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009EE RID: 2542
public class FriendDisplay : MonoBehaviour
{
	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x06003E18 RID: 15896 RVA: 0x0013B2D9 File Offset: 0x001394D9
	public bool InRemoveMode
	{
		get
		{
			return this.inRemoveMode;
		}
	}

	// Token: 0x06003E19 RID: 15897 RVA: 0x0013B2E4 File Offset: 0x001394E4
	private void Start()
	{
		this.InitFriendCards();
		this.InitLocalPlayerCard();
		this.UpdateLocalPlayerPrivacyButtons();
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
	}

	// Token: 0x06003E1A RID: 15898 RVA: 0x0013B354 File Offset: 0x00139554
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnJoinedRoom;
		}
		if (this.triggerNotifier != null)
		{
			this.triggerNotifier.TriggerEnterEvent -= this.TriggerEntered;
			this.triggerNotifier.TriggerExitEvent -= this.TriggerExited;
		}
	}

	// Token: 0x06003E1B RID: 15899 RVA: 0x0013B3CC File Offset: 0x001395CC
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh += this.OnGetFriendsReceived;
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
			this.localPlayerAtDisplay = true;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x06003E1C RID: 15900 RVA: 0x0013B42C File Offset: 0x0013962C
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh -= this.OnGetFriendsReceived;
			this.ClearFriendCards();
			this.ClearLocalPlayerCard();
			this.localPlayerAtDisplay = false;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x06003E1D RID: 15901 RVA: 0x0013B484 File Offset: 0x00139684
	private void OnJoinedRoom()
	{
		this.Refresh();
	}

	// Token: 0x06003E1E RID: 15902 RVA: 0x0013B48C File Offset: 0x0013968C
	private void Refresh()
	{
		if (this.localPlayerAtDisplay)
		{
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
		}
	}

	// Token: 0x06003E1F RID: 15903 RVA: 0x0013B4A8 File Offset: 0x001396A8
	public void LocalPlayerFullyVisiblePress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Visible);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06003E20 RID: 15904 RVA: 0x0013B4C3 File Offset: 0x001396C3
	public void LocalPlayerPublicOnlyPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.PublicOnly);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06003E21 RID: 15905 RVA: 0x0013B4DE File Offset: 0x001396DE
	public void LocalPlayerFullyHiddenPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Hidden);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06003E22 RID: 15906 RVA: 0x0013B4FC File Offset: 0x001396FC
	private void UpdateLocalPlayerPrivacyButtons()
	{
		FriendSystem.PlayerPrivacy localPlayerPrivacy = FriendSystem.Instance.LocalPlayerPrivacy;
		this.SetButtonAppearance(this._localPlayerFullyVisibleButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Visible);
		this.SetButtonAppearance(this._localPlayerPublicOnlyButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly);
		this.SetButtonAppearance(this._localPlayerFullyHiddenButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden);
	}

	// Token: 0x06003E23 RID: 15907 RVA: 0x0013B546 File Offset: 0x00139746
	private void SetButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06003E24 RID: 15908 RVA: 0x0013B558 File Offset: 0x00139758
	private void SetButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		Material[] sharedMaterials;
		switch (state)
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
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06003E25 RID: 15909 RVA: 0x0013B5B0 File Offset: 0x001397B0
	public void ToggleRemoveFriendMode()
	{
		this.inRemoveMode = !this.inRemoveMode;
		FriendCard[] array = this.friendCards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRemoveEnabled(this.inRemoveMode);
		}
		this.SetButtonAppearance(this._removeFriendButton, this.inRemoveMode ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06003E26 RID: 15910 RVA: 0x0013B608 File Offset: 0x00139808
	private void InitFriendCards()
	{
		float num = this.gridWidth / (float)this.gridDimension;
		float num2 = this.gridHeight / (float)this.gridDimension;
		Vector3 right = this.gridRoot.right;
		Vector3 a = -this.gridRoot.up;
		Vector3 a2 = this.gridRoot.position - right * (this.gridWidth * 0.5f - num * 0.5f) - a * (this.gridHeight * 0.5f - num2 * 0.5f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.gridDimension; i++)
		{
			for (int j = 0; j < this.gridDimension; j++)
			{
				FriendCard friendCard = this.friendCards[num4];
				friendCard.gameObject.SetActive(true);
				friendCard.transform.localScale = Vector3.one * (num / friendCard.Width);
				friendCard.transform.position = a2 + right * num * (float)j + a * num2 * (float)i;
				friendCard.transform.rotation = this.gridRoot.transform.rotation;
				friendCard.Init(this);
				friendCard.SetButton(this._friendCardButtons[num3++], this._buttonDefaultMaterials, this._buttonActiveMaterials, this._buttonAlertMaterials, this._friendCardButtonText[num4]);
				friendCard.SetEmpty();
				num4++;
			}
		}
	}

	// Token: 0x06003E27 RID: 15911 RVA: 0x0013B7A8 File Offset: 0x001399A8
	public void RandomizeFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].Randomize();
		}
	}

	// Token: 0x06003E28 RID: 15912 RVA: 0x0013B7D8 File Offset: 0x001399D8
	private void ClearFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
	}

	// Token: 0x06003E29 RID: 15913 RVA: 0x0013B805 File Offset: 0x00139A05
	public void OnGetFriendsReceived(List<FriendBackendController.Friend> friendsList)
	{
		this.PopulateFriendCards(friendsList);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06003E2A RID: 15914 RVA: 0x0013B81C File Offset: 0x00139A1C
	private void PopulateFriendCards(List<FriendBackendController.Friend> friendsList)
	{
		int num = 0;
		while (num < friendsList.Count && friendsList[num] != null)
		{
			this.friendCards[num].Populate(friendsList[num]);
			num++;
		}
	}

	// Token: 0x06003E2B RID: 15915 RVA: 0x0013B857 File Offset: 0x00139A57
	private void InitLocalPlayerCard()
	{
		this._localPlayerCard.Init(this);
		this.ClearLocalPlayerCard();
	}

	// Token: 0x06003E2C RID: 15916 RVA: 0x0013B86C File Offset: 0x00139A6C
	private void PopulateLocalPlayerCard()
	{
		string zone = PhotonNetworkController.Instance.CurrentRoomZone.GetName<GTZone>().ToUpper();
		this._localPlayerCard.SetName(NetworkSystem.Instance.LocalPlayer.NickName.ToUpper());
		if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.RoomName) || NetworkSystem.Instance.RoomName.Length <= 0)
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		bool flag = NetworkSystem.Instance.RoomName[0] == '@';
		bool flag2 = !NetworkSystem.Instance.SessionIsPrivate;
		if (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden || (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly && !flag2))
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		if (flag)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.Substring(1).ToUpper());
			this._localPlayerCard.SetZone("CUSTOM");
			return;
		}
		if (!flag2)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
			this._localPlayerCard.SetZone("PRIVATE");
			return;
		}
		this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
		this._localPlayerCard.SetZone(zone);
	}

	// Token: 0x06003E2D RID: 15917 RVA: 0x0013B9F9 File Offset: 0x00139BF9
	private void ClearLocalPlayerCard()
	{
		this._localPlayerCard.SetEmpty();
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x0013BA08 File Offset: 0x00139C08
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		float num = this.gridWidth * 0.5f;
		float num2 = this.gridHeight * 0.5f;
		float num3 = num;
		float num4 = num2;
		Vector3 a = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector2 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 b = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, -num4, 0f);
		for (int i = 0; i <= this.gridDimension; i++)
		{
			float t = (float)i / (float)this.gridDimension;
			Vector3 from = Vector3.Lerp(a, vector, t);
			Vector3 to = Vector3.Lerp(vector2, b, t);
			Gizmos.DrawLine(from, to);
			Vector3 from2 = Vector3.Lerp(a, vector2, t);
			Vector3 to2 = Vector3.Lerp(vector, b, t);
			Gizmos.DrawLine(from2, to2);
		}
	}

	// Token: 0x04004A1F RID: 18975
	[FormerlySerializedAs("gridCenter")]
	[SerializeField]
	private FriendCard[] friendCards = new FriendCard[9];

	// Token: 0x04004A20 RID: 18976
	[SerializeField]
	private Transform gridRoot;

	// Token: 0x04004A21 RID: 18977
	[SerializeField]
	private float gridWidth = 2f;

	// Token: 0x04004A22 RID: 18978
	[SerializeField]
	private float gridHeight = 1f;

	// Token: 0x04004A23 RID: 18979
	[SerializeField]
	private int gridDimension = 3;

	// Token: 0x04004A24 RID: 18980
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04004A25 RID: 18981
	[FormerlySerializedAs("_joinButtons")]
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableDelayButton[] _friendCardButtons;

	// Token: 0x04004A26 RID: 18982
	[SerializeField]
	private TextMeshProUGUI[] _friendCardButtonText;

	// Token: 0x04004A27 RID: 18983
	[SerializeField]
	private MeshRenderer _localPlayerFullyVisibleButton;

	// Token: 0x04004A28 RID: 18984
	[SerializeField]
	private MeshRenderer _localPlayerPublicOnlyButton;

	// Token: 0x04004A29 RID: 18985
	[SerializeField]
	private MeshRenderer _localPlayerFullyHiddenButton;

	// Token: 0x04004A2A RID: 18986
	[SerializeField]
	private MeshRenderer _removeFriendButton;

	// Token: 0x04004A2B RID: 18987
	[SerializeField]
	private FriendCard _localPlayerCard;

	// Token: 0x04004A2C RID: 18988
	[SerializeField]
	private Material[] _buttonDefaultMaterials;

	// Token: 0x04004A2D RID: 18989
	[SerializeField]
	private Material[] _buttonActiveMaterials;

	// Token: 0x04004A2E RID: 18990
	[SerializeField]
	private Material[] _buttonAlertMaterials;

	// Token: 0x04004A2F RID: 18991
	private MeshRenderer[] _joinButtonRenderers;

	// Token: 0x04004A30 RID: 18992
	private bool inRemoveMode;

	// Token: 0x04004A31 RID: 18993
	private bool localPlayerAtDisplay;

	// Token: 0x020009EF RID: 2543
	public enum ButtonState
	{
		// Token: 0x04004A33 RID: 18995
		Default,
		// Token: 0x04004A34 RID: 18996
		Active,
		// Token: 0x04004A35 RID: 18997
		Alert
	}
}
