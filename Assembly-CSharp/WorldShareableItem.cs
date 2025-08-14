using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
[NetworkBehaviourWeaved(0)]
public class WorldShareableItem : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000287 RID: 647
	// (get) Token: 0x060017A7 RID: 6055 RVA: 0x0007F8FC File Offset: 0x0007DAFC
	// (set) Token: 0x060017A8 RID: 6056 RVA: 0x0007F904 File Offset: 0x0007DB04
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x060017A9 RID: 6057 RVA: 0x0007F90D File Offset: 0x0007DB0D
	// (set) Token: 0x060017AA RID: 6058 RVA: 0x0007F915 File Offset: 0x0007DB15
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060017AB RID: 6059 RVA: 0x0007F91E File Offset: 0x0007DB1E
	// (set) Token: 0x060017AC RID: 6060 RVA: 0x0007F926 File Offset: 0x0007DB26
	public TransferrableObject.PositionState transferableObjectStateNetworked { get; set; }

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060017AD RID: 6061 RVA: 0x0007F92F File Offset: 0x0007DB2F
	// (set) Token: 0x060017AE RID: 6062 RVA: 0x0007F937 File Offset: 0x0007DB37
	public TransferrableObject.ItemStates transferableObjectItemStateNetworked { get; set; }

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x060017AF RID: 6063 RVA: 0x0007F940 File Offset: 0x0007DB40
	// (set) Token: 0x060017B0 RID: 6064 RVA: 0x0007F948 File Offset: 0x0007DB48
	[DevInspectorShow]
	public WorldTargetItem target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0007F951 File Offset: 0x0007DB51
	protected override void Awake()
	{
		base.Awake();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x0007F981 File Offset: 0x0007DB81
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x0007F9BC File Offset: 0x0007DBBC
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		if (this.target == null || !this.target.transferrableObject.isSceneObject)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x0007FA3C File Offset: 0x0007DC3C
	public void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x0007FA4C File Offset: 0x0007DC4C
	public void SetupSharableViewIDs(NetPlayer player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x0007FAD8 File Offset: 0x0007DCD8
	public void ResetViews()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x0007FB08 File Offset: 0x0007DD08
	public void SetupSharableObject(int itemIDx, NetPlayer owner, Transform targetXform)
	{
		if (this.target != null)
		{
			Debug.LogError("ERROR!!!  WorldShareableItem.SetupSharableObject: target is expected to be null before this call. In scene path = \"" + base.transform.GetPathQ() + "\"", this);
			return;
		}
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = (component.canDrop || component.shareable || component.allowWorldSharableInstance);
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x0007FC22 File Offset: 0x0007DE22
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x0007FC2C File Offset: 0x0007DE2C
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(newOwner);
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(previousOwner);
			this.onOwnerChangeCb(player, player2);
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x060017BA RID: 6074 RVA: 0x0007FC66 File Offset: 0x0007DE66
	// (set) Token: 0x060017BB RID: 6075 RVA: 0x0007FC6E File Offset: 0x0007DE6E
	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x0007FC78 File Offset: 0x0007DE78
	public void TriggeredUpdate()
	{
		if (!this.IsTargetValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			Vector3 position;
			Quaternion rotation;
			this.target.targetObject.GetPositionAndRotation(out position, out rotation);
			base.transform.SetPositionAndRotation(position, rotation);
			return;
		}
		if (!base.IsMine && this.EnableRemoteSync)
		{
			Vector3 position2;
			Quaternion rotation2;
			base.transform.GetPositionAndRotation(out position2, out rotation2);
			this.target.targetObject.SetPositionAndRotation(position2, rotation2);
		}
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x0007FCEE File Offset: 0x0007DEEE
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x0007FD10 File Offset: 0x0007DF10
	public void SetupSceneObjectOnNetwork(NetPlayer owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x0007FD20 File Offset: 0x0007DF20
	public bool IsTargetValid()
	{
		return this.target != null;
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0007FD2B File Offset: 0x0007DF2B
	public void Invalidate()
	{
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x0007FD44 File Offset: 0x0007DF44
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, out cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x0007FD8A File Offset: 0x0007DF8A
	public override void WriteDataFusion()
	{
		this.transferableObjectItemStateNetworked = this.transferableObjectItemState;
		this.transferableObjectStateNetworked = this.transferableObjectState;
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x0007FDA4 File Offset: 0x0007DFA4
	public override void ReadDataFusion()
	{
		this.transferableObjectItemState = this.transferableObjectItemStateNetworked;
		this.transferableObjectState = this.transferableObjectStateNetworked;
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x0007FDBE File Offset: 0x0007DFBE
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.transferableObjectState);
		stream.SendNext(this.transferableObjectItemState);
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x0007FDE4 File Offset: 0x0007DFE4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player != this.guard.actualOwner)
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(player, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x0007FE76 File Offset: 0x0007E076
	[PunRPC]
	internal void RPCWorldShareable(PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		if (this.rpcCallBack == null)
		{
			return;
		}
		this.rpcCallBack();
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return true;
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return true;
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x0007FEA8 File Offset: 0x0007E0A8
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04001F98 RID: 8088
	private bool validShareable = true;

	// Token: 0x04001F99 RID: 8089
	public RequestableOwnershipGuard guard;

	// Token: 0x04001F9A RID: 8090
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x04001F9B RID: 8091
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem _target;

	// Token: 0x04001F9C RID: 8092
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x04001F9D RID: 8093
	public Action rpcCallBack;

	// Token: 0x04001F9E RID: 8094
	private bool enableRemoteSync = true;

	// Token: 0x04001F9F RID: 8095
	public Dictionary<NetPlayer, WorldShareableItem.CachedData> cachedDatas = new Dictionary<NetPlayer, WorldShareableItem.CachedData>();

	// Token: 0x020003F9 RID: 1017
	// (Invoke) Token: 0x060017D0 RID: 6096
	public delegate void Delegate();

	// Token: 0x020003FA RID: 1018
	// (Invoke) Token: 0x060017D4 RID: 6100
	public delegate void OnOwnerChangeDelegate(NetPlayer newOwner, NetPlayer prevOwner);

	// Token: 0x020003FB RID: 1019
	public struct CachedData
	{
		// Token: 0x04001FA0 RID: 8096
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x04001FA1 RID: 8097
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
