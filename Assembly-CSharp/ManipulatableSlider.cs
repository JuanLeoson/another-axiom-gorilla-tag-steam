using System;
using UnityEngine;

// Token: 0x02000447 RID: 1095
public class ManipulatableSlider : ManipulatableObject
{
	// Token: 0x06001AD6 RID: 6870 RVA: 0x0008F352 File Offset: 0x0008D552
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x0008F376 File Offset: 0x0008D576
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x0008F394 File Offset: 0x0008D594
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x0008F3D4 File Offset: 0x0008D5D4
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x0008F46C File Offset: 0x0008D66C
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x0008F620 File Offset: 0x0008D820
	public void SetProgress(float x, float y, float z)
	{
		x = Mathf.Clamp(x, 0f, 1f);
		y = Mathf.Clamp(y, 0f, 1f);
		z = Mathf.Clamp(z, 0f, 1f);
		Vector3 localPosition = this.startingPos;
		localPosition.x += Mathf.Lerp(this.minXOffset, this.maxXOffset, x);
		localPosition.y += Mathf.Lerp(this.minYOffset, this.maxYOffset, y);
		localPosition.z += Mathf.Lerp(this.minZOffset, this.maxZOffset, z);
		base.transform.localPosition = localPosition;
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x0008F6CD File Offset: 0x0008D8CD
	public float GetProgressX()
	{
		return ((base.transform.localPosition - this.startingPos).x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x0008F6FF File Offset: 0x0008D8FF
	public float GetProgressY()
	{
		return ((base.transform.localPosition - this.startingPos).y - this.minYOffset) / (this.maxYOffset - this.minYOffset);
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x0008F731 File Offset: 0x0008D931
	public float GetProgressZ()
	{
		return ((base.transform.localPosition - this.startingPos).z - this.minZOffset) / (this.maxZOffset - this.minZOffset);
	}

	// Token: 0x0400231B RID: 8987
	public float breakDistance = 0.2f;

	// Token: 0x0400231C RID: 8988
	public float maxXOffset;

	// Token: 0x0400231D RID: 8989
	public float minXOffset;

	// Token: 0x0400231E RID: 8990
	public float maxYOffset;

	// Token: 0x0400231F RID: 8991
	public float minYOffset;

	// Token: 0x04002320 RID: 8992
	public float maxZOffset;

	// Token: 0x04002321 RID: 8993
	public float minZOffset;

	// Token: 0x04002322 RID: 8994
	public bool applyReleaseVelocity;

	// Token: 0x04002323 RID: 8995
	public float releaseDrag = 1f;

	// Token: 0x04002324 RID: 8996
	private Matrix4x4 localSpace;

	// Token: 0x04002325 RID: 8997
	private Vector3 startingPos;

	// Token: 0x04002326 RID: 8998
	private Vector3 velocity;
}
