using System;
using TMPro;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class UiVectorInspector : MonoBehaviour
{
	// Token: 0x0600163C RID: 5692 RVA: 0x00078F01 File Offset: 0x00077101
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x00078F0F File Offset: 0x0007710F
	public void SetValue(bool value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value);
	}

	// Token: 0x04001DFF RID: 7679
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04001E00 RID: 7680
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;
}
