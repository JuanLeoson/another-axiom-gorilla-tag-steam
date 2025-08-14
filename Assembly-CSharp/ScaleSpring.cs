using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class ScaleSpring : MonoBehaviour
{
	// Token: 0x0600005F RID: 95 RVA: 0x00003620 File Offset: 0x00001820
	public void Tick()
	{
		this.m_targetScale = ((this.m_targetScale == ScaleSpring.kSmallScale) ? ScaleSpring.kLargeScale : ScaleSpring.kSmallScale);
		this.m_lastTickTime = Time.time;
		base.GetComponent<BoingEffector>().MoveDistance = ScaleSpring.kMoveDistance * ((this.m_targetScale == ScaleSpring.kSmallScale) ? -1f : 1f);
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003681 File Offset: 0x00001881
	public void Start()
	{
		this.Tick();
		this.m_spring.Reset(this.m_targetScale * Vector3.one);
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000036A4 File Offset: 0x000018A4
	public void FixedUpdate()
	{
		if (Time.time - this.m_lastTickTime > ScaleSpring.kInterval)
		{
			this.Tick();
		}
		this.m_spring.TrackHalfLife(this.m_targetScale * Vector3.one, 6f, 0.05f, Time.fixedDeltaTime);
		base.transform.localScale = this.m_spring.Value;
		base.GetComponent<BoingEffector>().MoveDistance *= Mathf.Min(0.99f, 35f * Time.fixedDeltaTime);
	}

	// Token: 0x0400004F RID: 79
	private static readonly float kInterval = 2f;

	// Token: 0x04000050 RID: 80
	private static readonly float kSmallScale = 0.6f;

	// Token: 0x04000051 RID: 81
	private static readonly float kLargeScale = 2f;

	// Token: 0x04000052 RID: 82
	private static readonly float kMoveDistance = 30f;

	// Token: 0x04000053 RID: 83
	private Vector3Spring m_spring;

	// Token: 0x04000054 RID: 84
	private float m_targetScale;

	// Token: 0x04000055 RID: 85
	private float m_lastTickTime;
}
