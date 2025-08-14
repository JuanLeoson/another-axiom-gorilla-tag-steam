using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C0 RID: 192
[NetworkBehaviourWeaved(42)]
public class MonkeyeAI_ReplState : NetworkComponent
{
	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001C406 File Offset: 0x0001A606
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0001C430 File Offset: 0x0001A630
	[Networked]
	[NetworkedWeaved(0, 42)]
	private unsafe MonkeyeAI_ReplState.MonkeyeAI_RepStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001C45C File Offset: 0x0001A65C
	public override void WriteDataFusion()
	{
		MonkeyeAI_ReplState.MonkeyeAI_RepStateData data = new MonkeyeAI_ReplState.MonkeyeAI_RepStateData(this.userId, this.attackPos, this.timer, this.floorEnabled, this.portalEnabled, this.freezePlayer, this.alpha, this.state);
		this.Data = data;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001C4A8 File Offset: 0x0001A6A8
	public override void ReadDataFusion()
	{
		this.userId = this.Data.UserId.Value;
		this.attackPos = this.Data.AttackPos;
		this.timer = this.Data.Timer;
		this.floorEnabled = this.Data.FloorEnabled;
		this.portalEnabled = this.Data.PortalEnabled;
		this.freezePlayer = this.Data.FreezePlayer;
		this.alpha = this.Data.Alpha;
		this.state = this.Data.State;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001C56C File Offset: 0x0001A76C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.userId);
		stream.SendNext(this.attackPos);
		stream.SendNext(this.timer);
		stream.SendNext(this.floorEnabled);
		stream.SendNext(this.portalEnabled);
		stream.SendNext(this.freezePlayer);
		stream.SendNext(this.alpha);
		stream.SendNext(this.state);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001C5FC File Offset: 0x0001A7FC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.attackPos.SetValueSafe(vector);
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001C6D4 File Offset: 0x0001A8D4
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001C6EC File Offset: 0x0001A8EC
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040005B3 RID: 1459
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x040005B4 RID: 1460
	public string userId;

	// Token: 0x040005B5 RID: 1461
	public Vector3 attackPos;

	// Token: 0x040005B6 RID: 1462
	public float timer;

	// Token: 0x040005B7 RID: 1463
	public bool floorEnabled;

	// Token: 0x040005B8 RID: 1464
	public bool portalEnabled;

	// Token: 0x040005B9 RID: 1465
	public bool freezePlayer;

	// Token: 0x040005BA RID: 1466
	public float alpha;

	// Token: 0x040005BB RID: 1467
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 42)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MonkeyeAI_ReplState.MonkeyeAI_RepStateData _Data;

	// Token: 0x020000C1 RID: 193
	public enum EStates
	{
		// Token: 0x040005BD RID: 1469
		Sleeping,
		// Token: 0x040005BE RID: 1470
		Patrolling,
		// Token: 0x040005BF RID: 1471
		Chasing,
		// Token: 0x040005C0 RID: 1472
		ReturnToSleepPt,
		// Token: 0x040005C1 RID: 1473
		GoToSleep,
		// Token: 0x040005C2 RID: 1474
		BeginAttack,
		// Token: 0x040005C3 RID: 1475
		OpenFloor,
		// Token: 0x040005C4 RID: 1476
		DropPlayer,
		// Token: 0x040005C5 RID: 1477
		CloseFloor
	}

	// Token: 0x020000C2 RID: 194
	[NetworkStructWeaved(42)]
	[StructLayout(LayoutKind.Explicit, Size = 168)]
	public struct MonkeyeAI_RepStateData : INetworkStruct
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001C700 File Offset: 0x0001A900
		// (set) Token: 0x060004D0 RID: 1232 RVA: 0x0001C712 File Offset: 0x0001A912
		[Networked]
		public unsafe NetworkString<_32> UserId
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId) = value;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060004D1 RID: 1233 RVA: 0x0001C725 File Offset: 0x0001A925
		// (set) Token: 0x060004D2 RID: 1234 RVA: 0x0001C737 File Offset: 0x0001A937
		[Networked]
		public unsafe Vector3 AttackPos
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos) = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x0001C74A File Offset: 0x0001A94A
		// (set) Token: 0x060004D4 RID: 1236 RVA: 0x0001C758 File Offset: 0x0001A958
		[Networked]
		public unsafe float Timer
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer) = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x0001C767 File Offset: 0x0001A967
		// (set) Token: 0x060004D6 RID: 1238 RVA: 0x0001C76F File Offset: 0x0001A96F
		public NetworkBool FloorEnabled { readonly get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0001C778 File Offset: 0x0001A978
		// (set) Token: 0x060004D8 RID: 1240 RVA: 0x0001C780 File Offset: 0x0001A980
		public NetworkBool PortalEnabled { readonly get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0001C789 File Offset: 0x0001A989
		// (set) Token: 0x060004DA RID: 1242 RVA: 0x0001C791 File Offset: 0x0001A991
		public NetworkBool FreezePlayer { readonly get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x0001C79A File Offset: 0x0001A99A
		// (set) Token: 0x060004DC RID: 1244 RVA: 0x0001C7A8 File Offset: 0x0001A9A8
		[Networked]
		public unsafe float Alpha
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha) = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x0001C7B7 File Offset: 0x0001A9B7
		// (set) Token: 0x060004DE RID: 1246 RVA: 0x0001C7BF File Offset: 0x0001A9BF
		public MonkeyeAI_ReplState.EStates State { readonly get; set; }

		// Token: 0x060004DF RID: 1247 RVA: 0x0001C7C8 File Offset: 0x0001A9C8
		public MonkeyeAI_RepStateData(string id, Vector3 atPos, float timer, bool floorOn, bool portalOn, bool freezePlayer, float alpha, MonkeyeAI_ReplState.EStates state)
		{
			this.UserId = id;
			this.AttackPos = atPos;
			this.Timer = timer;
			this.FloorEnabled = floorOn;
			this.PortalEnabled = portalOn;
			this.FreezePlayer = freezePlayer;
			this.Alpha = alpha;
			this.State = state;
		}

		// Token: 0x040005C6 RID: 1478
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _UserId;

		// Token: 0x040005C7 RID: 1479
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@3 _AttackPos;

		// Token: 0x040005C8 RID: 1480
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(144)]
		private FixedStorage@1 _Timer;

		// Token: 0x040005CC RID: 1484
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(160)]
		private FixedStorage@1 _Alpha;
	}
}
