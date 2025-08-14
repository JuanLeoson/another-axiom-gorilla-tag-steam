using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class GragerHoldable : MonoBehaviour
{
	// Token: 0x06000A8D RID: 2701 RVA: 0x00038E24 File Offset: 0x00037024
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00038E98 File Offset: 0x00037098
	private void Update()
	{
		Vector3 target = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector2 = base.transform.position + (a - base.transform.position).ProjectOntoPlane(vector).normalized * this.centerOfMassRadius;
		vector2 = Vector3.MoveTowards(vector2, target, this.localFriction * Time.deltaTime);
		this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector2;
		base.transform.rotation = Quaternion.LookRotation(vector2 - base.transform.position, vector) * this.RotationCorrection;
		Vector3 a2 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((a2 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.GTPlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)], 1f);
			this.lastClackParentLocalPosition = a2;
		}
	}

	// Token: 0x04000CD8 RID: 3288
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04000CD9 RID: 3289
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04000CDA RID: 3290
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04000CDB RID: 3291
	[SerializeField]
	private float drag;

	// Token: 0x04000CDC RID: 3292
	[SerializeField]
	private float gravity;

	// Token: 0x04000CDD RID: 3293
	[SerializeField]
	private float localFriction;

	// Token: 0x04000CDE RID: 3294
	[SerializeField]
	private float distancePerClack;

	// Token: 0x04000CDF RID: 3295
	[SerializeField]
	private AudioSource clackAudio;

	// Token: 0x04000CE0 RID: 3296
	[SerializeField]
	private AudioClip[] allClacks;

	// Token: 0x04000CE1 RID: 3297
	private float centerOfMassRadius;

	// Token: 0x04000CE2 RID: 3298
	private Vector3 velocity;

	// Token: 0x04000CE3 RID: 3299
	private Vector3 lastWorldPosition;

	// Token: 0x04000CE4 RID: 3300
	private Vector3 lastClackParentLocalPosition;

	// Token: 0x04000CE5 RID: 3301
	private Quaternion RotationCorrection;
}
