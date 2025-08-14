using System;
using UnityEngine;

// Token: 0x0200094B RID: 2379
public class KIDUI_RestrictedAccessScreen : MonoBehaviour
{
	// Token: 0x06003A8F RID: 14991 RVA: 0x0012F23C File Offset: 0x0012D43C
	public void ShowRestrictedAccessScreen(SessionStatus? sessionStatus)
	{
		base.gameObject.SetActive(true);
		this._pendingStatusIndicator.SetActive(false);
		this._prohibitedStatusIndicator.SetActive(false);
		if (sessionStatus == null)
		{
			return;
		}
		if (sessionStatus != null)
		{
			switch (sessionStatus.GetValueOrDefault())
			{
			case SessionStatus.PASS:
			case SessionStatus.CHALLENGE:
			case SessionStatus.CHALLENGE_SESSION_UPGRADE:
				break;
			case SessionStatus.PROHIBITED:
				this._prohibitedStatusIndicator.SetActive(true);
				return;
			case SessionStatus.PENDING_AGE_APPEAL:
				this._pendingStatusIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x0012F2BC File Offset: 0x0012D4BC
	public void OnChangeAgePressed()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		base.gameObject.SetActive(false);
		this._ageAppealScreen.ShowAgeAppealScreen();
	}

	// Token: 0x040047DE RID: 18398
	[SerializeField]
	private KIDAgeAppeal _ageAppealScreen;

	// Token: 0x040047DF RID: 18399
	[SerializeField]
	private GameObject _pendingStatusIndicator;

	// Token: 0x040047E0 RID: 18400
	[SerializeField]
	private GameObject _prohibitedStatusIndicator;
}
