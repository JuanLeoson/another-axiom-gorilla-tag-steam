using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x0200091A RID: 2330
public class KIDUIButton : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x06003974 RID: 14708 RVA: 0x00129DBA File Offset: 0x00127FBA
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x00129DCB File Offset: 0x00127FCB
	protected override void OnEnable()
	{
		base.OnEnable();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x00129DF8 File Offset: 0x00127FF8
	private void PostUpdate()
	{
		if (!KIDUIButton._canTrigger)
		{
			KIDUIButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!base.interactable || !this.inside || !KIDUIButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown && !KIDUIButton._triggeredThisFrame)
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
			Button.ButtonClickedEvent onClick = base.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
			KIDUIButton._triggeredThisFrame = true;
			KIDUIButton._canTrigger = false;
		}
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x00129F44 File Offset: 0x00128144
	private void LateUpdate()
	{
		if (KIDUIButton._triggeredThisFrame)
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
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06003978 RID: 14712 RVA: 0x0012A029 File Offset: 0x00128229
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.inside = false;
	}

	// Token: 0x06003979 RID: 14713 RVA: 0x0012A039 File Offset: 0x00128239
	public void ResetButton()
	{
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x0012A048 File Offset: 0x00128248
	protected override void OnDisable()
	{
		this.FixStuckPressedState();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x0012A072 File Offset: 0x00128272
	private void FixStuckPressedState()
	{
		this.InstantClearState();
		this._buttonText.color = (base.interactable ? this._normalTextColor : this._disabledTextColor);
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x0012A0A8 File Offset: 0x001282A8
	protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		switch (state)
		{
		default:
			this._buttonText.color = this._normalTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Highlighted:
			this._buttonText.color = this._highlightedTextColor;
			this.SetIcons(false, true);
			return;
		case Selectable.SelectionState.Pressed:
			this._buttonText.color = this._pressedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Selected:
			this._buttonText.color = this._selectedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Disabled:
			this._buttonText.color = this._disabledTextColor;
			this.SetIcons(true, false);
			return;
		}
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x0012A158 File Offset: 0x00128358
	private void SetIcons(bool normalEnabled, bool highlightedEnabled)
	{
		if (this._normalIcon == null || this._highlightedIcon == null)
		{
			return;
		}
		GameObject normalIcon = this._normalIcon;
		if (normalIcon != null)
		{
			normalIcon.SetActive(normalEnabled);
		}
		GameObject highlightedIcon = this._highlightedIcon;
		if (highlightedIcon == null)
		{
			return;
		}
		highlightedIcon.SetActive(highlightedEnabled);
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x0012A1A8 File Offset: 0x001283A8
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.inside = true;
		if (!this.IsInteractable() || !this.IsActive())
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

	// Token: 0x0600397F RID: 14719 RVA: 0x0012A20C File Offset: 0x0012840C
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.inside = false;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._pressedVibrationStrength, this._pressedVibrationDuration);
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x0012A270 File Offset: 0x00128470
	public void SetText(string text)
	{
		this._buttonText.SetText(text, true);
	}

	// Token: 0x04004698 RID: 18072
	[SerializeField]
	private Image _borderImage;

	// Token: 0x04004699 RID: 18073
	[SerializeField]
	private RectTransform _fillImageRef;

	// Token: 0x0400469A RID: 18074
	[SerializeField]
	private TMP_Text _buttonText;

	// Token: 0x0400469B RID: 18075
	[Header("Transition States")]
	[Header("Normal")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalBorderColor;

	// Token: 0x0400469C RID: 18076
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalTextColor;

	// Token: 0x0400469D RID: 18077
	[SerializeField]
	private float _normalBorderSize;

	// Token: 0x0400469E RID: 18078
	[Header("Highlighted")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedBorderColor;

	// Token: 0x0400469F RID: 18079
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedTextColor;

	// Token: 0x040046A0 RID: 18080
	[SerializeField]
	private float _highlightedBorderSize;

	// Token: 0x040046A1 RID: 18081
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x040046A2 RID: 18082
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x040046A3 RID: 18083
	[Header("Pressed")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedBorderColor;

	// Token: 0x040046A4 RID: 18084
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedTextColor;

	// Token: 0x040046A5 RID: 18085
	[SerializeField]
	private float _pressedBorderSize;

	// Token: 0x040046A6 RID: 18086
	[SerializeField]
	private float _pressedVibrationStrength = 0.5f;

	// Token: 0x040046A7 RID: 18087
	[SerializeField]
	private float _pressedVibrationDuration = 0.1f;

	// Token: 0x040046A8 RID: 18088
	[Header("Selected")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedBorderColor;

	// Token: 0x040046A9 RID: 18089
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedTextColor;

	// Token: 0x040046AA RID: 18090
	[SerializeField]
	private float _selectedBorderSize;

	// Token: 0x040046AB RID: 18091
	[Header("Disabled")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledBorderColor;

	// Token: 0x040046AC RID: 18092
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledTextColor;

	// Token: 0x040046AD RID: 18093
	[SerializeField]
	private float _disabledBorderSize;

	// Token: 0x040046AE RID: 18094
	[Header("Icon Swap Settings")]
	[SerializeField]
	private GameObject _normalIcon;

	// Token: 0x040046AF RID: 18095
	[SerializeField]
	private GameObject _highlightedIcon;

	// Token: 0x040046B0 RID: 18096
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x040046B1 RID: 18097
	private bool inside;

	// Token: 0x040046B2 RID: 18098
	private static bool _triggeredThisFrame = false;

	// Token: 0x040046B3 RID: 18099
	private static bool _canTrigger = true;
}
