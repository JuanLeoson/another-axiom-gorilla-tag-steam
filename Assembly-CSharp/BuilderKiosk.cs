using System;
using System.Collections;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200052A RID: 1322
public class BuilderKiosk : MonoBehaviour
{
	// Token: 0x0600202E RID: 8238 RVA: 0x000AA02C File Offset: 0x000A822C
	private void Awake()
	{
		BuilderKiosk.nullItem = new BuilderSetManager.BuilderSetStoreItem
		{
			displayName = "NOTHING",
			playfabID = "NULL",
			isNullItem = true
		};
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000AA068 File Offset: 0x000A8268
	private void Start()
	{
		this.purchaseParticles.Stop();
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.OnOwnedSetsUpdated));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.leftPurchaseButton.onPressed += this.PressLeftPurchaseItemButton;
		this.rightPurchaseButton.onPressed += this.PressRightPurchaseItemButton;
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.AddListener(new UnityAction(this.UpdateCountdown));
		}
		this.UpdateCountdown();
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (!this.isMiniKiosk)
		{
			this.SetupSetButtons();
		}
		if (this.availableItems.Count <= 0 || !BuilderSetManager.instance.pulledStoreItems)
		{
			this.itemToBuy = BuilderKiosk.nullItem;
			return;
		}
		this.hasInitFromPlayfab = true;
		if (this.pieceSetForSale != null)
		{
			this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
			this.UpdateLabels();
			this.UpdateDiorama();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, false);
			return;
		}
		this.itemToBuy = BuilderKiosk.nullItem;
		this.UpdateLabels();
		this.UpdateDiorama();
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
		this.ProcessPurchaseItemState(null, false);
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000AA21C File Offset: 0x000A841C
	private void UpdateCountdown()
	{
		if (!this.useTitleCountDown)
		{
			return;
		}
		if (!string.IsNullOrEmpty(BuilderTable.nextUpdateOverride) && !BuilderTable.nextUpdateOverride.Equals(this.countdownOverride))
		{
			this.countdownOverride = BuilderTable.nextUpdateOverride;
			CountdownTextDate countdown = this.countdownText.Countdown;
			countdown.CountdownTo = this.countdownOverride;
			this.countdownText.Countdown = countdown;
		}
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x000AA280 File Offset: 0x000A8480
	private void SetupSetButtons()
	{
		this.setsPerPage = this.setButtons.Length;
		this.totalPages = this.availableItems.Count / this.setsPerPage;
		if (this.availableItems.Count % this.setsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.setButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x000AA3A4 File Offset: 0x000A85A4
	private void OnDestroy()
	{
		if (this.leftPurchaseButton != null)
		{
			this.leftPurchaseButton.onPressed -= this.PressLeftPurchaseItemButton;
		}
		if (this.rightPurchaseButton != null)
		{
			this.rightPurchaseButton.onPressed -= this.PressRightPurchaseItemButton;
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.OnOwnedSetsUpdated));
		}
		if (CosmeticsController.instance != null)
		{
			CosmeticsController instance = CosmeticsController.instance;
			instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		}
		if (!this.isMiniKiosk)
		{
			foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
			{
				if (gorillaPressableButton != null)
				{
					gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
				}
			}
			if (this.previousPageButton != null)
			{
				this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
			}
			if (this.nextPageButton != null)
			{
				this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
			}
		}
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.RemoveListener(new UnityAction(this.UpdateCountdown));
		}
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000AA554 File Offset: 0x000A8754
	private void OnOwnedSetsUpdated()
	{
		if (this.hasInitFromPlayfab || !BuilderSetManager.instance.pulledStoreItems)
		{
			if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed)
			{
				this.ProcessPurchaseItemState(null, false);
			}
			return;
		}
		this.hasInitFromPlayfab = true;
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (this.pieceSetForSale != null)
		{
			this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
			this.UpdateLabels();
			this.UpdateDiorama();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, false);
			return;
		}
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
		this.ProcessPurchaseItemState(null, false);
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000AA648 File Offset: 0x000A8848
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying && !this.animating)
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			int num = 0;
			for (int i = 0; i < this.setButtons.Length; i++)
			{
				if (button.Equals(this.setButtons[i]))
				{
					num = i;
					break;
				}
			}
			int num2 = this.pageIndex * this.setsPerPage + num;
			if (num2 < this.availableItems.Count)
			{
				BuilderPieceSet builderPieceSet = this.availableItems[num2];
				if (builderPieceSet.setName.Equals(this.itemToBuy.displayName))
				{
					this.itemToBuy = BuilderKiosk.nullItem;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				}
				else
				{
					this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(builderPieceSet.GetIntIdentifier());
					this.UpdateLabels();
					this.UpdateDiorama();
				}
				this.ProcessPurchaseItemState(null, isLeft);
			}
		}
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000AA720 File Offset: 0x000A8920
	private void OnPreviousPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000AA75C File Offset: 0x000A895C
	private void OnNextPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000AA798 File Offset: 0x000A8998
	private void UpdateLabels()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		for (int i = 0; i < this.setButtons.Length; i++)
		{
			int num = this.pageIndex * this.setsPerPage + i;
			if (num < this.availableItems.Count && this.availableItems[num] != null)
			{
				if (!this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(true);
					this.setButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.setButtons[i].myTmpText.text != this.availableItems[num].setName.ToUpper())
				{
					this.setButtons[i].myTmpText.text = this.availableItems[num].setName.ToUpper();
				}
				bool flag = !this.itemToBuy.isNullItem && this.availableItems[num].playfabID == this.itemToBuy.playfabID;
				if (flag != this.setButtons[i].isOn || !this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = flag;
					this.setButtons[i].buttonRenderer.material = (flag ? this.setButtons[i].pressedMaterial : this.setButtons[i].unpressedMaterial);
				}
				this.setButtons[i].enabled = true;
			}
			else
			{
				if (this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(false);
					this.setButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.setButtons[i].isOn || this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = false;
					this.setButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x000AAA30 File Offset: 0x000A8C30
	public void UpdateDiorama()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.itemToBuy.isNullItem)
		{
			this.countdownText.gameObject.SetActive(false);
		}
		else
		{
			this.countdownText.gameObject.SetActive(BuilderSetManager.instance.IsSetSeasonal(this.itemToBuy.playfabID));
		}
		if (this.animating)
		{
			base.StopCoroutine(this.PlaySwapAnimation());
			if (this.currentDiorama != null)
			{
				Object.Destroy(this.currentDiorama);
				this.currentDiorama = null;
			}
			this.currentDiorama = this.nextDiorama;
			this.nextDiorama = null;
		}
		this.animating = true;
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		if (!this.itemToBuy.isNullItem && this.itemToBuy.displayModel != null)
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.itemToBuy.displayModel, this.nextItemDisplayPos);
		}
		else
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.emptyDisplay, this.nextItemDisplayPos);
		}
		this.itemDisplayAnimation.Rewind();
		if (this.currentDiorama != null)
		{
			this.currentDiorama.transform.SetParent(this.itemDisplayPos, false);
		}
		base.StartCoroutine(this.PlaySwapAnimation());
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000AAB9B File Offset: 0x000A8D9B
	private IEnumerator PlaySwapAnimation()
	{
		this.itemDisplayAnimation.Play();
		yield return new WaitForSeconds(this.itemDisplayAnimation.clip.length);
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		this.currentDiorama = this.nextDiorama;
		this.nextDiorama = null;
		this.animating = false;
		yield break;
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x000AABAA File Offset: 0x000A8DAA
	public void PressLeftPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("left", isLeftHand);
		}
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x000AABC8 File Offset: 0x000A8DC8
	public void PressRightPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("right", isLeftHand);
		}
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x000AABE6 File Offset: 0x000A8DE6
	public void OnUpdateCurrencyBalance()
	{
		if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.ItemOwned)
		{
			this.ProcessPurchaseItemState(null, false);
		}
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000AAC0A File Offset: 0x000A8E0A
	public void ClearCheckout()
	{
		GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.itemToBuy);
		this.itemToBuy = BuilderKiosk.nullItem;
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000AAC34 File Offset: 0x000A8E34
	public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
	{
		switch (this.currentPurchaseItemStage)
		{
		case CosmeticsController.PurchaseItemStages.Start:
			this.itemToBuy = BuilderKiosk.nullItem;
			this.FormattedPurchaseText("SELECT AN ITEM TO PURCHASE!");
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.UpdateLabels();
			this.UpdateDiorama();
			return;
		case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
			if (this.availableItems.Count > 1)
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.itemToBuy);
			}
			if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.itemToBuy.setID))
			{
				this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!");
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
				return;
			}
			if ((ulong)this.itemToBuy.cost <= (ulong)((long)CosmeticsController.instance.currencyBalance))
			{
				this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?");
				this.leftPurchaseButton.myTmpText.text = "NO!";
				this.rightPurchaseButton.myTmpText.text = "YES!";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
				return;
			}
			this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!");
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			if (!this.isMiniKiosk)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			}
			break;
		case CosmeticsController.PurchaseItemStages.ItemSelected:
			if (buttonSide == "right")
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
				this.FormattedPurchaseText("ARE YOU REALLY SURE?");
				this.leftPurchaseButton.myTmpText.text = "YES! I NEED IT!";
				this.rightPurchaseButton.myTmpText.text = "LET ME THINK ABOUT IT";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, isLeftHand);
			return;
		case CosmeticsController.PurchaseItemStages.ItemOwned:
		case CosmeticsController.PurchaseItemStages.Buying:
			break;
		case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
			if (buttonSide == "left")
			{
				this.FormattedPurchaseText("PURCHASING ITEM...");
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
				this.isLastHandTouchedLeft = isLeftHand;
				this.PurchaseItem();
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, isLeftHand);
			return;
		case CosmeticsController.PurchaseItemStages.Success:
		{
			this.FormattedPurchaseText("SUCCESS! YOU CAN NOW SELECT " + this.itemToBuy.displayName.ToUpper() + " AT SHELVES.");
			this.audioSource.GTPlayOneShot(this.purchaseSetAudioClip, 1f);
			this.purchaseParticles.Play();
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.playfabID;
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			break;
		}
		case CosmeticsController.PurchaseItemStages.Failure:
			this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.");
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000AB0F8 File Offset: 0x000A92F8
	public void FormattedPurchaseText(string finalLineVar)
	{
		this.finalLine = finalLineVar;
		this.purchaseText.text = string.Concat(new string[]
		{
			"ITEM: ",
			this.itemToBuy.displayName.ToUpper(),
			"\nITEM COST: ",
			this.itemToBuy.cost.ToString(),
			"\nYOU HAVE: ",
			CosmeticsController.instance.currencyBalance.ToString(),
			"\n\n",
			this.finalLine
		});
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000AB185 File Offset: 0x000A9385
	public void PurchaseItem()
	{
		BuilderSetManager.instance.TryPurchaseItem(this.itemToBuy.setID, delegate(bool result)
		{
			if (result)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
				CosmeticsController.instance.currencyBalance -= (int)this.itemToBuy.cost;
				this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
			this.ProcessPurchaseItemState(null, false);
		});
	}

	// Token: 0x040028F7 RID: 10487
	public BuilderPieceSet pieceSetForSale;

	// Token: 0x040028F8 RID: 10488
	public GorillaPressableButton leftPurchaseButton;

	// Token: 0x040028F9 RID: 10489
	public GorillaPressableButton rightPurchaseButton;

	// Token: 0x040028FA RID: 10490
	public TMP_Text purchaseText;

	// Token: 0x040028FB RID: 10491
	[SerializeField]
	private bool isMiniKiosk;

	// Token: 0x040028FC RID: 10492
	[SerializeField]
	private bool useTitleCountDown = true;

	// Token: 0x040028FD RID: 10493
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableButton[] setButtons;

	// Token: 0x040028FE RID: 10494
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x040028FF RID: 10495
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x04002900 RID: 10496
	private BuilderPieceSet currentSet;

	// Token: 0x04002901 RID: 10497
	private int pageIndex;

	// Token: 0x04002902 RID: 10498
	private int setsPerPage = 3;

	// Token: 0x04002903 RID: 10499
	private int totalPages = 1;

	// Token: 0x04002904 RID: 10500
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002905 RID: 10501
	[SerializeField]
	private AudioClip purchaseSetAudioClip;

	// Token: 0x04002906 RID: 10502
	[SerializeField]
	private ParticleSystem purchaseParticles;

	// Token: 0x04002907 RID: 10503
	[SerializeField]
	private GameObject emptyDisplay;

	// Token: 0x04002908 RID: 10504
	private List<BuilderPieceSet> availableItems = new List<BuilderPieceSet>(10);

	// Token: 0x04002909 RID: 10505
	internal CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

	// Token: 0x0400290A RID: 10506
	private bool hasInitFromPlayfab;

	// Token: 0x0400290B RID: 10507
	internal BuilderSetManager.BuilderSetStoreItem itemToBuy;

	// Token: 0x0400290C RID: 10508
	public static BuilderSetManager.BuilderSetStoreItem nullItem;

	// Token: 0x0400290D RID: 10509
	private GameObject currentDiorama;

	// Token: 0x0400290E RID: 10510
	private GameObject nextDiorama;

	// Token: 0x0400290F RID: 10511
	private bool animating;

	// Token: 0x04002910 RID: 10512
	[SerializeField]
	private Transform itemDisplayPos;

	// Token: 0x04002911 RID: 10513
	[SerializeField]
	private Transform nextItemDisplayPos;

	// Token: 0x04002912 RID: 10514
	[SerializeField]
	private Animation itemDisplayAnimation;

	// Token: 0x04002913 RID: 10515
	[SerializeField]
	private CountdownText countdownText;

	// Token: 0x04002914 RID: 10516
	private string countdownOverride = string.Empty;

	// Token: 0x04002915 RID: 10517
	private bool isLastHandTouchedLeft;

	// Token: 0x04002916 RID: 10518
	private string finalLine;
}
