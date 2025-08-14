using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E6 RID: 486
public class DevInspector : MonoBehaviour
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x0004061F File Offset: 0x0003E81F
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000E95 RID: 3733
	public GameObject pivot;

	// Token: 0x04000E96 RID: 3734
	public Text outputInfo;

	// Token: 0x04000E97 RID: 3735
	public Component[] componentToInspect;

	// Token: 0x04000E98 RID: 3736
	public bool isEnabled;

	// Token: 0x04000E99 RID: 3737
	public bool autoFind = true;

	// Token: 0x04000E9A RID: 3738
	public GameObject canvas;

	// Token: 0x04000E9B RID: 3739
	public int sidewaysOffset;
}
