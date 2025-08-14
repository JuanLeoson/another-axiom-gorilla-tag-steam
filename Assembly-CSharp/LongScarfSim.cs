using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class LongScarfSim : MonoBehaviour
{
	// Token: 0x06000AB7 RID: 2743 RVA: 0x00039A7C File Offset: 0x00037C7C
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00039AE4 File Offset: 0x00037CE4
	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 a = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector = position + (a - position).normalized * this.centerOfMassLength;
		Vector3 vector2 = base.transform.InverseTransformPoint(vector);
		float num = Vector3.Dot(vector2, this.clampToPlane);
		if (num < 0f)
		{
			vector2 -= this.clampToPlane * num;
			vector = base.transform.TransformPoint(vector2);
		}
		Vector3 a2 = vector;
		this.velocity = (a2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = a2;
		float target = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, target, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion b = Quaternion.LookRotation(a2 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion a3 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(a3, b, this.currentBlend);
		}
	}

	// Token: 0x04000D1D RID: 3357
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x04000D1E RID: 3358
	[SerializeField]
	private float speedThreshold = 1f;

	// Token: 0x04000D1F RID: 3359
	[SerializeField]
	private float blendAmountPerSecond = 1f;

	// Token: 0x04000D20 RID: 3360
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000D21 RID: 3361
	private Quaternion[] baseLocalRotations;

	// Token: 0x04000D22 RID: 3362
	private float currentBlend;

	// Token: 0x04000D23 RID: 3363
	[SerializeField]
	private float centerOfMassLength;

	// Token: 0x04000D24 RID: 3364
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04000D25 RID: 3365
	[SerializeField]
	private float drag;

	// Token: 0x04000D26 RID: 3366
	[SerializeField]
	private Vector3 clampToPlane;

	// Token: 0x04000D27 RID: 3367
	private Vector3 lastCenterPos;

	// Token: 0x04000D28 RID: 3368
	private Vector3 velocity;
}
