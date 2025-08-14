using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class CurveBall : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x00003C08 File Offset: 0x00001E08
	public void Reset()
	{
		float f = Random.Range(0f, MathUtil.TwoPi);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		this.m_speedX = 40f * num;
		this.m_speedZ = 40f * num2;
		this.m_timer = 0f;
		Vector3 position = base.transform.position;
		position.x = -10f * num;
		position.z = -10f * num2;
		base.transform.position = position;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003C8A File Offset: 0x00001E8A
	public void Start()
	{
		this.Reset();
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003C94 File Offset: 0x00001E94
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_timer > this.Interval)
		{
			this.Reset();
		}
		Vector3 position = base.transform.position;
		position.x += this.m_speedX * deltaTime;
		position.z += this.m_speedZ * deltaTime;
		base.transform.position = position;
		this.m_timer += deltaTime;
	}

	// Token: 0x04000063 RID: 99
	public float Interval = 2f;

	// Token: 0x04000064 RID: 100
	private float m_speedX;

	// Token: 0x04000065 RID: 101
	private float m_speedZ;

	// Token: 0x04000066 RID: 102
	private float m_timer;
}
