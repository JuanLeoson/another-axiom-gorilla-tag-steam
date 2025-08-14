using System;
using TMPro;
using UnityEngine;

// Token: 0x020007C6 RID: 1990
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x060031DF RID: 12767 RVA: 0x0010385A File Offset: 0x00101A5A
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x00103880 File Offset: 0x00101A80
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x00103899 File Offset: 0x00101A99
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003DAB RID: 15787
	public WatchableStringSO textToCopy;

	// Token: 0x04003DAC RID: 15788
	private TextMeshPro myText;
}
