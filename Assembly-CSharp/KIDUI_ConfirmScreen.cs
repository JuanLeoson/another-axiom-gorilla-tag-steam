using System;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

// Token: 0x02000938 RID: 2360
public class KIDUI_ConfirmScreen : MonoBehaviour
{
	// Token: 0x06003A22 RID: 14882 RVA: 0x0012C87C File Offset: 0x0012AA7C
	private void Awake()
	{
		if (this._emailToConfirmTxt == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email To Confirm Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._setupScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x0012C8EE File Offset: 0x0012AAEE
	private void OnEnable()
	{
		this._confirmButton.interactable = true;
		this._backButton.interactable = true;
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x0012C908 File Offset: 0x0012AB08
	public void OnEmailSubmitted(string emailAddress)
	{
		this._submittedEmailAddress = emailAddress;
		this._emailToConfirmTxt.text = this._submittedEmailAddress;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x0012C930 File Offset: 0x0012AB30
	public void OnConfirmPressed()
	{
		KIDUI_ConfirmScreen.<OnConfirmPressed>d__16 <OnConfirmPressed>d__;
		<OnConfirmPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnConfirmPressed>d__.<>4__this = this;
		<OnConfirmPressed>d__.<>1__state = -1;
		<OnConfirmPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnConfirmPressed>d__16>(ref <OnConfirmPressed>d__);
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x0012C968 File Offset: 0x0012AB68
	public void OnBackPressed()
	{
		KIDUI_ConfirmScreen.<OnBackPressed>d__17 <OnBackPressed>d__;
		<OnBackPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnBackPressed>d__.<>4__this = this;
		<OnBackPressed>d__.<>1__state = -1;
		<OnBackPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnBackPressed>d__17>(ref <OnBackPressed>d__);
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x0012C99F File Offset: 0x0012AB9F
	public void NotifyOfResult(bool success)
	{
		this._hasCompletedSendEmailRequest = true;
		this._emailRequestResult = success;
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x0012C9B0 File Offset: 0x0012ABB0
	private void ShowErrorScreen(string errorMessage)
	{
		KIDUI_ConfirmScreen.<ShowErrorScreen>d__19 <ShowErrorScreen>d__;
		<ShowErrorScreen>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ShowErrorScreen>d__.<>4__this = this;
		<ShowErrorScreen>d__.errorMessage = errorMessage;
		<ShowErrorScreen>d__.<>1__state = -1;
		<ShowErrorScreen>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<ShowErrorScreen>d__19>(ref <ShowErrorScreen>d__);
	}

	// Token: 0x04004752 RID: 18258
	[SerializeField]
	private TMP_Text _emailToConfirmTxt;

	// Token: 0x04004753 RID: 18259
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04004754 RID: 18260
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;

	// Token: 0x04004755 RID: 18261
	[SerializeField]
	private KIDUI_ErrorScreen _errorScreen;

	// Token: 0x04004756 RID: 18262
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04004757 RID: 18263
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04004758 RID: 18264
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04004759 RID: 18265
	[SerializeField]
	private KIDUIButton _backButton;

	// Token: 0x0400475A RID: 18266
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x0400475B RID: 18267
	private string _submittedEmailAddress;

	// Token: 0x0400475C RID: 18268
	private CancellationTokenSource _cancellationTokenSource;

	// Token: 0x0400475D RID: 18269
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x0400475E RID: 18270
	private bool _emailRequestResult;
}
