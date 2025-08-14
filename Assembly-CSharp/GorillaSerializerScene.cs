using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006A2 RID: 1698
internal class GorillaSerializerScene : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060029A2 RID: 10658 RVA: 0x000DFDB9 File Offset: 0x000DDFB9
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000DFDC8 File Offset: 0x000DDFC8
	protected virtual void Start()
	{
		if (!this.targetComponent.IsNull())
		{
			IGorillaSerializeableScene gorillaSerializeableScene = this.targetComponent as IGorillaSerializeableScene;
			if (gorillaSerializeableScene != null)
			{
				gorillaSerializeableScene.OnSceneLinking(this);
				this.serializeTarget = gorillaSerializeableScene;
				this.sceneSerializeTarget = gorillaSerializeableScene;
				this.successfullInstantiate = true;
				this.photonView.AddCallbackTarget(this);
				return;
			}
		}
		Debug.LogError("GorillaSerializerscene: missing target component or invalid target", base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000DFE36 File Offset: 0x000DE036
	private void OnEnable()
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		if (!this.validDisable)
		{
			this.validDisable = true;
			return;
		}
		this.OnValidEnable();
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000DFE57 File Offset: 0x000DE057
	protected virtual void OnValidEnable()
	{
		this.sceneSerializeTarget.OnNetworkObjectEnable();
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000DFE64 File Offset: 0x000DE064
	private void OnDisable()
	{
		if (!this.successfullInstantiate || !this.validDisable)
		{
			return;
		}
		this.OnValidDisable();
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000DFE7D File Offset: 0x000DE07D
	protected virtual void OnValidDisable()
	{
		this.sceneSerializeTarget.OnNetworkObjectDisable();
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000DFE8C File Offset: 0x000DE08C
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		GorillaNot.instance.SendReport("bad net obj creation", info.Sender.UserId, info.Sender.NickName);
		if (info.photonView.IsMine)
		{
			PhotonNetwork.Destroy(info.photonView);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000DFEE4 File Offset: 0x000DE0E4
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.validDisable = false;
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000DFEED File Offset: 0x000DE0ED
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		if (!this.transferrable)
		{
			return info.Sender == PhotonNetwork.MasterClient;
		}
		return base.ValidOnSerialize(stream, info);
	}

	// Token: 0x0400359B RID: 13723
	[SerializeField]
	private bool transferrable;

	// Token: 0x0400359C RID: 13724
	[SerializeField]
	private MonoBehaviour targetComponent;

	// Token: 0x0400359D RID: 13725
	private IGorillaSerializeableScene sceneSerializeTarget;

	// Token: 0x0400359E RID: 13726
	protected bool validDisable = true;
}
