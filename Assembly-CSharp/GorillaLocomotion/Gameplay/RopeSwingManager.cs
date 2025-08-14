using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E2C RID: 3628
	public class RopeSwingManager : NetworkSceneObject
	{
		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06005A36 RID: 23094 RVA: 0x001C72AF File Offset: 0x001C54AF
		// (set) Token: 0x06005A37 RID: 23095 RVA: 0x001C72B6 File Offset: 0x001C54B6
		public static RopeSwingManager instance { get; private set; }

		// Token: 0x06005A38 RID: 23096 RVA: 0x001C72C0 File Offset: 0x001C54C0
		private void Awake()
		{
			if (RopeSwingManager.instance != null && RopeSwingManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of RopeSwingManager already exists. Destroying.", null);
				UnityEngine.Object.Destroy(this);
				return;
			}
			if (RopeSwingManager.instance == null)
			{
				RopeSwingManager.instance = this;
			}
		}

		// Token: 0x06005A39 RID: 23097 RVA: 0x001C730C File Offset: 0x001C550C
		private void RegisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Add(t.ropeId, t);
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x001C7320 File Offset: 0x001C5520
		private void UnregisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Remove(t.ropeId);
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x001C7334 File Offset: 0x001C5534
		public static void Register(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.RegisterInstance(t);
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x001C7341 File Offset: 0x001C5541
		public static void Unregister(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.UnregisterInstance(t);
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x001C7350 File Offset: 0x001C5550
		public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					ropeId,
					boneIndex,
					velocity,
					wholeRope
				});
				return;
			}
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, default(PhotonMessageInfoWrapped));
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x001C73BA File Offset: 0x001C55BA
		public bool TryGetRope(int ropeId, out GorillaRopeSwing result)
		{
			return this.ropes.TryGetValue(ropeId, out result);
		}

		// Token: 0x06005A3F RID: 23103 RVA: 0x001C73CC File Offset: 0x001C55CC
		[PunRPC]
		public void SetVelocity(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info2);
			Utils.Log("Receiving RPC for ropes");
		}

		// Token: 0x06005A40 RID: 23104 RVA: 0x001C73F8 File Offset: 0x001C55F8
		[Rpc]
		public unsafe static void RPC_SetVelocity(NetworkRunner runner, int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage == SimulationStages.Resimulate)
				{
					return;
				}
				if (runner.HasAnyActiveConnections())
				{
					int num = 8;
					num += 4;
					num += 4;
					num += 12;
					num += 4;
					SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")), data);
					*(int*)(data + num2) = ropeId;
					num2 += 4;
					*(int*)(data + num2) = boneIndex;
					num2 += 4;
					*(Vector3*)(data + num2) = velocity;
					num2 += 12;
					ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), wholeRope);
					num2 += 4;
					ptr->Offset = num2 * 8;
					ptr->SetStatic();
					runner.SendRpc(ptr);
				}
				info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
			}
			PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
			RopeSwingManager.instance.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info2);
		}

		// Token: 0x06005A41 RID: 23105 RVA: 0x001C7570 File Offset: 0x001C5770
		private void SetVelocityShared(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (info.Sender != null)
			{
				GorillaNot.IncrementRPCCall(info, "SetVelocityShared");
			}
			GorillaRopeSwing gorillaRopeSwing;
			if (this.TryGetRope(ropeId, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				gorillaRopeSwing.SetVelocity(boneIndex, velocity, wholeRope, info);
			}
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x001C75C8 File Offset: 0x001C57C8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_SetVelocity@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int ropeId = num2;
			int num3 = *(int*)(data + num);
			num += 4;
			int boneIndex = num3;
			Vector3 vector = *(Vector3*)(data + num);
			num += 12;
			Vector3 velocity = vector;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
			num += 4;
			bool wholeRope = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			RopeSwingManager.RPC_SetVelocity(runner, ropeId, boneIndex, velocity, wholeRope, info);
		}

		// Token: 0x04006505 RID: 25861
		private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();
	}
}
