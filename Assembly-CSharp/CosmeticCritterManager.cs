using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000574 RID: 1396
public class CosmeticCritterManager : NetworkSceneObject
{
	// Token: 0x1700036D RID: 877
	// (get) Token: 0x0600221D RID: 8733 RVA: 0x000B8B33 File Offset: 0x000B6D33
	// (set) Token: 0x0600221E RID: 8734 RVA: 0x000B8B3A File Offset: 0x000B6D3A
	public static CosmeticCritterManager Instance { get; private set; }

	// Token: 0x0600221F RID: 8735 RVA: 0x000B8B42 File Offset: 0x000B6D42
	public void RegisterLocalHoldable(CosmeticCritterHoldable holdable)
	{
		this.localHoldables.Add(holdable);
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000B8B50 File Offset: 0x000B6D50
	public void RegisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.AddIfNew(spawner);
			return;
		}
		this.remoteCritterSpawners.AddIfNew(spawner);
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000B8B73 File Offset: 0x000B6D73
	public void UnregisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.Remove(spawner);
			return;
		}
		this.remoteCritterSpawners.Remove(spawner);
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000B8B98 File Offset: 0x000B6D98
	public void RegisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.AddIfNew(catcher);
			return;
		}
		this.remoteCritterCatchers.AddIfNew(catcher);
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000B8BBB File Offset: 0x000B6DBB
	public void UnregisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.Remove(catcher);
			return;
		}
		this.remoteCritterCatchers.Remove(catcher);
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000B8BE0 File Offset: 0x000B6DE0
	public void RegisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (!this.tickForEachCritterOfType.TryGetValue(type, out list) || list == null)
		{
			list = new List<ICosmeticCritterTickForEach>();
			this.tickForEachCritterOfType.Add(type, list);
		}
		list.AddIfNew(target);
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000B8C1C File Offset: 0x000B6E1C
	public void UnregisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (this.tickForEachCritterOfType.TryGetValue(type, out list) && list != null)
		{
			list.Remove(target);
		}
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x000B8C44 File Offset: 0x000B6E44
	private void ResetLocalCallLimiters()
	{
		int i = 0;
		while (i < this.localHoldables.Count)
		{
			if (this.localHoldables[i] == null)
			{
				this.localHoldables.RemoveAt(i);
			}
			else
			{
				this.localHoldables[i].ResetCallLimiter();
				i++;
			}
		}
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000B8C9C File Offset: 0x000B6E9C
	private void ResetCosmeticCritters(NetPlayer player)
	{
		if (NetworkSystem.Instance.LocalPlayer != player)
		{
			return;
		}
		this.ResetLocalCallLimiters();
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			this.FreeCritter(this.activeCritters[i]);
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000B8CE8 File Offset: 0x000B6EE8
	private void Awake()
	{
		if (CosmeticCritterManager.Instance != null && CosmeticCritterManager.Instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		CosmeticCritterManager.Instance = this;
		this.localHoldables = new List<CosmeticCritterHoldable>();
		this.localCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.remoteCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.localCritterCatchers = new List<CosmeticCritterCatcher>();
		this.remoteCritterCatchers = new List<CosmeticCritterCatcher>();
		this.activeCritters = new List<CosmeticCritter>();
		this.activeCrittersPerType = new Dictionary<Type, int>();
		this.activeCrittersBySeed = new Dictionary<int, CosmeticCritter>();
		this.inactiveCrittersByType = new Dictionary<Type, Stack<CosmeticCritter>>();
		this.tickForEachCritterOfType = new Dictionary<Type, List<ICosmeticCritterTickForEach>>();
		NetworkSystem.Instance.OnPlayerJoined += this.ResetCosmeticCritters;
		NetworkSystem.Instance.OnPlayerLeft += this.ResetCosmeticCritters;
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000B8DCC File Offset: 0x000B6FCC
	private void ReuseOrSpawnNewCritter(CosmeticCritterSpawner spawner, int seed, double time)
	{
		Type critterType = spawner.GetCritterType();
		Stack<CosmeticCritter> stack;
		CosmeticCritter component;
		if (!this.inactiveCrittersByType.TryGetValue(critterType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(critterType, stack);
			component = UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		else if (stack.TryPop(out component))
		{
			component.gameObject.SetActive(true);
		}
		else
		{
			component = UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		component.SetSeedSpawnerTypeAndTime(seed, spawner, critterType, time);
		this.activeCritters.Add(component);
		if (!this.activeCrittersPerType.ContainsKey(critterType))
		{
			this.activeCrittersPerType.Add(critterType, 1);
		}
		else
		{
			Dictionary<Type, int> dictionary = this.activeCrittersPerType;
			Type key = critterType;
			dictionary[key]++;
		}
		this.activeCrittersBySeed.Add(seed, component);
		Random.State state = Random.state;
		Random.InitState(seed);
		spawner.SetRandomVariables(component);
		component.SetRandomVariables();
		Random.state = state;
		spawner.OnSpawn(component);
		component.OnSpawn();
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x000B8ED4 File Offset: 0x000B70D4
	private void FreeCritter(CosmeticCritter critter)
	{
		critter.OnDespawn();
		if (critter.Spawner != null)
		{
			critter.Spawner.OnDespawn(critter);
		}
		critter.gameObject.SetActive(false);
		Type cachedType = critter.CachedType;
		Stack<CosmeticCritter> stack;
		if (!this.inactiveCrittersByType.TryGetValue(cachedType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(cachedType, stack);
		}
		stack.Push(critter);
		this.activeCritters.Remove(critter);
		int num;
		if (this.activeCrittersPerType.TryGetValue(cachedType, out num))
		{
			this.activeCrittersPerType[cachedType] = Math.Max(num - 1, 0);
		}
		this.activeCrittersBySeed.Remove(critter.Seed);
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000B8F84 File Offset: 0x000B7184
	private void Update()
	{
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			CosmeticCritter cosmeticCritter = this.activeCritters[i];
			if (cosmeticCritter.Expired())
			{
				this.FreeCritter(cosmeticCritter);
			}
			else
			{
				cosmeticCritter.Tick();
				List<ICosmeticCritterTickForEach> list;
				if (this.tickForEachCritterOfType.TryGetValue(cosmeticCritter.CachedType, out list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].TickForEachCritter(cosmeticCritter);
					}
				}
				int k = 0;
				while (k < this.localCritterCatchers.Count)
				{
					CosmeticCritterCatcher cosmeticCritterCatcher = this.localCritterCatchers[k];
					CosmeticCritterAction localCatchAction = cosmeticCritterCatcher.GetLocalCatchAction(cosmeticCritter);
					if (localCatchAction != CosmeticCritterAction.None)
					{
						double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble;
						cosmeticCritterCatcher.OnCatch(cosmeticCritter, localCatchAction, num);
						if ((localCatchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
						{
							this.FreeCritter(cosmeticCritter);
							i--;
						}
						if ((localCatchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null)
						{
							this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), cosmeticCritter.Seed + 1, num);
						}
						if (PhotonNetwork.InRoom && (localCatchAction & CosmeticCritterAction.RPC) != CosmeticCritterAction.None)
						{
							this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[]
							{
								localCatchAction,
								cosmeticCritterCatcher.OwnerID,
								cosmeticCritter.Seed
							});
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
		}
		for (int l = 0; l < this.localCritterSpawners.Count; l++)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.localCritterSpawners[l];
			int num2;
			if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num2) || num2 < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnLocal())
			{
				int num3 = Random.Range(0, int.MaxValue);
				if (!this.activeCrittersBySeed.ContainsKey(num3))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, num3, PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
					if (PhotonNetwork.InRoom)
					{
						this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[]
						{
							CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn,
							cosmeticCritterSpawnerIndependent.OwnerID,
							num3
						});
					}
				}
			}
		}
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000B91CC File Offset: 0x000B73CC
	[PunRPC]
	private void CosmeticCritterRPC(CosmeticCritterAction action, int holdableID, int seed, PhotonMessageInfo info)
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "CosmeticCritterRPC");
		if ((action & CosmeticCritterAction.RPC) == CosmeticCritterAction.None)
		{
			return;
		}
		if (action == (CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn))
		{
			this.SpawnCosmeticCritterRPC(holdableID, seed, photonMessageInfoWrapped);
			return;
		}
		this.CatchCosmeticCritterRPC(action, holdableID, seed, photonMessageInfoWrapped);
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000B920C File Offset: 0x000B740C
	private void CatchCosmeticCritterRPC(CosmeticCritterAction catchAction, int catcherID, int seed, PhotonMessageInfoWrapped info)
	{
		CosmeticCritter critter;
		if (!this.activeCrittersBySeed.TryGetValue(seed, out critter))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterCatchers.Count)
		{
			CosmeticCritterCatcher cosmeticCritterCatcher = this.remoteCritterCatchers[i];
			if (cosmeticCritterCatcher.OwnerID == catcherID)
			{
				if (!cosmeticCritterCatcher.OwningPlayerMatches(info))
				{
					return;
				}
				if (cosmeticCritterCatcher.ValidateRemoteCatchAction(critter, catchAction, info.SentServerTime))
				{
					cosmeticCritterCatcher.OnCatch(critter, catchAction, info.SentServerTime);
					if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
					{
						this.FreeCritter(critter);
					}
					int num;
					if ((catchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null && (!this.activeCrittersPerType.TryGetValue(cosmeticCritterCatcher.GetLinkedSpawner().GetCritterType(), out num) || num < cosmeticCritterCatcher.GetLinkedSpawner().GetCritter().GetGlobalMaxCritters() + 1))
					{
						this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), seed + 1, info.SentServerTime);
					}
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000B92F0 File Offset: 0x000B74F0
	private void SpawnCosmeticCritterRPC(int spawnerID, int seed, PhotonMessageInfoWrapped info)
	{
		if (this.activeCrittersBySeed.ContainsKey(seed))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterSpawners.Count)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.remoteCritterSpawners[i];
			if (cosmeticCritterSpawnerIndependent.OwnerID == spawnerID)
			{
				if (!cosmeticCritterSpawnerIndependent.OwningPlayerMatches(info))
				{
					return;
				}
				int num;
				if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num) || num < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnRemote(info.SentServerTime))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, seed, info.SentServerTime);
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04002B9F RID: 11167
	private List<CosmeticCritterHoldable> localHoldables;

	// Token: 0x04002BA0 RID: 11168
	private List<CosmeticCritterSpawnerIndependent> localCritterSpawners;

	// Token: 0x04002BA1 RID: 11169
	private List<CosmeticCritterSpawnerIndependent> remoteCritterSpawners;

	// Token: 0x04002BA2 RID: 11170
	private List<CosmeticCritterCatcher> localCritterCatchers;

	// Token: 0x04002BA3 RID: 11171
	private List<CosmeticCritterCatcher> remoteCritterCatchers;

	// Token: 0x04002BA4 RID: 11172
	private List<CosmeticCritter> activeCritters;

	// Token: 0x04002BA5 RID: 11173
	private Dictionary<Type, int> activeCrittersPerType;

	// Token: 0x04002BA6 RID: 11174
	private Dictionary<int, CosmeticCritter> activeCrittersBySeed;

	// Token: 0x04002BA7 RID: 11175
	private Dictionary<Type, Stack<CosmeticCritter>> inactiveCrittersByType;

	// Token: 0x04002BA8 RID: 11176
	private Dictionary<Type, List<ICosmeticCritterTickForEach>> tickForEachCritterOfType;
}
