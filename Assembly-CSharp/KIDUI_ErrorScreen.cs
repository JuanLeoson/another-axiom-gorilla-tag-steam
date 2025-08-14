using System;
using TMPro;
using UnityEngine;

// Token: 0x02000942 RID: 2370
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x06003A4E RID: 14926 RVA: 0x0012D7C6 File Offset: 0x0012B9C6
	public void ShowErrorScreen(string title, string email, string errorMessage)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		this._errorTxt.text = errorMessage;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x0012D7F8 File Offset: 0x0012B9F8
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x000A1481 File Offset: 0x0009F681
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x0012D812 File Offset: 0x0012BA12
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x0400478D RID: 18317
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x0400478E RID: 18318
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x0400478F RID: 18319
	[SerializeField]
	private TMP_Text _errorTxt;

	// Token: 0x04004790 RID: 18320
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04004791 RID: 18321
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
