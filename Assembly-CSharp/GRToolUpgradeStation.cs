using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200068B RID: 1675
public class GRToolUpgradeStation : MonoBehaviour
{
	// Token: 0x060028FE RID: 10494 RVA: 0x000DC88C File Offset: 0x000DAA8C
	public void Init(GhostReactor reactor)
	{
		this._reactor = reactor;
		this.defaultCostText = this.CostText.text;
		this.defaultPageText = this.currentPageText.text;
		this._reactor.researchManager.researchManagerUpdated.AddListener(new UnityAction(this.ResearchTreeUpdated));
		this.ResetScreen();
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060028FF RID: 10495 RVA: 0x000DC8E9 File Offset: 0x000DAAE9
	public bool canInsertTool
	{
		get
		{
			return this.currentState == GRToolUpgradeStation.UpgradeStationState.Idle && !this.bIsToolInserted;
		}
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x000DC8FE File Offset: 0x000DAAFE
	public void ResearchTreeUpdated()
	{
		this.UpdateUI();
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x000DC906 File Offset: 0x000DAB06
	public void Update()
	{
		if (this.currentState == GRToolUpgradeStation.UpgradeStationState.Upgrading)
		{
			this.UpgradingUpdate(PhotonNetwork.Time);
		}
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x000DC91C File Offset: 0x000DAB1C
	public void ToolInserted(GRTool tool)
	{
		if (!this.canInsertTool)
		{
			return;
		}
		this.bIsToolInserted = true;
		this.currentPage = 0;
		this.upgradeIndexOffset = 0;
		this.insertedTool = tool;
		this.insertedToolId = this.insertedTool.ToolResearchID;
		this.upgradeNodes = this._reactor.researchManager.researchTree.GetChildrenNodes(this.insertedToolId);
		this.totalPages = this.upgradeNodes.Count / this.UpgradeTexts.Length;
		this.ResetScreen();
		this.UpdateUI();
		this.SelectUpgrade(0);
		this.LocalPlacedToolInUpgradeStation(tool.gameEntity.id);
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x000DC9C0 File Offset: 0x000DABC0
	public void TestUI(string ToolId)
	{
		this.ResetScreen();
		this.insertedToolId = ToolId;
		this.upgradeNodes = this._reactor.researchManager.researchTree.GetChildrenNodes(this.insertedToolId);
		this.selectedUpgradeIndex = 0;
		this.currentPage = 0;
		this.totalPages = this.upgradeNodes.Count / this.UpgradeTexts.Length;
		this.SelectUpgrade(0);
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x000DCA2A File Offset: 0x000DAC2A
	public void UpdateUI()
	{
		this.UpdateUpgradeTexts();
		this.UpdateCurrentPageText();
		this.UpdateSelectedUpgrade();
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000DCA40 File Offset: 0x000DAC40
	public void UpdateUpgradeTexts()
	{
		this.TitleText.text = this._reactor.researchManager.researchTree.GetDisplayName(this.insertedToolId);
		for (int i = 0; i < this.UpgradeTexts.Length; i++)
		{
			if (this.upgradeNodes.Count > i + this.upgradeIndexOffset && this.upgradeNodes[i + this.upgradeIndexOffset] != null)
			{
				this.UpgradeTexts[i].text = this.upgradeNodes[i + this.upgradeIndexOffset].name;
			}
		}
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000023F5 File Offset: 0x000005F5
	public void UnlockAllUpgrades()
	{
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x000DCAD5 File Offset: 0x000DACD5
	public void UpdateCurrentPageText()
	{
		this.currentPageText.text = string.Format(this.defaultPageText, this.currentPage + 1, this.totalPages);
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000DCB08 File Offset: 0x000DAD08
	public void UpdateSelectedUpgrade()
	{
		if (this.upgradeNodes != null && this.upgradeNodes.Count > this.selectedUpgradeIndex + this.upgradeIndexOffset && this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset] != null)
		{
			if (this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].unlocked)
			{
				this.DescriptionText.text = this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].description;
				int cost = this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].cost;
				this.CostText.text = string.Format(this.defaultCostText, cost.ToString());
				GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
				this.CostText.color = ((cost > grplayer.currency) ? this.lockedColor : this.unlockedColor);
				return;
			}
			this.CostText.text = "LOCKED NEEDS RESEARCH";
			this.CostText.color = this.lockedColor;
		}
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000DCC30 File Offset: 0x000DAE30
	public void ResetScreen()
	{
		this.DescriptionText.text = "PLEASE INSERT A TOOL";
		for (int i = 0; i < this.UpgradeTexts.Length; i++)
		{
			this.UpgradeTexts[i].text = "----";
			this.UpgradeTexts[i].color = this.lockedColor;
			this.MFD_ButtonTexts[i].color = this.unSelectedColor;
		}
		this.TitleText.text = "----";
		this.CostText.text = "-";
		this.TitleText.color = this.unSelectedColor;
		this.DescriptionText.color = this.unSelectedColor;
		this.CostText.color = this.unSelectedColor;
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x000DCCEC File Offset: 0x000DAEEC
	public void SelectUpgrade(int index)
	{
		if (index >= this.upgradeNodes.Count)
		{
			return;
		}
		this.selectedUpgradeIndex = index;
		for (int i = 0; i < this.UpgradeTexts.Length; i++)
		{
			if (i + this.currentPage * this.UpgradeTexts.Length < this.upgradeNodes.Count)
			{
				bool unlocked = this.upgradeNodes[i + this.upgradeIndexOffset].unlocked;
				this.UpgradeTexts[i].color = (unlocked ? this.unlockedColor : this.lockedColor);
				this.UpgradeLockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeLockedImage[i].gameObject.SetActive(true);
				this.UpgradeTexts[i].color = this.lockedColor;
			}
			this.UpgradeButtons[i].isOn = false;
			this.UpgradeButtons[i].UpdateColor();
		}
		if (this.upgradeNodes != null && this.upgradeNodes.Count > this.selectedUpgradeIndex && this.upgradeNodes[this.selectedUpgradeIndex] != null)
		{
			this.UpgradeButtons[this.selectedUpgradeIndex].isOn = true;
			this.UpgradeButtons[this.selectedUpgradeIndex].UpdateColor();
			this.DescriptionText.color = this.UpgradeTexts[this.selectedUpgradeIndex].color;
			this.CostText.color = this.UpgradeTexts[this.selectedUpgradeIndex].color;
		}
		this.UpdateUI();
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000DCE6C File Offset: 0x000DB06C
	public void UpgradeTool()
	{
		this._reactor.grManager.ToolUpgradeStationRequestUpgrade(this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].id, this.insertedToolEntity.GetNetId());
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000DCEA8 File Offset: 0x000DB0A8
	public void LocalPlacedToolInUpgradeStation(GameEntityId entityId)
	{
		GameEntity gameEntity = this._reactor.grManager.gameEntityManager.GetGameEntity(entityId);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.ItemInserted;
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
			this.PositionInsertedTool(gameEntity);
			this.SelectUpgrade(0);
		}
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x000DCF54 File Offset: 0x000DB154
	public void PositionInsertedTool(GameEntity entity)
	{
		this.insertedToolEntity = entity;
		entity.transform.SetParent(this.startingLocation);
		entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		entity.pickupable = false;
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x000DCFE4 File Offset: 0x000DB1E4
	public void PayForUpgrade(int Player)
	{
		if (Player == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			int cost = this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].cost;
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			bool flag = cost <= grplayer.currency;
			bool unlocked = this.upgradeNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].unlocked;
			if (flag && unlocked)
			{
				UnityEvent onSucceeded = this.IDCardScanner.onSucceeded;
				if (onSucceeded != null)
				{
					onSucceeded.Invoke();
				}
				this.StartUpgrade(PhotonNetwork.Time);
			}
		}
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x000DD074 File Offset: 0x000DB274
	public void StartUpgrade(double startTime)
	{
		if (this.currentState != GRToolUpgradeStation.UpgradeStationState.ItemInserted)
		{
			return;
		}
		this.upgradeStartTime = startTime;
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Upgrading;
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x000DD0C9 File Offset: 0x000DB2C9
	public void UpgradingUpdate(double currentTime)
	{
		if (currentTime >= this.upgradeStartTime + this.upgradeAnimationLength)
		{
			this.CompleteUpgrade();
		}
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x000DD0E1 File Offset: 0x000DB2E1
	public void CompleteUpgrade()
	{
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		this.ResetScreen();
		this.MoveToolToFinished();
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x000DD0F8 File Offset: 0x000DB2F8
	public void MoveItemToUpgradeSlot()
	{
		this.insertedToolEntity.transform.SetParent(this.upgradingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.upgradingLocation.position;
			component.rotation = this.upgradingLocation.rotation;
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = false;
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x000DD198 File Offset: 0x000DB398
	public void ScrollUpgradesUp()
	{
		this.currentPage = ((this.currentPage == this.totalPages) ? (this.currentPage = 0) : (this.currentPage + 1));
		this.upgradeIndexOffset = this.UpgradeTexts.Length * this.currentPage;
		this.ResetScreen();
		this.UpdateUI();
		this.UpdateUpgradeTexts();
		this.SelectUpgrade(0);
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x000DD1FC File Offset: 0x000DB3FC
	public void ScrollUpgradesDown()
	{
		this.currentPage = ((this.currentPage <= 0) ? this.totalPages : (this.currentPage - 1));
		this.upgradeIndexOffset = this.UpgradeTexts.Length * this.currentPage;
		this.ResetScreen();
		this.UpdateUI();
		this.UpdateUpgradeTexts();
		this.SelectUpgrade(0);
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000DD258 File Offset: 0x000DB458
	public void MoveToolToFinished()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.velocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.UpgradeTool();
		this.EjectToolFromEnd();
		this.ResetScreen();
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000DD320 File Offset: 0x000DB520
	public void EjectToolFromStart()
	{
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.velocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolId = "";
		this.bIsToolInserted = false;
		this.ResetScreen();
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x000DD410 File Offset: 0x000DB610
	public void EjectToolFromEnd()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.depositedLocation.position;
			component.rotation = this.depositedLocation.rotation;
			component.velocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolId = "";
		this.bIsToolInserted = false;
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x040034DE RID: 13534
	private GRTool insertedTool;

	// Token: 0x040034DF RID: 13535
	private string insertedToolId = "";

	// Token: 0x040034E0 RID: 13536
	private GameEntity insertedToolEntity;

	// Token: 0x040034E1 RID: 13537
	private GhostReactor _reactor;

	// Token: 0x040034E2 RID: 13538
	[NonSerialized]
	public bool bIsToolInserted;

	// Token: 0x040034E3 RID: 13539
	public Transform startingLocation;

	// Token: 0x040034E4 RID: 13540
	public Transform upgradingLocation;

	// Token: 0x040034E5 RID: 13541
	public Transform depositedLocation;

	// Token: 0x040034E6 RID: 13542
	public Transform ejectionTransform;

	// Token: 0x040034E7 RID: 13543
	public float ejectionVelocity;

	// Token: 0x040034E8 RID: 13544
	public Color selectedColor;

	// Token: 0x040034E9 RID: 13545
	public Color unSelectedColor;

	// Token: 0x040034EA RID: 13546
	public Color lockedColor;

	// Token: 0x040034EB RID: 13547
	public Color unlockedColor;

	// Token: 0x040034EC RID: 13548
	public TMP_Text[] UpgradeTexts;

	// Token: 0x040034ED RID: 13549
	public TMP_Text[] MFD_ButtonTexts;

	// Token: 0x040034EE RID: 13550
	public GorillaPressableButton[] UpgradeButtons;

	// Token: 0x040034EF RID: 13551
	public Image[] UpgradeLockedImage;

	// Token: 0x040034F0 RID: 13552
	public TMP_Text TitleText;

	// Token: 0x040034F1 RID: 13553
	public TMP_Text DescriptionText;

	// Token: 0x040034F2 RID: 13554
	public TMP_Text CostText;

	// Token: 0x040034F3 RID: 13555
	public TMP_Text currentPageText;

	// Token: 0x040034F4 RID: 13556
	private string defaultPageText;

	// Token: 0x040034F5 RID: 13557
	private string defaultCostText;

	// Token: 0x040034F6 RID: 13558
	public IDCardScanner IDCardScanner;

	// Token: 0x040034F7 RID: 13559
	private int selectedUpgradeIndex;

	// Token: 0x040034F8 RID: 13560
	private int upgradeIndexOffset;

	// Token: 0x040034F9 RID: 13561
	private int currentPage;

	// Token: 0x040034FA RID: 13562
	private int totalPages = 1;

	// Token: 0x040034FB RID: 13563
	private double upgradeStartTime;

	// Token: 0x040034FC RID: 13564
	public double upgradeAnimationLength;

	// Token: 0x040034FD RID: 13565
	public Vector3 rotationAnimation;

	// Token: 0x040034FE RID: 13566
	private GRToolUpgradeStation.UpgradeStationState currentState;

	// Token: 0x040034FF RID: 13567
	public GameEntity attachedItem;

	// Token: 0x04003500 RID: 13568
	private List<ResearchNode> upgradeNodes = new List<ResearchNode>();

	// Token: 0x0200068C RID: 1676
	private enum UpgradeStationState
	{
		// Token: 0x04003502 RID: 13570
		Idle,
		// Token: 0x04003503 RID: 13571
		ItemInserted,
		// Token: 0x04003504 RID: 13572
		Upgrading,
		// Token: 0x04003505 RID: 13573
		Complete
	}
}
