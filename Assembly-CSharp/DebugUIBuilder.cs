using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200030D RID: 781
public class DebugUIBuilder : MonoBehaviour
{
	// Token: 0x060012C0 RID: 4800 RVA: 0x00066F2C File Offset: 0x0006512C
	public void Awake()
	{
		DebugUIBuilder.instance = this;
		this.menuOffset = base.transform.position;
		base.gameObject.SetActive(false);
		this.rig = Object.FindObjectOfType<OVRCameraRig>();
		for (int i = 0; i < this.toEnable.Count; i++)
		{
			this.toEnable[i].SetActive(false);
		}
		this.insertPositions = new Vector2[this.targetContentPanels.Length];
		for (int j = 0; j < this.insertPositions.Length; j++)
		{
			this.insertPositions[j].x = this.marginH;
			this.insertPositions[j].y = -this.marginV;
		}
		this.insertedElements = new List<RectTransform>[this.targetContentPanels.Length];
		for (int k = 0; k < this.insertedElements.Length; k++)
		{
			this.insertedElements[k] = new List<RectTransform>();
		}
		if (this.uiHelpersToInstantiate)
		{
			Object.Instantiate<GameObject>(this.uiHelpersToInstantiate);
		}
		this.lp = Object.FindObjectOfType<LaserPointer>();
		if (!this.lp)
		{
			Debug.LogError("Debug UI requires use of a LaserPointer and will not function without it. Add one to your scene, or assign the UIHelpers prefab to the DebugUIBuilder in the inspector.");
			return;
		}
		this.lp.laserBeamBehavior = this.laserBeamBehavior;
		if (!this.toEnable.Contains(this.lp.gameObject))
		{
			this.toEnable.Add(this.lp.gameObject);
		}
		base.GetComponent<OVRRaycaster>().pointer = this.lp.gameObject;
		this.lp.gameObject.SetActive(false);
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000670BC File Offset: 0x000652BC
	public void Show()
	{
		this.Relayout();
		base.gameObject.SetActive(true);
		base.transform.position = this.rig.transform.TransformPoint(this.menuOffset);
		Vector3 eulerAngles = this.rig.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (this.reEnable == null || this.reEnable.Length < this.toDisable.Count)
		{
			this.reEnable = new bool[this.toDisable.Count];
		}
		this.reEnable.Initialize();
		int count = this.toDisable.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.toDisable[i])
			{
				this.reEnable[i] = this.toDisable[i].activeSelf;
				this.toDisable[i].SetActive(false);
			}
		}
		count = this.toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			this.toEnable[j].SetActive(true);
		}
		int num = this.targetContentPanels.Length;
		for (int k = 0; k < num; k++)
		{
			this.targetContentPanels[k].gameObject.SetActive(this.insertedElements[k].Count > 0);
		}
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00067244 File Offset: 0x00065444
	public void Hide()
	{
		base.gameObject.SetActive(false);
		for (int i = 0; i < this.reEnable.Length; i++)
		{
			if (this.toDisable[i] && this.reEnable[i])
			{
				this.toDisable[i].SetActive(true);
			}
		}
		int count = this.toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			this.toEnable[j].SetActive(false);
		}
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x000672CC File Offset: 0x000654CC
	private void StackedRelayout()
	{
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = 0f;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				rectTransform.anchoredPosition = new Vector2(num, num2);
				if (this.isHorizontal)
				{
					num += rectTransform.rect.width + this.elementSpacing;
				}
				else
				{
					num2 -= rectTransform.rect.height + this.elementSpacing;
				}
				num3 = Mathf.Max(rectTransform.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x000673DC File Offset: 0x000655DC
	private void PanelCentricRelayout()
	{
		if (!this.isHorizontal)
		{
			Debug.Log("Error:Panel Centeric relayout is implemented only for horizontal panels");
			return;
		}
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = num;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				num3 += rectTransform.rect.width + this.elementSpacing;
			}
			num3 -= this.elementSpacing;
			num3 += this.marginH;
			float num4 = num3;
			num = -0.5f * num4;
			num2 = -this.marginV;
			for (int k = 0; k < count; k++)
			{
				RectTransform rectTransform2 = list[k];
				if (k == 0)
				{
					num += this.marginH;
				}
				num += 0.5f * rectTransform2.rect.width;
				rectTransform2.anchoredPosition = new Vector2(num, num2);
				num += rectTransform2.rect.width * 0.5f + this.elementSpacing;
				num3 = Mathf.Max(rectTransform2.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00067567 File Offset: 0x00065767
	private void Relayout()
	{
		if (this.usePanelCentricRelayout)
		{
			this.PanelCentricRelayout();
			return;
		}
		this.StackedRelayout();
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00067580 File Offset: 0x00065780
	private void AddRect(RectTransform r, int targetCanvas)
	{
		if (targetCanvas > this.targetContentPanels.Length)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Attempted to add debug panel to canvas ",
				targetCanvas.ToString(),
				", but only ",
				this.targetContentPanels.Length.ToString(),
				" panels were provided. Fix in the inspector or pass a lower value for target canvas."
			}));
			return;
		}
		r.transform.SetParent(this.targetContentPanels[targetCanvas], false);
		this.insertedElements[targetCanvas].Add(r);
		if (base.gameObject.activeInHierarchy)
		{
			this.Relayout();
		}
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00067614 File Offset: 0x00065814
	public RectTransform AddButton(string label, DebugUIBuilder.OnClick handler = null, int buttonIndex = -1, int targetCanvas = 0, bool highResolutionText = false)
	{
		RectTransform component;
		if (buttonIndex == -1)
		{
			component = Object.Instantiate<RectTransform>(this.buttonPrefab).GetComponent<RectTransform>();
		}
		else
		{
			component = Object.Instantiate<RectTransform>(this.additionalButtonPrefab[buttonIndex]).GetComponent<RectTransform>();
		}
		Button componentInChildren = component.GetComponentInChildren<Button>();
		if (handler != null)
		{
			componentInChildren.onClick.AddListener(delegate()
			{
				handler();
			});
		}
		if (highResolutionText)
		{
			((TextMeshProUGUI)component.GetComponentsInChildren(typeof(TextMeshProUGUI), true)[0]).text = label;
		}
		else
		{
			((Text)component.GetComponentsInChildren(typeof(Text), true)[0]).text = label;
		}
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x000676CC File Offset: 0x000658CC
	public RectTransform AddLabel(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.labelPrefab).GetComponent<RectTransform>();
		component.GetComponent<Text>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00067700 File Offset: 0x00065900
	public RectTransform AddSlider(string label, float min, float max, DebugUIBuilder.OnSlider onValueChanged, bool wholeNumbersOnly = false, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.sliderPrefab);
		Slider componentInChildren = rectTransform.GetComponentInChildren<Slider>();
		componentInChildren.minValue = min;
		componentInChildren.maxValue = max;
		componentInChildren.onValueChanged.AddListener(delegate(float f)
		{
			onValueChanged(f);
		});
		componentInChildren.wholeNumbers = wholeNumbersOnly;
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00067764 File Offset: 0x00065964
	public RectTransform AddDivider(int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.dividerPrefab);
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x00067788 File Offset: 0x00065988
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.onValueChanged.AddListener(delegate(bool <p0>)
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x000677EC File Offset: 0x000659EC
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, bool defaultValue, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.isOn = defaultValue;
		t.onValueChanged.AddListener(delegate(bool <p0>)
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x0006785C File Offset: 0x00065A5C
	public RectTransform AddRadio(string label, string group, DebugUIBuilder.OnToggleValueChange handler, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.radioPrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle tb = rectTransform.GetComponentInChildren<Toggle>();
		if (group == null)
		{
			group = "default";
		}
		bool isOn = false;
		ToggleGroup toggleGroup;
		if (!this.radioGroups.ContainsKey(group))
		{
			toggleGroup = tb.gameObject.AddComponent<ToggleGroup>();
			this.radioGroups[group] = toggleGroup;
			isOn = true;
		}
		else
		{
			toggleGroup = this.radioGroups[group];
		}
		tb.group = toggleGroup;
		tb.isOn = isOn;
		tb.onValueChanged.AddListener(delegate(bool <p0>)
		{
			handler(tb);
		});
		return rectTransform;
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00067924 File Offset: 0x00065B24
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00067957 File Offset: 0x00065B57
	public void ToggleLaserPointer(bool isOn)
	{
		if (this.lp)
		{
			if (isOn)
			{
				this.lp.enabled = true;
				return;
			}
			this.lp.enabled = false;
		}
	}

	// Token: 0x04001A5F RID: 6751
	public const int DEBUG_PANE_CENTER = 0;

	// Token: 0x04001A60 RID: 6752
	public const int DEBUG_PANE_RIGHT = 1;

	// Token: 0x04001A61 RID: 6753
	public const int DEBUG_PANE_LEFT = 2;

	// Token: 0x04001A62 RID: 6754
	[SerializeField]
	private RectTransform buttonPrefab;

	// Token: 0x04001A63 RID: 6755
	[SerializeField]
	private RectTransform[] additionalButtonPrefab;

	// Token: 0x04001A64 RID: 6756
	[SerializeField]
	private RectTransform labelPrefab;

	// Token: 0x04001A65 RID: 6757
	[SerializeField]
	private RectTransform sliderPrefab;

	// Token: 0x04001A66 RID: 6758
	[SerializeField]
	private RectTransform dividerPrefab;

	// Token: 0x04001A67 RID: 6759
	[SerializeField]
	private RectTransform togglePrefab;

	// Token: 0x04001A68 RID: 6760
	[SerializeField]
	private RectTransform radioPrefab;

	// Token: 0x04001A69 RID: 6761
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x04001A6A RID: 6762
	[SerializeField]
	private GameObject uiHelpersToInstantiate;

	// Token: 0x04001A6B RID: 6763
	[SerializeField]
	private Transform[] targetContentPanels;

	// Token: 0x04001A6C RID: 6764
	private bool[] reEnable;

	// Token: 0x04001A6D RID: 6765
	[SerializeField]
	private List<GameObject> toEnable;

	// Token: 0x04001A6E RID: 6766
	[SerializeField]
	private List<GameObject> toDisable;

	// Token: 0x04001A6F RID: 6767
	public static DebugUIBuilder instance;

	// Token: 0x04001A70 RID: 6768
	public float elementSpacing = 16f;

	// Token: 0x04001A71 RID: 6769
	public float marginH = 16f;

	// Token: 0x04001A72 RID: 6770
	public float marginV = 16f;

	// Token: 0x04001A73 RID: 6771
	private Vector2[] insertPositions;

	// Token: 0x04001A74 RID: 6772
	private List<RectTransform>[] insertedElements;

	// Token: 0x04001A75 RID: 6773
	private Vector3 menuOffset;

	// Token: 0x04001A76 RID: 6774
	private OVRCameraRig rig;

	// Token: 0x04001A77 RID: 6775
	private Dictionary<string, ToggleGroup> radioGroups = new Dictionary<string, ToggleGroup>();

	// Token: 0x04001A78 RID: 6776
	private LaserPointer lp;

	// Token: 0x04001A79 RID: 6777
	private LineRenderer lr;

	// Token: 0x04001A7A RID: 6778
	public LaserPointer.LaserBeamBehavior laserBeamBehavior;

	// Token: 0x04001A7B RID: 6779
	public bool isHorizontal;

	// Token: 0x04001A7C RID: 6780
	public bool usePanelCentricRelayout;

	// Token: 0x0200030E RID: 782
	// (Invoke) Token: 0x060012D2 RID: 4818
	public delegate void OnClick();

	// Token: 0x0200030F RID: 783
	// (Invoke) Token: 0x060012D6 RID: 4822
	public delegate void OnToggleValueChange(Toggle t);

	// Token: 0x02000310 RID: 784
	// (Invoke) Token: 0x060012DA RID: 4826
	public delegate void OnSlider(float f);

	// Token: 0x02000311 RID: 785
	// (Invoke) Token: 0x060012DE RID: 4830
	public delegate bool ActiveUpdate();
}
