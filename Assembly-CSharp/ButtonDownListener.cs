using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200034B RID: 843
public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06001411 RID: 5137 RVA: 0x0006B3E0 File Offset: 0x000695E0
	// (remove) Token: 0x06001412 RID: 5138 RVA: 0x0006B418 File Offset: 0x00069618
	public event Action onButtonDown;

	// Token: 0x06001413 RID: 5139 RVA: 0x0006B44D File Offset: 0x0006964D
	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}
}
