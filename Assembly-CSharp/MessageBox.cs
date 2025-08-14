using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000903 RID: 2307
public class MessageBox : MonoBehaviour
{
	// Token: 0x17000589 RID: 1417
	// (get) Token: 0x06003900 RID: 14592 RVA: 0x00127913 File Offset: 0x00125B13
	// (set) Token: 0x06003901 RID: 14593 RVA: 0x0012791B File Offset: 0x00125B1B
	public MessageBoxResult Result { get; private set; }

	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x06003902 RID: 14594 RVA: 0x00127924 File Offset: 0x00125B24
	// (set) Token: 0x06003903 RID: 14595 RVA: 0x00127931 File Offset: 0x00125B31
	public string Header
	{
		get
		{
			return this._headerText.text;
		}
		set
		{
			this._headerText.text = value;
			this._headerText.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x06003904 RID: 14596 RVA: 0x00127958 File Offset: 0x00125B58
	// (set) Token: 0x06003905 RID: 14597 RVA: 0x00127965 File Offset: 0x00125B65
	public string Body
	{
		get
		{
			return this._bodyText.text;
		}
		set
		{
			this._bodyText.text = value;
		}
	}

	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x06003906 RID: 14598 RVA: 0x00127973 File Offset: 0x00125B73
	// (set) Token: 0x06003907 RID: 14599 RVA: 0x00127980 File Offset: 0x00125B80
	public string LeftButton
	{
		get
		{
			return this._leftButtonText.text;
		}
		set
		{
			this._leftButtonText.text = value;
			this._leftButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._rightButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition = Vector3.zero;
				return;
			}
			RectTransform component2 = this._rightButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0.5f);
			component2.anchorMax = new Vector2(1f, 0.5f);
			component2.pivot = new Vector2(1f, 0.5f);
			component2.anchoredPosition = Vector3.zero;
		}
	}

	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x06003908 RID: 14600 RVA: 0x00127A68 File Offset: 0x00125C68
	// (set) Token: 0x06003909 RID: 14601 RVA: 0x00127A78 File Offset: 0x00125C78
	public string RightButton
	{
		get
		{
			return this._rightButtonText.text;
		}
		set
		{
			this._rightButtonText.text = value;
			this._rightButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._leftButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition3D = Vector3.zero;
				return;
			}
			RectTransform component2 = this._leftButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.5f);
			component2.anchorMax = new Vector2(0f, 0.5f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition3D = Vector3.zero;
		}
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x00127B56 File Offset: 0x00125D56
	private void Start()
	{
		this.Result = MessageBoxResult.None;
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x00127B5F File Offset: 0x00125D5F
	public void OnClickLeftButton()
	{
		this.Result = MessageBoxResult.Left;
		this._leftButtonCallback.Invoke();
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x00127B73 File Offset: 0x00125D73
	public void OnClickRightButton()
	{
		this.Result = MessageBoxResult.Right;
		this._rightButtonCallback.Invoke();
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x00127B87 File Offset: 0x00125D87
	public GameObject GetCanvas()
	{
		return base.GetComponentInChildren<Canvas>(true).gameObject;
	}

	// Token: 0x04004628 RID: 17960
	[SerializeField]
	private TMP_Text _headerText;

	// Token: 0x04004629 RID: 17961
	[SerializeField]
	private TMP_Text _bodyText;

	// Token: 0x0400462A RID: 17962
	[SerializeField]
	private TMP_Text _leftButtonText;

	// Token: 0x0400462B RID: 17963
	[SerializeField]
	private TMP_Text _rightButtonText;

	// Token: 0x0400462C RID: 17964
	[SerializeField]
	private GameObject _leftButton;

	// Token: 0x0400462D RID: 17965
	[SerializeField]
	private GameObject _rightButton;

	// Token: 0x0400462F RID: 17967
	[SerializeField]
	private UnityEvent _leftButtonCallback = new UnityEvent();

	// Token: 0x04004630 RID: 17968
	[SerializeField]
	private UnityEvent _rightButtonCallback = new UnityEvent();
}
