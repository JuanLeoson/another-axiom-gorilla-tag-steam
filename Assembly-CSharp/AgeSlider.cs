using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000899 RID: 2201
public class AgeSlider : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000549 RID: 1353
	// (get) Token: 0x06003775 RID: 14197 RVA: 0x0011FCF5 File Offset: 0x0011DEF5
	// (set) Token: 0x06003776 RID: 14198 RVA: 0x0011FCFD File Offset: 0x0011DEFD
	public AgeSlider.SliderHeldEvent onHoldComplete
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

	// Token: 0x06003777 RID: 14199 RVA: 0x0011FD06 File Offset: 0x0011DF06
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0011FD2A File Offset: 0x0011DF2A
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x0011FD50 File Offset: 0x0011DF50
	protected void Update()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.ButtonDown && this._confirmButton.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			if (this.progress >= 1f)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				return;
			}
		}
		else
		{
			this.progress = 0f;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
		}
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0011FE5C File Offset: 0x0011E05C
	private void PostUpdate()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.IsLeftStick || ControllerBehaviour.Instance.IsUpStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
		if (ControllerBehaviour.Instance.IsRightStick || ControllerBehaviour.Instance.IsDownStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x0011FF49 File Offset: 0x0011E149
	public static void ToggleAgeGate(bool state)
	{
		AgeSlider._ageGateActive = state;
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x0011FF51 File Offset: 0x0011E151
	public bool BuildValidationCheck()
	{
		if (this._confirmButton == null)
		{
			Debug.LogError("[KID] Object [_confirmButton] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x04004409 RID: 17417
	private const int MIN_AGE = 13;

	// Token: 0x0400440A RID: 17418
	[SerializeField]
	private AgeSlider.SliderHeldEvent m_OnHoldComplete = new AgeSlider.SliderHeldEvent();

	// Token: 0x0400440B RID: 17419
	[SerializeField]
	private int _maxAge = 99;

	// Token: 0x0400440C RID: 17420
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x0400440D RID: 17421
	[SerializeField]
	private GameObject _confirmButton;

	// Token: 0x0400440E RID: 17422
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x0400440F RID: 17423
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04004410 RID: 17424
	private int _currentAge;

	// Token: 0x04004411 RID: 17425
	private static bool _ageGateActive;

	// Token: 0x04004412 RID: 17426
	private float progress;

	// Token: 0x0200089A RID: 2202
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
