using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200091C RID: 2332
public class KIDUIHoldableButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x06003996 RID: 14742 RVA: 0x0012A56B File Offset: 0x0012876B
	// (set) Token: 0x06003997 RID: 14743 RVA: 0x0012A573 File Offset: 0x00128773
	public KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x06003998 RID: 14744 RVA: 0x0012A57C File Offset: 0x0012877C
	public float HoldPercentage
	{
		get
		{
			return this._elapsedTime / this._holdDuration;
		}
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x0012A58C File Offset: 0x0012878C
	private void OnEnable()
	{
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x0012A5DF File Offset: 0x001287DF
	private void Update()
	{
		this.ManageButtonInteraction(false);
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x0012A5E8 File Offset: 0x001287E8
	public void OnPointerDown(PointerEventData eventData)
	{
		this._isHoldingMouse = true;
		this.ToggleHoldingButton(true);
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x0012A5F8 File Offset: 0x001287F8
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isHoldingMouse = false;
		this.ManageButtonInteraction(true);
		this.ToggleHoldingButton(false);
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x0012A610 File Offset: 0x00128810
	private void ToggleHoldingButton(bool isPointerDown)
	{
		this._isHoldingButton = (isPointerDown && this._button.interactable);
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (isPointerDown)
		{
			this._elapsedTime = 0f;
			KIDUIHoldableButton.ButtonHoldStartEvent onHoldStart = this.m_OnHoldStart;
			if (onHoldStart == null)
			{
				return;
			}
			onHoldStart.Invoke();
			return;
		}
		else
		{
			KIDUIHoldableButton.ButtonHoldReleaseEvent onHoldRelease = this.m_OnHoldRelease;
			if (onHoldRelease == null)
			{
				return;
			}
			onHoldRelease.Invoke();
			return;
		}
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x0012A688 File Offset: 0x00128888
	private void ManageButtonInteraction(bool isPointerUp = false)
	{
		if (!this._isHoldingButton)
		{
			return;
		}
		if (isPointerUp)
		{
			return;
		}
		if (this._holdDuration <= 0f)
		{
			this.HoldComplete();
			return;
		}
		this._elapsedTime += Time.deltaTime;
		bool flag = this._elapsedTime > this._holdDuration;
		float num = this._elapsedTime / this._holdDuration;
		this._holdProgressFill.rectTransform.localScale = new Vector3(num, 1f, 1f);
		HandRayController.Instance.PulseActiveHandray(num, 0.1f);
		if (flag)
		{
			this.HoldComplete();
		}
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x0012A71C File Offset: 0x0012891C
	private void HoldComplete()
	{
		this.ToggleHoldingButton(false);
		KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete = this.m_OnHoldComplete;
		if (onHoldComplete != null)
		{
			onHoldComplete.Invoke();
		}
		Debug.Log("[HOLD_BUTTON " + base.name + " ]: Hold Complete");
		this.ResetButton();
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x0012A756 File Offset: 0x00128956
	private void ResetButton()
	{
		this._elapsedTime = 0f;
		this.inside = false;
		KIDUIHoldableButton._triggeredThisFrame = false;
		this._button.ResetButton();
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x0012A77B File Offset: 0x0012897B
	protected void Awake()
	{
		if (this._button != null)
		{
			return;
		}
		this._button = base.GetComponentInChildren<KIDUIButton>();
		if (this._button == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [KIDUIButton] in children, trying to create a new one.");
			return;
		}
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x0012A7B4 File Offset: 0x001289B4
	private void PostUpdate()
	{
		if (!KIDUIHoldableButton._canTrigger)
		{
			KIDUIHoldableButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!this._button.interactable || !KIDUIHoldableButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && this.inside)
			{
				if (!this._isHoldingButton)
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
					this.ToggleHoldingButton(true);
					KIDUIHoldableButton._triggeredThisFrame = true;
					KIDUIHoldableButton._canTrigger = false;
					return;
				}
			}
			else if (this._isHoldingButton && !this._isHoldingMouse)
			{
				this.ToggleHoldingButton(false);
			}
		}
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x0012A918 File Offset: 0x00128B18
	private void LateUpdate()
	{
		if (KIDUIHoldableButton._triggeredThisFrame)
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
		KIDUIHoldableButton._triggeredThisFrame = false;
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x0012A9FD File Offset: 0x00128BFD
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
	}

	// Token: 0x060039A5 RID: 14757 RVA: 0x0012AA06 File Offset: 0x00128C06
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x0012AA0F File Offset: 0x00128C0F
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x040046C3 RID: 18115
	public KIDUIButton _button;

	// Token: 0x040046C4 RID: 18116
	[SerializeField]
	private float _holdDuration;

	// Token: 0x040046C5 RID: 18117
	[SerializeField]
	private Image _holdProgressFill;

	// Token: 0x040046C6 RID: 18118
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x040046C7 RID: 18119
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldCompleteEvent m_OnHoldComplete = new KIDUIHoldableButton.ButtonHoldCompleteEvent();

	// Token: 0x040046C8 RID: 18120
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldStartEvent m_OnHoldStart = new KIDUIHoldableButton.ButtonHoldStartEvent();

	// Token: 0x040046C9 RID: 18121
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldReleaseEvent m_OnHoldRelease = new KIDUIHoldableButton.ButtonHoldReleaseEvent();

	// Token: 0x040046CA RID: 18122
	private bool _isHoldingButton;

	// Token: 0x040046CB RID: 18123
	private float _elapsedTime;

	// Token: 0x040046CC RID: 18124
	private bool inside;

	// Token: 0x040046CD RID: 18125
	private bool _isHoldingMouse;

	// Token: 0x040046CE RID: 18126
	private static bool _triggeredThisFrame = false;

	// Token: 0x040046CF RID: 18127
	private static bool _canTrigger = true;

	// Token: 0x0200091D RID: 2333
	[Serializable]
	public class ButtonHoldCompleteEvent : UnityEvent
	{
	}

	// Token: 0x0200091E RID: 2334
	[Serializable]
	public class ButtonHoldStartEvent : UnityEvent
	{
	}

	// Token: 0x0200091F RID: 2335
	[Serializable]
	public class ButtonHoldReleaseEvent : UnityEvent
	{
	}
}
