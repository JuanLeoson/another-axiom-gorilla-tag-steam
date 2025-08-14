using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000886 RID: 2182
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x060036B2 RID: 14002 RVA: 0x0011DE7A File Offset: 0x0011C07A
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x0011DEB4 File Offset: 0x0011C0B4
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x0011DF24 File Offset: 0x0011C124
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x040043A6 RID: 17318
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x040043A7 RID: 17319
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x040043A8 RID: 17320
	[NonSerialized]
	private bool? _toggled;
}
