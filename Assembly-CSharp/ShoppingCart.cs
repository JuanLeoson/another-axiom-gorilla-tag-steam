using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x06001C53 RID: 7251 RVA: 0x00097F59 File Offset: 0x00096159
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x040024C9 RID: 9417
	[OnEnterPlay_SetNull]
	public static volatile ShoppingCart instance;
}
