using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000920 RID: 2336
public class KIDUIToggle : Slider
{
	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x060039AC RID: 14764 RVA: 0x0012AA79 File Offset: 0x00128C79
	// (set) Token: 0x060039AD RID: 14765 RVA: 0x0012AA81 File Offset: 0x00128C81
	public bool CurrentValue { get; private set; }

	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x060039AE RID: 14766 RVA: 0x0012AA8A File Offset: 0x00128C8A
	public bool IsOn
	{
		get
		{
			return this.CurrentValue;
		}
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x0012AA92 File Offset: 0x00128C92
	protected override void Awake()
	{
		base.Awake();
		this.SetupToggleComponent();
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x0012AAA0 File Offset: 0x00128CA0
	protected override void Start()
	{
		base.Start();
		base.interactable = false;
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x0012AAAF File Offset: 0x00128CAF
	protected override void OnEnable()
	{
		base.OnEnable();
		base.interactable = false;
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x0012AAE0 File Offset: 0x00128CE0
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.Toggle();
	}

	// Token: 0x060039B3 RID: 14771 RVA: 0x0012AAEF File Offset: 0x00128CEF
	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.SetHighlighted();
		this.inside = true;
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x0012AAFE File Offset: 0x00128CFE
	public override void OnPointerExit(PointerEventData pointerEventData)
	{
		this.SetNormal();
		this.inside = false;
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x0012AB10 File Offset: 0x00128D10
	protected virtual void SetupToggleComponent()
	{
		this.SetupSliderComponent();
		base.handleRect.anchorMin = new Vector2(0f, 0.5f);
		base.handleRect.anchorMax = new Vector3(0f, 0.5f);
		base.handleRect.pivot = new Vector2(0f, 0.5f);
		base.handleRect.sizeDelta = new Vector2(base.handleRect.sizeDelta.x, base.handleRect.sizeDelta.x);
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x0012ABA8 File Offset: 0x00128DA8
	protected virtual void SetupSliderComponent()
	{
		base.interactable = false;
		base.colors.disabledColor = Color.white;
		this.SetColors();
		base.transition = Selectable.Transition.None;
	}

	// Token: 0x060039B7 RID: 14775 RVA: 0x0012ABDC File Offset: 0x00128DDC
	public void RegisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.AddListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x060039B8 RID: 14776 RVA: 0x0012AC10 File Offset: 0x00128E10
	public void UnregisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.RemoveListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x060039B9 RID: 14777 RVA: 0x0012AC44 File Offset: 0x00128E44
	public void RegisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x0012AC78 File Offset: 0x00128E78
	public void UnregisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x0012ACAC File Offset: 0x00128EAC
	public void RegisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x0012ACE0 File Offset: 0x00128EE0
	public void UnregisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x0012AD11 File Offset: 0x00128F11
	private void SetColors()
	{
		base.colors = this._fillColors;
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x0012AD1F File Offset: 0x00128F1F
	private void Toggle()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetStateAndStartAnimation(!this.CurrentValue, false);
	}

	// Token: 0x060039BF RID: 14783 RVA: 0x0012AD3A File Offset: 0x00128F3A
	public void SetValue(bool newValue)
	{
		if (newValue == this.CurrentValue)
		{
			return;
		}
		this.SetStateAndStartAnimation(newValue, false);
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x0012AD50 File Offset: 0x00128F50
	private void SetStateAndStartAnimation(bool state, bool skipAnim = false)
	{
		if (this.CurrentValue == state)
		{
			Debug.Log("IS SAME STATE, WILL NOT CHANGE");
			return;
		}
		this.CurrentValue = state;
		UnityEvent onToggleChanged = this._onToggleChanged;
		if (onToggleChanged != null)
		{
			onToggleChanged.Invoke();
		}
		if (this.CurrentValue)
		{
			UnityEvent onToggleOn = this._onToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
		}
		else
		{
			UnityEvent onToggleOff = this._onToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
		}
		if (this._animationCoroutine != null)
		{
			base.StopCoroutine(this._animationCoroutine);
		}
		this._handleUnlockIcon.gameObject.SetActive(this.CurrentValue);
		this._handleLockIcon.gameObject.SetActive(!this.CurrentValue);
		if (this._animationDuration == 0f || skipAnim)
		{
			Debug.Log("[KID::UI::SetStateAndStartAnimation] Skipping animation. Setting value to " + (this.CurrentValue ? "1f" : "0f"));
			this.value = (this.CurrentValue ? 1f : 0f);
			return;
		}
		this._animationCoroutine = base.StartCoroutine(this.AnimateSlider());
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x0012AE59 File Offset: 0x00129059
	private IEnumerator AnimateSlider()
	{
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] is {1}", base.name, this.CurrentValue));
		float startValue = this.CurrentValue ? 0f : 1f;
		float endValue = this.CurrentValue ? 1f : 0f;
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] Start: {1}, End: {2}, Value: {3}", new object[]
		{
			base.name,
			startValue,
			endValue,
			this.value
		}));
		float time = 0f;
		while (time < this._animationDuration)
		{
			time += Time.deltaTime;
			float t = this._toggleEase.Evaluate(time / this._animationDuration);
			this.value = Mathf.Lerp(startValue, endValue, t);
			yield return null;
		}
		this.value = endValue;
		yield break;
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x0012AE68 File Offset: 0x00129068
	private void PostUpdate()
	{
		if (!this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && KIDUIToggle._canTrigger)
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
				this.Toggle();
				KIDUIToggle._triggeredThisFrame = true;
				KIDUIToggle._canTrigger = false;
				return;
			}
			if (!ControllerBehaviour.Instance.TriggerDown)
			{
				KIDUIToggle._canTrigger = true;
			}
		}
	}

	// Token: 0x060039C3 RID: 14787 RVA: 0x0012AF94 File Offset: 0x00129194
	private void LateUpdate()
	{
		if (KIDUIToggle._triggeredThisFrame)
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
		KIDUIToggle._triggeredThisFrame = false;
	}

	// Token: 0x060039C4 RID: 14788 RVA: 0x0012B079 File Offset: 0x00129279
	protected new void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x060039C5 RID: 14789 RVA: 0x0012B0A4 File Offset: 0x001292A4
	private void SetDisabled(bool isLockedButEnabled)
	{
		this.SetSwitchColors(this._borderColors.disabledColor, this._handleColors.disabledColor, this._fillColors.disabledColor);
		this.SetBorderSize(this._disabledBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x060039C6 RID: 14790 RVA: 0x0012B0E0 File Offset: 0x001292E0
	private void SetNormal()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.normalColor, this._handleColors.normalColor, this._fillColors.normalColor);
		this.SetBorderSize(this._normalBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x060039C7 RID: 14791 RVA: 0x0012B130 File Offset: 0x00129330
	private void SetSelected()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.selectedColor, this._handleColors.selectedColor, this._fillColors.selectedColor);
		this.SetBorderSize(this._selectedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060039C8 RID: 14792 RVA: 0x0012B180 File Offset: 0x00129380
	private void SetHighlighted()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.highlightedColor, this._handleColors.highlightedColor, this._fillColors.highlightedColor);
		this.SetBorderSize(this._highlightedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x0012B1D0 File Offset: 0x001293D0
	private void SetPressed()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.pressedColor, this._handleColors.pressedColor, this._fillColors.pressedColor);
		this.SetBorderSize(this._pressedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x0012B220 File Offset: 0x00129420
	private void SetSwitchColors(Color borderColor, Color handleColor, Color fillColor)
	{
		this._borderImg.color = borderColor;
		this._handleImg.color = handleColor;
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x0012B23A File Offset: 0x0012943A
	private void SetBorderSize(float borderScale)
	{
		this._borderImgRef.offsetMin = new Vector2(-borderScale, -borderScale * this._borderHeightRatio);
		this._borderImgRef.offsetMax = new Vector2(borderScale, borderScale * this._borderHeightRatio);
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x0012B270 File Offset: 0x00129470
	private void SetBackgroundActive(bool isActive)
	{
		this._fillImg.gameObject.SetActive(isActive);
		this._fillInactiveImg.gameObject.SetActive(!isActive);
		this.SetBackgroundLocksActive(isActive);
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x0012B2A0 File Offset: 0x001294A0
	private void SetBackgroundLocksActive(bool isActive)
	{
		Color color = isActive ? this._lockActiveColor : this._lockInactiveColor;
		this._lockIcon.color = color;
		this._unlockIcon.color = color;
	}

	// Token: 0x040046D0 RID: 18128
	[Header("Toggle Setup")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _initValue;

	// Token: 0x040046D1 RID: 18129
	[SerializeField]
	private Image _borderImg;

	// Token: 0x040046D2 RID: 18130
	[SerializeField]
	private float _borderHeightRatio = 2f;

	// Token: 0x040046D3 RID: 18131
	[SerializeField]
	private Image _fillImg;

	// Token: 0x040046D4 RID: 18132
	[SerializeField]
	private Image _fillInactiveImg;

	// Token: 0x040046D5 RID: 18133
	[SerializeField]
	private Image _handleImg;

	// Token: 0x040046D6 RID: 18134
	[SerializeField]
	private Image _lockIcon;

	// Token: 0x040046D7 RID: 18135
	[SerializeField]
	private Image _unlockIcon;

	// Token: 0x040046D8 RID: 18136
	[SerializeField]
	private Image _handleLockIcon;

	// Token: 0x040046D9 RID: 18137
	[SerializeField]
	private Image _handleUnlockIcon;

	// Token: 0x040046DA RID: 18138
	[SerializeField]
	private Color _lockActiveColor;

	// Token: 0x040046DB RID: 18139
	[SerializeField]
	private Color _lockInactiveColor;

	// Token: 0x040046DC RID: 18140
	[SerializeField]
	private RectTransform _borderImgRef;

	// Token: 0x040046DD RID: 18141
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x040046DE RID: 18142
	[Header("Animation")]
	[SerializeField]
	private float _animationDuration = 0.15f;

	// Token: 0x040046DF RID: 18143
	[SerializeField]
	private AnimationCurve _toggleEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040046E0 RID: 18144
	[Header("Fill Colors")]
	[SerializeField]
	private ColorBlock _fillColors;

	// Token: 0x040046E1 RID: 18145
	[Header("Border Colors")]
	[SerializeField]
	private ColorBlock _borderColors;

	// Token: 0x040046E2 RID: 18146
	[Header("Borders")]
	[SerializeField]
	private float _normalBorderSize = 1f;

	// Token: 0x040046E3 RID: 18147
	[SerializeField]
	private float _disabledBorderSize = 1f;

	// Token: 0x040046E4 RID: 18148
	[SerializeField]
	private float _highlightedBorderSize = 1f;

	// Token: 0x040046E5 RID: 18149
	[SerializeField]
	private float _pressedBorderSize = 1f;

	// Token: 0x040046E6 RID: 18150
	[SerializeField]
	private float _selectedBorderSize = 1f;

	// Token: 0x040046E7 RID: 18151
	[Header("Handle Colors")]
	[SerializeField]
	private ColorBlock _handleColors;

	// Token: 0x040046E8 RID: 18152
	[Header("Events")]
	[SerializeField]
	private UnityEvent _onToggleOn;

	// Token: 0x040046E9 RID: 18153
	[SerializeField]
	private UnityEvent _onToggleOff;

	// Token: 0x040046EA RID: 18154
	[SerializeField]
	private UnityEvent _onToggleChanged;

	// Token: 0x040046EB RID: 18155
	private bool _previousValue;

	// Token: 0x040046EC RID: 18156
	private bool _isDisabled;

	// Token: 0x040046ED RID: 18157
	private Coroutine _animationCoroutine;

	// Token: 0x040046EF RID: 18159
	private bool inside;

	// Token: 0x040046F0 RID: 18160
	private static bool _triggeredThisFrame = false;

	// Token: 0x040046F1 RID: 18161
	private static bool _canTrigger = true;
}
