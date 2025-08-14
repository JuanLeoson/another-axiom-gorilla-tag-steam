using System;
using UnityEngine;

// Token: 0x0200044D RID: 1101
public class OwlLook : MonoBehaviour
{
	// Token: 0x06001B15 RID: 6933 RVA: 0x00090863 File Offset: 0x0008EA63
	private void Awake()
	{
		this.overlapRigs = new VRRig[10];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x0009088C File Offset: 0x0008EA8C
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 rhs = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (rhs.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, rhs.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 rhs = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, rhs);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector2 = this.neck.InverseTransformDirection(vector);
		vector2.y = Mathf.Clamp(vector2.y, this.minNeckY, this.maxNeckY);
		vector = this.neck.TransformDirection(vector2.normalized);
		Vector3 forward = Vector3.RotateTowards(this.head.forward, vector, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(forward, this.neck.up);
	}

	// Token: 0x04002371 RID: 9073
	public Transform head;

	// Token: 0x04002372 RID: 9074
	public Transform lookTarget;

	// Token: 0x04002373 RID: 9075
	public Transform neck;

	// Token: 0x04002374 RID: 9076
	public float lookRadius = 0.5f;

	// Token: 0x04002375 RID: 9077
	public Collider[] overlapColliders;

	// Token: 0x04002376 RID: 9078
	public VRRig[] rigs = new VRRig[10];

	// Token: 0x04002377 RID: 9079
	public VRRig[] overlapRigs;

	// Token: 0x04002378 RID: 9080
	public float rotSpeed = 1f;

	// Token: 0x04002379 RID: 9081
	public float lookAtAngleDegrees = 60f;

	// Token: 0x0400237A RID: 9082
	public float maxNeckY;

	// Token: 0x0400237B RID: 9083
	public float minNeckY;

	// Token: 0x0400237C RID: 9084
	public VRRig myRig;
}
