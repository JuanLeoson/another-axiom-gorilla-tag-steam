using System;
using UnityEngine;

// Token: 0x020006B6 RID: 1718
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x06002A92 RID: 10898 RVA: 0x000E2E33 File Offset: 0x000E1033
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x04003602 RID: 13826
	[OnEnterPlay_SetNull]
	public static volatile GorillaBallWall instance;
}
