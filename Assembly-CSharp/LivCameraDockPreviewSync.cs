using System;
using Docking;
using UnityEngine;

// Token: 0x0200029E RID: 670
[ExecuteAlways]
public class LivCameraDockPreviewSync : MonoBehaviour
{
	// Token: 0x04001826 RID: 6182
	private LivCameraDock dock;

	// Token: 0x04001827 RID: 6183
	private Camera parentCamera;

	// Token: 0x04001828 RID: 6184
	private float _lastCameraFOV = -1f;

	// Token: 0x04001829 RID: 6185
	private float _lastDockFOV = -1f;
}
