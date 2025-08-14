using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002EE RID: 750
[RequireComponent(typeof(PhotonView), typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public class NetworkView : NetworkBehaviour, IStateAuthorityChanged, IPunOwnershipCallbacks
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x060011EB RID: 4587 RVA: 0x0006301D File Offset: 0x0006121D
	public bool IsMine
	{
		get
		{
			return this.punView != null && this.punView.IsMine;
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x060011EC RID: 4588 RVA: 0x0006303A File Offset: 0x0006123A
	public bool IsValid
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x060011ED RID: 4589 RVA: 0x0006303A File Offset: 0x0006123A
	public bool HasView
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x060011EE RID: 4590 RVA: 0x00063048 File Offset: 0x00061248
	public bool IsRoomView
	{
		get
		{
			return this.punView.IsRoomView;
		}
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x060011EF RID: 4591 RVA: 0x00063055 File Offset: 0x00061255
	public PhotonView GetView
	{
		get
		{
			return this.punView;
		}
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x060011F0 RID: 4592 RVA: 0x0006305D File Offset: 0x0006125D
	public NetPlayer Owner
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.punView.Owner);
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x060011F1 RID: 4593 RVA: 0x00063074 File Offset: 0x00061274
	public int ViewID
	{
		get
		{
			return this.punView.ViewID;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00063081 File Offset: 0x00061281
	// (set) Token: 0x060011F3 RID: 4595 RVA: 0x0006308E File Offset: 0x0006128E
	internal OwnershipOption OwnershipTransfer
	{
		get
		{
			return this.punView.OwnershipTransfer;
		}
		set
		{
			this.punView.OwnershipTransfer = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnershipTransfer = value;
			}
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x060011F4 RID: 4596 RVA: 0x000630B6 File Offset: 0x000612B6
	// (set) Token: 0x060011F5 RID: 4597 RVA: 0x000630C3 File Offset: 0x000612C3
	public int OwnerActorNr
	{
		get
		{
			return this.punView.OwnerActorNr;
		}
		set
		{
			this.punView.OwnerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnerActorNr = value;
			}
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060011F6 RID: 4598 RVA: 0x000630EB File Offset: 0x000612EB
	// (set) Token: 0x060011F7 RID: 4599 RVA: 0x000630F8 File Offset: 0x000612F8
	public int ControllerActorNr
	{
		get
		{
			return this.punView.ControllerActorNr;
		}
		set
		{
			this.punView.ControllerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.ControllerActorNr = value;
			}
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x00063120 File Offset: 0x00061320
	private void GetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		if (components.Length > 1)
		{
			if (components[0].Synchronization == ViewSynchronization.UnreliableOnChange)
			{
				this.punView = components[0];
				this.reliableView = components[1];
			}
			else if (components[0].Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				this.reliableView = components[0];
				this.punView = components[1];
			}
		}
		else
		{
			this.punView = components[0];
		}
		if (this.punView == null)
		{
			this.punView = base.GetComponent<PhotonView>();
		}
		if (this.fusionView == null)
		{
			this.fusionView = base.GetComponent<NetworkObject>();
		}
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000631B5 File Offset: 0x000613B5
	protected virtual void Awake()
	{
		this.GetViews();
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x000631BD File Offset: 0x000613BD
	protected virtual void Start()
	{
		if (this._sceneObject)
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		}
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x000631D8 File Offset: 0x000613D8
	public void SendRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Player playerRef = (targetPlayer as PunNetPlayer).PlayerRef;
		this.punView.RPC(method, playerRef, parameters);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x000631FF File Offset: 0x000613FF
	public void SendRPC(string method, RpcTarget target, params object[] parameters)
	{
		this.punView.RPC(method, target, parameters);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x00063210 File Offset: 0x00061410
	public void SendRPC(string method, int target, params object[] parameters)
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null || !currentRoom.Players.ContainsKey(target))
		{
			return;
		}
		this.punView.RPC(method, currentRoom.Players[target], parameters);
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x0006324E File Offset: 0x0006144E
	public override void Spawned()
	{
		base.Spawned();
		this._spawned = true;
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x0006325D File Offset: 0x0006145D
	public void RequestOwnership()
	{
		this.GetView.RequestOwnership();
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0006326A File Offset: 0x0006146A
	public void ReleaseOwnership()
	{
		this.changingStatAuth = true;
		base.Object.ReleaseStateAuthority();
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x0006327E File Offset: 0x0006147E
	public virtual void StateAuthorityChanged()
	{
		if (this.changingStatAuth)
		{
			this.changingStatAuth = false;
		}
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040019A7 RID: 6567
	[SerializeField]
	private PhotonView punView;

	// Token: 0x040019A8 RID: 6568
	[SerializeField]
	private PhotonView reliableView;

	// Token: 0x040019A9 RID: 6569
	[SerializeField]
	internal NetworkObject fusionView;

	// Token: 0x040019AA RID: 6570
	[SerializeField]
	protected bool _sceneObject;

	// Token: 0x040019AB RID: 6571
	private bool _spawned;

	// Token: 0x040019AC RID: 6572
	private bool changingStatAuth;
}
