using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E8 RID: 488
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x04000E9D RID: 3741
	public Text hintTextOutput;

	// Token: 0x04000E9E RID: 3742
	public float scanDistance = 10f;

	// Token: 0x04000E9F RID: 3743
	public float scanAngle = 30f;

	// Token: 0x04000EA0 RID: 3744
	public LayerMask scanLayerMask;

	// Token: 0x04000EA1 RID: 3745
	public string targetComponentName;

	// Token: 0x04000EA2 RID: 3746
	public float rayPerDegree = 10f;
}
