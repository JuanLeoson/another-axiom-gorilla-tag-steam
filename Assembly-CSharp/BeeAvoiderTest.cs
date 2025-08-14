using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class BeeAvoiderTest : MonoBehaviour
{
	// Token: 0x06000695 RID: 1685 RVA: 0x000268F0 File Offset: 0x00024AF0
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 target = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, target, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector, position - vector).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector - position3, normalized);
				Vector3 b = (this.avoidRadius - num) * normalized;
				vector += b;
				this.velocity += b;
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(position - vector);
		if ((vector - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	// Token: 0x0400080E RID: 2062
	public GameObject[] patrolPoints;

	// Token: 0x0400080F RID: 2063
	public GameObject[] avoidancePoints;

	// Token: 0x04000810 RID: 2064
	public float speed;

	// Token: 0x04000811 RID: 2065
	public float acceleration;

	// Token: 0x04000812 RID: 2066
	public float instability;

	// Token: 0x04000813 RID: 2067
	public float instabilityOffRadius;

	// Token: 0x04000814 RID: 2068
	public float drag;

	// Token: 0x04000815 RID: 2069
	public float avoidRadius;

	// Token: 0x04000816 RID: 2070
	public float patrolArrivedRadius;

	// Token: 0x04000817 RID: 2071
	private int nextPatrolPoint;

	// Token: 0x04000818 RID: 2072
	private Vector3 velocity;
}
