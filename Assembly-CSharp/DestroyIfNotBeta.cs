using System;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class DestroyIfNotBeta : MonoBehaviour
{
	// Token: 0x06001DE1 RID: 7649 RVA: 0x0004061F File Offset: 0x0003E81F
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
