using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F12 RID: 3858
	public class RCCosmeticNetworkSync : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
	{
		// Token: 0x06005F7D RID: 24445 RVA: 0x001E3748 File Offset: 0x001E1948
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			if (info.Sender == null)
			{
				this.DestroyThis();
				return;
			}
			if (info.Sender != base.photonView.Owner || base.photonView.IsRoomView)
			{
				GorillaNot.instance.SendReport("spoofed rc instantiate", info.Sender.UserId, info.Sender.NickName);
				this.DestroyThis();
				return;
			}
			object[] instantiationData = info.photonView.InstantiationData;
			if (instantiationData != null && instantiationData.Length >= 1)
			{
				object obj = instantiationData[0];
				if (obj is int)
				{
					int num = (int)obj;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && num > -1 && num < rigContainer.Rig.myBodyDockPositions.allObjects.Length)
					{
						this.rcRemote = (rigContainer.Rig.myBodyDockPositions.allObjects[num] as RCRemoteHoldable);
						if (this.rcRemote != null)
						{
							this.rcRemote.networkSync = this;
							this.rcRemote.WakeUpRemoteVehicle();
						}
					}
					if (this.rcRemote == null)
					{
						this.DestroyThis();
					}
					return;
				}
			}
			this.DestroyThis();
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x001E3878 File Offset: 0x001E1A78
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != base.photonView.Owner)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.syncedState.state);
				stream.SendNext(this.syncedState.position);
				stream.SendNext((int)BitPackUtils.PackRotation(this.syncedState.rotation, true));
				stream.SendNext(this.syncedState.dataA);
				stream.SendNext(this.syncedState.dataB);
				stream.SendNext(this.syncedState.dataC);
				return;
			}
			if (stream.IsReading)
			{
				byte state = this.syncedState.state;
				this.syncedState.state = (byte)stream.ReceiveNext();
				Vector3 vector = (Vector3)stream.ReceiveNext();
				ref this.syncedState.position.SetValueSafe(vector);
				Quaternion quaternion = BitPackUtils.UnpackRotation((uint)((int)stream.ReceiveNext()));
				ref this.syncedState.rotation.SetValueSafe(quaternion);
				this.syncedState.dataA = (byte)stream.ReceiveNext();
				this.syncedState.dataB = (byte)stream.ReceiveNext();
				this.syncedState.dataC = (byte)stream.ReceiveNext();
				if (state != this.syncedState.state && this.rcRemote != null && this.rcRemote.Vehicle != null && !this.rcRemote.Vehicle.enabled)
				{
					this.rcRemote.WakeUpRemoteVehicle();
				}
			}
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x001E3A2C File Offset: 0x001E1C2C
		[PunRPC]
		public void HitRCVehicleRPC(Vector3 hitVelocity, bool isProjectile, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "HitRCVehicleRPC");
			float num = 10000f;
			if (!hitVelocity.IsValid(num))
			{
				GorillaNot.instance.SendReport("nan rc hit", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			if (this.rcRemote != null && this.rcRemote.Vehicle != null)
			{
				this.rcRemote.Vehicle.AuthorityApplyImpact(hitVelocity, isProjectile);
			}
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x001E3AB0 File Offset: 0x001E1CB0
		private void DestroyThis()
		{
			if (base.photonView.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x04006A63 RID: 27235
		public RCCosmeticNetworkSync.SyncedState syncedState;

		// Token: 0x04006A64 RID: 27236
		private RCRemoteHoldable rcRemote;

		// Token: 0x02000F13 RID: 3859
		public struct SyncedState
		{
			// Token: 0x04006A65 RID: 27237
			public byte state;

			// Token: 0x04006A66 RID: 27238
			public Vector3 position;

			// Token: 0x04006A67 RID: 27239
			public Quaternion rotation;

			// Token: 0x04006A68 RID: 27240
			public byte dataA;

			// Token: 0x04006A69 RID: 27241
			public byte dataB;

			// Token: 0x04006A6A RID: 27242
			public byte dataC;
		}
	}
}
