using System;
using UnityEngine;

// Token: 0x02000366 RID: 870
public class EnableUnpremultipliedAlpha : MonoBehaviour
{
	// Token: 0x0600149C RID: 5276 RVA: 0x0006F1A0 File Offset: 0x0006D3A0
	private void Start()
	{
		OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
	}
}
