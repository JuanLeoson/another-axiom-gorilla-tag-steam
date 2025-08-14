using System;
using UnityEngine;

// Token: 0x0200087C RID: 2172
public class LerpScale : LerpComponent
{
	// Token: 0x0600367A RID: 13946 RVA: 0x0011D018 File Offset: 0x0011B218
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04004342 RID: 17218
	[Space]
	public Transform target;

	// Token: 0x04004343 RID: 17219
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x04004344 RID: 17220
	public Vector3 end = Vector3.one;

	// Token: 0x04004345 RID: 17221
	public Vector3 current;

	// Token: 0x04004346 RID: 17222
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
