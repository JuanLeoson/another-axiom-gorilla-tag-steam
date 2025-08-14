using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C5 RID: 1989
public class TextWatcher : MonoBehaviour
{
	// Token: 0x060031DB RID: 12763 RVA: 0x0010380D File Offset: 0x00101A0D
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x00103833 File Offset: 0x00101A33
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x0010384C File Offset: 0x00101A4C
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003DA9 RID: 15785
	public WatchableStringSO textToCopy;

	// Token: 0x04003DAA RID: 15786
	private Text myText;
}
