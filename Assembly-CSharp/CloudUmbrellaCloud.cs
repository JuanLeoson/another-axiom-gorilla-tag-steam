using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x06000616 RID: 1558 RVA: 0x000235BB File Offset: 0x000217BB
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x000235E0 File Offset: 0x000217E0
	protected void LateUpdate()
	{
		float time = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num = Mathf.Clamp01(this.scaleCurve.Evaluate(time));
		this.rendererOn = ((num > 0.09f && num < 0.1f) ? this.rendererOn : (num > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num, num, num);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x0400072C RID: 1836
	public UmbrellaItem umbrella;

	// Token: 0x0400072D RID: 1837
	public Transform cloudRotateXform;

	// Token: 0x0400072E RID: 1838
	public Renderer cloudRenderer;

	// Token: 0x0400072F RID: 1839
	public AnimationCurve scaleCurve;

	// Token: 0x04000730 RID: 1840
	private const float kHideAtScale = 0.1f;

	// Token: 0x04000731 RID: 1841
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x04000732 RID: 1842
	private bool rendererOn;

	// Token: 0x04000733 RID: 1843
	private Transform umbrellaXform;

	// Token: 0x04000734 RID: 1844
	private Transform cloudScaleXform;
}
