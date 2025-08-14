using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class UFOEffector : MonoBehaviour
{
	// Token: 0x06000050 RID: 80 RVA: 0x00003014 File Offset: 0x00001214
	public void Start()
	{
		BoingEffector component = base.GetComponent<BoingEffector>();
		this.m_radius = component.Radius;
		this.m_moveDistance = component.MoveDistance;
		this.m_rotateAngle = component.RotationAngle;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x0000304C File Offset: 0x0000124C
	public void FixedUpdate()
	{
		BoingEffector component = base.GetComponent<BoingEffector>();
		component.Radius = this.m_radius * (1f + 0.2f * Mathf.Sin(11f * Time.time) * Mathf.Sin(7f * Time.time + 1.54f));
		component.MoveDistance = this.m_moveDistance * (1f + 0.2f * Mathf.Sin(9.3f * Time.time + 5.19f) * Mathf.Sin(7.3f * Time.time + 4.73f));
		component.RotationAngle = this.m_rotateAngle * (1f + 0.2f * Mathf.Sin(7.9f * Time.time + 2.97f) * Mathf.Sin(8.3f * Time.time + 0.93f));
		base.transform.localPosition = Vector3.right * 0.25f * Mathf.Sin(5.23f * Time.time + 9.87f) + Vector3.forward * 0.25f * Mathf.Sin(4.93f * Time.time + 7.39f);
	}

	// Token: 0x0400003D RID: 61
	private float m_radius;

	// Token: 0x0400003E RID: 62
	private float m_moveDistance;

	// Token: 0x0400003F RID: 63
	private float m_rotateAngle;
}
