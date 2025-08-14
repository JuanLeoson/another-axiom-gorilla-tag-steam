using System;
using UnityEngine;

// Token: 0x02000882 RID: 2178
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x06003691 RID: 13969 RVA: 0x0011D83D File Offset: 0x0011BA3D
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x04004396 RID: 17302
	public Camera camera;
}
