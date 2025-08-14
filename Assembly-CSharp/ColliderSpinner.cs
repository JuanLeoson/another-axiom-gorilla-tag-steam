using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class ColliderSpinner : MonoBehaviour
{
	// Token: 0x0600004A RID: 74 RVA: 0x00002EC8 File Offset: 0x000010C8
	private void Start()
	{
		this.m_targetOffset = ((this.Target != null) ? (base.transform.position - this.Target.position) : Vector3.zero);
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002F24 File Offset: 0x00001124
	private void FixedUpdate()
	{
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000037 RID: 55
	public Transform Target;

	// Token: 0x04000038 RID: 56
	private Vector3 m_targetOffset;

	// Token: 0x04000039 RID: 57
	private Vector3Spring m_spring;
}
