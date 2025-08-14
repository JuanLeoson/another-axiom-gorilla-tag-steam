using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002DA RID: 730
[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent : NetworkView, IPunObservable, IStateAuthorityChanged, IOnPhotonViewOwnerChange, IPhotonViewCallback, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x06001117 RID: 4375 RVA: 0x0006205E File Offset: 0x0006025E
	internal virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.AddToNetwork();
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x0006206C File Offset: 0x0006026C
	internal virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0006207A File Offset: 0x0006027A
	protected override void Start()
	{
		base.Start();
		this.AddToNetwork();
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00062088 File Offset: 0x00060288
	private void AddToNetwork()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00062090 File Offset: 0x00060290
	public override void Spawned()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnSpawned();
		}
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x000620A4 File Offset: 0x000602A4
	public override void FixedUpdateNetwork()
	{
		this.WriteDataFusion();
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x000620AC File Offset: 0x000602AC
	public override void Render()
	{
		if (!base.HasStateAuthority)
		{
			this.ReadDataFusion();
		}
	}

	// Token: 0x0600111E RID: 4382
	public abstract void WriteDataFusion();

	// Token: 0x0600111F RID: 4383
	public abstract void ReadDataFusion();

	// Token: 0x06001120 RID: 4384 RVA: 0x000620BC File Offset: 0x000602BC
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x000620C4 File Offset: 0x000602C4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			this.WriteDataPUN(stream, info);
			return;
		}
		if (stream.IsReading)
		{
			this.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06001122 RID: 4386
	protected abstract void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001123 RID: 4387
	protected abstract void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001124 RID: 4388 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnSpawned()
	{
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x000620E7 File Offset: 0x000602E7
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(newMasterClient));
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x000620FC File Offset: 0x000602FC
	public override void StateAuthorityChanged()
	{
		base.StateAuthorityChanged();
		if (base.Object == null)
		{
			return;
		}
		if (base.Object.StateAuthority == default(PlayerRef))
		{
			return;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
			return;
		}
		this.OnOwnerSwitched(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00062172 File Offset: 0x00060372
	public void OnMasterClientSwitch(NetPlayer newMaster)
	{
		this.StateAuthorityChanged();
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
	{
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600112E RID: 4398 RVA: 0x0006217A File Offset: 0x0006037A
	public bool IsLocallyOwned
	{
		get
		{
			return base.IsMine;
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x0600112F RID: 4399 RVA: 0x00062182 File Offset: 0x00060382
	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001130 RID: 4400 RVA: 0x00062194 File Offset: 0x00060394
	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001131 RID: 4401 RVA: 0x000621A6 File Offset: 0x000603A6
	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x000621C0 File Offset: 0x000603C0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x000621CC File Offset: 0x000603CC
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
