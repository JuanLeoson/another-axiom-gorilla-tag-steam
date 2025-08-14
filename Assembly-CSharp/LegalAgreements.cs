using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000961 RID: 2401
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x06003AE1 RID: 15073 RVA: 0x00130BDE File Offset: 0x0012EDDE
	// (set) Token: 0x06003AE2 RID: 15074 RVA: 0x00130BE5 File Offset: 0x0012EDE5
	public static LegalAgreements instance { get; private set; }

	// Token: 0x06003AE3 RID: 15075 RVA: 0x00130BF0 File Offset: 0x0012EDF0
	protected virtual void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	// Token: 0x06003AE4 RID: 15076 RVA: 0x00130C48 File Offset: 0x0012EE48
	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (ControllerBehaviour.Instance.IsUpStick || ControllerBehaviour.Instance.IsDownStick)
		{
			if (ControllerBehaviour.Instance.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(ControllerBehaviour.Instance.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x00130DFC File Offset: 0x0012EFFC
	public virtual Task StartLegalAgreements()
	{
		LegalAgreements.<StartLegalAgreements>d__24 <StartLegalAgreements>d__;
		<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartLegalAgreements>d__.<>4__this = this;
		<StartLegalAgreements>d__.<>1__state = -1;
		<StartLegalAgreements>d__.<>t__builder.Start<LegalAgreements.<StartLegalAgreements>d__24>(ref <StartLegalAgreements>d__);
		return <StartLegalAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x00130E3F File Offset: 0x0012F03F
	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x00130E48 File Offset: 0x0012F048
	protected Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__27 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__27>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x00130E8C File Offset: 0x0012F08C
	private Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		LegalAgreements.<UpdateText>d__28 <UpdateText>d__;
		<UpdateText>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateText>d__.<>4__this = this;
		<UpdateText>d__.asset = asset;
		<UpdateText>d__.version = version;
		<UpdateText>d__.<>1__state = -1;
		<UpdateText>d__.<>t__builder.Start<LegalAgreements.<UpdateText>d__28>(ref <UpdateText>d__);
		return <UpdateText>d__.<>t__builder.Task;
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x00130EE0 File Offset: 0x0012F0E0
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.target = target;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x00130F3B File Offset: 0x0012F13B
	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x00130F44 File Offset: 0x0012F144
	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x00130F54 File Offset: 0x0012F154
	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__36 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__36>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x00130F98 File Offset: 0x0012F198
	private Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__37 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__37>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x00130FDC File Offset: 0x0012F1DC
	private Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__38 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__38>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x0400484B RID: 18507
	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	// Token: 0x0400484C RID: 18508
	[Header("Scroll Behavior")]
	[SerializeField]
	protected float _minScrollSpeed = 0.02f;

	// Token: 0x0400484D RID: 18509
	[SerializeField]
	private float _maxScrollSpeed = 3f;

	// Token: 0x0400484E RID: 18510
	[SerializeField]
	private float _scrollInterpTime = 3f;

	// Token: 0x0400484F RID: 18511
	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004851 RID: 18513
	[SerializeField]
	protected Transform uiParent;

	// Token: 0x04004852 RID: 18514
	[SerializeField]
	protected TMP_Text tmpBody;

	// Token: 0x04004853 RID: 18515
	[SerializeField]
	protected TMP_Text tmpTitle;

	// Token: 0x04004854 RID: 18516
	[SerializeField]
	protected Scrollbar scrollBar;

	// Token: 0x04004855 RID: 18517
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x04004856 RID: 18518
	[SerializeField]
	protected KIDUIButton _pressAndHoldToConfirmButton;

	// Token: 0x04004857 RID: 18519
	[SerializeField]
	private TMP_Text _scrollToBottomText;

	// Token: 0x04004858 RID: 18520
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04004859 RID: 18521
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x0400485A RID: 18522
	protected float stickHeldDuration;

	// Token: 0x0400485B RID: 18523
	protected float scrollSpeed;

	// Token: 0x0400485C RID: 18524
	private float scrollTime;

	// Token: 0x0400485D RID: 18525
	protected bool legalAgreementsStarted;

	// Token: 0x0400485E RID: 18526
	protected bool _accepted;

	// Token: 0x0400485F RID: 18527
	private string cachedText;

	// Token: 0x04004860 RID: 18528
	private int state;

	// Token: 0x04004861 RID: 18529
	private bool optIn;

	// Token: 0x04004862 RID: 18530
	private bool optional;
}
