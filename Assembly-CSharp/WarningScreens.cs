using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000952 RID: 2386
public class WarningScreens : MonoBehaviour
{
	// Token: 0x06003AA8 RID: 15016 RVA: 0x0012F805 File Offset: 0x0012DA05
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	// Token: 0x06003AA9 RID: 15017 RVA: 0x0012F82C File Offset: 0x0012DA2C
	private Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreenInternal>d__14 <StartWarningScreenInternal>d__;
		<StartWarningScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreenInternal>d__.<>4__this = this;
		<StartWarningScreenInternal>d__.cancellationToken = cancellationToken;
		<StartWarningScreenInternal>d__.<>1__state = -1;
		<StartWarningScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartWarningScreenInternal>d__14>(ref <StartWarningScreenInternal>d__);
		return <StartWarningScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x0012F878 File Offset: 0x0012DA78
	private Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreenInternal>d__15 <StartOptInFollowUpScreenInternal>d__;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreenInternal>d__.<>4__this = this;
		<StartOptInFollowUpScreenInternal>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreenInternal>d__.<>1__state = -1;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreenInternal>d__15>(ref <StartOptInFollowUpScreenInternal>d__);
		return <StartOptInFollowUpScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x0012F8C4 File Offset: 0x0012DAC4
	public static Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreen>d__16 <StartWarningScreen>d__;
		<StartWarningScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreen>d__.cancellationToken = cancellationToken;
		<StartWarningScreen>d__.<>1__state = -1;
		<StartWarningScreen>d__.<>t__builder.Start<WarningScreens.<StartWarningScreen>d__16>(ref <StartWarningScreen>d__);
		return <StartWarningScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x0012F908 File Offset: 0x0012DB08
	public static Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreen>d__17 <StartOptInFollowUpScreen>d__;
		<StartOptInFollowUpScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreen>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreen>d__.<>1__state = -1;
		<StartOptInFollowUpScreen>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreen>d__17>(ref <StartOptInFollowUpScreen>d__);
		return <StartOptInFollowUpScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x0012F94C File Offset: 0x0012DB4C
	private static Task WaitForResponse(CancellationToken cancellationToken)
	{
		WarningScreens.<WaitForResponse>d__18 <WaitForResponse>d__;
		<WaitForResponse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForResponse>d__.cancellationToken = cancellationToken;
		<WaitForResponse>d__.<>1__state = -1;
		<WaitForResponse>d__.<>t__builder.Start<WarningScreens.<WaitForResponse>d__18>(ref <WaitForResponse>d__);
		return <WaitForResponse>d__.<>t__builder.Task;
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x0012F98F File Offset: 0x0012DB8F
	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction();
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x0012F9BA File Offset: 0x0012DBBA
	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction();
	}

	// Token: 0x040047F4 RID: 18420
	private static WarningScreens _activeReference;

	// Token: 0x040047F5 RID: 18421
	[SerializeField]
	private MessageBox _messageBox;

	// Token: 0x040047F6 RID: 18422
	[SerializeField]
	private GameObject _imageContainerAfter;

	// Token: 0x040047F7 RID: 18423
	[SerializeField]
	private GameObject _imageContainerBefore;

	// Token: 0x040047F8 RID: 18424
	[SerializeField]
	private TMP_Text _withImageTextBefore;

	// Token: 0x040047F9 RID: 18425
	[SerializeField]
	private TMP_Text _withImageTextAfter;

	// Token: 0x040047FA RID: 18426
	[SerializeField]
	private TMP_Text _noImageText;

	// Token: 0x040047FB RID: 18427
	private Action _onLeftButtonPressedAction;

	// Token: 0x040047FC RID: 18428
	private Action _onRightButtonPressedAction;

	// Token: 0x040047FD RID: 18429
	private static WarningButtonResult _result;

	// Token: 0x040047FE RID: 18430
	private static WarningButtonResult _leftButtonResult;

	// Token: 0x040047FF RID: 18431
	private static WarningButtonResult _rightButtonResult;

	// Token: 0x04004800 RID: 18432
	private static bool _closedMessageBox;
}
