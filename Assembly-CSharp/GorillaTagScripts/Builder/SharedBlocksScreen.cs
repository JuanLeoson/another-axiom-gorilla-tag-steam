using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CC5 RID: 3269
	public class SharedBlocksScreen : MonoBehaviour
	{
		// Token: 0x06005113 RID: 20755 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnUpPressed()
		{
		}

		// Token: 0x06005114 RID: 20756 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnDownPressed()
		{
		}

		// Token: 0x06005115 RID: 20757 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnSelectPressed()
		{
		}

		// Token: 0x06005116 RID: 20758 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnDeletePressed()
		{
		}

		// Token: 0x06005117 RID: 20759 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnNumberPressed(int number)
		{
		}

		// Token: 0x06005118 RID: 20760 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnLetterPressed(string letter)
		{
		}

		// Token: 0x06005119 RID: 20761 RVA: 0x0019471E File Offset: 0x0019291E
		public virtual void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600511A RID: 20762 RVA: 0x00194739 File Offset: 0x00192939
		public virtual void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04005AA5 RID: 23205
		public SharedBlocksTerminal.ScreenType screenType;

		// Token: 0x04005AA6 RID: 23206
		public SharedBlocksTerminal terminal;
	}
}
