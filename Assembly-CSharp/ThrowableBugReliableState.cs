using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B38 RID: 2872
[NetworkBehaviourWeaved(3)]
public class ThrowableBugReliableState : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000681 RID: 1665
	// (get) Token: 0x0600450F RID: 17679 RVA: 0x00158A71 File Offset: 0x00156C71
	// (set) Token: 0x06004510 RID: 17680 RVA: 0x00158A9B File Offset: 0x00156C9B
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe ThrowableBugReliableState.BugData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ThrowableBugReliableState.BugData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ThrowableBugReliableState.BugData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06004511 RID: 17681 RVA: 0x00158AC6 File Offset: 0x00156CC6
	public override void WriteDataFusion()
	{
		this.Data = new ThrowableBugReliableState.BugData(this.travelingDirection);
	}

	// Token: 0x06004512 RID: 17682 RVA: 0x00158ADC File Offset: 0x00156CDC
	public override void ReadDataFusion()
	{
		this.travelingDirection = this.Data.tDirection;
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x00158AFD File Offset: 0x00156CFD
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.travelingDirection);
	}

	// Token: 0x06004514 RID: 17684 RVA: 0x00158B10 File Offset: 0x00156D10
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.travelingDirection.SetValueSafe(vector);
	}

	// Token: 0x06004515 RID: 17685 RVA: 0x00002628 File Offset: 0x00000828
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x00002628 File Offset: 0x00000828
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x00002628 File Offset: 0x00000828
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06004518 RID: 17688 RVA: 0x00002628 File Offset: 0x00000828
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06004519 RID: 17689 RVA: 0x00002628 File Offset: 0x00000828
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600451B RID: 17691 RVA: 0x00158B49 File Offset: 0x00156D49
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600451C RID: 17692 RVA: 0x00158B61 File Offset: 0x00156D61
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004F55 RID: 20309
	public Vector3 travelingDirection = Vector3.zero;

	// Token: 0x04004F56 RID: 20310
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ThrowableBugReliableState.BugData _Data;

	// Token: 0x02000B39 RID: 2873
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct BugData : INetworkStruct
	{
		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x0600451D RID: 17693 RVA: 0x00158B75 File Offset: 0x00156D75
		// (set) Token: 0x0600451E RID: 17694 RVA: 0x00158B87 File Offset: 0x00156D87
		[Networked]
		public unsafe Vector3 tDirection
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection) = value;
			}
		}

		// Token: 0x0600451F RID: 17695 RVA: 0x00158B9A File Offset: 0x00156D9A
		public BugData(Vector3 dir)
		{
			this.tDirection = dir;
		}

		// Token: 0x04004F57 RID: 20311
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@3 _tDirection;
	}
}
