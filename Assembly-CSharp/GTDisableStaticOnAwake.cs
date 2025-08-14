using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x06000C13 RID: 3091 RVA: 0x00041264 File Offset: 0x0003F464
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
