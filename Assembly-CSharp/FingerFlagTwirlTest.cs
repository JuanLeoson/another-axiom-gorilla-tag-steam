using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x0600059C RID: 1436 RVA: 0x000209CC File Offset: 0x0001EBCC
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x040006AC RID: 1708
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x040006AD RID: 1709
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x040006AE RID: 1710
	public AnimationCurve rotXAnimCurve;

	// Token: 0x040006AF RID: 1711
	public AnimationCurve rotYAnimCurve;

	// Token: 0x040006B0 RID: 1712
	public AnimationCurve rotZAnimCurve;

	// Token: 0x040006B1 RID: 1713
	private Vector3 animTimes = Vector3.zero;
}
