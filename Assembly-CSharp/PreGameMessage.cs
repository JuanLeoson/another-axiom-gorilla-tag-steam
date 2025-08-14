using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000909 RID: 2313
public class PreGameMessage : MonoBehaviour
{
	// Token: 0x06003924 RID: 14628 RVA: 0x001286AE File Offset: 0x001268AE
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x001286D2 File Offset: 0x001268D2
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x001286F8 File Offset: 0x001268F8
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x06003927 RID: 14631 RVA: 0x0012879C File Offset: 0x0012699C
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmationButton, string messageAlternativeButton, Action onConfirmationAction, Action onAlternativeAction, float bodyFontSize = 0.5f)
	{
		this._confirmButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageAlternativeConfirmationTxt.text = messageConfirmationButton;
		this._messageAlternativeButtonTxt.text = messageAlternativeButton;
		this._confirmationAction = onConfirmationAction;
		this._alternativeAction = onAlternativeAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null || this._alternativeAction == null)
		{
			Debug.LogError("[KID] Trying to show a mesasge with multiple buttons, but one or both callbacks are null");
			this._multiButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageAlternativeConfirmationTxt.text) && !string.IsNullOrEmpty(this._messageAlternativeButtonTxt.text))
		{
			this._multiButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x06003928 RID: 14632 RVA: 0x00128874 File Offset: 0x00126A74
	public Task ShowMessageWithAwait(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		PreGameMessage.<ShowMessageWithAwait>d__20 <ShowMessageWithAwait>d__;
		<ShowMessageWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowMessageWithAwait>d__.<>4__this = this;
		<ShowMessageWithAwait>d__.messageTitle = messageTitle;
		<ShowMessageWithAwait>d__.messageBody = messageBody;
		<ShowMessageWithAwait>d__.messageConfirmation = messageConfirmation;
		<ShowMessageWithAwait>d__.onConfirmationAction = onConfirmationAction;
		<ShowMessageWithAwait>d__.bodyFontSize = bodyFontSize;
		<ShowMessageWithAwait>d__.<>1__state = -1;
		<ShowMessageWithAwait>d__.<>t__builder.Start<PreGameMessage.<ShowMessageWithAwait>d__20>(ref <ShowMessageWithAwait>d__);
		return <ShowMessageWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06003929 RID: 14633 RVA: 0x001288E4 File Offset: 0x00126AE4
	public void UpdateMessage(string newMessageBody, string newConfirmButton)
	{
		this._messageBodyTxt.text = newMessageBody;
		this._messageConfirmationTxt.text = newConfirmButton;
		if (string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(false);
			return;
		}
		if (this._confirmationAction != null)
		{
			this._confirmButtonRoot.SetActive(true);
		}
	}

	// Token: 0x0600392A RID: 14634 RVA: 0x0012893C File Offset: 0x00126B3C
	public void CloseMessage()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
	}

	// Token: 0x0600392B RID: 14635 RVA: 0x00128950 File Offset: 0x00126B50
	private Task WaitForCompletion()
	{
		PreGameMessage.<WaitForCompletion>d__23 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<PreGameMessage.<WaitForCompletion>d__23>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	// Token: 0x0600392C RID: 14636 RVA: 0x00128994 File Offset: 0x00126B94
	private void PostUpdate()
	{
		bool isLeftStick = ControllerBehaviour.Instance.IsLeftStick;
		bool isRightStick = ControllerBehaviour.Instance.IsRightStick;
		bool buttonDown = ControllerBehaviour.Instance.ButtonDown;
		if (this._multiButtonRoot.activeInHierarchy)
		{
			if (isLeftStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarR.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarR.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else if (isRightStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnAlternativePressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
		if (this._confirmButtonRoot.activeInHierarchy)
		{
			if (buttonDown)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x00128C6B File Offset: 0x00126E6B
	private void OnConfirmedPressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action confirmationAction = this._confirmationAction;
		if (confirmationAction == null)
		{
			return;
		}
		confirmationAction();
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x00128C94 File Offset: 0x00126E94
	private void OnAlternativePressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action alternativeAction = this._alternativeAction;
		if (alternativeAction == null)
		{
			return;
		}
		alternativeAction();
	}

	// Token: 0x04004645 RID: 17989
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04004646 RID: 17990
	[SerializeField]
	private TMP_Text _messageTitleTxt;

	// Token: 0x04004647 RID: 17991
	[SerializeField]
	private TMP_Text _messageBodyTxt;

	// Token: 0x04004648 RID: 17992
	[SerializeField]
	private GameObject _confirmButtonRoot;

	// Token: 0x04004649 RID: 17993
	[SerializeField]
	private GameObject _multiButtonRoot;

	// Token: 0x0400464A RID: 17994
	[SerializeField]
	private TMP_Text _messageConfirmationTxt;

	// Token: 0x0400464B RID: 17995
	[SerializeField]
	private TMP_Text _messageAlternativeConfirmationTxt;

	// Token: 0x0400464C RID: 17996
	[SerializeField]
	private TMP_Text _messageAlternativeButtonTxt;

	// Token: 0x0400464D RID: 17997
	private Action _confirmationAction;

	// Token: 0x0400464E RID: 17998
	private Action _alternativeAction;

	// Token: 0x0400464F RID: 17999
	private bool _hasCompleted;

	// Token: 0x04004650 RID: 18000
	private float progress;

	// Token: 0x04004651 RID: 18001
	[SerializeField]
	private float holdTime;

	// Token: 0x04004652 RID: 18002
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04004653 RID: 18003
	[SerializeField]
	private LineRenderer progressBarL;

	// Token: 0x04004654 RID: 18004
	[SerializeField]
	private LineRenderer progressBarR;
}
