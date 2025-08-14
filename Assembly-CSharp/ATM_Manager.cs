using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaNetworking;
using GorillaNetworking.Store;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000479 RID: 1145
public class ATM_Manager : MonoBehaviour
{
	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001C6B RID: 7275 RVA: 0x00098205 File Offset: 0x00096405
	// (set) Token: 0x06001C6C RID: 7276 RVA: 0x0009820D File Offset: 0x0009640D
	public string ValidatedCreatorCode { get; set; }

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001C6D RID: 7277 RVA: 0x00098216 File Offset: 0x00096416
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x00098220 File Offset: 0x00096420
	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: ";
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000982AC File Offset: 0x000964AC
	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
		this.currentCreatorCode = "";
		if (PlayerPrefs.HasKey("CodeUsedTime"))
		{
			this.codeFirstUsedTime = PlayerPrefs.GetString("CodeUsedTime");
			DateTime d = DateTime.Parse(this.codeFirstUsedTime);
			if ((DateTime.Now - d).TotalDays > 14.0)
			{
				PlayerPrefs.SetString("CreatorCode", "");
			}
			else
			{
				this.currentCreatorCode = PlayerPrefs.GetString("CreatorCode", "");
				this.initialCode = this.currentCreatorCode;
				Debug.Log("Initial code: " + this.initialCode);
				if (string.IsNullOrEmpty(this.currentCreatorCode))
				{
					this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
				}
				else
				{
					this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
				}
				foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
				{
					creatorCodeSmallDisplay.SetCode(this.currentCreatorCode);
				}
			}
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = this.currentCreatorCode;
		}
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x00098464 File Offset: 0x00096664
	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Validating)
		{
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeTitle.text = "CREATOR CODE:";
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				if (this.currentCreatorCode.Length > 0)
				{
					this.currentCreatorCode = this.currentCreatorCode.Substring(0, this.currentCreatorCode.Length - 1);
					if (this.currentCreatorCode.Length == 0)
					{
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
						this.ValidatedCreatorCode = "";
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SetCode("");
						}
						PlayerPrefs.SetString("CreatorCode", "");
						PlayerPrefs.Save();
					}
					else
					{
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
					}
				}
			}
			else if (this.currentCreatorCode.Length < 10)
			{
				string str = this.currentCreatorCode;
				string str2;
				if (buttonPressed >= GorillaATMKeyBindings.delete)
				{
					str2 = buttonPressed.ToString();
				}
				else
				{
					int num = (int)buttonPressed;
					str2 = num.ToString();
				}
				this.currentCreatorCode = str + str2;
				this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
			}
			foreach (ATM_UI atm_UI2 in this.atmUIs)
			{
				atm_UI2.creatorCodeField.text = this.currentCreatorCode;
			}
		}
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x00098620 File Offset: 0x00096820
	public void ProcessATMState(string currencyButton)
	{
		switch (this.currentATMStage)
		{
		case ATM_Manager.ATMStages.Unavailable:
		case ATM_Manager.ATMStages.Purchasing:
			break;
		case ATM_Manager.ATMStages.Begin:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			return;
		case ATM_Manager.ATMStages.Menu:
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				if (currencyButton == "one")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Balance);
					return;
				}
				if (!(currencyButton == "four"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Begin);
				return;
			}
			else
			{
				if (currencyButton == "one")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Balance);
					return;
				}
				if (currencyButton == "two")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Choose);
					return;
				}
				if (!(currencyButton == "back"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Begin);
				return;
			}
			break;
		case ATM_Manager.ATMStages.Balance:
			if (currencyButton == "back")
			{
				this.SwitchToStage(ATM_Manager.ATMStages.Menu);
				return;
			}
			break;
		case ATM_Manager.ATMStages.Choose:
			if (currencyButton == "one")
			{
				this.numShinyRocksToBuy = 1000;
				this.shinyRocksCost = 4.99f;
				CosmeticsController.instance.itemToPurchase = "1000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "two")
			{
				this.numShinyRocksToBuy = 2200;
				this.shinyRocksCost = 9.99f;
				CosmeticsController.instance.itemToPurchase = "2200SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "three")
			{
				this.numShinyRocksToBuy = 5000;
				this.shinyRocksCost = 19.99f;
				CosmeticsController.instance.itemToPurchase = "5000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "four")
			{
				this.numShinyRocksToBuy = 11000;
				this.shinyRocksCost = 39.99f;
				CosmeticsController.instance.itemToPurchase = "11000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (!(currencyButton == "back"))
			{
				return;
			}
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			return;
		case ATM_Manager.ATMStages.Confirm:
			if (!(currencyButton == "one"))
			{
				if (!(currencyButton == "back"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Choose);
				return;
			}
			else
			{
				if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Empty)
				{
					CosmeticsController.instance.SteamPurchase();
					this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
					return;
				}
				base.StartCoroutine(this.CheckValidationCoroutine());
				return;
			}
			break;
		default:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			break;
		}
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x0009888B File Offset: 0x00096A8B
	public void AddATM(ATM_UI newATM)
	{
		this.atmUIs.Add(newATM);
		newATM.creatorCodeField.text = this.currentCreatorCode;
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x000988B6 File Offset: 0x00096AB6
	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x000988C8 File Offset: 0x00096AC8
	public void SetTemporaryCreatorCode(string creatorCode, bool onlyIfEmpty = true, Action<bool> OnComplete = null)
	{
		if (onlyIfEmpty && (this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Empty || !this.currentCreatorCode.IsNullOrEmpty()))
		{
			Action<bool> onComplete = OnComplete;
			if (onComplete == null)
			{
				return;
			}
			onComplete(false);
			return;
		}
		else
		{
			string pattern = "^[a-zA-Z0-9]+$";
			if (creatorCode.Length <= 10 && Regex.IsMatch(creatorCode, pattern))
			{
				NexusManager.instance.VerifyCreatorCode(creatorCode, delegate(Member member)
				{
					if (this.currentATMStage > ATM_Manager.ATMStages.Confirm)
					{
						Action<bool> onComplete3 = OnComplete;
						if (onComplete3 == null)
						{
							return;
						}
						onComplete3(false);
						return;
					}
					else if (onlyIfEmpty && (this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Empty || !this.currentCreatorCode.IsNullOrEmpty()))
					{
						Action<bool> onComplete4 = OnComplete;
						if (onComplete4 == null)
						{
							return;
						}
						onComplete4(false);
						return;
					}
					else
					{
						this.temporaryOverrideCode = creatorCode;
						this.currentCreatorCode = creatorCode;
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SetCode(this.currentCreatorCode);
						}
						foreach (ATM_UI atm_UI in this.atmUIs)
						{
							atm_UI.creatorCodeField.text = this.currentCreatorCode;
						}
						Action<bool> onComplete5 = OnComplete;
						if (onComplete5 == null)
						{
							return;
						}
						onComplete5(true);
						return;
					}
				}, delegate
				{
					Action<bool> onComplete3 = OnComplete;
					if (onComplete3 == null)
					{
						return;
					}
					onComplete3(false);
				});
				return;
			}
			Action<bool> onComplete2 = OnComplete;
			if (onComplete2 == null)
			{
				return;
			}
			onComplete2(false);
			return;
		}
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x00098988 File Offset: 0x00096B88
	public void ResetTemporaryCreatorCode()
	{
		if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Unchecked && this.currentCreatorCode.Equals(this.temporaryOverrideCode))
		{
			this.currentCreatorCode = "";
			this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
			foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
			{
				creatorCodeSmallDisplay.SetCode("");
			}
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeField.text = this.currentCreatorCode;
			}
		}
		this.temporaryOverrideCode = "";
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x00098A68 File Offset: 0x00096C68
	private void ResetCreatorCode()
	{
		Debug.Log("Resetting creator code");
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE:";
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode("");
		}
		this.currentCreatorCode = "";
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
		this.supportedMember = default(Member);
		this.ValidatedCreatorCode = "";
		PlayerPrefs.SetString("CreatorCode", "");
		PlayerPrefs.Save();
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.creatorCodeField.text = this.currentCreatorCode;
		}
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x00098B98 File Offset: 0x00096D98
	private IEnumerator CheckValidationCoroutine()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
		this.VerifyCreatorCode();
		while (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Validating)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Valid)
		{
			foreach (ATM_UI atm_UI2 in this.atmUIs)
			{
				atm_UI2.creatorCodeTitle.text = "CREATOR CODE: VALID";
			}
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
			CosmeticsController.instance.SteamPurchase();
		}
		yield break;
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x00098BA8 File Offset: 0x00096DA8
	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (!atm_UI.atmText)
			{
				break;
			}
			this.currentATMStage = newStage;
			switch (newStage)
			{
			case ATM_Manager.ATMStages.Unavailable:
				atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Begin:
				atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "BEGIN";
				atm_UI.ATM_RightColumnArrowText[3].enabled = true;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Menu:
				if (PlayFabAuthenticator.instance.GetSafety())
				{
					atm_UI.atmText.text = "CHECK YOUR BALANCE.";
					atm_UI.ATM_RightColumnButtonText[0].text = "BALANCE";
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
				}
				else
				{
					atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
					atm_UI.ATM_RightColumnButtonText[0].text = "BALANCE";
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "PURCHASE";
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
				}
				break;
			case ATM_Manager.ATMStages.Balance:
				atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Choose:
				atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
				atm_UI.ATM_RightColumnButtonText[0].text = "1000 for $4.99";
				atm_UI.ATM_RightColumnArrowText[0].enabled = true;
				atm_UI.ATM_RightColumnButtonText[1].text = "2200 for $9.99\n(10% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[1].enabled = true;
				atm_UI.ATM_RightColumnButtonText[2].text = "5000 for $19.99\n(25% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[2].enabled = true;
				atm_UI.ATM_RightColumnButtonText[3].text = "11000 for $39.99\n(37% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[3].enabled = true;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Confirm:
				atm_UI.atmText.text = string.Concat(new string[]
				{
					"YOU HAVE CHOSEN TO PURCHASE ",
					this.numShinyRocksToBuy.ToString(),
					" SHINY ROCKS FOR $",
					this.shinyRocksCost.ToString(),
					". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
				});
				atm_UI.ATM_RightColumnButtonText[0].text = "CONFIRM";
				atm_UI.ATM_RightColumnArrowText[0].enabled = true;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(true);
				break;
			case ATM_Manager.ATMStages.Purchasing:
				atm_UI.atmText.text = "PURCHASING IN STEAM...";
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Success:
				atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
				if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Valid)
				{
					string name = this.supportedMember.name;
					if (!string.IsNullOrEmpty(name))
					{
						Text atmText = atm_UI.atmText;
						atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SuccessfulPurchase(name);
						}
					}
				}
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Failure:
				atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.SafeAccount:
				atm_UI.atmText.text = "Out Of Order.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			}
		}
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000993C8 File Offset: 0x000975C8
	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x00099420 File Offset: 0x00097620
	public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
	{
		this.ProcessATMState(currencyPurchaseSize);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x00099429 File Offset: 0x00097629
	public void VerifyCreatorCode()
	{
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Validating;
		NexusManager.instance.VerifyCreatorCode(this.currentCreatorCode, new Action<Member>(this.OnCreatorCodeSucess), new Action(this.OnCreatorCodeFailure));
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x0009945C File Offset: 0x0009765C
	private void OnCreatorCodeSucess(Member member)
	{
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Valid;
		this.supportedMember = member;
		this.ValidatedCreatorCode = this.currentCreatorCode;
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(this.ValidatedCreatorCode);
		}
		PlayerPrefs.SetString("CreatorCode", this.ValidatedCreatorCode);
		if (this.initialCode != this.ValidatedCreatorCode)
		{
			PlayerPrefs.SetString("CodeUsedTime", DateTime.Now.ToString());
		}
		PlayerPrefs.Save();
		Debug.Log("ATM CODE SUCCESS: " + this.supportedMember.name);
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x00099528 File Offset: 0x00097728
	private void OnCreatorCodeFailure()
	{
		this.supportedMember = default(Member);
		this.ResetCreatorCode();
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}
		Debug.Log("ATM CODE FAILURE");
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x000023F5 File Offset: 0x000005F5
	public void LeaveSystemMenu()
	{
	}

	// Token: 0x040024D5 RID: 9429
	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	// Token: 0x040024D6 RID: 9430
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x040024D7 RID: 9431
	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	// Token: 0x040024D8 RID: 9432
	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	// Token: 0x040024D9 RID: 9433
	private string currentCreatorCode;

	// Token: 0x040024DA RID: 9434
	private string codeFirstUsedTime;

	// Token: 0x040024DB RID: 9435
	private string initialCode;

	// Token: 0x040024DC RID: 9436
	private string temporaryOverrideCode;

	// Token: 0x040024DE RID: 9438
	private ATM_Manager.CreatorCodeStatus creatorCodeStatus;

	// Token: 0x040024DF RID: 9439
	private ATM_Manager.ATMStages currentATMStage;

	// Token: 0x040024E0 RID: 9440
	public int numShinyRocksToBuy;

	// Token: 0x040024E1 RID: 9441
	public float shinyRocksCost;

	// Token: 0x040024E2 RID: 9442
	private Member supportedMember;

	// Token: 0x040024E3 RID: 9443
	public bool alreadyBegan;

	// Token: 0x0200047A RID: 1146
	public enum CreatorCodeStatus
	{
		// Token: 0x040024E5 RID: 9445
		Empty,
		// Token: 0x040024E6 RID: 9446
		Unchecked,
		// Token: 0x040024E7 RID: 9447
		Validating,
		// Token: 0x040024E8 RID: 9448
		Valid
	}

	// Token: 0x0200047B RID: 1147
	public enum ATMStages
	{
		// Token: 0x040024EA RID: 9450
		Unavailable,
		// Token: 0x040024EB RID: 9451
		Begin,
		// Token: 0x040024EC RID: 9452
		Menu,
		// Token: 0x040024ED RID: 9453
		Balance,
		// Token: 0x040024EE RID: 9454
		Choose,
		// Token: 0x040024EF RID: 9455
		Confirm,
		// Token: 0x040024F0 RID: 9456
		Purchasing,
		// Token: 0x040024F1 RID: 9457
		Success,
		// Token: 0x040024F2 RID: 9458
		Failure,
		// Token: 0x040024F3 RID: 9459
		SafeAccount
	}
}
