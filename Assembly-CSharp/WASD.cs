using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class WASD : MonoBehaviour
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000085 RID: 133 RVA: 0x00004A49 File Offset: 0x00002C49
	public Vector3 Velocity
	{
		get
		{
			return this.m_velocity;
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00004A54 File Offset: 0x00002C54
	public void Update()
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		if (Input.GetKey(KeyCode.W))
		{
			zero.z += 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zero.z -= 1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			zero.x += 1f;
		}
		Vector3 vector = (zero.sqrMagnitude > 0f) ? (zero.normalized * this.Speed * Time.deltaTime) : Vector3.zero;
		Quaternion lhs = Quaternion.AngleAxis(num * this.Omega * 57.29578f * Time.deltaTime, Vector3.up);
		this.m_velocity = vector / Time.deltaTime;
		base.transform.position += vector;
		base.transform.rotation = lhs * base.transform.rotation;
	}

	// Token: 0x040000A2 RID: 162
	public float Speed = 1f;

	// Token: 0x040000A3 RID: 163
	public float Omega = 1f;

	// Token: 0x040000A4 RID: 164
	public Vector3 m_velocity;
}
