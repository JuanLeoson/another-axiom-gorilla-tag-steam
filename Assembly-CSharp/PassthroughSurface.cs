using System;
using UnityEngine;

// Token: 0x0200037C RID: 892
public class PassthroughSurface : MonoBehaviour
{
	// Token: 0x06001516 RID: 5398 RVA: 0x00072EDB File Offset: 0x000710DB
	private void Start()
	{
		Object.Destroy(this.projectionObject.GetComponent<MeshRenderer>());
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
	}

	// Token: 0x04001CB1 RID: 7345
	public OVRPassthroughLayer passthroughLayer;

	// Token: 0x04001CB2 RID: 7346
	public MeshFilter projectionObject;
}
