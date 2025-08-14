using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000358 RID: 856
public class DebugUISample : MonoBehaviour
{
	// Token: 0x06001448 RID: 5192 RVA: 0x0006D1E4 File Offset: 0x0006B3E4
	private void Start()
	{
		DebugUIBuilder.instance.AddButton("Button Pressed", new DebugUIBuilder.OnClick(this.LogButtonPressed), -1, 0, false);
		DebugUIBuilder.instance.AddLabel("Label", 0);
		RectTransform rectTransform = DebugUIBuilder.instance.AddSlider("Slider", 1f, 10f, new DebugUIBuilder.OnSlider(this.SliderPressed), true, 0);
		Text[] componentsInChildren = rectTransform.GetComponentsInChildren<Text>();
		this.sliderText = componentsInChildren[1];
		this.sliderText.text = rectTransform.GetComponentInChildren<Slider>().value.ToString();
		DebugUIBuilder.instance.AddDivider(0);
		DebugUIBuilder.instance.AddToggle("Toggle", new DebugUIBuilder.OnToggleValueChange(this.TogglePressed), 0);
		DebugUIBuilder.instance.AddRadio("Radio1", "group", delegate(Toggle t)
		{
			this.RadioPressed("Radio1", "group", t);
		}, 0);
		DebugUIBuilder.instance.AddRadio("Radio2", "group", delegate(Toggle t)
		{
			this.RadioPressed("Radio2", "group", t);
		}, 0);
		DebugUIBuilder.instance.AddLabel("Secondary Tab", 1);
		DebugUIBuilder.instance.AddDivider(1);
		DebugUIBuilder.instance.AddRadio("Side Radio 1", "group2", delegate(Toggle t)
		{
			this.RadioPressed("Side Radio 1", "group2", t);
		}, 1);
		DebugUIBuilder.instance.AddRadio("Side Radio 2", "group2", delegate(Toggle t)
		{
			this.RadioPressed("Side Radio 2", "group2", t);
		}, 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x0006D358 File Offset: 0x0006B558
	public void TogglePressed(Toggle t)
	{
		Debug.Log("Toggle pressed. Is on? " + t.isOn.ToString());
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0006D384 File Offset: 0x0006B584
	public void RadioPressed(string radioLabel, string group, Toggle t)
	{
		Debug.Log(string.Concat(new string[]
		{
			"Radio value changed: ",
			radioLabel,
			", from group ",
			group,
			". New value: ",
			t.isOn.ToString()
		}));
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0006D3D2 File Offset: 0x0006B5D2
	public void SliderPressed(float f)
	{
		Debug.Log("Slider: " + f.ToString());
		this.sliderText.text = f.ToString();
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0006D3FC File Offset: 0x0006B5FC
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0006D454 File Offset: 0x0006B654
	private void LogButtonPressed()
	{
		Debug.Log("Button pressed");
	}

	// Token: 0x04001BDC RID: 7132
	private bool inMenu;

	// Token: 0x04001BDD RID: 7133
	private Text sliderText;
}
