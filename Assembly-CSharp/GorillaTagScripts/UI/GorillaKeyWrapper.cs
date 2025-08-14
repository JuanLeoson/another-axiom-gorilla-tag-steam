using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000C6F RID: 3183
	public class GorillaKeyWrapper<TBinding> : MonoBehaviour where TBinding : Enum
	{
		// Token: 0x06004ECB RID: 20171 RVA: 0x0018841C File Offset: 0x0018661C
		public void Start()
		{
			if (!this.defineButtonsManually)
			{
				this.FindMatchingButtons(base.gameObject);
				return;
			}
			if (this.buttons.Count > 0)
			{
				for (int i = this.buttons.Count - 1; i >= 0; i--)
				{
					if (this.buttons[i].IsNull())
					{
						this.buttons.RemoveAt(i);
					}
					else
					{
						this.buttons[i].OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
					}
				}
			}
		}

		// Token: 0x06004ECC RID: 20172 RVA: 0x001884A8 File Offset: 0x001866A8
		public void OnDestroy()
		{
			for (int i = 0; i < this.buttons.Count; i++)
			{
				if (this.buttons[i].IsNotNull())
				{
					this.buttons[i].OnKeyButtonPressed.RemoveListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
				}
			}
		}

		// Token: 0x06004ECD RID: 20173 RVA: 0x00188500 File Offset: 0x00186700
		public void FindMatchingButtons(GameObject obj)
		{
			if (obj.IsNull())
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				Transform child = obj.transform.GetChild(i);
				if (child.IsNotNull())
				{
					this.FindMatchingButtons(child.gameObject);
				}
			}
			GorillaKeyButton<TBinding> component = obj.GetComponent<GorillaKeyButton<TBinding>>();
			if (component.IsNotNull() && !this.buttons.Contains(component))
			{
				this.buttons.Add(component);
				component.OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
			}
		}

		// Token: 0x06004ECE RID: 20174 RVA: 0x0018858D File Offset: 0x0018678D
		private void OnKeyButtonPressed(TBinding binding)
		{
			UnityEvent<TBinding> onKeyPressed = this.OnKeyPressed;
			if (onKeyPressed == null)
			{
				return;
			}
			onKeyPressed.Invoke(binding);
		}

		// Token: 0x0400579C RID: 22428
		public UnityEvent<TBinding> OnKeyPressed = new UnityEvent<TBinding>();

		// Token: 0x0400579D RID: 22429
		public bool defineButtonsManually;

		// Token: 0x0400579E RID: 22430
		public List<GorillaKeyButton<TBinding>> buttons = new List<GorillaKeyButton<TBinding>>();
	}
}
