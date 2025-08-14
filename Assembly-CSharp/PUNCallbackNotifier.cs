using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class PUNCallbackNotifier : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x06001298 RID: 4760 RVA: 0x00066C02 File Offset: 0x00064E02
	private void Start()
	{
		this.parentSystem = base.GetComponent<NetworkSystemPUN>();
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x00066C10 File Offset: 0x00064E10
	public override void OnConnectedToMaster()
	{
		this.parentSystem.OnConnectedtoMaster();
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x00066C1D File Offset: 0x00064E1D
	public override void OnJoinedRoom()
	{
		this.parentSystem.OnJoinedRoom();
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00066C2A File Offset: 0x00064E2A
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00066C2A File Offset: 0x00064E2A
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x00066C39 File Offset: 0x00064E39
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnCreateRoomFailed(returnCode, message);
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00066C48 File Offset: 0x00064E48
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.parentSystem.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x00066C56 File Offset: 0x00064E56
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.parentSystem.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00066C64 File Offset: 0x00064E64
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnect callback, cause:" + cause.ToString());
		this.parentSystem.OnDisconnected(cause);
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00066C8E File Offset: 0x00064E8E
	public void OnEvent(EventData photonEvent)
	{
		this.parentSystem.RaiseEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x00066CAD File Offset: 0x00064EAD
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		this.parentSystem.OnMasterClientSwitched(newMasterClient);
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x00066CBB File Offset: 0x00064EBB
	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		base.OnCustomAuthenticationResponse(data);
		NetworkSystem.Instance.CustomAuthenticationResponse(data);
	}

	// Token: 0x04001A4F RID: 6735
	private NetworkSystemPUN parentSystem;
}
