using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x06000DAF RID: 3503 RVA: 0x00053E14 File Offset: 0x00052014
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x04001580 RID: 5504
	public OVRManager ovrManager;

	// Token: 0x04001581 RID: 5505
	public GameObject[] betaDisableObjects;

	// Token: 0x04001582 RID: 5506
	public GameObject[] betaEnableObjects;
}
