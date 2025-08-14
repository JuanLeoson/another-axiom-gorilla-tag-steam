using System;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x06002B85 RID: 11141 RVA: 0x000E60A4 File Offset: 0x000E42A4
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000E60E0 File Offset: 0x000E42E0
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000E6110 File Offset: 0x000E4310
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x000E6140 File Offset: 0x000E4340
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x040036CF RID: 14031
	public GorillaUITransformFollow[] transformFollowers;
}
