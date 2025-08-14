using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002DD RID: 733
[RequireComponent(typeof(PhotonView))]
public class NetworkSceneObject : SimulationBehaviour
{
	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x0600113F RID: 4415 RVA: 0x00062228 File Offset: 0x00060428
	public bool IsMine
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x00062235 File Offset: 0x00060435
	protected virtual void Start()
	{
		if (this.photonView == null)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x00062251 File Offset: 0x00060451
	protected virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x00062259 File Offset: 0x00060459
	protected virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x00062264 File Offset: 0x00060464
	private void RegisterOnRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.AddGlobal(this);
		}
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0006229C File Offset: 0x0006049C
	private void RemoveFromRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.RemoveGlobal(this);
		}
	}

	// Token: 0x0400195A RID: 6490
	public PhotonView photonView;
}
