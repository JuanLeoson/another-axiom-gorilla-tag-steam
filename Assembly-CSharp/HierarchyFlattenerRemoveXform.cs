using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerRemoveXform : MonoBehaviour
{
	// Token: 0x06000C51 RID: 3153 RVA: 0x00042976 File Offset: 0x00040B76
	protected void Awake()
	{
		this._DoIt();
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x00042980 File Offset: 0x00040B80
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (base.GetComponentInChildren<HierarchyFlattenerRemoveXform>(true) != null)
		{
			return;
		}
		HierarchyFlattenerRemoveXform componentInParent = base.GetComponentInParent<HierarchyFlattenerRemoveXform>(true);
		this._didIt = true;
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SetParent(transform.parent, true);
		}
		Object.Destroy(base.gameObject);
		if (componentInParent != null)
		{
			componentInParent._DoIt();
		}
	}

	// Token: 0x04000F2A RID: 3882
	private bool _didIt;
}
