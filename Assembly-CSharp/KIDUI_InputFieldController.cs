using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Valve.VR;

// Token: 0x02000943 RID: 2371
public class KIDUI_InputFieldController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003A53 RID: 14931 RVA: 0x00129DBA File Offset: 0x00127FBA
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x0012D82C File Offset: 0x0012BA2C
	protected void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x0012D894 File Offset: 0x0012BA94
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Remove(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Remove(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06003A56 RID: 14934 RVA: 0x0012D8FC File Offset: 0x0012BAFC
	private void Update()
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		SteamVR.instance.overlay.GetKeyboardText(this._inputStringBuilder, 1024U);
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] String BUilder Says: [" + this._inputStringBuilder.ToString() + "]");
		this._inputField.text = this._inputBuffer;
		this._inputField.stringPosition = this._inputBuffer.Length;
	}

	// Token: 0x06003A57 RID: 14935 RVA: 0x0012D974 File Offset: 0x0012BB74
	private void PostUpdate()
	{
		if (!this._inputField.interactable || !this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnClick is pressed. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
			this.OnClickedInputField("");
		}
	}

	// Token: 0x06003A58 RID: 14936 RVA: 0x0012DA88 File Offset: 0x0012BC88
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
		if (!this._inputField.IsInteractable() || !this._inputField.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06003A59 RID: 14937 RVA: 0x0012DAEF File Offset: 0x0012BCEF
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06003A5A RID: 14938 RVA: 0x0012DAF8 File Offset: 0x0012BCF8
	private void OnClickedInputField(string _ = "")
	{
		if (this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUT_FIELD_CONTROLLER] Selecting and Activating Input Field");
		EVROverlayError evroverlayError = OpenVR.Overlay.ShowKeyboard(0, 0, 1U, "Enter Email", 1024U, this._inputField.text ?? "", 0UL);
		if (evroverlayError != EVROverlayError.None)
		{
			Debug.LogError("[KID::INPUT_FIELD_CONTROLLER] Failed to open keyboard. Resulted with error: [" + evroverlayError.ToString() + "]");
			return;
		}
		this._inputBuffer = (this._inputField.text ?? "");
		this.keyboardShowing = true;
		HandRayController.Instance.DisableHandRays();
	}

	// Token: 0x06003A5B RID: 14939 RVA: 0x0012DB98 File Offset: 0x0012BD98
	private void OnChar(VREvent_t ev)
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		char c = ev.data.keyboard.cNewInput[0];
		if (c == '\b')
		{
			this._inputBuffer = this._inputBuffer.Remove(this._inputBuffer.Length - 1, 1);
			return;
		}
		if (this.IsIllegalChar(c))
		{
			return;
		}
		this._inputBuffer += c.ToString();
	}

	// Token: 0x06003A5C RID: 14940 RVA: 0x0012DC0C File Offset: 0x0012BE0C
	private void OnKeyboardClosed(VREvent_t ev)
	{
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Trying to close Keyboard");
		if (!this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Closing Keyboard");
		OpenVR.Overlay.HideKeyboard();
		this._inputField.text = this._inputBuffer;
		this._inputField.DeactivateInputField(false);
		HandRayController.Instance.EnableHandRays();
		this.keyboardShowing = false;
	}

	// Token: 0x06003A5D RID: 14941 RVA: 0x0012DC6E File Offset: 0x0012BE6E
	private bool IsIllegalChar(char c)
	{
		return c == '\t' || c == '\n';
	}

	// Token: 0x04004792 RID: 18322
	[Header("Haptics")]
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04004793 RID: 18323
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04004794 RID: 18324
	[Header("Steam Settings")]
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x04004795 RID: 18325
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04004796 RID: 18326
	public bool testMinimal;

	// Token: 0x04004797 RID: 18327
	public bool minimalMode;

	// Token: 0x04004798 RID: 18328
	private bool inside;

	// Token: 0x04004799 RID: 18329
	private bool keyboardShowing;

	// Token: 0x0400479A RID: 18330
	private bool _canTrigger = true;

	// Token: 0x0400479B RID: 18331
	private string _testStr = string.Empty;

	// Token: 0x0400479C RID: 18332
	private string previousStr = string.Empty;

	// Token: 0x0400479D RID: 18333
	private StringBuilder _inputStringBuilder = new StringBuilder(1024);

	// Token: 0x0400479E RID: 18334
	private string _inputBuffer = "";
}
