using System;
using GorillaTagScripts.UI;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200084A RID: 2122
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x06003543 RID: 13635
	public abstract void Initialize();

	// Token: 0x06003544 RID: 13636 RVA: 0x001173E4 File Offset: 0x001155E4
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard == null)
			{
				return;
			}
			customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
		}
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x00117421 File Offset: 0x00115621
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard == null)
			{
				return;
			}
			customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
		}
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void PressButton(CustomMapKeyboardBinding pressedButton)
	{
	}

	// Token: 0x04004240 RID: 16960
	public CustomMapsKeyboard terminalKeyboard;
}
