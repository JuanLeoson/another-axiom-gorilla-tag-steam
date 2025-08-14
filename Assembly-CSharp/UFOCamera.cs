using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class UFOCamera : MonoBehaviour
{
	// Token: 0x0600004D RID: 77 RVA: 0x00002F6C File Offset: 0x0000116C
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00002FC0 File Offset: 0x000011C0
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x0400003A RID: 58
	public Transform Target;

	// Token: 0x0400003B RID: 59
	private Vector3 m_targetOffset;

	// Token: 0x0400003C RID: 60
	private Vector3Spring m_spring;
}
