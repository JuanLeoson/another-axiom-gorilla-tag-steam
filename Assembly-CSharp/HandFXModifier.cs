using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class HandFXModifier : FXModifier
{
	// Token: 0x06000C4D RID: 3149 RVA: 0x0004291B File Offset: 0x00040B1B
	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0004292E File Offset: 0x00040B2E
	private void OnDisable()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00042941 File Offset: 0x00040B41
	public override void UpdateScale(float scale)
	{
		scale = Mathf.Clamp(scale, this.minScale, this.maxScale);
		base.transform.localScale = this.originalScale * scale;
	}

	// Token: 0x04000F25 RID: 3877
	private Vector3 originalScale;

	// Token: 0x04000F26 RID: 3878
	[SerializeField]
	private float minScale;

	// Token: 0x04000F27 RID: 3879
	[SerializeField]
	private float maxScale;

	// Token: 0x04000F28 RID: 3880
	[SerializeField]
	private ParticleSystem dustBurst;

	// Token: 0x04000F29 RID: 3881
	[SerializeField]
	private ParticleSystem dustLinger;
}
