using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Fusion;
using GorillaExtensions;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x020005AD RID: 1453
[NetworkBehaviourWeaved(0)]
public class GameEntityManager : NetworkComponent, IMatchmakingCallbacks, IInRoomCallbacks, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1400004E RID: 78
	// (add) Token: 0x06002383 RID: 9091 RVA: 0x000BDEB8 File Offset: 0x000BC0B8
	// (remove) Token: 0x06002384 RID: 9092 RVA: 0x000BDEF0 File Offset: 0x000BC0F0
	public event GameEntityManager.ZoneStartEvent onZoneStart;

	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06002385 RID: 9093 RVA: 0x000BDF28 File Offset: 0x000BC128
	// (remove) Token: 0x06002386 RID: 9094 RVA: 0x000BDF60 File Offset: 0x000BC160
	public event GameEntityManager.ZoneClearEvent onZoneClear;

	// Token: 0x06002387 RID: 9095 RVA: 0x000BDF98 File Offset: 0x000BC198
	protected override void Awake()
	{
		base.Awake();
		this.entities = new List<GameEntity>(64);
		this.gameEntityData = new List<GameEntityData>(64);
		this.netIdToIndex = new Dictionary<int, int>(16384);
		this.netIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.zoneIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.createdItemTypeCount = new Dictionary<int, int>();
		this.zoneStateData = new GameEntityManager.ZoneStateData
		{
			zoneStateRequests = new List<GameEntityManager.ZoneStateRequest>(),
			zonePlayers = new List<Player>(),
			recievedStateBytes = new byte[15360],
			numRecievedStateBytes = 0
		};
		this.BuildFactory();
		this.guard.AddCallbackTarget(this);
		this.netIdsForCreate = new List<int>();
		this.entityTypeIdsForCreate = new List<int>();
		this.zoneIdsForCreate = new List<int>();
		this.packedPositionsForCreate = new List<long>();
		this.packedRotationsForCreate = new List<int>();
		this.createDataForCreate = new List<long>();
		this.netIdsForDelete = new List<int>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<long>();
		this.zoneComponents = new List<IGameEntityZoneComponent>(8);
		if (this.ghostReactorManager != null)
		{
			this.zoneComponents.Add(this.ghostReactorManager);
		}
		if (this.customMapsManager != null)
		{
			this.zoneComponents.Add(this.customMapsManager);
		}
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000BE0FB File Offset: 0x000BC2FB
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.netIds.Dispose();
		this.zoneIds.Dispose();
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000BE11C File Offset: 0x000BC31C
	private void Update()
	{
		this.UpdateZoneState();
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.netIdsForCreate.Count > 0 && Time.time > this.lastCreateSent + this.createCooldown)
		{
			this.lastCreateSent = Time.time;
			this.photonView.RPC("CreateItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForCreate.ToArray(),
				this.zoneIdsForCreate.ToArray(),
				this.entityTypeIdsForCreate.ToArray(),
				this.packedPositionsForCreate.ToArray(),
				this.packedRotationsForCreate.ToArray(),
				this.createDataForCreate.ToArray()
			});
			this.netIdsForCreate.Clear();
			this.zoneIdsForCreate.Clear();
			this.entityTypeIdsForCreate.Clear();
			this.packedPositionsForCreate.Clear();
			this.packedRotationsForCreate.Clear();
			this.createDataForCreate.Clear();
		}
		if (this.netIdsForDelete.Count > 0 && Time.time > this.lastDestroySent + this.destroyCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("DestroyItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForDelete.ToArray()
			});
			this.netIdsForDelete.Clear();
		}
		if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSent + this.stateCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("ApplyStateRPC", RpcTarget.All, new object[]
			{
				this.netIdsForState.ToArray(),
				this.statesForState.ToArray()
			});
			this.netIdsForState.Clear();
			this.statesForState.Clear();
		}
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000BE2EC File Offset: 0x000BC4EC
	public GameEntityId AddGameEntity(int netId, int zoneId, GameEntity gameEntity)
	{
		int num = this.FindNewEntityIndex();
		this.entities[num] = gameEntity;
		GameEntityData item = default(GameEntityData);
		this.gameEntityData.Add(item);
		gameEntity.id = new GameEntityId
		{
			index = num
		};
		this.netIdToIndex[netId] = num;
		this.netIds[num] = netId;
		this.zoneIds[num] = zoneId;
		return gameEntity.id;
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x000BE368 File Offset: 0x000BC568
	private int FindNewEntityIndex()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] == null)
			{
				return i;
			}
		}
		this.entities.Add(null);
		return this.entities.Count - 1;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000BE3BC File Offset: 0x000BC5BC
	public void RemoveGameEntity(GameEntity entity)
	{
		int index = entity.id.index;
		if (index < 0 || index > this.entities.Count)
		{
			return;
		}
		if (this.entities[index] == entity)
		{
			this.entities[index] = null;
		}
		CustomGameMode.OnGameEntityRemoved(entity);
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000BE40F File Offset: 0x000BC60F
	public List<GameEntity> GetGameEntities()
	{
		return this.entities;
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000BE418 File Offset: 0x000BC618
	public bool IsValidNetId(int netId)
	{
		int num;
		return this.netIdToIndex.TryGetValue(netId, out num) && num >= 0 && num < this.entities.Count;
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000BE44C File Offset: 0x000BC64C
	public int FindOpenIndex()
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (this.netIds[i] != -1)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000BE484 File Offset: 0x000BC684
	public GameEntityId GetEntityIdFromNetId(int netId)
	{
		int index;
		if (this.netIdToIndex.TryGetValue(netId, out index))
		{
			return new GameEntityId
			{
				index = index
			};
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000BE4B8 File Offset: 0x000BC6B8
	public int GetNetIdFromEntityId(GameEntityId id)
	{
		if (id.index < 0 || id.index >= this.netIds.Length)
		{
			return -1;
		}
		return this.netIds[id.index];
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000BE4E9 File Offset: 0x000BC6E9
	public virtual bool IsAuthority()
	{
		return !NetworkSystem.Instance.InRoom || this.guard.isTrulyMine;
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000BE504 File Offset: 0x000BC704
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return player != null && this.IsAuthorityPlayer(player.GetPlayerRef());
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000BE517 File Offset: 0x000BC717
	public bool IsAuthorityPlayer(Player player)
	{
		return player != null && this.guard.actualOwner != null && player == this.guard.actualOwner.GetPlayerRef();
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000BE53E File Offset: 0x000BC73E
	public bool IsZoneAuthority()
	{
		return this.IsAuthority();
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000BE546 File Offset: 0x000BC746
	public bool HasAuthority()
	{
		return this.GetAuthorityPlayer() != null;
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000BE551 File Offset: 0x000BC751
	public Player GetAuthorityPlayer()
	{
		if (this.guard.actualOwner != null)
		{
			return this.guard.actualOwner.GetPlayerRef();
		}
		return null;
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000BE572 File Offset: 0x000BC772
	public virtual bool IsZoneActive()
	{
		return this.zoneStateData.state == GameEntityManager.ZoneState.Active;
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000BE584 File Offset: 0x000BC784
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.zoneLimit == null || this.zoneLimit.bounds.Contains(pos);
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000BE5B5 File Offset: 0x000BC7B5
	public virtual bool IsValidClientRPC(Player sender)
	{
		return this.IsAuthorityPlayer(sender) && this.IsZoneActive();
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000BE5C8 File Offset: 0x000BC7C8
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.IsValidClientRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000BE5DC File Offset: 0x000BC7DC
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidClientRPC(sender, entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000BE5F1 File Offset: 0x000BC7F1
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.IsValidClientRPC(sender) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000BE605 File Offset: 0x000BC805
	public bool IsValidAuthorityRPC()
	{
		return this.IsAuthority() && this.IsZoneActive();
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000BE617 File Offset: 0x000BC817
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.IsValidAuthorityRPC() && this.IsValidNetId(entityNetId);
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000BE62A File Offset: 0x000BC82A
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000BE63E File Offset: 0x000BC83E
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.IsValidAuthorityRPC() && this.IsPositionInZone(pos);
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000BE651 File Offset: 0x000BC851
	public bool IsValidEntity(GameEntityId id)
	{
		return this.GetGameEntity(id) != null;
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000BE660 File Offset: 0x000BC860
	public GameEntity GetGameEntity(GameEntityId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		return this.GetGameEntity(id.index);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000BE67C File Offset: 0x000BC87C
	private GameEntity GetGameEntity(int index)
	{
		if (index == -1)
		{
			return null;
		}
		if (index < 0 || index >= this.entities.Count)
		{
			Debug.LogErrorFormat("Cannot Get Game Entity Index {0} {1}", new object[]
			{
				index,
				this.entities.Count
			});
			return null;
		}
		return this.entities[index];
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000BE6DC File Offset: 0x000BC8DC
	public T GetGameComponent<T>(GameEntityId id) where T : Component
	{
		GameEntity gameEntity = this.GetGameEntity(id);
		if (gameEntity == null)
		{
			return default(T);
		}
		return gameEntity.GetComponent<T>();
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000BE70C File Offset: 0x000BC90C
	private void BuildFactory()
	{
		this.itemPrefabFactory = new Dictionary<int, GameObject>(1024);
		this.priceLookupByEntityId = new Dictionary<int, int>();
		for (int i = 0; i < this.tempFactoryItems.Count; i++)
		{
			GameObject gameObject = this.tempFactoryItems[i].gameObject;
			int staticHash = gameObject.name.GetStaticHash();
			if (gameObject.GetComponent<GRToolLantern>())
			{
				this.priceLookupByEntityId.Add(staticHash, 50);
			}
			else if (gameObject.GetComponent<GRToolCollector>())
			{
				this.priceLookupByEntityId.Add(staticHash, 50);
			}
			Debug.LogFormat("Entity Factory {0} {1}", new object[]
			{
				gameObject.name,
				staticHash
			});
			this.itemPrefabFactory.Add(staticHash, gameObject);
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000BE7D5 File Offset: 0x000BC9D5
	private int CreateNetId()
	{
		int result = this.nextNetId;
		this.nextNetId++;
		return result;
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000BE7EC File Offset: 0x000BC9EC
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive() || !this.IsPositionInZone(position))
		{
			GTDev.LogError<string>(string.Format("[GameEntityManager::RequestCreateItem] Failed: Authority={0}, ", this.IsZoneAuthority()) + string.Format("ZoneActive={0}, PositionInZone={1}", this.IsZoneActive(), this.IsPositionInZone(position)), null);
			return GameEntityId.Invalid;
		}
		long item = BitPackUtils.PackWorldPosForNetwork(position);
		int item2 = BitPackUtils.PackQuaternionForNetwork(rotation);
		int num = this.CreateNetId();
		this.netIdsForCreate.Add(num);
		this.zoneIdsForCreate.Add((int)this.zone);
		this.entityTypeIdsForCreate.Add(entityTypeId);
		this.packedPositionsForCreate.Add(item);
		this.packedRotationsForCreate.Add(item2);
		this.createDataForCreate.Add(createData);
		return this.CreateItemLocal(num, (int)this.zone, entityTypeId, position, rotation, createData);
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000BE8D0 File Offset: 0x000BCAD0
	[PunRPC]
	public void CreateItemRPC(int[] netId, int[] zoneId, int[] entityTypeId, long[] packedPos, int[] packedRot, long[] createData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItem))
		{
			return;
		}
		if (netId == null || zoneId == null || entityTypeId == null || packedPos == null || createData == null || netId.Length != zoneId.Length || netId.Length != entityTypeId.Length || netId.Length != packedPos.Length || netId.Length != packedRot.Length || netId.Length != createData.Length)
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos[i]);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRot[i]);
			float num = 10000f;
			if (!vector.IsValid(num) || !rotation.IsValid() || !this.FactoryHasEntity(entityTypeId[i]) || !this.IsPositionInZone(vector))
			{
				return;
			}
			this.CreateItemLocal(netId[i], zoneId[i], entityTypeId[i], vector, rotation, createData[i]);
		}
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000BE9A0 File Offset: 0x000BCBA0
	public void RequestCreateItems(List<GameEntityCreateData> entityData)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive())
		{
			GTDev.LogError<string>(string.Format("[GameEntityManager::RequestCreateItems] Cannot create items. Zone Auth: {0} ", this.IsZoneAuthority()) + string.Format("| Zone Active: {0}", this.IsZoneActive()), null);
			return;
		}
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(GameEntityManager.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(entityData.Count);
		for (int i = 0; i < entityData.Count; i++)
		{
			GameEntityCreateData gameEntityCreateData = entityData[i];
			int value = this.CreateNetId();
			long value2 = BitPackUtils.PackWorldPosForNetwork(gameEntityCreateData.localPosition);
			int value3 = BitPackUtils.PackQuaternionForNetwork(gameEntityCreateData.localRotation);
			binaryWriter.Write(value);
			binaryWriter.Write(gameEntityCreateData.entityTypeId);
			binaryWriter.Write(value2);
			binaryWriter.Write(value3);
			binaryWriter.Write(gameEntityCreateData.createData);
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		this.photonView.RPC("CreateItemsRPC", RpcTarget.All, new object[]
		{
			(int)this.zone,
			array
		});
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000BEAC8 File Offset: 0x000BCCC8
	[PunRPC]
	public void CreateItemsRPC(int zoneId, byte[] stateData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || stateData == null || stateData.Length >= 15360 || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItems))
		{
			return;
		}
		try
		{
			byte[] array = GZipStream.UncompressBuffer(stateData);
			int num = array.Length;
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num2 = binaryReader.ReadInt32();
					for (int i = 0; i < num2; i++)
					{
						int netId = binaryReader.ReadInt32();
						int entityTypeId = binaryReader.ReadInt32();
						long data = binaryReader.ReadInt64();
						int data2 = binaryReader.ReadInt32();
						long createData = binaryReader.ReadInt64();
						Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
						Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
						float num3 = 10000f;
						if (vector.IsValid(num3) && rotation.IsValid() && this.FactoryHasEntity(entityTypeId) && this.IsPositionInZone(vector))
						{
							this.CreateItemLocal(netId, zoneId, entityTypeId, vector, rotation, createData);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000BEBE8 File Offset: 0x000BCDE8
	public bool FactoryHasEntity(int entityTypeId)
	{
		GameObject gameObject;
		return this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject);
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000BEC03 File Offset: 0x000BCE03
	public bool PriceLookup(int entityTypeId, out int price)
	{
		if (this.priceLookupByEntityId.TryGetValue(entityTypeId, out price))
		{
			return true;
		}
		price = -1;
		return false;
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000BEC1C File Offset: 0x000BCE1C
	private void ValidateThatNetIdIsNotAlreadyUsed(int netId, int newTypeId)
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (i < this.entities.Count && this.netIds[i] == netId)
			{
				if (this.entities[i] == null)
				{
					Debug.LogErrorFormat("Error creating entity type {0} Net Id {1} is being reused by null entity at index {2} next netid is {3}", new object[]
					{
						newTypeId,
						netId,
						i,
						this.nextNetId
					});
				}
				else
				{
					Debug.LogErrorFormat("Error creating entity type {0} Net Id {1} is being reused by {2} entity at index {3} {4}", new object[]
					{
						newTypeId,
						netId,
						this.entities[i].gameObject.name,
						i,
						this.nextNetId
					});
				}
			}
		}
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000BED0C File Offset: 0x000BCF0C
	public GameEntityId CreateItemLocal(int netId, int zoneId, int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		this.nextNetId = Mathf.Max(netId + 1, this.nextNetId);
		GameObject original;
		if (!this.itemPrefabFactory.TryGetValue(entityTypeId, out original))
		{
			Debug.LogFormat("[GameEntityManager::CreateItemLocal] Failed: Cannot find prefab for name {0}", new object[]
			{
				entityTypeId
			});
			return GameEntityId.Invalid;
		}
		if (!this.createdItemTypeCount.ContainsKey(entityTypeId))
		{
			this.createdItemTypeCount[entityTypeId] = 0;
		}
		if (this.createdItemTypeCount[entityTypeId] > 100)
		{
			return GameEntityId.Invalid;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int num = dictionary[entityTypeId];
		dictionary[entityTypeId] = num + 1;
		GameEntity componentInChildren = UnityEngine.Object.Instantiate<GameObject>(original, position, rotation).GetComponentInChildren<GameEntity>();
		GameEntityId result = this.AddGameEntity(netId, zoneId, componentInChildren);
		componentInChildren.Init(this, entityTypeId, createData);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].OnCreateGameEntity(componentInChildren);
		}
		return result;
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000BEDFA File Offset: 0x000BCFFA
	public void RequestDestroyItem(GameEntityId entityId)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(entityId)))
		{
			this.netIdsForDelete.Add(this.GetNetIdFromEntityId(entityId));
		}
		this.DestroyItemLocal(entityId);
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000BEE34 File Offset: 0x000BD034
	public void RequestDestroyItems(List<GameEntityId> entityIds)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < entityIds.Count; i++)
		{
			list.Add(this.GetNetIdFromEntityId(entityIds[i]));
		}
		this.photonView.RPC("DestroyItemRPC", RpcTarget.All, new object[]
		{
			list.ToArray()
		});
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000BEE94 File Offset: 0x000BD094
	[PunRPC]
	public void DestroyItemRPC(int[] entityNetId, PhotonMessageInfo info)
	{
		if (entityNetId == null || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.DestroyItem))
		{
			return;
		}
		for (int i = 0; i < entityNetId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, entityNetId[i]))
			{
				return;
			}
			this.DestroyItemLocal(this.GetEntityIdFromNetId(entityNetId[i]));
		}
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000BEEE4 File Offset: 0x000BD0E4
	public void DestroyItemLocal(GameEntityId entityId)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		if (!this.createdItemTypeCount.ContainsKey(gameEntity.typeId))
		{
			this.createdItemTypeCount[gameEntity.typeId] = 1;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int typeId = gameEntity.typeId;
		int num = dictionary[typeId];
		dictionary[typeId] = num - 1;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.ClearGrabbedIfHeld(gameEntity.id);
		}
		if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
		{
			GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id);
		}
		this.RemoveGameEntity(gameEntity);
		UnityEngine.Object.Destroy(gameEntity.gameObject);
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000BEFA8 File Offset: 0x000BD1A8
	public void RequestState(GameEntityId entityId, long newState)
	{
		this.photonView.RPC("RequestStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			newState
		});
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000BEFE0 File Offset: 0x000BD1E0
	[PunRPC]
	public void RequestStateRPC(int entityNetId, long newState, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(entityNetId))
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer.IsNull() || !gamePlayer.netStateLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		if (gameEntity == null || gameEntity.IsNull())
		{
			return;
		}
		bool flag = false;
		GRToolClub component = gameEntity.GetComponent<GRToolClub>();
		GRToolCollector component2 = gameEntity.GetComponent<GRToolCollector>();
		GRToolRevive component3 = gameEntity.GetComponent<GRToolRevive>();
		GRToolLantern component4 = gameEntity.GetComponent<GRToolLantern>();
		GRToolFlash component5 = gameEntity.GetComponent<GRToolFlash>();
		GRToolDirectionalShield component6 = gameEntity.GetComponent<GRToolDirectionalShield>();
		GRToolShieldGun component7 = gameEntity.GetComponent<GRToolShieldGun>();
		if (component == null && component2 == null && component3 == null && component4 == null && component5 == null && component6 == null && component7 == null)
		{
			flag = this.IsAuthorityPlayer(info.Sender);
		}
		bool flag2 = gamePlayer.IsHoldingEntity(entityIdFromNetId, false) || gamePlayer.IsHoldingEntity(entityIdFromNetId, true);
		bool flag3 = gameEntity.lastHeldByActorNumber == info.Sender.ActorNumber;
		if (!flag && (flag2 || flag3))
		{
			if (component4 != null)
			{
				flag = component4.CanChangeState(newState);
			}
			if (component5 != null)
			{
				flag = component5.CanChangeState(newState);
			}
			if (component != null || component2 != null || component3 != null || component6 != null || component7 != null)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.netIdsForState.Contains(entityNetId))
			{
				this.statesForState[this.netIdsForState.IndexOf(entityNetId)] = newState;
				return;
			}
			this.netIdsForState.Add(entityNetId);
			this.statesForState.Add(newState);
		}
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000BF1A0 File Offset: 0x000BD3A0
	[PunRPC]
	public void ApplyStateRPC(int[] netId, long[] newState, PhotonMessageInfo info)
	{
		if (netId == null || newState == null || netId.Length != newState.Length || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netId[i]))
			{
				return;
			}
			GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(netId[i]);
			this.entities[entityIdFromNetId.index].SetState(newState[i]);
		}
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000BF210 File Offset: 0x000BD410
	public void RequestGrabEntity(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		if (!this.IsAuthority())
		{
			this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
		this.photonView.RPC("RequestGrabEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(gameEntityId),
			isLeftHand,
			num
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000BF288 File Offset: 0x000BD488
	[PunRPC]
	public void RequestGrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(entityNetId))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!vector.IsValid(num) || !quaternion.IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null || !this.IsPlayerHandNearEntity(gamePlayer, entityNetId, isLeftHand, false, 16f) || this.IsValidEntity(gamePlayer.GetGameEntityId(isLeftHand)) || !gamePlayer.netGrabLimiter.CheckCallTime(Time.time) || gamePlayer.IsHoldingEntity(this, isLeftHand))
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(entityNetId));
		if (gameEntity == null)
		{
			return;
		}
		bool flag = gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != info.Sender.ActorNumber;
		bool flag2 = gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != info.Sender.ActorNumber && GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber) != null;
		if (!gameEntity.pickupable || flag2 || flag)
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("GrabEntityRPC", RpcTarget.All, new object[]
			{
				entityNetId,
				isLeftHand,
				packedPosRot,
				info.Sender
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000BF3EC File Offset: 0x000BD5EC
	[PunRPC]
	public void GrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.GrabEntity))
		{
			return;
		}
		Vector3 localPosition;
		Quaternion localRotation;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
		float num = 10000f;
		if (!localPosition.IsValid(num) || !localRotation.IsValid() || localPosition.sqrMagnitude > 6400f)
		{
			return;
		}
		this.GrabEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer));
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000BF464 File Offset: 0x000BD664
	private void GrabEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(grabbedByPlayer.ActorNumber), out rigContainer))
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		if (gameEntity == null)
		{
			return;
		}
		if (grabbedByPlayer == null)
		{
			return;
		}
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		if (grabbedByPlayer.IsLocal && gameEntity.heldByActorNumber == grabbedByPlayer.ActorNumber && gameEntity.heldByHandIndex == handIndex)
		{
			return;
		}
		GamePlayer gamePlayer = (gameEntity.heldByActorNumber < 0) ? null : GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		int num = (gamePlayer == null) ? -1 : gamePlayer.FindHandIndex(gameEntityId);
		bool flag = gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		if (gamePlayer != null)
		{
			gamePlayer.ClearGrabbedIfHeld(gameEntityId);
			if (num != -1 && flag)
			{
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
		}
		Transform handTransform = GamePlayer.GetHandTransform(rigContainer.Rig, handIndex);
		gameEntity.transform.SetParent(handTransform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GamePlayer gamePlayer2 = GamePlayer.GetGamePlayer(grabbedByPlayer.ActorNumber);
		gameEntity.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameEntity.heldByHandIndex = handIndex;
		gameEntity.lastHeldByActorNumber = gameEntity.heldByActorNumber;
		gamePlayer2.SetGrabbed(gameEntityId, handIndex);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.SetGrabbed(gameEntityId, GamePlayer.GetHandIndex(isLeftHand));
			GamePlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		gameEntity.PlayCatchFx();
		Action onGrabbed = gameEntity.OnGrabbed;
		if (onGrabbed == null)
		{
			return;
		}
		onGrabbed();
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000BF625 File Offset: 0x000BD825
	public void GrabEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, grabbedByPlayer);
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000BF634 File Offset: 0x000BD834
	public GameEntityId TryGrabLocal(Vector3 handPosition)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		GameEntityId result = GameEntityId.Invalid;
		float num = float.MaxValue;
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (!(this.entities[i] == null) && this.entities[i].pickupable && (this.entities[i].onlyGrabActorNumber == -1 || this.entities[i].onlyGrabActorNumber == actorNumber) && (this.entities[i].heldByActorNumber == -1 || this.entities[i].heldByActorNumber == actorNumber || !(GamePlayer.GetGamePlayer(this.entities[i].heldByActorNumber) != null)))
			{
				float sqrMagnitude = this.entities[i].GetVelocity().sqrMagnitude;
				double num2 = 0.0625;
				if (sqrMagnitude > 2f)
				{
					num2 = 0.25;
				}
				float sqrMagnitude2 = (handPosition - this.entities[i].transform.position).sqrMagnitude;
				if ((double)sqrMagnitude2 < num2 && sqrMagnitude2 < num)
				{
					result = this.entities[i].id;
					num = sqrMagnitude2;
				}
			}
		}
		return result;
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000BF798 File Offset: 0x000BD998
	public void RequestThrowEntity(GameEntityId entityId, bool isLeftHand, Vector3 headPosition, Vector3 velocity, Vector3 angVelocity)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 vector = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		Vector3 vector2 = vector - headPosition;
		float magnitude = vector2.magnitude;
		if (magnitude > 0f)
		{
			vector2 /= magnitude;
			int mask = LayerMask.GetMask(new string[]
			{
				"Default",
				"GorillaObject"
			});
			RaycastHit raycastHit;
			if (Physics.SphereCast(headPosition, 0.05f, vector2, out raycastHit, magnitude, mask, QueryTriggerInteraction.Ignore))
			{
				vector = headPosition + vector2 * (raycastHit.distance * 0.95f);
			}
		}
		if (!this.IsAuthority())
		{
			this.ThrowEntityLocal(entityId, isLeftHand, vector, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		this.photonView.RPC("RequestThrowEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			isLeftHand,
			vector,
			rotation,
			velocity,
			angVelocity
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000BF8C0 File Offset: 0x000BDAC0
	[PunRPC]
	public void RequestThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(entityNetId))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f && this.IsPositionInZone(position))
					{
						GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
						if (gamePlayer == null || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netThrowLimiter.CheckCallTime(Time.time))
						{
							return;
						}
						this.photonView.RPC("ThrowEntityRPC", RpcTarget.All, new object[]
						{
							entityNetId,
							isLeftHand,
							position,
							rotation,
							velocity,
							angVelocity,
							info.Sender,
							info.SentServerTime
						});
						PhotonNetwork.SendAllOutgoingCommands();
						return;
					}
				}
			}
		}
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000BF9E4 File Offset: 0x000BDBE4
	[PunRPC]
	public void ThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownByPlayer, double throwTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f)
					{
						this.ThrowEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(thrownByPlayer));
						return;
					}
				}
			}
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000BFA78 File Offset: 0x000BDC78
	private void ThrowEntityLocal(GameEntityId gameBallId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		if (gameBallId.index < 0 || gameBallId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameBallId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (thrownByPlayer == null)
		{
			return;
		}
		if (thrownByPlayer.IsLocal && gameEntity.heldByActorNumber != thrownByPlayer.ActorNumber && gameEntity.lastHeldByActorNumber == thrownByPlayer.ActorNumber)
		{
			return;
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.velocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GamePlayer gamePlayerRef = rigContainer.Rig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearGrabbedIfHeld(gameBallId);
			}
		}
		gameEntity.PlayThrowFx();
		Action onReleased = gameEntity.OnReleased;
		if (onReleased != null)
		{
			onReleased();
		}
		GRBadge component2 = gameEntity.GetComponent<GRBadge>();
		if (component2 != null)
		{
			GRPlayer grplayer = GRPlayer.Get(thrownByPlayer.ActorNumber);
			if (grplayer != null)
			{
				grplayer.AttachBadge(component2);
			}
		}
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000BFC1C File Offset: 0x000BDE1C
	public void RequestHit(GameHitData hit)
	{
		GameHittable gameComponent = this.GetGameComponent<GameHittable>(hit.hitEntityId);
		if (gameComponent == null)
		{
			return;
		}
		gameComponent.ApplyHit(hit);
		base.SendRPC("RequestHitRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(hit.hitEntityId),
			this.GetNetIdFromEntityId(hit.hitByEntityId),
			hit.hitTypeId,
			hit.hitEntityPosition,
			hit.hitPosition,
			hit.hitImpulse
		});
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000BFCC4 File Offset: 0x000BDEC4
	[PunRPC]
	public void RequestHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (entityPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitPosition.IsValid(num2))
			{
				float num3 = 10000f;
				if (hitImpulse.IsValid(num3) && this.IsValidAuthorityRPC(hittableNetId, entityPosition) && this.IsPositionInZone(hitPosition))
				{
					GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
					if (gamePlayer == null || !gamePlayer.netImpulseLimiter.CheckCallTime(Time.time))
					{
						return;
					}
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
					GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
					if (gameComponent == null)
					{
						return;
					}
					GameHitData hitData = new GameHitData
					{
						hitTypeId = hitTypeId,
						hitEntityId = entityIdFromNetId,
						hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
						hitEntityPosition = entityPosition,
						hitPosition = hitPosition,
						hitImpulse = hitImpulse
					};
					if (!gameComponent.IsHitValid(hitData))
					{
						return;
					}
					base.SendRPC("ApplyHitRPC", RpcTarget.All, new object[]
					{
						hittableNetId,
						hitByNetId,
						hitTypeId,
						entityPosition,
						hitPosition,
						hitImpulse,
						info.Sender
					});
					return;
				}
			}
		}
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000BFE04 File Offset: 0x000BE004
	[PunRPC]
	public void ApplyHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, Player player, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (hitPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitImpulse.IsValid(num2) && this.IsValidClientRPC(info.Sender, hittableNetId, entityPosition) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.HitEntity) && player != null)
			{
				if (player.IsLocal)
				{
					return;
				}
				if (this.GetGameEntity(this.GetEntityIdFromNetId(hittableNetId)) == null)
				{
					return;
				}
				hitImpulse = Vector3.ClampMagnitude(hitImpulse, 100f);
				GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
				GameHitData hitData = new GameHitData
				{
					hitTypeId = hitTypeId,
					hitEntityId = entityIdFromNetId,
					hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
					hitEntityPosition = entityPosition,
					hitPosition = hitPosition,
					hitImpulse = hitImpulse,
					hitAmount = 1
				};
				GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(hitByNetId));
				if (gameEntity != null)
				{
					GRTool component = gameEntity.GetComponent<GRTool>();
					if (hitTypeId == 0)
					{
						hitData.hitAmount = component.attributes.CalculateFinalValueForAttribute(GRAttributeType.BatonDamage);
					}
					else if (hitTypeId == 1)
					{
						hitData.hitAmount = component.attributes.CalculateFinalValueForAttribute(GRAttributeType.FlashDamage);
					}
				}
				GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
				if (gameComponent != null)
				{
					gameComponent.ApplyHit(hitData);
				}
				return;
			}
		}
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000BFF48 File Offset: 0x000BE148
	public bool IsPlayerHandNearEntity(GamePlayer player, int entityNetId, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && GameEntityManager.IsPlayerHandNearPosition(player, gameEntity.transform.position, isLeftHand, checkBothHands, acceptableRadius);
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000BFF88 File Offset: 0x000BE188
	public static bool IsPlayerHandNearPosition(GamePlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		bool flag = true;
		if (player != null && player.rig != null)
		{
			if (isLeftHand || checkBothHands)
			{
				flag = ((worldPosition - player.rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
			}
			if (!isLeftHand || checkBothHands)
			{
				float sqrMagnitude = (worldPosition - player.rig.rightHandTransform.position).sqrMagnitude;
				flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
			}
		}
		return flag;
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000C0014 File Offset: 0x000BE214
	public bool IsEntityNearEntity(int entityNetId, int otherEntityNetId, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(otherEntityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && this.IsEntityNearPosition(entityNetId, gameEntity.transform.position, acceptableRadius);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000C0050 File Offset: 0x000BE250
	public bool IsEntityNearPosition(int entityNetId, Vector3 position, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && Vector3.SqrMagnitude(gameEntity.transform.position - position) < acceptableRadius * acceptableRadius;
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x000C0094 File Offset: 0x000BE294
	private void ClearZone(GameEntityManager.ZoneStateData zoneStateData)
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] != null)
			{
				UnityEngine.Object.Destroy(this.entities[i].gameObject);
			}
		}
		this.entities.Clear();
		this.gameEntityData.Clear();
		this.createdItemTypeCount.Clear();
		GamePlayer gamePlayerRef = VRRig.LocalRig.GamePlayerRef;
		if (gamePlayerRef != null)
		{
			gamePlayerRef.Clear();
		}
		for (int j = 0; j < this.zoneComponents.Count; j++)
		{
			this.zoneComponents[j].OnZoneClear(this.zoneClearReason);
		}
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000C014C File Offset: 0x000BE34C
	public int SerializeGameState(int zoneId, byte[] bytes, int maxBytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].SerializeZoneData(binaryWriter);
		}
		GameEntityManager.tempEntitiesToSerialize.Clear();
		for (int j = 0; j < this.entities.Count; j++)
		{
			GameEntity gameEntity = this.entities[j];
			if (!(gameEntity == null))
			{
				GameEntityManager.tempEntitiesToSerialize.Add(gameEntity);
			}
		}
		binaryWriter.Write(GameEntityManager.tempEntitiesToSerialize.Count);
		for (int k = 0; k < GameEntityManager.tempEntitiesToSerialize.Count; k++)
		{
			GameEntity gameEntity2 = GameEntityManager.tempEntitiesToSerialize[k];
			if (!(gameEntity2 == null))
			{
				int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity2.id);
				binaryWriter.Write(netIdFromEntityId);
				binaryWriter.Write(gameEntity2.typeId);
				binaryWriter.Write(gameEntity2.createData);
				binaryWriter.Write(gameEntity2.GetState());
				long value = BitPackUtils.PackWorldPosForNetwork(gameEntity2.transform.position);
				int value2 = BitPackUtils.PackQuaternionForNetwork(gameEntity2.transform.rotation);
				binaryWriter.Write(value);
				binaryWriter.Write(value2);
				int heldByActorNumber = gameEntity2.heldByActorNumber;
				binaryWriter.Write(heldByActorNumber);
				GameAgent component = gameEntity2.GetComponent<GameAgent>();
				bool flag = component != null;
				binaryWriter.Write(flag);
				if (flag)
				{
					long value3 = BitPackUtils.PackWorldPosForNetwork(component.navAgent.destination);
					binaryWriter.Write(value3);
					int value4 = (component.targetPlayer == null) ? -1 : component.targetPlayer.ActorNumber;
					binaryWriter.Write(value4);
				}
				byte b = (byte)gameEntity2.entitySerialize.Count;
				binaryWriter.Write(b);
				for (int l = 0; l < (int)b; l++)
				{
					gameEntity2.entitySerialize[l].OnGameEntitySerialize(binaryWriter);
				}
				for (int m = 0; m < this.zoneComponents.Count; m++)
				{
					this.zoneComponents[m].SerializeZoneEntityData(binaryWriter, gameEntity2);
				}
			}
		}
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
		int count = GameEntityManager.tempRigs.Count;
		binaryWriter.Write(count);
		for (int n = 0; n < GameEntityManager.tempRigs.Count; n++)
		{
			VRRig vrrig = GameEntityManager.tempRigs[n];
			NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
			binaryWriter.Write(owningNetPlayer.ActorNumber);
			GamePlayer gamePlayerRef = vrrig.GamePlayerRef;
			bool flag2 = gamePlayerRef != null;
			binaryWriter.Write(flag2);
			if (flag2)
			{
				gamePlayerRef.SerializeNetworkState(binaryWriter, owningNetPlayer, this);
			}
			GRPlayer component2 = vrrig.GetComponent<GRPlayer>();
			bool flag3 = component2 != null;
			binaryWriter.Write(flag3);
			if (flag3)
			{
				component2.SerializeNetworkState(binaryWriter, owningNetPlayer);
			}
		}
		return (int)memoryStream.Position;
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000C043C File Offset: 0x000BE63C
	public void DeserializeTableState(int zoneId, byte[] bytes, int numBytes)
	{
		if (numBytes <= 0)
		{
			return;
		}
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				for (int i = 0; i < this.zoneComponents.Count; i++)
				{
					this.zoneComponents[i].DeserializeZoneData(binaryReader);
				}
				int num = binaryReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					int netId = binaryReader.ReadInt32();
					int entityTypeId = binaryReader.ReadInt32();
					long createData = binaryReader.ReadInt64();
					long state = binaryReader.ReadInt64();
					long data = binaryReader.ReadInt64();
					int data2 = binaryReader.ReadInt32();
					Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(data);
					Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
					GameEntityId id = this.CreateItemLocal(netId, zoneId, entityTypeId, position, rotation, createData);
					this.GetNetIdFromEntityId(id);
					GameEntity gameEntity = this.GetGameEntity(id);
					if (!(gameEntity == null))
					{
						gameEntity.SetState(state);
						binaryReader.ReadInt32();
						if (binaryReader.ReadBoolean())
						{
							long data3 = binaryReader.ReadInt64();
							int playerID = binaryReader.ReadInt32();
							Vector3 destination = BitPackUtils.UnpackWorldPosFromNetwork(data3);
							GameAgent component = gameEntity.GetComponent<GameAgent>();
							if (component != null)
							{
								if (component.IsOnNavMesh())
								{
									component.navAgent.destination = destination;
								}
								component.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
							}
						}
						byte b = binaryReader.ReadByte();
						for (int k = 0; k < (int)b; k++)
						{
							gameEntity.entitySerialize[k].OnGameEntityDeserialize(binaryReader);
						}
						for (int l = 0; l < this.zoneComponents.Count; l++)
						{
							this.zoneComponents[l].DeserializeZoneEntityData(binaryReader, gameEntity);
						}
					}
				}
				int num2 = binaryReader.ReadInt32();
				for (int m = 0; m < num2; m++)
				{
					int actorNumber = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						GamePlayer gamePlayer = GamePlayer.GetGamePlayer(actorNumber);
						GamePlayer.DeserializeNetworkState(binaryReader, gamePlayer, this);
					}
					if (binaryReader.ReadBoolean())
					{
						GRPlayer player = GRPlayer.Get(actorNumber);
						GRPlayer.DeserializeNetworkStateAndBurn(binaryReader, player, this.ghostReactorManager);
					}
				}
			}
		}
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000C0680 File Offset: 0x000BE880
	private void UpdateZoneState()
	{
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
		this.UpdateAuthority(this.zoneStateData, GameEntityManager.tempRigs);
		if (this.IsAuthority())
		{
			this.UpdateClientsFromAuthority(this.zoneStateData, GameEntityManager.tempRigs);
			this.UpdateZoneStateAuthority(this.zoneStateData);
		}
		else
		{
			this.UpdateZoneStateClient(this.zoneStateData);
		}
		for (int i = this.zoneStateData.zonePlayers.Count - 1; i >= 0; i--)
		{
			if (this.zoneStateData.zonePlayers[i] == null)
			{
				this.zoneStateData.zonePlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000C073C File Offset: 0x000BE93C
	private void UpdateAuthority(GameEntityManager.ZoneStateData zoneStateData, List<VRRig> allRigs)
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (!this.IsAuthority())
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
		}
		else if (this.IsAuthority() && !this.IsInZone(this.zone))
		{
			Player player = null;
			if (this.useRandomCheckForAuthority)
			{
				int num = 0;
				while (player == null)
				{
					if (num >= 10)
					{
						break;
					}
					num++;
					int index = Random.Range(0, allRigs.Count);
					VRRig vrrig = allRigs[index];
					GamePlayer gamePlayer = GamePlayer.GetGamePlayer(vrrig);
					if (!(gamePlayer == null) && !(gamePlayer.rig == null) && gamePlayer.rig.OwningNetPlayer != null && !gamePlayer.rig.isLocal && vrrig.zoneEntity.currentZone == this.zone)
					{
						player = gamePlayer.rig.OwningNetPlayer.GetPlayerRef();
					}
				}
			}
			else
			{
				for (int i = 0; i < allRigs.Count; i++)
				{
					VRRig vrrig2 = allRigs[i];
					GamePlayer gamePlayer2 = GamePlayer.GetGamePlayer(vrrig2);
					if (!(gamePlayer2 == null) && !(gamePlayer2.rig == null) && gamePlayer2.rig.OwningNetPlayer != null && !gamePlayer2.rig.isLocal && vrrig2.zoneEntity.currentZone == this.zone)
					{
						player = gamePlayer2.rig.OwningNetPlayer.GetPlayerRef();
					}
				}
			}
			if (player != null && player != null)
			{
				this.guard.TransferOwnership(player, "");
			}
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000C08D8 File Offset: 0x000BEAD8
	private void UpdateClientsFromAuthority(GameEntityManager.ZoneStateData zoneStateData, List<VRRig> allRigs)
	{
		if (!this.IsInZone(this.zone))
		{
			return;
		}
		for (int i = 0; i < zoneStateData.zoneStateRequests.Count; i++)
		{
			GameEntityManager.ZoneStateRequest zoneStateRequest = zoneStateData.zoneStateRequests[i];
			if (zoneStateRequest.player != null && zoneStateRequest.zone == this.zone)
			{
				this.SendZoneStateToPlayerOrTarget(zoneStateRequest.zone, zoneStateRequest.player, RpcTarget.MasterClient);
				zoneStateRequest.completed = true;
				zoneStateData.zoneStateRequests[i] = zoneStateRequest;
				zoneStateData.zoneStateRequests.RemoveAt(i);
				return;
			}
			zoneStateData.zoneStateRequests.RemoveAt(i);
			i--;
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000C0974 File Offset: 0x000BEB74
	public void TestSerializeTableState()
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		int num = this.SerializeGameState((int)this.zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		Debug.LogFormat("Test Serialize Game State Buffer Size Uncompressed {0}", new object[]
		{
			num
		});
		Debug.LogFormat("Test Serialize Game State Buffer Size Compressed {0}", new object[]
		{
			array.Length
		});
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000C09E4 File Offset: 0x000BEBE4
	public static void ClearByteBuffer(byte[] buffer)
	{
		int num = buffer.Length;
		for (int i = 0; i < num; i++)
		{
			buffer[i] = 0;
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000C0A08 File Offset: 0x000BEC08
	private void SendZoneStateToPlayerOrTarget(GTZone zone, Player player, RpcTarget target)
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		this.SerializeGameState((int)zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		byte[] array2 = new byte[512];
		int i = 0;
		int num = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			int num3 = Mathf.Min(512, num2 - i);
			Array.Copy(array, i, array2, 0, num3);
			if (player != null)
			{
				this.photonView.RPC("SendTableDataRPC", player, new object[]
				{
					num,
					num2,
					array2
				});
			}
			else
			{
				this.photonView.RPC("SendTableDataRPC", target, new object[]
				{
					num,
					num2,
					array2
				});
			}
			i += num3;
			num++;
		}
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000C0AE4 File Offset: 0x000BECE4
	[PunRPC]
	public void SendTableDataRPC(int packetNum, int totalBytes, byte[] bytes, PhotonMessageInfo info)
	{
		if (!this.IsAuthorityPlayer(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.SendTableData) || bytes == null || bytes.Length >= 15360)
		{
			return;
		}
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (packetNum == 0)
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
		}
		Array.Copy(bytes, 0, this.zoneStateData.recievedStateBytes, this.zoneStateData.numRecievedStateBytes, bytes.Length);
		this.zoneStateData.numRecievedStateBytes += bytes.Length;
		if (this.zoneStateData.numRecievedStateBytes >= totalBytes)
		{
			this.ClearZone(this.zoneStateData);
			try
			{
				byte[] array = GZipStream.UncompressBuffer(this.zoneStateData.recievedStateBytes);
				int numBytes = array.Length;
				this.DeserializeTableState((int)this.zone, array, numBytes);
				this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.Active);
				for (int j = 0; j < this.zoneComponents.Count; j++)
				{
					this.zoneComponents[j].OnZoneInit();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000C0C1C File Offset: 0x000BEE1C
	private void UpdateZoneStateAuthority(GameEntityManager.ZoneStateData zoneStateData)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(VRRig.LocalRig);
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone(this.zone) && zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
			this.zoneClearReason = ZoneClearReason.LeaveZone;
			return;
		}
		GameEntityManager.ZoneState state = zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - zoneStateData.stateStartTime > 1.0)
			{
				this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[]
				{
					(int)this.zone
				});
			}
		}
		else if (this.IsInZone(this.zone))
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.Active);
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				this.zoneComponents[i].OnZoneInit();
			}
			return;
		}
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x000C0D18 File Offset: 0x000BEF18
	private void UpdateZoneStateClient(GameEntityManager.ZoneStateData zoneStateData)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(VRRig.LocalRig);
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone(this.zone) && zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
			this.zoneClearReason = ZoneClearReason.LeaveZone;
			return;
		}
		GameEntityManager.ZoneState state = zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - zoneStateData.stateStartTime > 1.0)
			{
				this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[]
				{
					(int)this.zone
				});
			}
		}
		else if (this.HasAuthority() && this.IsInZone(this.zone) && !this.IsAuthority())
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToRequestState);
			return;
		}
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000C0DFC File Offset: 0x000BEFFC
	private bool IsInZone(GTZone zone)
	{
		bool flag = ZoneManagement.instance.IsZoneActive(zone);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag &= this.zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000C0E40 File Offset: 0x000BF040
	private void SetZoneState(GameEntityManager.ZoneStateData zoneStateData, GameEntityManager.ZoneState newState)
	{
		if (newState == zoneStateData.state)
		{
			return;
		}
		zoneStateData.state = newState;
		zoneStateData.stateStartTime = Time.timeAsDouble;
		Debug.LogFormat("Set new zone State {0} {1}", new object[]
		{
			this.zone,
			zoneStateData.state
		});
		switch (zoneStateData.state)
		{
		case GameEntityManager.ZoneState.WaitingToEnterZone:
			this.ClearZone(zoneStateData);
			return;
		case GameEntityManager.ZoneState.WaitingToRequestState:
			break;
		case GameEntityManager.ZoneState.WaitingForState:
			zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < zoneStateData.recievedStateBytes.Length; i++)
			{
				zoneStateData.recievedStateBytes[i] = 0;
			}
			return;
		case GameEntityManager.ZoneState.Active:
			GamePlayerLocal.instance.currGameEntityManager = this;
			break;
		default:
			return;
		}
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000C0EEC File Offset: 0x000BF0EC
	public void DebugSendState()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToRequestState);
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000C0EFC File Offset: 0x000BF0FC
	[PunRPC]
	public void RequestZoneStateRPC(int zoneId, PhotonMessageInfo info)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (zoneId != (int)this.zone || this.zoneStateData.zoneStateRequests == null)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null)
		{
			return;
		}
		if (!gamePlayer.newJoinZoneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			if (this.zoneStateData.zoneStateRequests[i].player == info.Sender)
			{
				return;
			}
		}
		this.zoneStateData.zoneStateRequests.Add(new GameEntityManager.ZoneStateRequest
		{
			player = info.Sender,
			zone = this.zone,
			completed = false
		});
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000C0FC7 File Offset: 0x000BF1C7
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
		this.zoneClearReason = ZoneClearReason.JoinZone;
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000C0FDD File Offset: 0x000BF1DD
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
		this.zoneClearReason = ZoneClearReason.LeaveZone;
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player newPlayer)
	{
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002D18 RID: 11544
	private const int MAX_STATE_BYTES = 15360;

	// Token: 0x04002D19 RID: 11545
	private const int MAX_CHUNK_BYTES = 512;

	// Token: 0x04002D1A RID: 11546
	public const float MAX_LOCAL_MAGNITUDE_SQ = 6400f;

	// Token: 0x04002D1B RID: 11547
	public const float MAX_DISTANCE_FROM_HAND = 16f;

	// Token: 0x04002D1C RID: 11548
	public const float MAX_ENTITY_DIST = 16f;

	// Token: 0x04002D1D RID: 11549
	public const float MAX_THROW_SPEED_SQ = 1600f;

	// Token: 0x04002D1E RID: 11550
	public const int MAX_ENTITY_COUNT_PER_TYPE = 100;

	// Token: 0x04002D1F RID: 11551
	public const int INVALID_ID = -1;

	// Token: 0x04002D20 RID: 11552
	public const int INVALID_INDEX = -1;

	// Token: 0x04002D21 RID: 11553
	public GTZone zone;

	// Token: 0x04002D22 RID: 11554
	public PhotonView photonView;

	// Token: 0x04002D23 RID: 11555
	public RequestableOwnershipGuard guard;

	// Token: 0x04002D24 RID: 11556
	public Player prevAuthorityPlayer;

	// Token: 0x04002D25 RID: 11557
	public BoxCollider zoneLimit;

	// Token: 0x04002D26 RID: 11558
	public bool useRandomCheckForAuthority;

	// Token: 0x04002D27 RID: 11559
	public GameAgentManager gameAgentManager;

	// Token: 0x04002D28 RID: 11560
	public GhostReactorManager ghostReactorManager;

	// Token: 0x04002D29 RID: 11561
	public CustomMapsGameManager customMapsManager;

	// Token: 0x04002D2A RID: 11562
	private List<IGameEntityZoneComponent> zoneComponents;

	// Token: 0x04002D2B RID: 11563
	private List<GameEntity> entities;

	// Token: 0x04002D2C RID: 11564
	private List<GameEntityData> gameEntityData;

	// Token: 0x04002D2D RID: 11565
	public List<GameEntity> tempFactoryItems;

	// Token: 0x04002D30 RID: 11568
	private Dictionary<int, GameObject> itemPrefabFactory;

	// Token: 0x04002D31 RID: 11569
	private Dictionary<int, int> priceLookupByEntityId;

	// Token: 0x04002D32 RID: 11570
	private List<GameEntity> tempEntities = new List<GameEntity>();

	// Token: 0x04002D33 RID: 11571
	private List<int> netIdsForCreate;

	// Token: 0x04002D34 RID: 11572
	private List<int> zoneIdsForCreate;

	// Token: 0x04002D35 RID: 11573
	private List<int> entityTypeIdsForCreate;

	// Token: 0x04002D36 RID: 11574
	private List<int> packedRotationsForCreate;

	// Token: 0x04002D37 RID: 11575
	private List<long> packedPositionsForCreate;

	// Token: 0x04002D38 RID: 11576
	private List<long> createDataForCreate;

	// Token: 0x04002D39 RID: 11577
	private float createCooldown = 0.24f;

	// Token: 0x04002D3A RID: 11578
	private float lastCreateSent;

	// Token: 0x04002D3B RID: 11579
	private List<int> netIdsForDelete;

	// Token: 0x04002D3C RID: 11580
	private float destroyCooldown = 0.25f;

	// Token: 0x04002D3D RID: 11581
	private float lastDestroySent;

	// Token: 0x04002D3E RID: 11582
	private List<int> netIdsForState;

	// Token: 0x04002D3F RID: 11583
	private List<long> statesForState;

	// Token: 0x04002D40 RID: 11584
	private float lastStateSent;

	// Token: 0x04002D41 RID: 11585
	private float stateCooldown;

	// Token: 0x04002D42 RID: 11586
	private Dictionary<int, int> netIdToIndex;

	// Token: 0x04002D43 RID: 11587
	private NativeArray<int> netIds;

	// Token: 0x04002D44 RID: 11588
	private NativeArray<int> zoneIds;

	// Token: 0x04002D45 RID: 11589
	private Dictionary<int, int> createdItemTypeCount;

	// Token: 0x04002D46 RID: 11590
	private ZoneClearReason zoneClearReason;

	// Token: 0x04002D47 RID: 11591
	private GameEntityManager.ZoneStateData zoneStateData;

	// Token: 0x04002D48 RID: 11592
	private int nextNetId = 1;

	// Token: 0x04002D49 RID: 11593
	public CallLimitersList<CallLimiter, GameEntityManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameEntityManager.RPC>();

	// Token: 0x04002D4A RID: 11594
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04002D4B RID: 11595
	private static List<GameEntity> tempEntitiesToSerialize = new List<GameEntity>(512);

	// Token: 0x04002D4C RID: 11596
	private static byte[] tempSerializeGameState = new byte[15360];

	// Token: 0x020005AE RID: 1454
	// (Invoke) Token: 0x060023F3 RID: 9203
	public delegate void ZoneStartEvent(GTZone zoneId);

	// Token: 0x020005AF RID: 1455
	// (Invoke) Token: 0x060023F7 RID: 9207
	public delegate void ZoneClearEvent(GTZone zoneId);

	// Token: 0x020005B0 RID: 1456
	private enum ZoneState
	{
		// Token: 0x04002D4E RID: 11598
		WaitingToEnterZone,
		// Token: 0x04002D4F RID: 11599
		WaitingToRequestState,
		// Token: 0x04002D50 RID: 11600
		WaitingForState,
		// Token: 0x04002D51 RID: 11601
		Active
	}

	// Token: 0x020005B1 RID: 1457
	private struct ZoneStateRequest
	{
		// Token: 0x04002D52 RID: 11602
		public Player player;

		// Token: 0x04002D53 RID: 11603
		public GTZone zone;

		// Token: 0x04002D54 RID: 11604
		public bool completed;
	}

	// Token: 0x020005B2 RID: 1458
	private class ZoneStateData
	{
		// Token: 0x04002D55 RID: 11605
		public GameEntityManager.ZoneState state;

		// Token: 0x04002D56 RID: 11606
		public double stateStartTime;

		// Token: 0x04002D57 RID: 11607
		public List<GameEntityManager.ZoneStateRequest> zoneStateRequests;

		// Token: 0x04002D58 RID: 11608
		public List<Player> zonePlayers;

		// Token: 0x04002D59 RID: 11609
		public byte[] recievedStateBytes;

		// Token: 0x04002D5A RID: 11610
		public int numRecievedStateBytes;
	}

	// Token: 0x020005B3 RID: 1459
	public enum RPC
	{
		// Token: 0x04002D5C RID: 11612
		CreateItem,
		// Token: 0x04002D5D RID: 11613
		CreateItems,
		// Token: 0x04002D5E RID: 11614
		DestroyItem,
		// Token: 0x04002D5F RID: 11615
		ApplyState,
		// Token: 0x04002D60 RID: 11616
		GrabEntity,
		// Token: 0x04002D61 RID: 11617
		ThrowEntity,
		// Token: 0x04002D62 RID: 11618
		SendTableData,
		// Token: 0x04002D63 RID: 11619
		HitEntity
	}
}
