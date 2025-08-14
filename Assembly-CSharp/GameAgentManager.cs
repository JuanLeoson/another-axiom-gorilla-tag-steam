using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
[NetworkBehaviourWeaved(0)]
public class GameAgentManager : NetworkComponent
{
	// Token: 0x06002320 RID: 8992 RVA: 0x000BD0C4 File Offset: 0x000BB2C4
	protected override void Awake()
	{
		this.agents = new List<GameAgent>(128);
		this.netIdsForDestination = new List<int>();
		this.destinationsForDestination = new List<Vector3>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<byte>();
		this.netIdsForBehavior = new List<int>();
		this.behaviorsForBehavior = new List<byte>();
		this.nextAgentIndexUpdate = 0;
		this.nextAgentIndexThink = 0;
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000BD131 File Offset: 0x000BB331
	public static GameAgentManager Get(GameEntity gameEntity)
	{
		if (!(gameEntity == null) && !(gameEntity.manager == null))
		{
			return gameEntity.manager.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000BD157 File Offset: 0x000BB357
	public List<GameAgent> GetAgents()
	{
		return this.agents;
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000BD15F File Offset: 0x000BB35F
	public int GetGameAgentCount()
	{
		return this.agents.Count;
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000BD16C File Offset: 0x000BB36C
	public void AddGameAgent(GameAgent gameAgent)
	{
		this.agents.Add(gameAgent);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000BD17A File Offset: 0x000BB37A
	public void RemoveGameAgent(GameAgent gameAgent)
	{
		this.agents.Remove(gameAgent);
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000BD189 File Offset: 0x000BB389
	public GameAgent GetGameAgent(GameEntityId id)
	{
		return this.entityManager.GetGameEntity(id).GetComponent<GameAgent>();
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000BD19C File Offset: 0x000BB39C
	public void Update()
	{
		if (this.IsAuthority())
		{
			int num = Mathf.Min(1, this.agents.Count);
			for (int i = 0; i < num; i++)
			{
				if (this.nextAgentIndexThink >= this.agents.Count)
				{
					this.nextAgentIndexThink = 0;
				}
				this.agents[this.nextAgentIndexThink].OnThink(Time.deltaTime);
				this.nextAgentIndexThink++;
			}
		}
		for (int j = 0; j < this.agents.Count; j++)
		{
			if (this.agents[j] != null)
			{
				this.agents[j].OnUpdate();
			}
		}
		if (this.IsAuthority())
		{
			if (this.netIdsForDestination.Count > 0 && Time.time > this.lastDestinationSentTime + this.destinationCooldown)
			{
				this.lastDestinationSentTime = Time.time;
				base.SendRPC("ApplyDestinationRPC", RpcTarget.All, new object[]
				{
					this.netIdsForDestination.ToArray(),
					this.destinationsForDestination.ToArray()
				});
				this.netIdsForDestination.Clear();
				this.destinationsForDestination.Clear();
			}
			if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSentTime + this.stateCooldown)
			{
				this.lastStateSentTime = Time.time;
				base.SendRPC("ApplyStateRPC", RpcTarget.All, new object[]
				{
					this.netIdsForState.ToArray(),
					this.statesForState.ToArray()
				});
				this.netIdsForState.Clear();
				this.statesForState.Clear();
			}
			if (this.netIdsForBehavior.Count > 0 && Time.time > this.lastBehaviorSentTime + this.behaviorCooldown)
			{
				this.lastBehaviorSentTime = Time.time;
				base.SendRPC("ApplyBehaviorRPC", RpcTarget.All, new object[]
				{
					this.netIdsForBehavior.ToArray(),
					this.behaviorsForBehavior.ToArray()
				});
				this.netIdsForBehavior.Clear();
				this.behaviorsForBehavior.Clear();
			}
		}
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000BD3AB File Offset: 0x000BB5AB
	public bool IsAuthority()
	{
		return this.entityManager.IsAuthority();
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000BD3B8 File Offset: 0x000BB5B8
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000BD3C6 File Offset: 0x000BB5C6
	public bool IsAuthorityPlayer(Player player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000BD3D4 File Offset: 0x000BB5D4
	public Player GetAuthorityPlayer()
	{
		return this.entityManager.GetAuthorityPlayer();
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000BD3E1 File Offset: 0x000BB5E1
	public bool IsZoneActive()
	{
		return this.entityManager.IsZoneActive();
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000BD3EE File Offset: 0x000BB5EE
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.entityManager.IsPositionInZone(pos);
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000BD3FC File Offset: 0x000BB5FC
	public bool IsValidClientRPC(Player sender)
	{
		return this.entityManager.IsValidClientRPC(sender);
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000BD40A File Offset: 0x000BB60A
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000BD419 File Offset: 0x000BB619
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000BD429 File Offset: 0x000BB629
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000BD438 File Offset: 0x000BB638
	public bool IsValidAuthorityRPC()
	{
		return this.entityManager.IsValidAuthorityRPC();
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000BD445 File Offset: 0x000BB645
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.entityManager.IsValidAuthorityRPC(entityNetId);
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000BD453 File Offset: 0x000BB653
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(entityNetId, pos);
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000BD462 File Offset: 0x000BB662
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(pos);
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000BD470 File Offset: 0x000BB670
	public void RequestDestination(GameAgent agent, Vector3 dest)
	{
		if (!this.IsAuthority())
		{
			Debug.LogError("RequestDestination should only be called from the master client");
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForDestination.Contains(netIdFromEntityId))
		{
			this.destinationsForDestination[this.netIdsForDestination.IndexOf(netIdFromEntityId)] = dest;
			return;
		}
		this.netIdsForDestination.Add(netIdFromEntityId);
		this.destinationsForDestination.Add(dest);
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000BD4E8 File Offset: 0x000BB6E8
	[PunRPC]
	public void ApplyDestinationRPC(int[] netEntityId, Vector3[] dest, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyDestination))
		{
			return;
		}
		if (netEntityId == null || dest == null || netEntityId.Length != dest.Length)
		{
			return;
		}
		int i = 0;
		while (i < netEntityId.Length)
		{
			if (this.IsValidClientRPC(info.Sender, netEntityId[i], dest[i]))
			{
				int num = i;
				float num2 = 10000f;
				if (dest[num].IsValid(num2))
				{
					i++;
					continue;
				}
			}
			return;
		}
		for (int j = 0; j < netEntityId.Length; j++)
		{
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[j]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.ApplyDestination(dest[j]);
		}
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000BD5AC File Offset: 0x000BB7AC
	public void RequestState(GameAgent agent, byte state)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = state;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(state);
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000BD618 File Offset: 0x000BB818
	[PunRPC]
	public void ApplyStateRPC(int[] netEntityId, byte[] state, PhotonMessageInfo info)
	{
		if (netEntityId == null || state == null || netEntityId.Length != state.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.OnBodyStateChanged(state[i]);
		}
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000BD6A0 File Offset: 0x000BB8A0
	public void RequestBehavior(GameAgent agent, byte behavior)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForBehavior.Contains(netIdFromEntityId))
		{
			this.behaviorsForBehavior[this.netIdsForBehavior.IndexOf(netIdFromEntityId)] = behavior;
			return;
		}
		this.netIdsForBehavior.Add(netIdFromEntityId);
		this.behaviorsForBehavior.Add(behavior);
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000BD70C File Offset: 0x000BB90C
	[PunRPC]
	public void ApplyBehaviorRPC(int[] netEntityId, byte[] behavior, PhotonMessageInfo info)
	{
		if (netEntityId == null || behavior == null || netEntityId.Length != behavior.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyBehaviour))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component != null)
			{
				component.OnBehaviorStateChanged(behavior[i]);
			}
		}
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000BD794 File Offset: 0x000BB994
	public void RequestTarget(GameAgent agent, NetPlayer player)
	{
		if (player == agent.targetPlayer)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.targetPlayer = player;
		base.SendRPC("ApplyTargetRPC", RpcTarget.Others, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			(player == null) ? null : player.GetPlayerRef()
		});
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000BD804 File Offset: 0x000BBA04
	[PunRPC]
	public void ApplyTargetRPC(int agentNetId, Player player, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, agentNetId) || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget) || player == null)
		{
			return;
		}
		GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
		if (gameEntity == null)
		{
			return;
		}
		GameAgent component = gameEntity.GetComponent<GameAgent>();
		if (component == null)
		{
			return;
		}
		component.targetPlayer = player;
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000BD874 File Offset: 0x000BBA74
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		int num = Mathf.Min(4, this.agents.Count);
		stream.SendNext(num);
		for (int i = 0; i < num; i++)
		{
			if (this.nextAgentIndexUpdate >= this.agents.Count)
			{
				this.nextAgentIndexUpdate = 0;
			}
			stream.SendNext(this.entityManager.GetNetIdFromEntityId(this.agents[this.nextAgentIndexUpdate].entity.id));
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.agents[this.nextAgentIndexUpdate].transform.position);
			stream.SendNext(num2);
			int num3 = BitPackUtils.PackQuaternionForNetwork(this.agents[this.nextAgentIndexUpdate].transform.rotation);
			stream.SendNext(num3);
			this.nextAgentIndexUpdate++;
		}
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000BD964 File Offset: 0x000BBB64
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int netId = (int)stream.ReceiveNext();
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork((int)stream.ReceiveNext());
			if (this.IsPositionInZone(vector) && this.entityManager.IsValidNetId(netId))
			{
				GameEntityId entityIdFromNetId = this.entityManager.GetEntityIdFromNetId(netId);
				GameAgent gameAgent = this.GetGameAgent(entityIdFromNetId);
				if (gameAgent != null)
				{
					gameAgent.ApplyNetworkUpdate(vector, rotation);
				}
			}
		}
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002CD5 RID: 11477
	public GameEntityManager entityManager;

	// Token: 0x04002CD6 RID: 11478
	public PhotonView photonView;

	// Token: 0x04002CD7 RID: 11479
	private List<GameAgent> agents;

	// Token: 0x04002CD8 RID: 11480
	private float lastDestinationSentTime;

	// Token: 0x04002CD9 RID: 11481
	private float destinationCooldown;

	// Token: 0x04002CDA RID: 11482
	private List<int> netIdsForDestination;

	// Token: 0x04002CDB RID: 11483
	private List<Vector3> destinationsForDestination;

	// Token: 0x04002CDC RID: 11484
	private List<int> netIdsForState;

	// Token: 0x04002CDD RID: 11485
	private List<byte> statesForState;

	// Token: 0x04002CDE RID: 11486
	private float lastStateSentTime;

	// Token: 0x04002CDF RID: 11487
	private float stateCooldown;

	// Token: 0x04002CE0 RID: 11488
	private List<int> netIdsForBehavior;

	// Token: 0x04002CE1 RID: 11489
	private List<byte> behaviorsForBehavior;

	// Token: 0x04002CE2 RID: 11490
	private float lastBehaviorSentTime;

	// Token: 0x04002CE3 RID: 11491
	private float behaviorCooldown = 0.25f;

	// Token: 0x04002CE4 RID: 11492
	private const int MAX_UPDATES_PER_FRAME = 4;

	// Token: 0x04002CE5 RID: 11493
	private int nextAgentIndexUpdate;

	// Token: 0x04002CE6 RID: 11494
	private const int MAX_THINK_PER_FRAME = 1;

	// Token: 0x04002CE7 RID: 11495
	private int nextAgentIndexThink;

	// Token: 0x04002CE8 RID: 11496
	public CallLimitersList<CallLimiter, GameAgentManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameAgentManager.RPC>();

	// Token: 0x020005A1 RID: 1441
	public enum RPC
	{
		// Token: 0x04002CEA RID: 11498
		ApplyDestination,
		// Token: 0x04002CEB RID: 11499
		ApplyState,
		// Token: 0x04002CEC RID: 11500
		ApplyBehaviour,
		// Token: 0x04002CED RID: 11501
		ApplyImpact,
		// Token: 0x04002CEE RID: 11502
		ApplyTarget
	}
}
