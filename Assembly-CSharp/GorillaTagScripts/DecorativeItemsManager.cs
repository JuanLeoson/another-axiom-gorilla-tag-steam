using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000C29 RID: 3113
	[NetworkBehaviourWeaved(1)]
	public class DecorativeItemsManager : NetworkComponent
	{
		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06004C8A RID: 19594 RVA: 0x0017B7FC File Offset: 0x001799FC
		public static DecorativeItemsManager Instance
		{
			get
			{
				return DecorativeItemsManager._instance;
			}
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x0017B804 File Offset: 0x00179A04
		protected override void Awake()
		{
			base.Awake();
			if (DecorativeItemsManager._instance != null && DecorativeItemsManager._instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				DecorativeItemsManager._instance = this;
			}
			this.currentIndex = -1;
			this.shouldRunUpdate = true;
			this.zone = base.GetComponent<ZoneBasedObject>();
			foreach (DecorativeItem decorativeItem in this.decorativeItemsContainer.GetComponentsInChildren<DecorativeItem>(false))
			{
				if (decorativeItem)
				{
					this.itemsList.Add(decorativeItem);
					DecorativeItem decorativeItem2 = decorativeItem;
					decorativeItem2.respawnItem = (UnityAction<DecorativeItem>)Delegate.Combine(decorativeItem2.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
				}
			}
			foreach (AttachPoint attachPoint in this.respawnableHooksContainer.GetComponentsInChildren<AttachPoint>(false))
			{
				if (attachPoint)
				{
					this.respawnableHooks.Add(attachPoint);
				}
			}
			this.allHooks.AddRange(this.respawnableHooks);
			foreach (GameObject gameObject in this.nonRespawnableHooksContainer)
			{
				foreach (AttachPoint attachPoint2 in gameObject.GetComponentsInChildren<AttachPoint>(false))
				{
					if (attachPoint2)
					{
						this.allHooks.Add(attachPoint2);
					}
				}
			}
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x0017B96C File Offset: 0x00179B6C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (DecorativeItem decorativeItem in this.itemsList)
			{
				decorativeItem.respawnItem = (UnityAction<DecorativeItem>)Delegate.Remove(decorativeItem.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
			}
			this.itemsList.Clear();
			this.respawnableHooks.Clear();
			if (DecorativeItemsManager._instance == this)
			{
				DecorativeItemsManager._instance = null;
			}
		}

		// Token: 0x06004C8D RID: 19597 RVA: 0x0017BA08 File Offset: 0x00179C08
		private void Update()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (this.wasInZone != this.zone.IsLocalPlayerInZone())
			{
				this.shouldRunUpdate = true;
			}
			if (!this.shouldRunUpdate)
			{
				return;
			}
			if (base.IsMine)
			{
				if (this.wasInZone != this.zone.IsLocalPlayerInZone())
				{
					foreach (AttachPoint attachPoint in this.allHooks)
					{
						attachPoint.SetIsHook(false);
					}
					for (int i = 0; i < this.itemsList.Count; i++)
					{
						this.itemsList[i].itemState = TransferrableObject.ItemStates.State2;
						this.SpawnItem(i);
					}
					this.shouldRunUpdate = false;
				}
				this.wasInZone = this.zone.IsLocalPlayerInZone();
				this.SpawnItem(this.UpdateListPerFrame());
			}
		}

		// Token: 0x06004C8E RID: 19598 RVA: 0x0017BAF8 File Offset: 0x00179CF8
		private void SpawnItem(int index)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (index < 0 || index >= this.itemsList.Count)
			{
				return;
			}
			if (this.respawnableHooks == null)
			{
				return;
			}
			if (this.itemsList == null)
			{
				return;
			}
			if (this.itemsList.Count > this.respawnableHooks.Count)
			{
				Debug.LogError("Trying to snap more decorative items than allowed! Some items will be left un-hooked!");
				return;
			}
			Transform transform = this.RandomSpawn();
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			DecorativeItem decorativeItem = this.itemsList[index];
			decorativeItem.WorldShareableRequestOwnership();
			decorativeItem.Respawn(position, rotation);
			base.SendRPC("RespawnItemRPC", RpcTarget.Others, new object[]
			{
				index,
				position,
				rotation
			});
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x0017BBBF File Offset: 0x00179DBF
		[PunRPC]
		private void RespawnItemRPC(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfo info)
		{
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x0017BBD4 File Offset: 0x00179DD4
		[Rpc]
		private unsafe void RPC_RespawnItem(int index, Vector3 _transformPos, Quaternion _transformRot, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							num += 4;
							num += 12;
							num += 16;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							*(int*)(data + num2) = index;
							num2 += 4;
							*(Vector3*)(data + num2) = _transformPos;
							num2 += 12;
							*(Quaternion*)(data + num2) = _transformRot;
							num2 += 16;
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_12;
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x0017BD74 File Offset: 0x00179F74
		protected void RespawnItemShared(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfoWrapped info)
		{
			if (index >= 0 && index <= this.itemsList.Count - 1)
			{
				float num = 10000f;
				if (_transformPos.IsValid(num) && _transformRot.IsValid() && info.Sender == NetworkSystem.Instance.MasterClient)
				{
					GorillaNot.IncrementRPCCall(info, "RespawnItemRPC");
					this.itemsList[index].Respawn(_transformPos, _transformRot);
					return;
				}
			}
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x0017BDE4 File Offset: 0x00179FE4
		private Transform RandomSpawn()
		{
			this.lastIndex = this.currentIndex;
			bool flag = false;
			bool flag2 = this.zone.IsLocalPlayerInZone();
			int index = Random.Range(0, this.respawnableHooks.Count);
			while (!flag)
			{
				index = Random.Range(0, this.respawnableHooks.Count);
				if (!this.respawnableHooks[index].inForest == flag2)
				{
					flag = true;
				}
			}
			if (!this.respawnableHooks[index].IsHooked())
			{
				this.currentIndex = index;
			}
			else
			{
				this.currentIndex = -1;
			}
			if (this.currentIndex != this.lastIndex && this.currentIndex > -1)
			{
				return this.respawnableHooks[this.currentIndex].attachPoint;
			}
			this.currentIndex = -1;
			return null;
		}

		// Token: 0x06004C93 RID: 19603 RVA: 0x0017BEA6 File Offset: 0x0017A0A6
		private int UpdateListPerFrame()
		{
			this.arrayIndex++;
			if (this.arrayIndex >= this.itemsList.Count || this.arrayIndex < 0)
			{
				this.shouldRunUpdate = false;
				return -1;
			}
			return this.arrayIndex;
		}

		// Token: 0x06004C94 RID: 19604 RVA: 0x0017BEE4 File Offset: 0x0017A0E4
		private void OnRequestToRespawn(DecorativeItem item)
		{
			if (base.IsMine)
			{
				if (item == null)
				{
					return;
				}
				int index = this.itemsList.IndexOf(item);
				this.SpawnItem(index);
			}
		}

		// Token: 0x06004C95 RID: 19605 RVA: 0x0017BF18 File Offset: 0x0017A118
		public AttachPoint getCurrentAttachPointByPosition(Vector3 _attachPoint)
		{
			foreach (AttachPoint attachPoint in this.allHooks)
			{
				if (attachPoint.attachPoint.position == _attachPoint)
				{
					return attachPoint;
				}
			}
			return null;
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06004C96 RID: 19606 RVA: 0x0017BF80 File Offset: 0x0017A180
		// (set) Token: 0x06004C97 RID: 19607 RVA: 0x0017BFA6 File Offset: 0x0017A1A6
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[0];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[0] = value;
			}
		}

		// Token: 0x06004C98 RID: 19608 RVA: 0x0017BFCD File Offset: 0x0017A1CD
		public override void WriteDataFusion()
		{
			this.Data = this.currentIndex;
		}

		// Token: 0x06004C99 RID: 19609 RVA: 0x0017BFDB File Offset: 0x0017A1DB
		public override void ReadDataFusion()
		{
			this.currentIndex = this.Data;
		}

		// Token: 0x06004C9A RID: 19610 RVA: 0x0017BFE9 File Offset: 0x0017A1E9
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentIndex);
		}

		// Token: 0x06004C9B RID: 19611 RVA: 0x0017C00A File Offset: 0x0017A20A
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			this.currentIndex = (int)stream.ReceiveNext();
		}

		// Token: 0x06004C9D RID: 19613 RVA: 0x0017C066 File Offset: 0x0017A266
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004C9E RID: 19614 RVA: 0x0017C07E File Offset: 0x0017A27E
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x06004C9F RID: 19615 RVA: 0x0017C094 File Offset: 0x0017A294
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RespawnItem@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			int num2 = *(int*)(data + num);
			num += 4;
			int index = num2;
			Vector3 vector = *(Vector3*)(data + num);
			num += 12;
			Vector3 transformPos = vector;
			Quaternion quaternion = *(Quaternion*)(data + num);
			num += 16;
			Quaternion transformRot = quaternion;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((DecorativeItemsManager)behaviour).RPC_RespawnItem(index, transformPos, transformRot, info);
		}

		// Token: 0x040055AA RID: 21930
		public GameObject decorativeItemsContainer;

		// Token: 0x040055AB RID: 21931
		public GameObject respawnableHooksContainer;

		// Token: 0x040055AC RID: 21932
		public List<GameObject> nonRespawnableHooksContainer = new List<GameObject>();

		// Token: 0x040055AD RID: 21933
		private readonly List<DecorativeItem> itemsList = new List<DecorativeItem>();

		// Token: 0x040055AE RID: 21934
		private readonly List<AttachPoint> respawnableHooks = new List<AttachPoint>();

		// Token: 0x040055AF RID: 21935
		private readonly List<AttachPoint> allHooks = new List<AttachPoint>();

		// Token: 0x040055B0 RID: 21936
		private int lastIndex;

		// Token: 0x040055B1 RID: 21937
		private int currentIndex;

		// Token: 0x040055B2 RID: 21938
		private int arrayIndex = -1;

		// Token: 0x040055B3 RID: 21939
		private bool shouldRunUpdate;

		// Token: 0x040055B4 RID: 21940
		private ZoneBasedObject zone;

		// Token: 0x040055B5 RID: 21941
		private bool wasInZone;

		// Token: 0x040055B6 RID: 21942
		[OnEnterPlay_SetNull]
		private static DecorativeItemsManager _instance;

		// Token: 0x040055B7 RID: 21943
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int _Data;
	}
}
