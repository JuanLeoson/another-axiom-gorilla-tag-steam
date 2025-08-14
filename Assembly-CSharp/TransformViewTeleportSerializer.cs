using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A1A RID: 2586
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x06003EFF RID: 16127 RVA: 0x0014003D File Offset: 0x0013E23D
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x06003F00 RID: 16128 RVA: 0x00140051 File Offset: 0x0013E251
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x06003F01 RID: 16129 RVA: 0x0014005A File Offset: 0x0013E25A
	// (set) Token: 0x06003F02 RID: 16130 RVA: 0x00140084 File Offset: 0x0013E284
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003F03 RID: 16131 RVA: 0x001400AF File Offset: 0x0013E2AF
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x06003F04 RID: 16132 RVA: 0x001400C9 File Offset: 0x0013E2C9
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06003F05 RID: 16133 RVA: 0x001400E3 File Offset: 0x0013E2E3
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x06003F06 RID: 16134 RVA: 0x0014011E File Offset: 0x0013E31E
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06003F08 RID: 16136 RVA: 0x00140159 File Offset: 0x0013E359
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003F09 RID: 16137 RVA: 0x00140171 File Offset: 0x0013E371
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004B05 RID: 19205
	private bool willTeleport;

	// Token: 0x04004B06 RID: 19206
	private GorillaNetworkTransform transformView;

	// Token: 0x04004B07 RID: 19207
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkBool _Data;
}
