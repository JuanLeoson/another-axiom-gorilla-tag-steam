using System;
using Cinemachine.Utility;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class FakeWheelDriver : MonoBehaviour
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000A36 RID: 2614 RVA: 0x00037B1C File Offset: 0x00035D1C
	// (set) Token: 0x06000A37 RID: 2615 RVA: 0x00037B24 File Offset: 0x00035D24
	public bool hasCollision { get; private set; }

	// Token: 0x06000A38 RID: 2616 RVA: 0x00037B2D File Offset: 0x00035D2D
	public void SetThrust(Vector3 thrust)
	{
		this.thrust = thrust;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00037B38 File Offset: 0x00035D38
	private void OnCollisionStay(Collision collision)
	{
		int num = 0;
		Vector3 a = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.thisCollider == this.wheelCollider)
			{
				a += contactPoint.point;
				num++;
			}
		}
		if (num > 0)
		{
			this.collisionNormal = collision.contacts[0].normal;
			this.collisionPoint = a / (float)num;
			this.hasCollision = true;
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00037BC4 File Offset: 0x00035DC4
	private void FixedUpdate()
	{
		if (this.hasCollision)
		{
			Vector3 vector = base.transform.rotation * this.thrust;
			if (this.myRigidBody.velocity.IsShorterThan(this.maxSpeed))
			{
				vector = vector.ProjectOntoPlane(this.collisionNormal).normalized * this.thrust.magnitude;
				this.myRigidBody.AddForceAtPosition(vector, this.collisionPoint);
			}
			Vector3 vector2 = this.myRigidBody.velocity.ProjectOntoPlane(this.collisionNormal).ProjectOntoPlane(vector.normalized);
			if (vector2.IsLongerThan(this.lateralFrictionForce))
			{
				this.myRigidBody.AddForceAtPosition(-vector2.normalized * this.lateralFrictionForce, this.collisionPoint);
			}
			else
			{
				this.myRigidBody.AddForceAtPosition(-vector2, this.collisionPoint);
			}
		}
		this.hasCollision = false;
	}

	// Token: 0x04000C5C RID: 3164
	[SerializeField]
	private Rigidbody myRigidBody;

	// Token: 0x04000C5D RID: 3165
	[SerializeField]
	private Vector3 thrust;

	// Token: 0x04000C5E RID: 3166
	[SerializeField]
	private Collider wheelCollider;

	// Token: 0x04000C5F RID: 3167
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04000C60 RID: 3168
	[SerializeField]
	private float lateralFrictionForce;

	// Token: 0x04000C62 RID: 3170
	private Vector3 collisionPoint;

	// Token: 0x04000C63 RID: 3171
	private Vector3 collisionNormal;
}
