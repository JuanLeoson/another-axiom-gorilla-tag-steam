using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x0600005B RID: 91 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Start()
	{
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003574 File Offset: 0x00001774
	public void Update()
	{
		this.m_phase += OrbitCamera.kOrbitSpeed * MathUtil.TwoPi * Time.deltaTime;
		base.transform.position = new Vector3(-4f * Mathf.Cos(this.m_phase), 6f, 4f * Mathf.Sin(this.m_phase));
		base.transform.rotation = Quaternion.LookRotation((new Vector3(0f, 3f, 0f) - base.transform.position).normalized);
	}

	// Token: 0x0400004D RID: 77
	private static readonly float kOrbitSpeed = 0.01f;

	// Token: 0x0400004E RID: 78
	private float m_phase;
}
