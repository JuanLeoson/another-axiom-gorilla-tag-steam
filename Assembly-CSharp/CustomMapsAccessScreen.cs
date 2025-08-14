using System;
using TMPro;
using UnityEngine;

// Token: 0x0200083E RID: 2110
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x060034C1 RID: 13505 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void Initialize()
	{
	}

	// Token: 0x060034C2 RID: 13506 RVA: 0x00112F10 File Offset: 0x00111110
	public override void Show()
	{
		base.Show();
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
		for (int i = 0; i < this.buttonsToHide.Length; i++)
		{
			this.buttonsToHide[i].SetActive(false);
		}
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x00112F77 File Offset: 0x00111177
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
		base.Hide();
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x00112FB2 File Offset: 0x001111B2
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
	}

	// Token: 0x060034C5 RID: 13509 RVA: 0x00112FE8 File Offset: 0x001111E8
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x00113034 File Offset: 0x00111234
	public void ShowTerminalControlPrompt()
	{
		this.errorText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
	}

	// Token: 0x040041A2 RID: 16802
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040041A3 RID: 16803
	[SerializeField]
	private TMP_Text loginRequiredText;

	// Token: 0x040041A4 RID: 16804
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x040041A5 RID: 16805
	[SerializeField]
	private GameObject[] buttonsToHide;
}
