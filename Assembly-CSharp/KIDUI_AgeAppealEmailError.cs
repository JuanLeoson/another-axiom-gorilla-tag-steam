using System;
using TMPro;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class KIDUI_AgeAppealEmailError : MonoBehaviour
{
	// Token: 0x060039FC RID: 14844 RVA: 0x0012BF16 File Offset: 0x0012A116
	public void ShowAgeAppealEmailErrorScreen(bool hasChallenge, int newAge, string email)
	{
		this.hasChallenge = hasChallenge;
		this.newAge = newAge;
		this._emailText.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x0012BF3E File Offset: 0x0012A13E
	public void onBackPressed()
	{
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAge);
	}

	// Token: 0x04004720 RID: 18208
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04004721 RID: 18209
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04004722 RID: 18210
	private bool hasChallenge;

	// Token: 0x04004723 RID: 18211
	private int newAge;
}
