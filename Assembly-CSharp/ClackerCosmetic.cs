using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class ClackerCosmetic : MonoBehaviour
{
	// Token: 0x060009CC RID: 2508 RVA: 0x00035898 File Offset: 0x00033A98
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00035954 File Offset: 0x00033B54
	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		ref Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 a = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 b = a + vector;
			Vector3 b2 = a - vector;
			if ((lastWorldPosition - b).IsLongerThan(lastWorldPosition - b2))
			{
				vector = -vector;
			}
			this.arm1.SetPosition(a + vector);
			this.arm2.SetPosition(a - vector);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 b3 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + b3;
			this.arm2.velocity = this.arm2.velocity - b3;
		}
	}

	// Token: 0x04000BA3 RID: 2979
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04000BA4 RID: 2980
	[SerializeField]
	private Transform clackerArm1;

	// Token: 0x04000BA5 RID: 2981
	[SerializeField]
	private Transform clackerArm2;

	// Token: 0x04000BA6 RID: 2982
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04000BA7 RID: 2983
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04000BA8 RID: 2984
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04000BA9 RID: 2985
	[SerializeField]
	private float drag;

	// Token: 0x04000BAA RID: 2986
	[SerializeField]
	private float gravity;

	// Token: 0x04000BAB RID: 2987
	[SerializeField]
	private float localFriction;

	// Token: 0x04000BAC RID: 2988
	[SerializeField]
	private float minimumClackSpeed;

	// Token: 0x04000BAD RID: 2989
	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	// Token: 0x04000BAE RID: 2990
	[SerializeField]
	private float mediumClackSpeed;

	// Token: 0x04000BAF RID: 2991
	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	// Token: 0x04000BB0 RID: 2992
	[SerializeField]
	private float heavyClackSpeed;

	// Token: 0x04000BB1 RID: 2993
	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	// Token: 0x04000BB2 RID: 2994
	[SerializeField]
	private float collisionDistance;

	// Token: 0x04000BB3 RID: 2995
	private float centerOfMassRadius;

	// Token: 0x04000BB4 RID: 2996
	[SerializeField]
	private float pushApartStrength;

	// Token: 0x04000BB5 RID: 2997
	private ClackerCosmetic.PerArmData arm1;

	// Token: 0x04000BB6 RID: 2998
	private ClackerCosmetic.PerArmData arm2;

	// Token: 0x04000BB7 RID: 2999
	private Quaternion RotationCorrection;

	// Token: 0x02000180 RID: 384
	private struct PerArmData
	{
		// Token: 0x060009CF RID: 2511 RVA: 0x00035BE0 File Offset: 0x00033DE0
		public void UpdateArm()
		{
			Vector3 target = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector2 = this.transform.position + (a - this.transform.position).ProjectOntoPlane(vector).normalized * this.parent.centerOfMassRadius;
			vector2 = Vector3.MoveTowards(vector2, target, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector2;
			this.transform.rotation = Quaternion.LookRotation(vector, vector2 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00035D48 File Offset: 0x00033F48
		public void SetPosition(Vector3 newPosition)
		{
			Vector3 forward = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(forward, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x04000BB8 RID: 3000
		public ClackerCosmetic parent;

		// Token: 0x04000BB9 RID: 3001
		public Transform transform;

		// Token: 0x04000BBA RID: 3002
		public Vector3 velocity;

		// Token: 0x04000BBB RID: 3003
		public Vector3 lastWorldPosition;
	}
}
