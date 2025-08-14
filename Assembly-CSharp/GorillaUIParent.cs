using System;
using UnityEngine;

// Token: 0x02000736 RID: 1846
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06002E30 RID: 11824 RVA: 0x000F4EBD File Offset: 0x000F30BD
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04003A0E RID: 14862
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
