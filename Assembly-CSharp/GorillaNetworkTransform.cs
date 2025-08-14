using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200069E RID: 1694
[NetworkBehaviourWeaved(15)]
internal class GorillaNetworkTransform : NetworkComponent
{
	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002985 RID: 10629 RVA: 0x000DF4E3 File Offset: 0x000DD6E3
	public bool RespectOwnership
	{
		get
		{
			return this.respectOwnership;
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06002986 RID: 10630 RVA: 0x000DF4EB File Offset: 0x000DD6EB
	// (set) Token: 0x06002987 RID: 10631 RVA: 0x000DF515 File Offset: 0x000DD715
	[Networked]
	[NetworkedWeaved(0, 15)]
	private unsafe GorillaNetworkTransform.NetTransformData data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000DF540 File Offset: 0x000DD740
	public new void Awake()
	{
		this.m_StoredPosition = base.transform.localPosition;
		this.m_NetworkPosition = Vector3.zero;
		this.m_NetworkScale = Vector3.zero;
		this.m_NetworkRotation = Quaternion.identity;
		this.maxDistanceSquare = this.maxDistance * this.maxDistance;
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000DF592 File Offset: 0x000DD792
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.m_firstTake = true;
		if (this.clampToSpawn)
		{
			this.clampOriginPoint = (this.m_UseLocal ? base.transform.localPosition : base.transform.position);
		}
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000DF5D0 File Offset: 0x000DD7D0
	public void Update()
	{
		if (!base.IsLocallyOwned)
		{
			if (this.m_UseLocal)
			{
				base.transform.SetLocalPositionAndRotation(Vector3.MoveTowards(base.transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
				return;
			}
			base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x000DF6C0 File Offset: 0x000DD8C0
	public override void WriteDataFusion()
	{
		GorillaNetworkTransform.NetTransformData data = this.SharedWrite();
		double sentTime = NetworkSystem.Instance.SimTick / 1000.0;
		data.SentTime = sentTime;
		this.data = data;
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x000DF6FA File Offset: 0x000DD8FA
	public override void ReadDataFusion()
	{
		this.SharedRead(this.data);
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000DF708 File Offset: 0x000DD908
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		if (this.m_SynchronizePosition)
		{
			stream.SendNext(netTransformData.position);
			stream.SendNext(netTransformData.velocity);
		}
		if (this.m_SynchronizeRotation)
		{
			stream.SendNext(netTransformData.rotation);
		}
		if (this.m_SynchronizeScale)
		{
			stream.SendNext(netTransformData.scale);
		}
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000DF79C File Offset: 0x000DD99C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData data = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			data.position = (Vector3)stream.ReceiveNext();
			data.velocity = (Vector3)stream.ReceiveNext();
		}
		if (this.m_SynchronizeRotation)
		{
			data.rotation = (Quaternion)stream.ReceiveNext();
		}
		if (this.m_SynchronizeScale)
		{
			data.scale = (Vector3)stream.ReceiveNext();
		}
		data.SentTime = (double)((float)info.SentServerTime);
		this.SharedRead(data);
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x000DF84C File Offset: 0x000DDA4C
	private void SharedRead(GorillaNetworkTransform.NetTransformData data)
	{
		if (this.m_SynchronizePosition)
		{
			ref this.m_NetworkPosition.SetValueSafe(data.position);
			ref this.m_Velocity.SetValueSafe(data.velocity);
			if (this.clampDistanceFromSpawn && Vector3.SqrMagnitude(this.clampOriginPoint - this.m_NetworkPosition) > this.maxDistanceSquare)
			{
				this.m_NetworkPosition = this.clampOriginPoint + this.m_Velocity.normalized * this.maxDistance;
				this.m_Velocity = Vector3.zero;
			}
			if (this.m_firstTake)
			{
				if (this.m_UseLocal)
				{
					base.transform.localPosition = this.m_NetworkPosition;
				}
				else
				{
					base.transform.position = this.m_NetworkPosition;
				}
				this.m_Distance = 0f;
			}
			else
			{
				float d = Mathf.Abs((float)(NetworkSystem.Instance.SimTime - data.SentTime));
				this.m_NetworkPosition += this.m_Velocity * d;
				if (this.m_UseLocal)
				{
					this.m_Distance = Vector3.Distance(base.transform.localPosition, this.m_NetworkPosition);
				}
				else
				{
					this.m_Distance = Vector3.Distance(base.transform.position, this.m_NetworkPosition);
				}
			}
		}
		if (this.m_SynchronizeRotation)
		{
			ref this.m_NetworkRotation.SetValueSafe(data.rotation);
			if (this.m_firstTake)
			{
				this.m_Angle = 0f;
				if (this.m_UseLocal)
				{
					base.transform.localRotation = this.m_NetworkRotation;
				}
				else
				{
					base.transform.rotation = this.m_NetworkRotation;
				}
			}
			else if (this.m_UseLocal)
			{
				this.m_Angle = Quaternion.Angle(base.transform.localRotation, this.m_NetworkRotation);
			}
			else
			{
				this.m_Angle = Quaternion.Angle(base.transform.rotation, this.m_NetworkRotation);
			}
		}
		if (this.m_SynchronizeScale)
		{
			ref this.m_NetworkScale.SetValueSafe(data.scale);
			base.transform.localScale = this.m_NetworkScale;
		}
		if (this.m_firstTake)
		{
			this.m_firstTake = false;
		}
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x000DFA74 File Offset: 0x000DDC74
	private GorillaNetworkTransform.NetTransformData SharedWrite()
	{
		GorillaNetworkTransform.NetTransformData result = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			if (this.m_UseLocal)
			{
				this.m_Velocity = base.transform.localPosition - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.localPosition;
				result.position = base.transform.localPosition;
				result.velocity = this.m_Velocity;
			}
			else
			{
				this.m_Velocity = base.transform.position - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.position;
				result.position = base.transform.position;
				result.velocity = this.m_Velocity;
			}
		}
		if (this.m_SynchronizeRotation)
		{
			if (this.m_UseLocal)
			{
				result.rotation = base.transform.localRotation;
			}
			else
			{
				result.rotation = base.transform.rotation;
			}
		}
		if (this.m_SynchronizeScale)
		{
			result.scale = base.transform.localScale;
		}
		return result;
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000DFB87 File Offset: 0x000DDD87
	public void GTAddition_DoTeleport()
	{
		this.m_firstTake = true;
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000DFBBF File Offset: 0x000DDDBF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.data = this._data;
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000DFBD7 File Offset: 0x000DDDD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._data = this.data;
	}

	// Token: 0x0400357E RID: 13694
	[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
	public bool m_UseLocal;

	// Token: 0x0400357F RID: 13695
	[SerializeField]
	private bool respectOwnership;

	// Token: 0x04003580 RID: 13696
	[SerializeField]
	private bool clampDistanceFromSpawn = true;

	// Token: 0x04003581 RID: 13697
	[SerializeField]
	private float maxDistance = 100f;

	// Token: 0x04003582 RID: 13698
	private float maxDistanceSquare;

	// Token: 0x04003583 RID: 13699
	[SerializeField]
	private bool clampToSpawn = true;

	// Token: 0x04003584 RID: 13700
	[Tooltip("Use this if clampToSpawn is false, to set the center point to check the synced position against")]
	[SerializeField]
	private Vector3 clampOriginPoint;

	// Token: 0x04003585 RID: 13701
	public bool m_SynchronizePosition = true;

	// Token: 0x04003586 RID: 13702
	public bool m_SynchronizeRotation = true;

	// Token: 0x04003587 RID: 13703
	public bool m_SynchronizeScale;

	// Token: 0x04003588 RID: 13704
	private float m_Distance;

	// Token: 0x04003589 RID: 13705
	private float m_Angle;

	// Token: 0x0400358A RID: 13706
	private Vector3 m_Velocity;

	// Token: 0x0400358B RID: 13707
	private Vector3 m_NetworkPosition;

	// Token: 0x0400358C RID: 13708
	private Vector3 m_StoredPosition;

	// Token: 0x0400358D RID: 13709
	private Vector3 m_NetworkScale;

	// Token: 0x0400358E RID: 13710
	private Quaternion m_NetworkRotation;

	// Token: 0x0400358F RID: 13711
	private bool m_firstTake;

	// Token: 0x04003590 RID: 13712
	[WeaverGenerated]
	[DefaultForProperty("data", 0, 15)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GorillaNetworkTransform.NetTransformData _data;

	// Token: 0x0200069F RID: 1695
	[NetworkStructWeaved(15)]
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	private struct NetTransformData : INetworkStruct
	{
		// Token: 0x04003591 RID: 13713
		[FieldOffset(0)]
		public Vector3 position;

		// Token: 0x04003592 RID: 13714
		[FieldOffset(12)]
		public Vector3 velocity;

		// Token: 0x04003593 RID: 13715
		[FieldOffset(24)]
		public Quaternion rotation;

		// Token: 0x04003594 RID: 13716
		[FieldOffset(40)]
		public Vector3 scale;

		// Token: 0x04003595 RID: 13717
		[FieldOffset(52)]
		public double SentTime;
	}
}
