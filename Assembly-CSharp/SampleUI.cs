using System;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class SampleUI : MonoBehaviour
{
	// Token: 0x06001454 RID: 5204 RVA: 0x0006D4AC File Offset: 0x0006B6AC
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Enable Firebase in your project before running this sample", 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x0006D4D0 File Offset: 0x0006B6D0
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

	// Token: 0x04001BDE RID: 7134
	private RectTransform collectionButton;

	// Token: 0x04001BDF RID: 7135
	private RectTransform inputText;

	// Token: 0x04001BE0 RID: 7136
	private RectTransform valueText;

	// Token: 0x04001BE1 RID: 7137
	private bool inMenu;
}
