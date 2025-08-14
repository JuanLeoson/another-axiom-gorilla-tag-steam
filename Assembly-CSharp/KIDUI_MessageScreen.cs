using System;
using TMPro;
using UnityEngine;

// Token: 0x0200094A RID: 2378
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x06003A8C RID: 14988 RVA: 0x0012F1FA File Offset: 0x0012D3FA
	public void Show(string errorMessage)
	{
		base.gameObject.SetActive(true);
		if (errorMessage != null && errorMessage.Length > 0)
		{
			this._errorTxt.text = errorMessage;
		}
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x0012F220 File Offset: 0x0012D420
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x040047DC RID: 18396
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040047DD RID: 18397
	[SerializeField]
	private TMP_Text _errorTxt;
}
