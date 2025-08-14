using System;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020009F3 RID: 2547
public class FriendingStation : MonoBehaviour
{
	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x06003E52 RID: 15954 RVA: 0x0013D226 File Offset: 0x0013B426
	public TextMeshProUGUI Player1Text
	{
		get
		{
			return this.player1Text;
		}
	}

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x06003E53 RID: 15955 RVA: 0x0013D22E File Offset: 0x0013B42E
	public TextMeshProUGUI Player2Text
	{
		get
		{
			return this.player2Text;
		}
	}

	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x06003E54 RID: 15956 RVA: 0x0013D236 File Offset: 0x0013B436
	public TextMeshProUGUI StatusText
	{
		get
		{
			return this.statusText;
		}
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x06003E55 RID: 15957 RVA: 0x0013D23E File Offset: 0x0013B43E
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x06003E56 RID: 15958 RVA: 0x0013D246 File Offset: 0x0013B446
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x0013D278 File Offset: 0x0013B478
	private void OnEnable()
	{
		FriendingManager.Instance.RegisterFriendingStation(this);
		if (PhotonNetwork.InRoom)
		{
			this.displayedData.actorNumberA = -1;
			this.displayedData.actorNumberB = -1;
			this.displayedData.state = FriendingManager.FriendStationState.WaitingForPlayers;
		}
		else
		{
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
		}
		this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
		this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
		this.UpdateDisplayedState(this.displayedData.state);
	}

	// Token: 0x06003E58 RID: 15960 RVA: 0x0013D324 File Offset: 0x0013B524
	private void OnDisable()
	{
		FriendingManager.Instance.UnregisterFriendingStation(this);
	}

	// Token: 0x06003E59 RID: 15961 RVA: 0x0013D334 File Offset: 0x0013B534
	private void UpdatePlayerText(TextMeshProUGUI playerText, int playerId)
	{
		if (playerId == -2)
		{
			playerText.text = "";
			return;
		}
		if (playerId == -1)
		{
			playerText.text = "PLAYER:\nNONE";
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerId);
		if (netPlayerByID != null)
		{
			playerText.text = "PLAYER:\n" + netPlayerByID.SanitizedNickName;
			return;
		}
		playerText.text = "PLAYER:\nNONE";
	}

	// Token: 0x06003E5A RID: 15962 RVA: 0x0013D394 File Offset: 0x0013B594
	private void UpdateDisplayedState(FriendingManager.FriendStationState state)
	{
		switch (state)
		{
		case FriendingManager.FriendStationState.NotInRoom:
			this.statusText.text = "JOIN A ROOM TO USE";
			return;
		case FriendingManager.FriendStationState.WaitingForPlayers:
			this.statusText.text = "";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusBoth:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonBoth:
			this.statusText.text = "PRESS [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerA:
			this.statusText.text = "PRESS [       ] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerB:
			this.statusText.text = "READY [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer0:
			this.statusText.text = "READY [       ] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer1:
			this.statusText.text = "READY [-     -] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer2:
			this.statusText.text = "READY [--   --] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer3:
			this.statusText.text = "READY [--- ---] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer4:
			this.statusText.text = "READY [-------] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestBoth:
			this.statusText.text = " SENT [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerA:
			this.statusText.text = " SENT [-------] DONE ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerB:
			this.statusText.text = " DONE [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.RequestFailed:
			this.statusText.text = "FRIEND REQUEST FAILED";
			return;
		case FriendingManager.FriendStationState.Friends:
			this.statusText.text = "\\O/ FRIENDS \\O/";
			return;
		case FriendingManager.FriendStationState.AlreadyFriends:
			this.statusText.text = "ALREADY FRIENDS";
			return;
		default:
			return;
		}
	}

	// Token: 0x06003E5B RID: 15963 RVA: 0x0013D538 File Offset: 0x0013B738
	private void UpdateAddFriendButton()
	{
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
		{
			this.addFriendButton.isOn = true;
		}
		else
		{
			this.addFriendButton.isOn = false;
		}
		this.addFriendButton.UpdateColor();
	}

	// Token: 0x06003E5C RID: 15964 RVA: 0x0013D5D8 File Offset: 0x0013B7D8
	private void UpdateDisplay(ref FriendingManager.FriendStationData data)
	{
		if (this.displayedData.actorNumberA != data.actorNumberA)
		{
			this.UpdatePlayerText(this.player1Text, data.actorNumberA);
		}
		if (this.displayedData.actorNumberB != data.actorNumberB)
		{
			this.UpdatePlayerText(this.player2Text, data.actorNumberB);
		}
		if (this.displayedData.state != data.state)
		{
			this.UpdateDisplayedState(data.state);
		}
		this.displayedData = data;
		this.UpdateAddFriendButton();
	}

	// Token: 0x06003E5D RID: 15965 RVA: 0x0013D660 File Offset: 0x0013B860
	public void UpdateState(FriendingManager.FriendStationData data)
	{
		this.UpdateDisplay(ref data);
	}

	// Token: 0x06003E5E RID: 15966 RVA: 0x0013D66C File Offset: 0x0013B86C
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null && component.OwningNetPlayer != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerEnteredStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x0013D744 File Offset: 0x0013B944
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerExitedStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x0013D814 File Offset: 0x0013BA14
	public void FriendButtonPressed()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (!this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonPressedRPC", RpcTarget.MasterClient, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if (this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonBoth || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB))
			{
				this.addFriendButton.isOn = true;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x0013D8E8 File Offset: 0x0013BAE8
	public void FriendButtonReleased()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonUnpressedRPC", RpcTarget.MasterClient, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
			{
				this.addFriendButton.isOn = false;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x04004A55 RID: 19029
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04004A56 RID: 19030
	[SerializeField]
	private TextMeshProUGUI player1Text;

	// Token: 0x04004A57 RID: 19031
	[SerializeField]
	private TextMeshProUGUI player2Text;

	// Token: 0x04004A58 RID: 19032
	[SerializeField]
	private TextMeshProUGUI statusText;

	// Token: 0x04004A59 RID: 19033
	[SerializeField]
	private GTZone zone;

	// Token: 0x04004A5A RID: 19034
	[SerializeField]
	private GorillaPressableButton addFriendButton;

	// Token: 0x04004A5B RID: 19035
	private FriendingManager.FriendStationData displayedData;
}
