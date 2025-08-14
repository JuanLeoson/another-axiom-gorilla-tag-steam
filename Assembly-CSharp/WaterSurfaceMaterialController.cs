using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x06000495 RID: 1173 RVA: 0x0001A87D File Offset: 0x00018A7D
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001A89C File Offset: 0x00018A9C
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(ShaderProps._ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		if (this.renderer)
		{
			this.renderer.SetPropertyBlock(this.matPropBlock);
		}
	}

	// Token: 0x04000562 RID: 1378
	public float ScrollX = 0.6f;

	// Token: 0x04000563 RID: 1379
	public float ScrollY = 0.6f;

	// Token: 0x04000564 RID: 1380
	public float Scale = 1f;

	// Token: 0x04000565 RID: 1381
	private Renderer renderer;

	// Token: 0x04000566 RID: 1382
	private MaterialPropertyBlock matPropBlock;
}
