using System;
using UnityEngine;

// Token: 0x02000444 RID: 1092
public class TestManipulatableCube : ManipulatableObject
{
	// Token: 0x06001AC3 RID: 6851 RVA: 0x0008EC10 File Offset: 0x0008CE10
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x0008EC34 File Offset: 0x0008CE34
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x0008EC50 File Offset: 0x0008CE50
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x0008EC90 File Offset: 0x0008CE90
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x0008ED28 File Offset: 0x0008CF28
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

	// Token: 0x06001AC9 RID: 6857 RVA: 0x0008EED9 File Offset: 0x0008D0D9
	public Matrix4x4 GetLocalSpace()
	{
		return this.localSpace;
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x0008EEE4 File Offset: 0x0008D0E4
	public void SetCubeToSpecificPosition(Vector3 pos)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(pos);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0008EF74 File Offset: 0x0008D174
	public void SetCubeToSpecificPosition(float x, float y, float z)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = Mathf.Clamp(x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x04002301 RID: 8961
	public float breakDistance = 0.2f;

	// Token: 0x04002302 RID: 8962
	public float maxXOffset;

	// Token: 0x04002303 RID: 8963
	public float minXOffset;

	// Token: 0x04002304 RID: 8964
	public float maxYOffset;

	// Token: 0x04002305 RID: 8965
	public float minYOffset;

	// Token: 0x04002306 RID: 8966
	public float maxZOffset;

	// Token: 0x04002307 RID: 8967
	public float minZOffset;

	// Token: 0x04002308 RID: 8968
	public bool applyReleaseVelocity;

	// Token: 0x04002309 RID: 8969
	public float releaseDrag = 1f;

	// Token: 0x0400230A RID: 8970
	private Matrix4x4 localSpace;

	// Token: 0x0400230B RID: 8971
	private Vector3 startingPos;

	// Token: 0x0400230C RID: 8972
	private Vector3 velocity;
}
