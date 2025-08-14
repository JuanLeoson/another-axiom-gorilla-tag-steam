using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class PropSelector : MonoBehaviour
{
	// Token: 0x0600098E RID: 2446 RVA: 0x00034980 File Offset: 0x00032B80
	private void Start()
	{
		foreach (GameObject gameObject in new List<GameObject>((from x in this._props
		orderby PropSelector._gRandom.Next()
		select x).Take(this._desiredActivePropsNum)))
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x04000B51 RID: 2897
	[SerializeField]
	private List<GameObject> _props = new List<GameObject>();

	// Token: 0x04000B52 RID: 2898
	[SerializeField]
	private int _desiredActivePropsNum = 1;

	// Token: 0x04000B53 RID: 2899
	private static readonly Random _gRandom = new Random();
}
