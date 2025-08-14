using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000B8 RID: 184
[NetworkBehaviourWeaved(3)]
public class BarrelCannon : NetworkComponent
{
	// Token: 0x06000477 RID: 1143 RVA: 0x0001A01D File Offset: 0x0001821D
	private void Update()
	{
		if (base.IsMine)
		{
			this.AuthorityUpdate();
		}
		else
		{
			this.ClientUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0001A03C File Offset: 0x0001823C
	private void AuthorityUpdate()
	{
		float time = Time.time;
		this.syncedState.hasAuthorityPassenger = this.localPlayerInside;
		switch (this.syncedState.currentState)
		{
		default:
			if (this.localPlayerInside)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Loaded;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Loaded:
			if (time - this.stateStartTime > this.cannonEntryDelayTime)
			{
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.MovingToFirePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.MovingToFirePosition:
			if (this.moveToFiringPositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = Mathf.Clamp01((time - this.stateStartTime) / this.moveToFiringPositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 1f;
			}
			if (this.syncedState.firingPositionLerpValue >= 1f - Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Firing;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.Firing:
			if (this.localPlayerInside && this.localPlayerRigidbody != null)
			{
				Vector3 b = base.transform.position - GorillaTagger.Instance.headCollider.transform.position;
				this.localPlayerRigidbody.MovePosition(this.localPlayerRigidbody.position + b);
			}
			if (time - this.stateStartTime > this.preFiringDelayTime)
			{
				base.transform.localPosition = this.firingPositionOffset;
				base.transform.localRotation = Quaternion.Euler(this.firingRotationOffset);
				this.FireBarrelCannonLocal(base.transform.position, base.transform.up);
				if (PhotonNetwork.InRoom && GorillaGameManager.instance != null)
				{
					base.SendRPC("FireBarrelCannonRPC", RpcTarget.Others, new object[]
					{
						base.transform.position,
						base.transform.up
					});
				}
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.PostFireCooldown;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.PostFireCooldown:
			if (time - this.stateStartTime > this.postFiringCooldownTime)
			{
				Collider[] array = this.colliders;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.ReturningToIdlePosition;
				return;
			}
			break;
		case BarrelCannon.BarrelCannonState.ReturningToIdlePosition:
			if (this.returnToIdlePositionTime > Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 1f - Mathf.Clamp01((time - this.stateStartTime) / this.returnToIdlePositionTime);
			}
			else
			{
				this.syncedState.firingPositionLerpValue = 0f;
			}
			if (this.syncedState.firingPositionLerpValue <= Mathf.Epsilon)
			{
				this.syncedState.firingPositionLerpValue = 0f;
				this.stateStartTime = time;
				this.syncedState.currentState = BarrelCannon.BarrelCannonState.Idle;
			}
			break;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0001A340 File Offset: 0x00018540
	private void ClientUpdate()
	{
		if (!this.syncedState.hasAuthorityPassenger && this.syncedState.currentState == BarrelCannon.BarrelCannonState.Idle && this.localPlayerInside)
		{
			base.RequestOwnership();
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0001A36C File Offset: 0x0001856C
	private void SharedUpdate()
	{
		if (this.syncedState.firingPositionLerpValue != this.localFiringPositionLerpValue)
		{
			this.localFiringPositionLerpValue = this.syncedState.firingPositionLerpValue;
			base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.firingPositionOffset, this.firePositionAnimationCurve.Evaluate(this.localFiringPositionLerpValue));
			base.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, this.firingRotationOffset, this.fireRotationAnimationCurve.Evaluate(this.localFiringPositionLerpValue)));
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001A3FA File Offset: 0x000185FA
	[PunRPC]
	private void FireBarrelCannonRPC(Vector3 cannonCenter, Vector3 firingDirection)
	{
		this.FireBarrelCannonLocal(cannonCenter, firingDirection);
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001A404 File Offset: 0x00018604
	private void FireBarrelCannonLocal(Vector3 cannonCenter, Vector3 firingDirection)
	{
		if (this.audioSource != null)
		{
			this.audioSource.GTPlay();
		}
		if (this.localPlayerInside && this.localPlayerRigidbody != null)
		{
			Vector3 b = cannonCenter - GorillaTagger.Instance.headCollider.transform.position;
			this.localPlayerRigidbody.position = this.localPlayerRigidbody.position + b;
			this.localPlayerRigidbody.velocity = firingDirection * this.firingSpeed;
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001A490 File Offset: 0x00018690
	private void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = true;
			this.localPlayerRigidbody = rigidbody;
		}
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001A4B8 File Offset: 0x000186B8
	private void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody;
		if (this.LocalPlayerTriggerFilter(other, out rigidbody))
		{
			this.localPlayerInside = false;
			this.localPlayerRigidbody = null;
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0001A4DE File Offset: 0x000186DE
	private bool LocalPlayerTriggerFilter(Collider other, out Rigidbody rb)
	{
		rb = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
		}
		return rb != null;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0001A514 File Offset: 0x00018714
	private bool IsLocalPlayerInCannon()
	{
		Vector3 point;
		Vector3 point2;
		this.GetCapsulePoints(this.triggerCollider, out point, out point2);
		Physics.OverlapCapsuleNonAlloc(point, point2, this.triggerCollider.radius, this.triggerOverlapResults);
		for (int i = 0; i < this.triggerOverlapResults.Length; i++)
		{
			Rigidbody rigidbody;
			if (this.LocalPlayerTriggerFilter(this.triggerOverlapResults[i], out rigidbody))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001A574 File Offset: 0x00018774
	private void GetCapsulePoints(CapsuleCollider capsule, out Vector3 pointA, out Vector3 pointB)
	{
		float d = capsule.height * 0.5f - capsule.radius;
		pointA = capsule.transform.position + capsule.transform.up * d;
		pointB = capsule.transform.position - capsule.transform.up * d;
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000482 RID: 1154 RVA: 0x0001A5E3 File Offset: 0x000187E3
	// (set) Token: 0x06000483 RID: 1155 RVA: 0x0001A60D File Offset: 0x0001880D
	[Networked]
	[NetworkedWeaved(0, 3)]
	private unsafe BarrelCannon.BarrelCannonSyncedStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BarrelCannon.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BarrelCannon.BarrelCannonSyncedStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001A638 File Offset: 0x00018838
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001A64C File Offset: 0x0001884C
	public override void ReadDataFusion()
	{
		this.syncedState.currentState = this.Data.CurrentState;
		this.syncedState.hasAuthorityPassenger = this.Data.HasAuthorityPassenger;
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001A690 File Offset: 0x00018890
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.syncedState.currentState);
		stream.SendNext(this.syncedState.hasAuthorityPassenger);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001A6BE File Offset: 0x000188BE
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.syncedState.currentState = (BarrelCannon.BarrelCannonState)stream.ReceiveNext();
		this.syncedState.hasAuthorityPassenger = (bool)stream.ReceiveNext();
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001A6EC File Offset: 0x000188EC
	public override void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
		if (!this.localPlayerInside)
		{
			targetView.TransferOwnership(requestingPlayer);
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001A7C1 File Offset: 0x000189C1
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001A7D9 File Offset: 0x000189D9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000541 RID: 1345
	[SerializeField]
	private float firingSpeed = 10f;

	// Token: 0x04000542 RID: 1346
	[Header("Cannon's Movement Before Firing")]
	[SerializeField]
	private Vector3 firingPositionOffset = Vector3.zero;

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private Vector3 firingRotationOffset = Vector3.zero;

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private AnimationCurve firePositionAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000545 RID: 1349
	[SerializeField]
	private AnimationCurve fireRotationAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000546 RID: 1350
	[Header("Cannon State Change Timing Parameters")]
	[SerializeField]
	private float moveToFiringPositionTime = 0.5f;

	// Token: 0x04000547 RID: 1351
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float cannonEntryDelayTime = 0.25f;

	// Token: 0x04000548 RID: 1352
	[SerializeField]
	[Tooltip("The minimum time to wait after a gorilla enters the cannon before it starts moving into the firing position.")]
	private float preFiringDelayTime = 0.25f;

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	[Tooltip("The minimum time to wait after the cannon fires before it starts moving back to the idle position.")]
	private float postFiringCooldownTime = 0.25f;

	// Token: 0x0400054A RID: 1354
	[SerializeField]
	private float returnToIdlePositionTime = 1f;

	// Token: 0x0400054B RID: 1355
	[Header("Component References")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	private CapsuleCollider triggerCollider;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private Collider[] colliders;

	// Token: 0x0400054E RID: 1358
	private BarrelCannon.BarrelCannonSyncedState syncedState = new BarrelCannon.BarrelCannonSyncedState();

	// Token: 0x0400054F RID: 1359
	private Collider[] triggerOverlapResults = new Collider[16];

	// Token: 0x04000550 RID: 1360
	private bool localPlayerInside;

	// Token: 0x04000551 RID: 1361
	private Rigidbody localPlayerRigidbody;

	// Token: 0x04000552 RID: 1362
	private float stateStartTime;

	// Token: 0x04000553 RID: 1363
	private float localFiringPositionLerpValue;

	// Token: 0x04000554 RID: 1364
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BarrelCannon.BarrelCannonSyncedStateData _Data;

	// Token: 0x020000B9 RID: 185
	private enum BarrelCannonState
	{
		// Token: 0x04000556 RID: 1366
		Idle,
		// Token: 0x04000557 RID: 1367
		Loaded,
		// Token: 0x04000558 RID: 1368
		MovingToFirePosition,
		// Token: 0x04000559 RID: 1369
		Firing,
		// Token: 0x0400055A RID: 1370
		PostFireCooldown,
		// Token: 0x0400055B RID: 1371
		ReturningToIdlePosition
	}

	// Token: 0x020000BA RID: 186
	private class BarrelCannonSyncedState
	{
		// Token: 0x0400055C RID: 1372
		public BarrelCannon.BarrelCannonState currentState;

		// Token: 0x0400055D RID: 1373
		public bool hasAuthorityPassenger;

		// Token: 0x0400055E RID: 1374
		public float firingPositionLerpValue;
	}

	// Token: 0x020000BB RID: 187
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	private struct BarrelCannonSyncedStateData : INetworkStruct
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x0001A7ED File Offset: 0x000189ED
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x0001A7FF File Offset: 0x000189FF
		[Networked]
		public unsafe BarrelCannon.BarrelCannonState CurrentState
		{
			readonly get
			{
				return *(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState);
			}
			set
			{
				*(BarrelCannon.BarrelCannonState*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentState) = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x0001A812 File Offset: 0x00018A12
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x0001A824 File Offset: 0x00018A24
		[Networked]
		public unsafe NetworkBool HasAuthorityPassenger
		{
			readonly get
			{
				return *(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger);
			}
			set
			{
				*(NetworkBool*)Native.ReferenceToPointer<FixedStorage@1>(ref this._HasAuthorityPassenger) = value;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x0001A837 File Offset: 0x00018A37
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x0001A83F File Offset: 0x00018A3F
		public float FiringPositionLerpValue { readonly get; set; }

		// Token: 0x06000493 RID: 1171 RVA: 0x0001A848 File Offset: 0x00018A48
		public BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonState state, bool hasAuthPassenger, float firingPosLerpVal)
		{
			this.CurrentState = state;
			this.HasAuthorityPassenger = hasAuthPassenger;
			this.FiringPositionLerpValue = firingPosLerpVal;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001A864 File Offset: 0x00018A64
		public static implicit operator BarrelCannon.BarrelCannonSyncedStateData(BarrelCannon.BarrelCannonSyncedState state)
		{
			return new BarrelCannon.BarrelCannonSyncedStateData(state.currentState, state.hasAuthorityPassenger, state.firingPositionLerpValue);
		}

		// Token: 0x0400055F RID: 1375
		[FixedBufferProperty(typeof(BarrelCannon.BarrelCannonState), typeof(UnityValueSurrogate@ReaderWriter@BarrelCannon__BarrelCannonState), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _CurrentState;

		// Token: 0x04000560 RID: 1376
		[FixedBufferProperty(typeof(NetworkBool), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _HasAuthorityPassenger;
	}
}
