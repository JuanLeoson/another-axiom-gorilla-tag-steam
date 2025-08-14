using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class TasselPhysics : MonoBehaviour
{
	// Token: 0x06000B46 RID: 2886 RVA: 0x0003BD48 File Offset: 0x00039F48
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
		if (this.LockXAxis)
		{
			this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, this.localCenterOfMass));
			return;
		}
		this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(this.localCenterOfMass));
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003BDA0 File Offset: 0x00039FA0
	private void Update()
	{
		float y = base.transform.lossyScale.y;
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * y * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 a = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 a2 = position + (a - position).normalized * this.centerOfMassLength * y;
		this.velocity = (a2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = a2;
		if (this.LockXAxis)
		{
			foreach (GameObject gameObject in this.tasselInstances)
			{
				gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.right, a2 - position) * this.rotCorrection;
			}
			return;
		}
		foreach (GameObject gameObject2 in this.tasselInstances)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(a2 - position, gameObject2.transform.position - position) * this.rotCorrection;
		}
	}

	// Token: 0x04000DD5 RID: 3541
	[SerializeField]
	private GameObject[] tasselInstances;

	// Token: 0x04000DD6 RID: 3542
	[SerializeField]
	private Vector3 localCenterOfMass;

	// Token: 0x04000DD7 RID: 3543
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04000DD8 RID: 3544
	[SerializeField]
	private float drag;

	// Token: 0x04000DD9 RID: 3545
	[SerializeField]
	private bool LockXAxis;

	// Token: 0x04000DDA RID: 3546
	private Vector3 lastCenterPos;

	// Token: 0x04000DDB RID: 3547
	private Vector3 velocity;

	// Token: 0x04000DDC RID: 3548
	private float centerOfMassLength;

	// Token: 0x04000DDD RID: 3549
	private Quaternion rotCorrection;
}
