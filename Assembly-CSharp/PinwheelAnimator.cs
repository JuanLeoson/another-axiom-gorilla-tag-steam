using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x06000621 RID: 1569 RVA: 0x000239FE File Offset: 0x00021BFE
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x00023A1C File Offset: 0x00021C1C
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float b = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, b, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x04000745 RID: 1861
	public Transform spinnerTransform;

	// Token: 0x04000746 RID: 1862
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x04000747 RID: 1863
	public float spinSpeedMultiplier = 5f;

	// Token: 0x04000748 RID: 1864
	public float damping = 0.5f;

	// Token: 0x04000749 RID: 1865
	private Vector3 oldPos;

	// Token: 0x0400074A RID: 1866
	private float spinSpeed;
}
