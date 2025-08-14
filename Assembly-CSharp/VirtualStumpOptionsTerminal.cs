using System;
using System.Collections.Generic;
using System.Text;
using GorillaExtensions;
using GorillaTagScripts.UI;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200084B RID: 2123
public class VirtualStumpOptionsTerminal : MonoBehaviour
{
	// Token: 0x06003548 RID: 13640 RVA: 0x00117460 File Offset: 0x00115660
	public void Start()
	{
		this.optionList.gameObject.SetActive(true);
		this.mainScreenText.gameObject.SetActive(true);
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoginStarted.AddListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.AddListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserProfileUpdated.AddListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
	}

	// Token: 0x06003549 RID: 13641 RVA: 0x0011751C File Offset: 0x0011571C
	public void OnDestroy()
	{
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoginStarted.RemoveListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.RemoveListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserProfileUpdated.RemoveListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
	}

	// Token: 0x0600354A RID: 13642 RVA: 0x001175B9 File Offset: 0x001157B9
	public void OnEnable()
	{
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x001175D0 File Offset: 0x001157D0
	private void OnKeyPressed(CustomMapKeyboardBinding pressedButton)
	{
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.cachedError = null;
			this.RefreshButtonState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.up)
		{
			int num = this.currentState - VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE;
			if (num < 0)
			{
				num = 1;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.down)
		{
			int num2 = (int)(this.currentState + 1);
			if (num2 >= 2)
			{
				num2 = 0;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num2);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.OnKeyPressed_ModIOAccount(pressedButton);
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.OnKeyPressed_RoomSize(pressedButton);
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x0011766C File Offset: 0x0011586C
	private void ChangeState(VirtualStumpOptionsTerminal.ETerminalState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		this.currentState = newState;
		this.RefreshButtonState();
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x00117688 File Offset: 0x00115888
	private void RefreshButtonState()
	{
		for (int i = 0; i < this.contextualButtons.Count; i++)
		{
			if (this.contextualButtons[i].IsNotNull())
			{
				this.contextualButtons[i].SetActive(false);
			}
		}
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.OKButton.SetActive(true);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			for (int j = 0; j < this.buttonsToShow_MODIO.Count; j++)
			{
				if (this.buttonsToShow_MODIO[j].IsNotNull())
				{
					this.buttonsToShow_MODIO[j].SetActive(true);
				}
			}
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		for (int k = 0; k < this.buttonsToShow_ROOMSIZE.Count; k++)
		{
			if (this.buttonsToShow_ROOMSIZE[k].IsNotNull())
			{
				this.buttonsToShow_ROOMSIZE[k].SetActive(true);
			}
		}
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x00117770 File Offset: 0x00115970
	private void UpdateOptionListForCurrentState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 2; i++)
		{
			stringBuilder.Append(this.optionStrings[i]);
			if (i == (int)this.currentState)
			{
				stringBuilder.Append(" <-");
			}
			stringBuilder.Append("\n");
		}
		this.optionList.text = stringBuilder.ToString();
	}

	// Token: 0x0600354F RID: 13647 RVA: 0x001177D4 File Offset: 0x001159D4
	private void UpdateScreen()
	{
		this.mainScreenText.text = "";
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.RefreshButtonState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.cachedError);
			TMP_Text tmp_Text = this.mainScreenText;
			string str = "<color=\"red\">";
			StringBuilder stringBuilder2 = stringBuilder;
			tmp_Text.text = str + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.mainScreenText.text = this.UpdateScreen_ModIOAccount();
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.mainScreenText.text = this.UpdateScreen_RoomSize();
	}

	// Token: 0x06003550 RID: 13648 RVA: 0x0011786C File Offset: 0x00115A6C
	public void OnModIOLoginStarted()
	{
		ModIOManager.CancelExternalAuthentication();
		this.UpdateScreen();
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x00117879 File Offset: 0x00115A79
	public void OnModIOLoggedIn()
	{
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x0011789E File Offset: 0x00115A9E
	private void OnModIOLoggedOut()
	{
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.CancelExternalAuthentication();
		this.processingAccountLink = false;
		this.UpdateScreen();
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x001178C8 File Offset: 0x00115AC8
	private void OnModIOLoginFailed(string error)
	{
		this.processingAccountLink = false;
		this.cachedError = error;
		this.UpdateScreen();
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x001178DE File Offset: 0x00115ADE
	private void OnModIOUserProfileUpdated(UserProfile profile)
	{
		this.UpdateScreen();
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x001178E8 File Offset: 0x00115AE8
	private void OnKeyPressed_ModIOAccount(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.option1 && !this.processingAccountLink && (!ModIOManager.IsLoggedIn() || ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount))
		{
			this.StartAccountLinkingProcess();
		}
		if (pressedButton == CustomMapKeyboardBinding.option2 && ModIOManager.IsLoggedIn())
		{
			ModIOManager.LogoutFromModIO();
		}
		if (pressedButton == CustomMapKeyboardBinding.option3 && !ModIOManager.IsLoggedIn())
		{
			ModIOManager.CancelExternalAuthentication();
			ModIOManager.RequestPlatformLogin(null);
		}
	}

	// Token: 0x06003556 RID: 13654 RVA: 0x00117940 File Offset: 0x00115B40
	public void StartAccountLinkingProcess()
	{
		if (!this.processingAccountLink)
		{
			this.processingAccountLink = true;
			ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOUserProfileUpdated.RemoveListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
			ModIOManager.IsAuthenticated(delegate(Result isAuthenticatedResult)
			{
				if (isAuthenticatedResult.Succeeded())
				{
					if (ModIOManager.GetLastAuthMethod() == ModIOManager.ModIOAuthMethod.LinkedAccount)
					{
						ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
						ModIOManager.OnModIOUserProfileUpdated.RemoveListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
						ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
						ModIOManager.OnModIOUserProfileUpdated.AddListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
						this.processingAccountLink = false;
						return;
					}
					ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
					ModIOManager.LogoutFromModIO();
					ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
					ModIOManager.OnModIOUserProfileUpdated.RemoveListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
					ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
					ModIOManager.OnModIOUserProfileUpdated.AddListener(new UnityAction<UserProfile>(this.OnModIOUserProfileUpdated));
				}
				ModIOManager.RequestAccountLinkCode(delegate(ModIORequestResult requestLinkCodeResult, string linkURL, string linkCode)
				{
					if (requestLinkCodeResult.success)
					{
						this.cachedLinkURL = linkURL;
						this.cachedLinkCode = linkCode;
						this.UpdateScreen();
						return;
					}
					this.cachedError = requestLinkCodeResult.message + "\n\nPRESS THE 'LINK MOD.IO ACCOUNT' BUTTON TO RETRY.";
					this.processingAccountLink = false;
					this.UpdateScreen();
				}, delegate(ModIORequestResult externalAuthenticationResult)
				{
					if (!externalAuthenticationResult.success)
					{
						Debug.LogError("[ModIOAccountLinkingTerminal::StartAccountLinkingProcess] Failed to log in to mod.io: " + externalAuthenticationResult.message);
						this.cachedError = externalAuthenticationResult.message + "\n\nPRESS THE 'LINK MOD.IO ACCOUNT' BUTTON TO RETRY.";
						this.processingAccountLink = false;
						this.UpdateScreen();
					}
				});
			});
			return;
		}
		this.UpdateScreen();
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x001179A0 File Offset: 0x00115BA0
	private string UpdateScreen_ModIOAccount()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (ModIOManager.IsLoggedIn())
		{
			stringBuilder.Append(this.loggedInAsString + "\n");
			stringBuilder.Append("   " + ModIOManager.GetCurrentUsername() + "\n\n");
			if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
			{
				stringBuilder.Append(this.linkAccountPromptString + "\n");
			}
			else
			{
				stringBuilder.Append(this.alreadyLinkedAccountString + "\n");
			}
		}
		else if (ModIOManager.IsLoggingIn() && !this.processingAccountLink)
		{
			stringBuilder.Append(this.loggingInString);
		}
		else if (this.processingAccountLink)
		{
			stringBuilder.Append(this.linkAccountPromptString + "\n\n");
			stringBuilder.Append(this.urlLabelString + this.cachedLinkURL + "\n");
			stringBuilder.Append(this.linkCodeLabelString + this.cachedLinkCode + "\n");
		}
		else
		{
			stringBuilder.Append(this.notLoggedInString + "\n\n");
			stringBuilder.Append(this.loginPromptString);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x00117ACF File Offset: 0x00115CCF
	private void OnKeyPressed_RoomSize(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.left)
		{
			this.DecrementRoomSize();
		}
		if (pressedButton == CustomMapKeyboardBinding.right)
		{
			this.IncrementRoomSize();
		}
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x00117AE7 File Offset: 0x00115CE7
	private void DecrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() - 1);
		this.UpdateScreen();
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x00117AFC File Offset: 0x00115CFC
	private void IncrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() + 1);
		this.UpdateScreen();
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x00117B14 File Offset: 0x00115D14
	private string UpdateScreen_RoomSize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.roomSizeDescriptionString + "\n\n");
		stringBuilder.Append(this.roomSizeLabelString + RoomSystem.GetOverridenRoomSize().ToString());
		return stringBuilder.ToString();
	}

	// Token: 0x04004241 RID: 16961
	[SerializeField]
	private TMP_Text optionList;

	// Token: 0x04004242 RID: 16962
	[SerializeField]
	private TMP_Text mainScreenText;

	// Token: 0x04004243 RID: 16963
	[SerializeField]
	private CustomMapsKeyboard keyboard;

	// Token: 0x04004244 RID: 16964
	[SerializeField]
	private List<string> optionStrings = new List<string>
	{
		"MOD.IO",
		"ROOM SIZE"
	};

	// Token: 0x04004245 RID: 16965
	[SerializeField]
	private string loggedInAsString = "LOGGED INTO MOD.IO AS: ";

	// Token: 0x04004246 RID: 16966
	[SerializeField]
	private string notLoggedInString = "LOGGED OUT OF MOD.IO";

	// Token: 0x04004247 RID: 16967
	[SerializeField]
	private string loginPromptString = "PRESS THE 'PLATFORM LOGIN' OR 'LINK MOD.IO ACCOUNT' BUTTON TO LOGIN";

	// Token: 0x04004248 RID: 16968
	[SerializeField]
	private string loggingInString = "LOGGING IN TO MOD.IO...";

	// Token: 0x04004249 RID: 16969
	[SerializeField]
	private string linkAccountPromptString = "IF YOU HAVE AN EXISTING MOD.IO ACCOUNT, YOU CAN LINK IT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON.";

	// Token: 0x0400424A RID: 16970
	[SerializeField]
	private string alreadyLinkedAccountString = "YOU'VE ALREADY LINKED YOUR MOD.IO ACCOUNT.";

	// Token: 0x0400424B RID: 16971
	[SerializeField]
	private string accountLinkingPromptString = "PLEASE GO TO THIS URL IN YOUR BROWSER AND LOG IN TO YOUR MOD.IO ACCOUNT. ONCE LOGGED IN, ENTER THE FOLLOWING CODE TO PROCEED: ";

	// Token: 0x0400424C RID: 16972
	[SerializeField]
	private string urlLabelString = "URL: ";

	// Token: 0x0400424D RID: 16973
	[SerializeField]
	private string linkCodeLabelString = "CODE: ";

	// Token: 0x0400424E RID: 16974
	[SerializeField]
	private string roomSizeDescriptionString = "THIS SETTING WILL CHANGE THE MAXIMUM AMOUNT OF PLAYERS ALLOWED IN PRIVATE ROOMS YOU CREATE. WHEN JOINING A PUBLIC ROOM, THE MAP YOU'VE LOADED WILL CONTROL THE ROOM SIZE.";

	// Token: 0x0400424F RID: 16975
	[SerializeField]
	private string roomSizeLabelString = "MAX PLAYERS: ";

	// Token: 0x04004250 RID: 16976
	[SerializeField]
	private GameObject OKButton;

	// Token: 0x04004251 RID: 16977
	[SerializeField]
	private List<GameObject> contextualButtons = new List<GameObject>();

	// Token: 0x04004252 RID: 16978
	[SerializeField]
	private List<GameObject> buttonsToShow_MODIO = new List<GameObject>();

	// Token: 0x04004253 RID: 16979
	[SerializeField]
	private List<GameObject> buttonsToShow_ROOMSIZE = new List<GameObject>();

	// Token: 0x04004254 RID: 16980
	private bool processingAccountLink;

	// Token: 0x04004255 RID: 16981
	private string cachedLinkURL = "";

	// Token: 0x04004256 RID: 16982
	private string cachedLinkCode = "";

	// Token: 0x04004257 RID: 16983
	private string cachedError;

	// Token: 0x04004258 RID: 16984
	private VirtualStumpOptionsTerminal.ETerminalState currentState;

	// Token: 0x0200084C RID: 2124
	private enum ETerminalState
	{
		// Token: 0x0400425A RID: 16986
		MODIO_ACCOUNT,
		// Token: 0x0400425B RID: 16987
		ROOM_SIZE,
		// Token: 0x0400425C RID: 16988
		NUM_STATES
	}
}
