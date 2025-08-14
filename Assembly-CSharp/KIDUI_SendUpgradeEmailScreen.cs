using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200094C RID: 2380
public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	// Token: 0x06003A92 RID: 14994 RVA: 0x0012F2E0 File Offset: 0x0012D4E0
	public Task SendUpgradeEmail(List<string> requestedPermissions)
	{
		KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4 <SendUpgradeEmail>d__;
		<SendUpgradeEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SendUpgradeEmail>d__.<>4__this = this;
		<SendUpgradeEmail>d__.requestedPermissions = requestedPermissions;
		<SendUpgradeEmail>d__.<>1__state = -1;
		<SendUpgradeEmail>d__.<>t__builder.Start<KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4>(ref <SendUpgradeEmail>d__);
		return <SendUpgradeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x0012F32B File Offset: 0x0012D52B
	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x0012F345 File Offset: 0x0012D545
	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show(null);
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x0012F35F File Offset: 0x0012D55F
	private void OnFailure(string errorMessage)
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show(errorMessage);
	}

	// Token: 0x040047E1 RID: 18401
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x040047E2 RID: 18402
	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	// Token: 0x040047E3 RID: 18403
	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	// Token: 0x040047E4 RID: 18404
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
