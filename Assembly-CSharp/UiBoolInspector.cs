using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003BE RID: 958
public class UiBoolInspector : MonoBehaviour
{
	// Token: 0x06001628 RID: 5672 RVA: 0x00078ABB File Offset: 0x00076CBB
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x00078AC9 File Offset: 0x00076CC9
	public void SetValue(bool value)
	{
		this.m_toggle.isOn = value;
	}

	// Token: 0x04001DE7 RID: 7655
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04001DE8 RID: 7656
	[SerializeField]
	private Toggle m_toggle;
}
