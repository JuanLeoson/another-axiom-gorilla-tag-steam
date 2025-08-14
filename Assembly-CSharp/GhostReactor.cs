using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
public class GhostReactor : MonoBehaviour
{
	// Token: 0x06002452 RID: 9298 RVA: 0x000C2428 File Offset: 0x000C0628
	public static GhostReactor Get(GameEntity gameEntity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(gameEntity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return ghostReactorManager.reactor;
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000C2450 File Offset: 0x000C0650
	private void Awake()
	{
		GhostReactor.instance = this;
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
		this.vrRigs = new List<VRRig>();
		for (int j = 0; j < this.itemPurchaseStands.Count; j++)
		{
			if (this.itemPurchaseStands[j] == null)
			{
				Debug.LogErrorFormat("Null Item Purchase Stand {0}", new object[]
				{
					j
				});
			}
			else
			{
				this.itemPurchaseStands[j].Setup(j);
			}
		}
		for (int k = 0; k < this.toolPurchasingStations.Count; k++)
		{
			if (this.toolPurchasingStations[k] == null)
			{
				Debug.LogErrorFormat("Null Tool Purchasing Station {0}", new object[]
				{
					k
				});
			}
			else
			{
				this.toolPurchasingStations[k].PurchaseStationId = k;
			}
		}
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000C2574 File Offset: 0x000C0774
	private void OnEnable()
	{
		this.grManager = GhostReactorManager.instance;
		this.grManager.reactor = this;
		this.grManager.gameEntityManager.zoneLimit = this.zoneLimit;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		VRRigCache.OnRigActivated += this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated += this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged += this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnLocalPlayerConnectedToRoom;
		}
		for (int i = 0; i < this.toolPurchasingStations.Count; i++)
		{
			this.toolPurchasingStations[i].Init(this.grManager, this);
		}
		this.currencyDepositor.Init(this);
		this.levelGenerator.Init(this);
		this.employeeBadges.Init(this);
		this.shiftManager.Init(this.grManager);
		this.recycler.Init(this);
		this.RefreshDepth();
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000C2694 File Offset: 0x000C0894
	private void OnDisable()
	{
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
		VRRigCache.OnRigActivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged -= this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnLocalPlayerConnectedToRoom;
		}
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000C270F File Offset: 0x000C090F
	public GRPatrolPath GetPatrolPath(int patrolPathId)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		return this.levelGenerator.GetPatrolPath(patrolPathId);
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000C2730 File Offset: 0x000C0930
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.grManager.gameEntityManager.IsAuthority() && Time.timeAsDouble - this.lastCollectibleDispenserUpdateTime > (double)this.collectibleDispenserUpdateFrequency)
		{
			this.lastCollectibleDispenserUpdateTime = Time.timeAsDouble;
			for (int i = 0; i < this.collectibleDispensers.Count; i++)
			{
				if (this.collectibleDispensers[i] != null && this.collectibleDispensers[i].ReadyToDispenseNewCollectible)
				{
					this.collectibleDispensers[i].RequestDispenseCollectible();
				}
			}
		}
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x000C27C4 File Offset: 0x000C09C4
	private void OnLocalPlayerConnectedToRoom()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			grplayer.Reset();
		}
		this.shiftManager.shiftStats.ResetShiftStats();
		this.shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000C2806 File Offset: 0x000C0A06
	private void OnVRRigsChanged(RigContainer container)
	{
		this.VRRigRefresh();
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000C2810 File Offset: 0x000C0A10
	public void VRRigRefresh()
	{
		this.vrRigs.Clear();
		this.vrRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.vrRigs.Sort(delegate(VRRig a, VRRig b)
		{
			if (a == null || a.OwningNetPlayer == null)
			{
				return 1;
			}
			if (b == null || b.OwningNetPlayer == null)
			{
				return -1;
			}
			return a.OwningNetPlayer.ActorNumber.CompareTo(b.OwningNetPlayer.ActorNumber);
		});
		this.promotionBot.Refresh();
		this.RefreshScoreboards();
		this.RefreshDepth();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null && this.vrRigs.Count > grplayer.maxNumberOfPlayersInShift)
		{
			grplayer.maxNumberOfPlayersInShift = this.vrRigs.Count;
		}
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000C28C4 File Offset: 0x000C0AC4
	public void UpdateScoreboardScreen(GRUIScoreboard.ScoreboardScreen newScreen)
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].SwitchToScreen(newScreen);
		}
		this.RefreshScoreboards();
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000C2900 File Offset: 0x000C0B00
	public void RefreshScoreboards()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].Refresh(this.vrRigs);
			if (this.shiftManager.ShiftActive)
			{
				this.scoreboards[i].total.text = "-AWAITING SHIFT END-";
			}
			else if (this.shiftManager.ShiftTotalEarned < 0)
			{
				this.scoreboards[i].total.text = "-SHIFT NOT ACTIVE-";
			}
			else
			{
				this.scoreboards[i].total.text = this.shiftManager.ShiftTotalEarned.ToString();
			}
		}
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000C29C0 File Offset: 0x000C0BC0
	public int GetItemCost(int entityTypeId)
	{
		int result;
		if (!this.grManager.gameEntityManager.PriceLookup(entityTypeId, out result))
		{
			return 100;
		}
		return result;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000C29E6 File Offset: 0x000C0BE6
	public static void UpdateRemoteScoreboardScreen(GRUIScoreboard.ScoreboardScreen scoreboardPage)
	{
		GhostReactorManager.instance.photonView.RPC("BroadcastScoreboardPage", RpcTarget.Others, new object[]
		{
			scoreboardPage
		});
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000C2A0C File Offset: 0x000C0C0C
	public void DelveDeeper(int levelChange, bool noFX = false)
	{
		this.depthLevel += levelChange;
		this.depthLevel = Mathf.Clamp(this.depthLevel, 0, this.levelGenerator.depthConfigs.Count);
		this.shiftManager.authorizedToDelveDeeper = false;
		if (!noFX)
		{
			this.shiftManager.depthDisplay.PlayDelveDeeperFX();
		}
		this.RefreshDepth();
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000C2A6E File Offset: 0x000C0C6E
	public void ResetDepth()
	{
		if (this.shiftManager.ShiftActive)
		{
			return;
		}
		this.depthLevel = 0;
		this.RefreshDepth();
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000C2A8B File Offset: 0x000C0C8B
	public void RefreshDepth()
	{
		this.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000C2A98 File Offset: 0x000C0C98
	public int GetDepthLevel()
	{
		return this.depthLevel;
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000C2AA0 File Offset: 0x000C0CA0
	public GhostReactorLevelDepthConfig GetDepthLevelConfig(int level)
	{
		level = Mathf.Clamp(level, 0, this.levelGenerator.depthConfigs.Count - 1);
		return this.levelGenerator.depthConfigs[level];
	}

	// Token: 0x04002DB8 RID: 11704
	public static GhostReactor instance;

	// Token: 0x04002DB9 RID: 11705
	public Transform restartMarker;

	// Token: 0x04002DBA RID: 11706
	public PhotonView photonView;

	// Token: 0x04002DBB RID: 11707
	public AudioSource entryRoomAudio;

	// Token: 0x04002DBC RID: 11708
	public AudioClip entryRoomDeathSound;

	// Token: 0x04002DBD RID: 11709
	public BoxCollider zoneLimit;

	// Token: 0x04002DBE RID: 11710
	public BoxCollider safeZoneLimit;

	// Token: 0x04002DBF RID: 11711
	public List<GhostReactor.TempEnemySpawnInfo> tempSpawnEnemies;

	// Token: 0x04002DC0 RID: 11712
	public GameEntity overrideEnemySpawn;

	// Token: 0x04002DC1 RID: 11713
	public List<GameEntity> tempSpawnItems;

	// Token: 0x04002DC2 RID: 11714
	public Transform tempSpawnItemsMarker;

	// Token: 0x04002DC3 RID: 11715
	public List<GRUIBuyItem> itemPurchaseStands;

	// Token: 0x04002DC4 RID: 11716
	public List<GRToolPurchaseStation> toolPurchasingStations;

	// Token: 0x04002DC5 RID: 11717
	public List<GRUIScoreboard> scoreboards;

	// Token: 0x04002DC6 RID: 11718
	public List<GRCollectibleDispenser> collectibleDispensers = new List<GRCollectibleDispenser>();

	// Token: 0x04002DC7 RID: 11719
	public List<GRSentientCore> sentientCores = new List<GRSentientCore>();

	// Token: 0x04002DC8 RID: 11720
	public GRUIStationEmployeeBadges employeeBadges;

	// Token: 0x04002DC9 RID: 11721
	public GRUIEmployeeTerminal employeeTerminal;

	// Token: 0x04002DCA RID: 11722
	public GhostReactorShiftManager shiftManager;

	// Token: 0x04002DCB RID: 11723
	public GhostReactorLevelGeneratorV2 levelGenerator;

	// Token: 0x04002DCC RID: 11724
	public GRCurrencyDepositor currencyDepositor;

	// Token: 0x04002DCD RID: 11725
	public GRDistillery distillery;

	// Token: 0x04002DCE RID: 11726
	public GRResearchManager researchManager;

	// Token: 0x04002DCF RID: 11727
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x04002DD0 RID: 11728
	public GRRecycler recycler;

	// Token: 0x04002DD1 RID: 11729
	public LayerMask envLayerMask;

	// Token: 0x04002DD2 RID: 11730
	[ReadOnly]
	public List<GRReviveStation> reviveStations;

	// Token: 0x04002DD3 RID: 11731
	public List<GRVendingMachine> vendingMachines;

	// Token: 0x04002DD4 RID: 11732
	public List<VRRig> vrRigs;

	// Token: 0x04002DD5 RID: 11733
	private float collectibleDispenserUpdateFrequency = 3f;

	// Token: 0x04002DD6 RID: 11734
	private double lastCollectibleDispenserUpdateTime = -10.0;

	// Token: 0x04002DD7 RID: 11735
	private int sentientCoreUpdateIndex;

	// Token: 0x04002DD8 RID: 11736
	private SRand randomGenerator;

	// Token: 0x04002DD9 RID: 11737
	[ReadOnly]
	public int depthLevel;

	// Token: 0x04002DDA RID: 11738
	public Dictionary<int, double> playerProgressionData;

	// Token: 0x04002DDB RID: 11739
	public GRDropZone dropZone;

	// Token: 0x04002DDC RID: 11740
	public static float DROP_ZONE_REPEL = 2.25f;

	// Token: 0x04002DDD RID: 11741
	public GRUIPromotionBot promotionBot;

	// Token: 0x04002DDE RID: 11742
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x020005C8 RID: 1480
	[Serializable]
	public class TempEnemySpawnInfo
	{
		// Token: 0x04002DDF RID: 11743
		public GameEntity prefab;

		// Token: 0x04002DE0 RID: 11744
		public Transform spawnMarker;

		// Token: 0x04002DE1 RID: 11745
		public int patrolPath;
	}

	// Token: 0x020005C9 RID: 1481
	public enum EntityGroupTypes
	{
		// Token: 0x04002DE3 RID: 11747
		EnemyChaser,
		// Token: 0x04002DE4 RID: 11748
		EnemyChaserArmored,
		// Token: 0x04002DE5 RID: 11749
		EnemyRanged,
		// Token: 0x04002DE6 RID: 11750
		EnemyRangedArmored,
		// Token: 0x04002DE7 RID: 11751
		CollectibleFlower,
		// Token: 0x04002DE8 RID: 11752
		BarrierEnergyCostGate,
		// Token: 0x04002DE9 RID: 11753
		BarrierSpectralWall,
		// Token: 0x04002DEA RID: 11754
		HazardSpectralLiquid
	}

	// Token: 0x020005CA RID: 1482
	public enum EnemyType
	{
		// Token: 0x04002DEC RID: 11756
		Chaser,
		// Token: 0x04002DED RID: 11757
		Ranged,
		// Token: 0x04002DEE RID: 11758
		Phantom,
		// Token: 0x04002DEF RID: 11759
		Environment
	}
}
