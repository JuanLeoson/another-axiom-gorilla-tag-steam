using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200089B RID: 2203
public class AgeSliderWithProgressBar : MonoBehaviour
{
	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x0600377F RID: 14207 RVA: 0x0011FF9C File Offset: 0x0011E19C
	// (set) Token: 0x06003780 RID: 14208 RVA: 0x0011FFA4 File Offset: 0x0011E1A4
	public AgeSliderWithProgressBar.SliderHeldEvent onHoldComplete
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

	// Token: 0x1700054B RID: 1355
	// (get) Token: 0x06003781 RID: 14209 RVA: 0x0011FFAD File Offset: 0x0011E1AD
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x1700054C RID: 1356
	// (get) Token: 0x06003782 RID: 14210 RVA: 0x0011FFB5 File Offset: 0x0011E1B5
	// (set) Token: 0x06003783 RID: 14211 RVA: 0x0011FFBD File Offset: 0x0011E1BD
	public bool ControllerActive
	{
		get
		{
			return this.controllerActive;
		}
		set
		{
			if (value)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			else
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.controllerActive = value;
		}
	}

	// Token: 0x1700054D RID: 1357
	// (get) Token: 0x06003784 RID: 14212 RVA: 0x0011FFF7 File Offset: 0x0011E1F7
	// (set) Token: 0x06003785 RID: 14213 RVA: 0x0011FFFF File Offset: 0x0011E1FF
	public string LockMessage
	{
		get
		{
			return this._lockMessage;
		}
		set
		{
			this._lockMessage = value;
		}
	}

	// Token: 0x1700054E RID: 1358
	// (get) Token: 0x06003786 RID: 14214 RVA: 0x00120008 File Offset: 0x0011E208
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x06003787 RID: 14215 RVA: 0x00120010 File Offset: 0x0011E210
	private void Awake()
	{
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x00120030 File Offset: 0x0011E230
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x0012003C File Offset: 0x0011E23C
	private void OnEnable()
	{
		if (this._progressBarContainer != null && this.progressBarFill != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
		}
	}

	// Token: 0x0600378A RID: 14218 RVA: 0x001200BC File Offset: 0x0011E2BC
	protected void Update()
	{
		if (!this._progressBarContainer)
		{
			return;
		}
		if (!this.ControllerActive)
		{
			return;
		}
		if (!this._lockMessage.IsNullOrEmpty())
		{
			this.progress = 0f;
			if (this._messageText)
			{
				this._messageText.text = this.LockMessage;
			}
		}
		else
		{
			if (this._messageText)
			{
				this._messageText.text = this._originalText;
			}
			if ((double)this.progress == 1.0)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				this.progress = 0f;
			}
			if (ControllerBehaviour.Instance.ButtonDown && this._progressBarContainer != null && (this._currentAge > 0 || !this.AdjustAge))
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progress = Mathf.Clamp01(this.progress);
			}
			else
			{
				this.progress = 0f;
			}
		}
		if (this._progressBarContainer != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
		}
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x00120200 File Offset: 0x0011E400
	private void PostUpdate()
	{
		if (this.ControllerActive && this._ageValueTxt && this._ageSlidable && !this._incrementButtonsLockingSlider)
		{
			if (ControllerBehaviour.Instance.IsLeftStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
			if (ControllerBehaviour.Instance.IsRightStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = this.GetAgeString();
			if (this._progressBarContainer != null)
			{
				this._progressBarContainer.SetActive(this._currentAge > 0);
			}
		}
	}

	// Token: 0x0600378C RID: 14220 RVA: 0x00120324 File Offset: 0x0011E524
	public void EnableEditing()
	{
		this._ageSlidable = true;
	}

	// Token: 0x0600378D RID: 14221 RVA: 0x0012032D File Offset: 0x0011E52D
	public void DisableEditing()
	{
		this._ageSlidable = false;
	}

	// Token: 0x0600378E RID: 14222 RVA: 0x00120338 File Offset: 0x0011E538
	public string GetAgeString()
	{
		if (this._confirmButton)
		{
			this._confirmButton.interactable = true;
		}
		if (this._currentAge == 0)
		{
			if (this._confirmButton)
			{
				this._confirmButton.interactable = false;
			}
			return "?";
		}
		if (this._currentAge == this._maxAge)
		{
			return this._maxAge.ToString() + "+";
		}
		return this._currentAge.ToString();
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x001203B4 File Offset: 0x0011E5B4
	public void ForceAddAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Min(this._currentAge + number, this._maxAge);
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x001203D6 File Offset: 0x0011E5D6
	public void ForceSubtractAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Max(this._currentAge - number, 1);
	}

	// Token: 0x04004413 RID: 17427
	private const int MIN_AGE = 13;

	// Token: 0x04004414 RID: 17428
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x04004415 RID: 17429
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x04004416 RID: 17430
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x04004417 RID: 17431
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04004418 RID: 17432
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04004419 RID: 17433
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x0400441A RID: 17434
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x0400441B RID: 17435
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x0400441C RID: 17436
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x0400441D RID: 17437
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x0400441E RID: 17438
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x0400441F RID: 17439
	private bool _ageSlidable = true;

	// Token: 0x04004420 RID: 17440
	private bool _incrementButtonsLockingSlider;

	// Token: 0x04004421 RID: 17441
	private bool controllerActive;

	// Token: 0x04004422 RID: 17442
	[SerializeField]
	private string _lockMessage;

	// Token: 0x04004423 RID: 17443
	private string _originalText;

	// Token: 0x04004424 RID: 17444
	private int _currentAge;

	// Token: 0x04004425 RID: 17445
	private float progress;

	// Token: 0x0200089C RID: 2204
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
