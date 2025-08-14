using System;
using UnityEngine;

// Token: 0x02000209 RID: 521
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	// Token: 0x06000C54 RID: 3156 RVA: 0x000429FA File Offset: 0x00040BFA
	protected void Awake()
	{
		if (base.enabled)
		{
			this._DoIt();
		}
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x00042A0A File Offset: 0x00040C0A
	protected void OnEnable()
	{
		this._DoIt();
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00042A12 File Offset: 0x00040C12
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (this.newParent != null)
		{
			base.transform.SetParent(this.newParent, true);
		}
		Object.Destroy(this);
	}

	// Token: 0x04000F2B RID: 3883
	public Transform newParent;

	// Token: 0x04000F2C RID: 3884
	private bool _didIt;
}
