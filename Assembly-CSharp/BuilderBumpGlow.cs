using System;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class BuilderBumpGlow : MonoBehaviour
{
	// Token: 0x06001FDC RID: 8156 RVA: 0x000A8308 File Offset: 0x000A6508
	public void Awake()
	{
		this.blendIn = 1f;
		this.intensity = 0f;
		this.UpdateRender();
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000A8326 File Offset: 0x000A6526
	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
		this.UpdateRender();
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x000A8335 File Offset: 0x000A6535
	public void SetBlendIn(float blendIn)
	{
		this.blendIn = blendIn;
		this.UpdateRender();
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x000023F5 File Offset: 0x000005F5
	private void UpdateRender()
	{
	}

	// Token: 0x0400289D RID: 10397
	public MeshRenderer glowRenderer;

	// Token: 0x0400289E RID: 10398
	private float blendIn;

	// Token: 0x0400289F RID: 10399
	private float intensity;
}
