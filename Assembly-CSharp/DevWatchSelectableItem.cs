using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200009A RID: 154
public class DevWatchSelectableItem : MonoBehaviour
{
	// Token: 0x060003C6 RID: 966 RVA: 0x00017094 File Offset: 0x00015294
	public void Init(NetworkObject obj)
	{
		this.SelectedObject = obj;
		this.ItemName.text = obj.name;
		this.Button.onClick.AddListener(delegate()
		{
			this.OnSelected(this.ItemName.text, this.SelectedObject);
		});
	}

	// Token: 0x0400044D RID: 1101
	public Button Button;

	// Token: 0x0400044E RID: 1102
	public TextMeshProUGUI ItemName;

	// Token: 0x0400044F RID: 1103
	public NetworkObject SelectedObject;

	// Token: 0x04000450 RID: 1104
	public Action<string, NetworkObject> OnSelected;
}
