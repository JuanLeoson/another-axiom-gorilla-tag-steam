using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003BC RID: 956
public class UiAxis1dInspector : MonoBehaviour
{
	// Token: 0x06001620 RID: 5664 RVA: 0x00078834 File Offset: 0x00076A34
	public void SetExtents(float minExtent, float maxExtent)
	{
		this.m_minExtent = minExtent;
		this.m_maxExtent = maxExtent;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x00078844 File Offset: 0x00076A44
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x00078854 File Offset: 0x00076A54
	public void SetValue(float value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value.ToString("f2"));
		this.m_slider.minValue = Mathf.Min(value, this.m_minExtent);
		this.m_slider.maxValue = Mathf.Max(value, this.m_maxExtent);
		this.m_slider.value = value;
	}

	// Token: 0x04001DDD RID: 7645
	[Header("Settings")]
	[SerializeField]
	private float m_minExtent;

	// Token: 0x04001DDE RID: 7646
	[SerializeField]
	private float m_maxExtent = 1f;

	// Token: 0x04001DDF RID: 7647
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04001DE0 RID: 7648
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	// Token: 0x04001DE1 RID: 7649
	[SerializeField]
	private Slider m_slider;
}
