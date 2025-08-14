using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

// Token: 0x020008C8 RID: 2248
public class KIDAgeAppeal : MonoBehaviour
{
	// Token: 0x06003802 RID: 14338 RVA: 0x001211E8 File Offset: 0x0011F3E8
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x00121228 File Offset: 0x0011F428
	public void OnNewAgeConfirmed()
	{
		KIDAgeAppeal.<OnNewAgeConfirmed>d__6 <OnNewAgeConfirmed>d__;
		<OnNewAgeConfirmed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnNewAgeConfirmed>d__.<>4__this = this;
		<OnNewAgeConfirmed>d__.<>1__state = -1;
		<OnNewAgeConfirmed>d__.<>t__builder.Start<KIDAgeAppeal.<OnNewAgeConfirmed>d__6>(ref <OnNewAgeConfirmed>d__);
	}

	// Token: 0x040044C4 RID: 17604
	[SerializeField]
	private TMP_Text _ageText;

	// Token: 0x040044C5 RID: 17605
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x040044C6 RID: 17606
	[SerializeField]
	private GameObject _inputsContainer;

	// Token: 0x040044C7 RID: 17607
	[SerializeField]
	private GameObject _monkeLoader;

	// Token: 0x040044C8 RID: 17608
	private AgeSliderWithProgressBar _ageSlider;
}
