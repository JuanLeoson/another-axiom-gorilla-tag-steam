using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000914 RID: 2324
public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	// Token: 0x0600395D RID: 14685 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnEnable()
	{
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x00129702 File Offset: 0x00127902
	public void ShowRestrictedAccessScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x00020127 File Offset: 0x0001E327
	public void OnChangeAgePressed()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400467D RID: 18045
	[SerializeField]
	private KIDUIButton _changeAgeButton;

	// Token: 0x0400467E RID: 18046
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x0400467F RID: 18047
	private string _submittedEmailAddress;

	// Token: 0x04004680 RID: 18048
	private CancellationTokenSource _cancellationTokenSource;
}
