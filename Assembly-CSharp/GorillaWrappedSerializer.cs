using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060029AC RID: 10668 RVA: 0x000DFF1C File Offset: 0x000DE11C
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060029AD RID: 10669 RVA: 0x000DFF24 File Offset: 0x000DE124
	// (set) Token: 0x060029AE RID: 10670 RVA: 0x000DFF2C File Offset: 0x000DE12C
	protected virtual object data { get; set; }

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060029AF RID: 10671 RVA: 0x000DFF35 File Offset: 0x000DE135
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060029B0 RID: 10672 RVA: 0x000DFF42 File Offset: 0x000DE142
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000DFF4F File Offset: 0x000DE14F
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000DFF6C File Offset: 0x000DE16C
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000DFFA4 File Offset: 0x000DE1A4
	public override void Spawned()
	{
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000DFFE4 File Offset: 0x000DE1E4
	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000E005F File Offset: 0x000DE25F
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x060029B6 RID: 10678
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x060029B7 RID: 10679 RVA: 0x000E0078 File Offset: 0x000DE278
	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
		if (this.netView.IsMine)
		{
			PhotonNetwork.Destroy(this.netView.GetView);
			return;
		}
		this.netView.GetView.ObservedComponents.Remove(this);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060029B8 RID: 10680
	protected abstract void OnFailedSpawn();

	// Token: 0x060029B9 RID: 10681 RVA: 0x000DFD25 File Offset: 0x000DDF25
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000E00D0 File Offset: 0x000DE2D0
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000E00E3 File Offset: 0x000DE2E3
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000E0104 File Offset: 0x000DE304
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000E0150 File Offset: 0x000DE350
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x000E0150 File Offset: 0x000DE350
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060029BF RID: 10687
	protected abstract void OnBeforeDespawn();

	// Token: 0x060029C0 RID: 10688 RVA: 0x000E0158 File Offset: 0x000DE358
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000E0188 File Offset: 0x000DE388
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget target = targetOthers ? RpcTarget.Others : RpcTarget.MasterClient;
		this.netView.SendRPC(rpcName, target, data);
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000E01AB File Offset: 0x000DE3AB
	public void SendRPC(string rpcName, NetPlayer targetPlayer, params object[] data)
	{
		this.netView.GetView.RPC(rpcName, ((PunNetPlayer)targetPlayer).PlayerRef, data);
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x0400359F RID: 13727
	protected bool successfullInstantiate;

	// Token: 0x040035A0 RID: 13728
	protected IWrappedSerializable serializeTarget;

	// Token: 0x040035A1 RID: 13729
	private Type targetType;

	// Token: 0x040035A2 RID: 13730
	protected GameObject targetObject;

	// Token: 0x040035A3 RID: 13731
	[SerializeField]
	protected NetworkView netView;
}
