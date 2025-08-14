using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005DB RID: 1499
[NetworkBehaviourWeaved(0)]
public class GhostReactorManager : NetworkComponent, IGameEntityZoneComponent
{
	// Token: 0x060024A9 RID: 9385 RVA: 0x000C5504 File Offset: 0x000C3704
	protected override void Awake()
	{
		base.Awake();
		GhostReactorManager.instance = this;
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000C5512 File Offset: 0x000C3712
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x0005BDE3 File Offset: 0x00059FE3
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000C5520 File Offset: 0x000C3720
	public bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000C552D File Offset: 0x000C372D
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000C553B File Offset: 0x000C373B
	private bool IsAuthorityPlayer(Player player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000C5549 File Offset: 0x000C3749
	private Player GetAuthorityPlayer()
	{
		return this.gameEntityManager.GetAuthorityPlayer();
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x000C5556 File Offset: 0x000C3756
	public bool IsZoneActive()
	{
		return this.gameEntityManager.IsZoneActive();
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x000C5563 File Offset: 0x000C3763
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.gameEntityManager.IsPositionInZone(pos);
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x000C5571 File Offset: 0x000C3771
	public bool IsValidClientRPC(Player sender)
	{
		return this.gameEntityManager.IsValidClientRPC(sender);
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x000C557F File Offset: 0x000C377F
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x000C558E File Offset: 0x000C378E
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000C559E File Offset: 0x000C379E
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000C55AD File Offset: 0x000C37AD
	public bool IsValidAuthorityRPC()
	{
		return this.gameEntityManager.IsValidAuthorityRPC();
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000C55BA File Offset: 0x000C37BA
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(entityNetId);
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000C55C8 File Offset: 0x000C37C8
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(entityNetId, pos);
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000C55D7 File Offset: 0x000C37D7
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(pos);
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000C55E5 File Offset: 0x000C37E5
	public static GhostReactorManager Get(GameEntity gameEntity)
	{
		if (gameEntity == null || gameEntity.manager == null)
		{
			return null;
		}
		return gameEntity.manager.ghostReactorManager;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000C560C File Offset: 0x000C380C
	public void RequestCollectItem(GameEntityId collectibleEntityId, GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestCollectItemRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000C5660 File Offset: 0x000C3860
	public void RequestDepositCollectible(GameEntityId collectibleEntityId)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				-1,
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000C56D0 File Offset: 0x000C38D0
	[PunRPC]
	public void RequestCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(collectibleEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestCollectItemLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsEntityNearEntity(collectibleEntityNetId, collectorEntityNetId, 16f))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				collectibleEntityNetId,
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000C5778 File Offset: 0x000C3978
	[PunRPC]
	public void ApplyCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyCollectItem))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(collectingPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity2 != null)
			{
				GRToolCollector component2 = gameEntity2.GetComponent<GRToolCollector>();
				if (component2 != null && component2.tool != null)
				{
					component2.PerformCollection(component);
					bool sentientCore = false;
					this.ReportCoreCollection(sentientCore);
				}
			}
			else
			{
				if (component.unlockPointsCollectible)
				{
					grplayer.unlockPointsCurrency += component.energyValue;
					grplayer.sentientCoresCollected += component.energyValue;
					if (this.reactor.distillery != null)
					{
						this.reactor.distillery.DepositCore();
					}
				}
				else
				{
					grplayer.currency += component.energyValue;
					grplayer.coresCollected += component.energyValue;
				}
				this.reactor.RefreshScoreboards();
				this.ReportCoreCollection(component.unlockPointsCollectible);
			}
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000C591C File Offset: 0x000C3B1C
	public void RequestChargeTool(GameEntityId collectorEntityId, GameEntityId targetToolId)
	{
		this.photonView.RPC("RequestChargeToolRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(targetToolId)
		});
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000C5970 File Offset: 0x000C3B70
	[PunRPC]
	public void RequestChargeToolRPC(int collectorEntityNetId, int targetToolNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC() || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId) || !this.gameEntityManager.IsEntityNearEntity(collectorEntityNetId, targetToolNetId, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), collectorEntityNetId, false, true, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), targetToolNetId, false, true, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestChargeToolLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyChargeToolRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				targetToolNetId,
				info.Sender
			});
		}
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000C5A64 File Offset: 0x000C3C64
	[PunRPC]
	public void ApplyChargeToolRPC(int collectorEntityNetId, int targetToolNetId, Player collectingPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyChargeTool) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId))
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(targetToolNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity != null && gameEntity2 != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				if (component != null && component.tool != null && component2 != null)
				{
					int b = Mathf.Max(component2.GetEnergyMax() - component2.energy, 0);
					int num = Mathf.Min(Mathf.Min(component.tool.energy, 100), b);
					if (num > 0)
					{
						component.tool.SetEnergy(component.tool.energy - num);
						component2.RefillEnergy(num, entityIdFromNetId);
						component.PlayChargeEffect(component2);
					}
				}
			}
		}
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000C5B96 File Offset: 0x000C3D96
	public void RequestDepositCurrency(GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestDepositCurrencyRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000C5BC8 File Offset: 0x000C3DC8
	[PunRPC]
	public void RequestDepositCurrencyRPC(int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(collectorEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestDepositCurrencyLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
		this.gameEntityManager.GetGameEntity(entityIdFromNetId);
		if (this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), collectorEntityNetId, false, true, 16f) & (grplayer.transform.position - this.reactor.currencyDepositor.transform.position).magnitude < 16f)
		{
			this.photonView.RPC("ApplyDepositCurrencyRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000C5CB4 File Offset: 0x000C3EB4
	[PunRPC]
	public void ApplyDepositCurrencyRPC(int collectorEntityNetId, int targetPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectorEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyDepositCurrency))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(targetPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null && grplayer != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				if (component != null && component.tool != null)
				{
					int energy = component.tool.energy;
					if (energy > 0)
					{
						grplayer.currency += energy;
						grplayer.coresCollected += energy;
						component.tool.SetEnergy(0);
						this.reactor.RefreshScoreboards();
						component.PlayChargeEffect(this.reactor.currencyDepositor);
					}
				}
			}
		}
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000C5DA6 File Offset: 0x000C3FA6
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition
		});
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000C5DE8 File Offset: 0x000C3FE8
	[PunRPC]
	private void ApplyEnemyHitPlayerRPC(GhostReactor.EnemyType type, int entityNetId, Vector3 hitPosition, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidNetId(entityNetId))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.applyEnemyHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnEnemyHitPlayerInternal(type, entityIdFromNetId, grplayer, hitPosition);
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000C5E49 File Offset: 0x000C4049
	private void OnEnemyHitPlayerInternal(GhostReactor.EnemyType type, GameEntityId entityId, GRPlayer player, Vector3 hitPosition)
	{
		if (type == GhostReactor.EnemyType.Chaser || type == GhostReactor.EnemyType.Phantom || type == GhostReactor.EnemyType.Ranged)
		{
			player.OnPlayerHit(hitPosition, this);
		}
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000C5E5F File Offset: 0x000C405F
	public void ReportLocalPlayerHit()
	{
		base.GetView.RPC("ReportLocalPlayerHitRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000C5E78 File Offset: 0x000C4078
	[PunRPC]
	private void ReportLocalPlayerHitRPC(PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.reportLocalHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000C5EBC File Offset: 0x000C40BC
	public void RequestPlayerRevive(GRReviveStation reviveStation, GRPlayer player)
	{
		if ((NetworkSystem.Instance.InRoom && this.IsAuthority()) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("ApplyPlayerRevivedRPC", RpcTarget.All, new object[]
			{
				reviveStation.Index,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber
			});
		}
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000C5F2C File Offset: 0x000C412C
	[PunRPC]
	private void ApplyPlayerRevivedRPC(int reviveStationIndex, int playerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyPlayerRevived))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (reviveStationIndex < 0 || reviveStationIndex >= this.reactor.reviveStations.Count)
		{
			return;
		}
		GRReviveStation grreviveStation = this.reactor.reviveStations[reviveStationIndex];
		if (grreviveStation == null)
		{
			return;
		}
		grreviveStation.RevivePlayer(grplayer);
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000C5FA4 File Offset: 0x000C41A4
	public void RequestPlayerStateChange(GRPlayer player, GRPlayer.GRPlayerState newState)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("PlayerStateChangeRPC", RpcTarget.All, new object[]
			{
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
				(int)newState
			});
			return;
		}
		player.ChangePlayerState(newState, this);
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000C6004 File Offset: 0x000C4204
	[PunRPC]
	private void PlayerStateChangeRPC(int playerActorNumber, int newState, PhotonMessageInfo info)
	{
		bool flag = this.IsValidClientRPC(info.Sender);
		bool flag2 = newState == 1 && info.Sender.ActorNumber == playerActorNumber;
		bool flag3 = newState == 0 && flag;
		if (!flag2 && !flag3)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || grplayer2.IsNull() || !grplayer2.playerStateChangeLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState((GRPlayer.GRPlayerState)newState, this);
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000C6088 File Offset: 0x000C4288
	public void RequestGrantPlayerShield(GRPlayer player, int shieldHp)
	{
		base.GetView.RPC("RequestGrantPlayerShieldRPC", this.GetAuthorityPlayer(), new object[]
		{
			player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
			shieldHp
		});
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x000C60D8 File Offset: 0x000C42D8
	[PunRPC]
	private void RequestGrantPlayerShieldRPC(int playerToGrantShieldActorNumber, int shieldHp, PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (!this.IsValidAuthorityRPC() || grplayer.IsNull() || !grplayer.fireShieldLimiter.CheckCallTime(Time.unscaledTime) || grplayer2.IsNull() || !grplayer2.CanActivateShield(shieldHp))
		{
			return;
		}
		base.GetView.RPC("ApplyGrantPlayerShieldRPC", RpcTarget.All, new object[]
		{
			playerToGrantShieldActorNumber,
			shieldHp
		});
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000C615C File Offset: 0x000C435C
	[PunRPC]
	private void ApplyGrantPlayerShieldRPC(int playerToGrantShieldActorNumber, int shieldHp, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.GrantPlayerShield))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (grplayer == null)
		{
			return;
		}
		grplayer.TryActivateShield(shieldHp);
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000C61A0 File Offset: 0x000C43A0
	public void RequestFireProjectile(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if ((NetworkSystem.Instance.InRoom && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("RequestFireProjectileRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(entityId),
				firingPosition,
				targetPosition,
				networkTime
			});
		}
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000C6220 File Offset: 0x000C4420
	[PunRPC]
	private void RequestFireProjectileRPC(int entityNetId, Vector3 firingPosition, Vector3 targetPosition, double networkTime, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId, targetPosition) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RequestFireProjectile) || !this.gameEntityManager.IsEntityNearPosition(entityNetId, firingPosition, 16f))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		this.OnRequestFireProjectileInternal(entityIdFromNetId, firingPosition, targetPosition, networkTime);
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000C627C File Offset: 0x000C447C
	private void OnRequestFireProjectileInternal(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		GREnemyRanged gameComponent = this.gameEntityManager.GetGameComponent<GREnemyRanged>(entityId);
		if (gameComponent != null)
		{
			gameComponent.RequestRangedAttack(firingPosition, targetPosition, networkTime);
		}
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000C62A9 File Offset: 0x000C44A9
	public void RequestShiftStart()
	{
		this.photonView.RPC("RequestShiftStartRPC", this.GetAuthorityPlayer(), Array.Empty<object>());
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000C62C8 File Offset: 0x000C44C8
	[PunRPC]
	public void RequestShiftStartRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			double time = PhotonNetwork.Time;
			SRand srand = new SRand(Mathf.FloorToInt(Time.time * 100f));
			int num = srand.NextInt(0, int.MaxValue);
			string text = Guid.NewGuid().ToString();
			this.photonView.RPC("ApplyShiftStartRPC", RpcTarget.All, new object[]
			{
				time,
				num,
				text
			});
		}
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000C63A8 File Offset: 0x000C45A8
	[PunRPC]
	public void ApplyShiftStartRPC(double shiftStartTime, int randomSeed, string gameIdGuid, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftStartTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		double num = PhotonNetwork.Time - shiftStartTime;
		if (num < 0.0 || num > 10.0)
		{
			return;
		}
		levelGenerator.Generate(randomSeed);
		if (this.gameEntityManager.IsAuthority())
		{
			if (this.activeSpawnSectionEntitiesCoroutine != null)
			{
				base.StopCoroutine(this.activeSpawnSectionEntitiesCoroutine);
			}
			this.activeSpawnSectionEntitiesCoroutine = base.StartCoroutine(this.SpawnSectionEntitiesCoroutine());
		}
		shiftManager.shiftStats.ResetShiftStats();
		shiftManager.ResetJudgment();
		shiftManager.RefreshShiftStatsDisplay();
		shiftManager.OnShiftStarted(gameIdGuid, shiftStartTime, true);
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000C647D File Offset: 0x000C467D
	private IEnumerator SpawnSectionEntitiesCoroutine()
	{
		int initialFrameCount = Time.frameCount;
		while (initialFrameCount == Time.frameCount)
		{
			yield return this.spawnSectionEntitiesWait;
		}
		if (this.gameEntityManager.IsAuthority())
		{
			this.reactor.levelGenerator.SpawnEntitiesInEachSection();
		}
		yield break;
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000C648C File Offset: 0x000C468C
	public void RequestShiftEnd()
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		GhostReactorManager.tempEntitiesToDestroy.Clear();
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			GameEntity gameEntity = gameEntities[i];
			if (gameEntity != null && !this.ShouldEntitySurviveShift(gameEntity))
			{
				GhostReactorManager.tempEntitiesToDestroy.Add(gameEntity.id);
			}
		}
		this.gameEntityManager.RequestDestroyItems(GhostReactorManager.tempEntitiesToDestroy);
		this.photonView.RPC("ApplyShiftEndRPC", RpcTarget.Others, new object[]
		{
			PhotonNetwork.Time
		});
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(PhotonNetwork.Time, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000C6594 File Offset: 0x000C4794
	[PunRPC]
	public void ApplyShiftEndRPC(double networkedTime, PhotonMessageInfo info)
	{
		if (!double.IsFinite(networkedTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftEnd))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(networkedTime, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000C662C File Offset: 0x000C482C
	private bool ShouldEntitySurviveShift(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return true;
		}
		if (this.reactor == null)
		{
			return false;
		}
		if (gameEntity.GetComponent<GREnemyChaser>() != null || gameEntity.GetComponent<GREnemyRanged>() != null || gameEntity.GetComponent<GREnemyPhantom>() != null || gameEntity.GetComponent<GREnemyPest>() != null)
		{
			return false;
		}
		Collider safeZoneLimit = this.reactor.safeZoneLimit;
		Vector3 position = gameEntity.gameObject.transform.position;
		return safeZoneLimit.bounds.Contains(position) || gameEntity.GetComponent<GRBadge>() != null;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000C66D0 File Offset: 0x000C48D0
	public void ReportEnemyDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.EnemyDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRKillEnemy", 1);
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000C6708 File Offset: 0x000C4908
	public void ReportCoreCollection(bool sentientCore)
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		if (sentientCore)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.SentientCoresCollected);
		}
		else
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
		}
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRCollectCore", 1);
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000C675E File Offset: 0x000C495E
	public void ReportPlayerDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.PlayerDeaths);
		shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000C678B File Offset: 0x000C498B
	public void PromotionBotActivePlayerRequest(int promotionBotAction)
	{
		this.photonView.RPC("PromotionBotActivePlayerRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			promotionBotAction
		});
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000C67B4 File Offset: 0x000C49B4
	[PunRPC]
	public void PromotionBotActivePlayerRequestRPC(int promotionBotAction, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.promotionBotLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (promotionBot == null)
		{
			return;
		}
		if (!GRUIPromotionBot.ValidBotAction((GRUIPromotionBot.BotActions)promotionBotAction))
		{
			return;
		}
		promotionBot.PlayerInteraction(grplayer, (GRUIPromotionBot.BotActions)promotionBotAction, false);
		this.photonView.RPC("PromotionBotActivePlayerResponseRPC", RpcTarget.Others, new object[]
		{
			info.Sender.ActorNumber,
			promotionBotAction
		});
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000C6860 File Offset: 0x000C4A60
	[PunRPC]
	public void PromotionBotActivePlayerResponseRPC(int actorNumber, int promotionBotAction, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		if (grplayer == null || promotionBot == null || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.PromotionBotResponse) || !GRUIPromotionBot.ValidBotAction((GRUIPromotionBot.BotActions)promotionBotAction))
		{
			return;
		}
		promotionBot.PlayerInteraction(grplayer, (GRUIPromotionBot.BotActions)promotionBotAction, false);
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000C68D0 File Offset: 0x000C4AD0
	[PunRPC]
	public void BroadcastScoreboardPage(int scoreboardPage, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.scoreboardPageLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (GRUIScoreboard.ValidPage((GRUIScoreboard.ScoreboardScreen)scoreboardPage))
		{
			GhostReactor.instance.UpdateScoreboardScreen((GRUIScoreboard.ScoreboardScreen)scoreboardPage);
		}
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000C692C File Offset: 0x000C4B2C
	[PunRPC]
	public void BroadcastStartingProgression(int points, int redeemedPoints, double shiftJoinedTime, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftJoinedTime) || double.IsInfinity(shiftJoinedTime))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.progressionBroadcastLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.SetProgressionData(points, redeemedPoints, false);
		grplayer.shiftJoinTime = Math.Clamp(shiftJoinedTime, PhotonNetwork.Time - 10.0, PhotonNetwork.Time);
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000C69B0 File Offset: 0x000C4BB0
	public void ToolPurchaseStationRequest(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action)
	{
		this.photonView.RPC("ToolPurchaseStationRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			stationIndex,
			action
		});
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x000C69E0 File Offset: 0x000C4BE0
	[PunRPC]
	public void ToolPurchaseStationRequestRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidAuthorityRPC() || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestToolPurchaseStationLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (action)
		{
		case GhostReactorManager.ToolPurchaseStationAction.ShiftLeft:
			grtoolPurchaseStation.ShiftLeftAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.ShiftRight:
			grtoolPurchaseStation.ShiftRightAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.TryPurchase:
		{
			bool flag = false;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetNetPlayerByID(info.Sender.ActorNumber), out rigContainer))
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				int num;
				if (component != null && grtoolPurchaseStation.TryPurchaseAuthority(component, out num))
				{
					this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
					{
						stationIndex,
						GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded,
						info.Sender.ActorNumber,
						num
					});
					this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded, info.Sender.ActorNumber, num);
					flag = true;
				}
			}
			if (!flag)
			{
				this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
				{
					stationIndex,
					GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed,
					info.Sender.ActorNumber,
					0
				});
				this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed, info.Sender.ActorNumber, 0);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000C6C20 File Offset: 0x000C4E20
	[PunRPC]
	public void ToolPurchaseStationResponseRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidClientRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolPurchaseResponse))
		{
			return;
		}
		this.ToolPurchaseResponseLocal(stationIndex, responseType, dataA, dataB);
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000C6C80 File Offset: 0x000C4E80
	private void ToolPurchaseResponseLocal(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (responseType)
		{
		case GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate:
			grtoolPurchaseStation.OnSelectionUpdate(dataA);
			return;
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded:
		{
			grtoolPurchaseStation.OnPurchaseSucceeded();
			GRPlayer grplayer = GRPlayer.Get(dataA);
			if (grplayer != null)
			{
				grplayer.coresSpentOnItems += dataB;
				if (!this.reactor.shiftManager.ShiftActive)
				{
					grplayer.coresSpentWhileWaiting += dataB;
				}
				grplayer.itemsPurchased.Add(grtoolPurchaseStation.GetCurrentToolName());
				grplayer.currency = Mathf.Max(grplayer.currency - dataB, 0);
				this.reactor.RefreshScoreboards();
				return;
			}
			break;
		}
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed:
			grtoolPurchaseStation.OnPurchaseFailed();
			break;
		default:
			return;
		}
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ToolUpgradeStationRequestUpgrade(string UpgradeID, int entityNetId)
	{
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x0001D558 File Offset: 0x0001B758
	private bool DoesUserHaveResearchUnlocked(int UserID, string ResearchID)
	{
		return true;
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ToolPlacedInUpgradeStation(GameEntity entity)
	{
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000023F5 File Offset: 0x000005F5
	public void UpgradeToolAtToolStation()
	{
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x000023F5 File Offset: 0x000005F5
	public void LocalEjectToolInUpgradeStation()
	{
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000C6D64 File Offset: 0x000C4F64
	public void EntityEnteredDropZone(GameEntity entity)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRUIStationEmployeeBadges employeeBadges = this.reactor.employeeBadges;
		long num = BitPackUtils.PackWorldPosForNetwork(entity.transform.position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(entity.transform.rotation);
		if (entity.gameObject.GetComponent<GRBadge>() != null)
		{
			GRUIEmployeeBadgeDispenser gruiemployeeBadgeDispenser = employeeBadges.badgeDispensers[entity.gameObject.GetComponent<GRBadge>().dispenserIndex];
			if (gruiemployeeBadgeDispenser != null)
			{
				num = BitPackUtils.PackWorldPosForNetwork(gruiemployeeBadgeDispenser.GetSpawnPosition());
				num2 = BitPackUtils.PackQuaternionForNetwork(gruiemployeeBadgeDispenser.GetSpawnRotation());
			}
		}
		this.photonView.RPC("EntityEnteredDropZoneRPC", RpcTarget.All, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id),
			num,
			num2
		});
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000C6E4C File Offset: 0x000C504C
	[PunRPC]
	public void EntityEnteredDropZoneRPC(int entityNetId, long position, int rotation, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.EntityEnteredDropZone))
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "EntityEnteredDropZoneRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
		float num = 10000f;
		if (!vector.IsValid(num))
		{
			return;
		}
		Quaternion rotation2 = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
		if (!rotation2.IsValid())
		{
			return;
		}
		if (!this.IsPositionInZone(vector))
		{
			return;
		}
		if ((vector - this.reactor.dropZone.transform.position).magnitude > 5f)
		{
			return;
		}
		this.LocalEntityEnteredDropZone(this.gameEntityManager.GetEntityIdFromNetId(entityNetId), vector, rotation2);
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000C6EF8 File Offset: 0x000C50F8
	private void LocalEntityEnteredDropZone(GameEntityId entityId, Vector3 position, Quaternion rotation)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRDropZone dropZone = this.reactor.dropZone;
		Vector3 velocity = dropZone.GetRepelDirectionWorld() * GhostReactor.DROP_ZONE_REPEL;
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityId);
		if (gameEntity.heldByActorNumber >= 0)
		{
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
			int handIndex = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
				GamePlayerLocal.instance.ClearGrabbed(handIndex);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		if (!(gameEntity.gameObject.GetComponent<GRBadge>() != null))
		{
			Rigidbody component = gameEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.position = position;
				component.rotation = rotation;
				component.velocity = velocity;
				component.angularVelocity = Vector3.zero;
			}
		}
		dropZone.PlayEffect();
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x000C7017 File Offset: 0x000C5217
	public void RequestRecycleScanItem(GRRecycler.GRToolType toolType)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		base.SendRPC("ApplRecycleScanItemRPC", RpcTarget.All, new object[]
		{
			toolType
		});
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x000C703D File Offset: 0x000C523D
	[PunRPC]
	public void ApplRecycleScanItemRPC(GRRecycler.GRToolType toolType, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplRecycleScanItem))
		{
			return;
		}
		this.reactor.recycler.ScanItem(toolType);
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x000C7078 File Offset: 0x000C5278
	public void RequestRecycleItem(int lastHeldActorNumber, GameEntityId toolId, GRRecycler.GRToolType toolType)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.gameEntityManager == null)
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(toolId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleItemRPC", RpcTarget.All, new object[]
		{
			lastHeldActorNumber,
			netIdFromEntityId,
			toolType
		});
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x000C70DC File Offset: 0x000C52DC
	[PunRPC]
	public void ApplyRecycleItemRPC(int lastHeldActorNumber, int toolNetId, GRRecycler.GRToolType toolType, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyRecycleItem) || !this.gameEntityManager.IsEntityNearPosition(toolNetId, this.reactor.recycler.transform.position, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(lastHeldActorNumber);
		if (grplayer != null)
		{
			grplayer.currency += this.reactor.recycler.GetRecycleValue(toolType);
		}
		this.reactor.RefreshScoreboards();
		this.reactor.recycler.RecycleItem();
		this.gameEntityManager.DestroyItemLocal(this.gameEntityManager.GetEntityIdFromNetId(toolNetId));
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000C7198 File Offset: 0x000C5398
	protected void OnNewPlayerEnteredGhostReactor()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityZoneClear(GTZone zoneId)
	{
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x000C71B4 File Offset: 0x000C53B4
	public void OnZoneInit()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
		GRPlayer grplayer = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			this.photonView.RPC("BroadcastStartingProgression", RpcTarget.All, new object[]
			{
				grplayer.CurrentProgression.points,
				grplayer.CurrentProgression.redeemedPoints,
				PhotonNetwork.Time
			});
		}
		if (this.reactor.employeeTerminal != null)
		{
			this.reactor.employeeTerminal.Setup();
		}
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000C7266 File Offset: 0x000C5466
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.levelGenerator.ClearLevelSections();
		this.reactor.shiftManager.OnShiftEnded(0.0, false, reason);
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000C72A2 File Offset: 0x000C54A2
	public bool IsZoneReady()
	{
		return this.reactor != null;
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000C72B0 File Offset: 0x000C54B0
	public void SerializeZoneData(BinaryWriter writer)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		writer.Write(this.reactor.depthLevel);
		writer.Write(shiftManager.ShiftActive);
		writer.Write(shiftManager.ShiftStartNetworkTime);
		shiftManager.shiftStats.Serialize(writer);
		writer.Write(shiftManager.ShiftId);
		writer.Write(levelGenerator.seed);
		writer.Write(promotionBot.GetCurrentPlayerActorNumber());
		writer.Write((int)promotionBot.currentAction);
		for (int i = 0; i < array.Length; i++)
		{
			writer.Write((int)array[i].currentScreen);
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		writer.Write(toolPurchasingStations.Count);
		for (int j = 0; j < toolPurchasingStations.Count; j++)
		{
			writer.Write(toolPurchasingStations[j].ActiveEntryIndex);
		}
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000C73BC File Offset: 0x000C55BC
	public void DeserializeZoneData(BinaryReader reader)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGeneratorV2 levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		int depthLevel = reader.ReadInt32();
		this.reactor.depthLevel = depthLevel;
		bool flag = reader.ReadBoolean();
		double shiftStartTime = reader.ReadDouble();
		shiftManager.shiftStats.Deserialize(reader);
		shiftManager.RefreshShiftStatsDisplay();
		string gameId = reader.ReadString();
		int inputSeed = reader.ReadInt32();
		if (flag)
		{
			levelGenerator.Generate(inputSeed);
			shiftManager.OnShiftStarted(gameId, shiftStartTime, false);
		}
		int actorNumber = reader.ReadInt32();
		int action = reader.ReadInt32();
		if (GRUIPromotionBot.ValidBotAction((GRUIPromotionBot.BotActions)action))
		{
			promotionBot.PlayerInteraction(GRPlayer.Get(actorNumber), (GRUIPromotionBot.BotActions)action, true);
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i].currentScreen = (GRUIScoreboard.ScoreboardScreen)reader.ReadInt32();
		}
		GhostReactor.instance.RefreshScoreboards();
		GhostReactor.instance.RefreshDepth();
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		int num = reader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			int newSelectedIndex = reader.ReadInt32();
			if (j < toolPurchasingStations.Count && toolPurchasingStations[j] != null)
			{
				toolPurchasingStations[j].OnSelectionUpdate(newSelectedIndex);
			}
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000023F5 File Offset: 0x000005F5
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000023F5 File Offset: 0x000005F5
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002501 RID: 9473 RVA: 0x00002076 File Offset: 0x00000276
	public static bool AggroDisabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002E7A RID: 11898
	private const string EVENT_CORE_COLLECTED = "GRCollectCore";

	// Token: 0x04002E7B RID: 11899
	private const string EVENT_ENEMY_KILLED = "GRKillEnemy";

	// Token: 0x04002E7C RID: 11900
	public const string EVENT_BREAKABLE_BROKEN = "GRSmashBreakable";

	// Token: 0x04002E7D RID: 11901
	public const string EVENT_ENEMY_ARMOR_BREAK = "GRArmorBreak";

	// Token: 0x04002E7E RID: 11902
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x04002E7F RID: 11903
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x04002E80 RID: 11904
	public GameEntityManager gameEntityManager;

	// Token: 0x04002E81 RID: 11905
	public GameAgentManager gameAgentManager;

	// Token: 0x04002E82 RID: 11906
	public static GhostReactorManager instance;

	// Token: 0x04002E83 RID: 11907
	public PhotonView photonView;

	// Token: 0x04002E84 RID: 11908
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x04002E85 RID: 11909
	public CallLimitersList<CallLimiter, GhostReactorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GhostReactorManager.RPC>();

	// Token: 0x04002E86 RID: 11910
	private Coroutine activeSpawnSectionEntitiesCoroutine;

	// Token: 0x04002E87 RID: 11911
	private WaitForSeconds spawnSectionEntitiesWait = new WaitForSeconds(0.1f);

	// Token: 0x04002E88 RID: 11912
	private static List<GameEntityId> tempEntitiesToDestroy = new List<GameEntityId>();

	// Token: 0x04002E89 RID: 11913
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x04002E8A RID: 11914
	public static bool entityDebugEnabled = false;

	// Token: 0x020005DC RID: 1500
	public enum RPC
	{
		// Token: 0x04002E8C RID: 11916
		ApplyCollectItem,
		// Token: 0x04002E8D RID: 11917
		ApplyChargeTool,
		// Token: 0x04002E8E RID: 11918
		ApplyDepositCurrency,
		// Token: 0x04002E8F RID: 11919
		ApplyPlayerRevived,
		// Token: 0x04002E90 RID: 11920
		GrantPlayerShield,
		// Token: 0x04002E91 RID: 11921
		RequestFireProjectile,
		// Token: 0x04002E92 RID: 11922
		ApplyShiftStart,
		// Token: 0x04002E93 RID: 11923
		ApplyShiftEnd,
		// Token: 0x04002E94 RID: 11924
		ToolPurchaseResponse,
		// Token: 0x04002E95 RID: 11925
		ApplyBreakableBroken,
		// Token: 0x04002E96 RID: 11926
		EntityEnteredDropZone,
		// Token: 0x04002E97 RID: 11927
		PromotionBotResponse,
		// Token: 0x04002E98 RID: 11928
		DistillItem,
		// Token: 0x04002E99 RID: 11929
		ApplySentientCoreDestination,
		// Token: 0x04002E9A RID: 11930
		ApplyRecycleItem,
		// Token: 0x04002E9B RID: 11931
		ApplRecycleScanItem
	}

	// Token: 0x020005DD RID: 1501
	public enum GRPlayerAction
	{
		// Token: 0x04002E9D RID: 11933
		ButtonShiftStart,
		// Token: 0x04002E9E RID: 11934
		DelveDeeper,
		// Token: 0x04002E9F RID: 11935
		DEBUG_ResetDepth,
		// Token: 0x04002EA0 RID: 11936
		DEBUG_DelveDeeper,
		// Token: 0x04002EA1 RID: 11937
		DEBUG_DelveShallower
	}

	// Token: 0x020005DE RID: 1502
	public enum ToolPurchaseStationAction
	{
		// Token: 0x04002EA3 RID: 11939
		ShiftLeft,
		// Token: 0x04002EA4 RID: 11940
		ShiftRight,
		// Token: 0x04002EA5 RID: 11941
		TryPurchase
	}

	// Token: 0x020005DF RID: 1503
	public enum ToolPurchaseStationResponse
	{
		// Token: 0x04002EA7 RID: 11943
		SelectionUpdate,
		// Token: 0x04002EA8 RID: 11944
		PurchaseSucceeded,
		// Token: 0x04002EA9 RID: 11945
		PurchaseFailed
	}
}
