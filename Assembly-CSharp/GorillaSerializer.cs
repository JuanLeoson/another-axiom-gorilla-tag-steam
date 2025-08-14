using System;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006A0 RID: 1696
[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	// Token: 0x06002995 RID: 10645 RVA: 0x000DFBEC File Offset: 0x000DDDEC
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsReading)
		{
			this.serializeTarget.OnSerializeRead(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeWrite(stream, info);
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000DFC38 File Offset: 0x000DDE38
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		this.successfullInstantiate = this.OnInstantiateSetup(info, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			if (this.targetType != null && this.targetObject.IsNotNull())
			{
				IGorillaSerializeable gorillaSerializeable = this.targetObject.GetComponent(this.targetType) as IGorillaSerializeable;
				if (gorillaSerializeable != null)
				{
					this.serializeTarget = gorillaSerializeable;
				}
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (PhotonNetwork.InRoom && this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(this.photonView);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x000DFD0E File Offset: 0x000DDF0E
	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000DFD25 File Offset: 0x000DDF25
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000DFD3D File Offset: 0x000DDF3D
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T result = base.gameObject.AddComponent<T>();
		this.photonView.RefreshRpcMonoBehaviourCache();
		return result;
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000DFD58 File Offset: 0x000DDF58
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget target = targetOthers ? RpcTarget.Others : RpcTarget.MasterClient;
		this.photonView.RPC(rpcName, target, data);
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000DFD7B File Offset: 0x000DDF7B
	public void SendRPC(string rpcName, Player targetPlayer, params object[] data)
	{
		this.photonView.RPC(rpcName, targetPlayer, data);
	}

	// Token: 0x04003596 RID: 13718
	protected bool successfullInstantiate;

	// Token: 0x04003597 RID: 13719
	protected IGorillaSerializeable serializeTarget;

	// Token: 0x04003598 RID: 13720
	private Type targetType;

	// Token: 0x04003599 RID: 13721
	protected GameObject targetObject;

	// Token: 0x0400359A RID: 13722
	[SerializeField]
	protected PhotonView photonView;
}
