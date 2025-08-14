using System;
using UnityEngine;

// Token: 0x0200094F RID: 2383
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x06003AA2 RID: 15010 RVA: 0x00129702 File Offset: 0x00127902
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003AA3 RID: 15011 RVA: 0x000A1481 File Offset: 0x0009F681
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
