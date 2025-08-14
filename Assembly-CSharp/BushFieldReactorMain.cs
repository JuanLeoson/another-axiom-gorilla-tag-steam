using System;
using System.Collections.Generic;
using BoingKit;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class BushFieldReactorMain : MonoBehaviour
{
	// Token: 0x06000071 RID: 113 RVA: 0x000042B0 File Offset: 0x000024B0
	public void Start()
	{
		Random.InitState(0);
		for (int i = 0; i < this.NumBushes; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
			float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
			gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject.transform.localScale = num * Vector3.one;
			BoingBehavior component = gameObject.GetComponent<BoingBehavior>();
			if (component != null)
			{
				component.Reboot();
			}
		}
		for (int j = 0; j < this.NumBlossoms; j++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num2 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num2 * Vector3.one;
			BoingBehavior component2 = gameObject2.GetComponent<BoingBehavior>();
			if (component2 != null)
			{
				component2.Reboot();
			}
		}
		this.m_aSphere = new List<GameObject>(this.NumSpheresPerCircle * this.NumCircles);
		for (int k = 0; k < this.NumCircles; k++)
		{
			for (int l = 0; l < this.NumSpheresPerCircle; l++)
			{
				this.m_aSphere.Add(Object.Instantiate<GameObject>(this.Sphere));
			}
		}
		this.m_basePhase = 0f;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x0000451C File Offset: 0x0000271C
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : -1f);
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
	}

	// Token: 0x04000079 RID: 121
	public GameObject Bush;

	// Token: 0x0400007A RID: 122
	public GameObject Blossom;

	// Token: 0x0400007B RID: 123
	public GameObject Sphere;

	// Token: 0x0400007C RID: 124
	public int NumBushes;

	// Token: 0x0400007D RID: 125
	public Vector2 BushScaleRange;

	// Token: 0x0400007E RID: 126
	public int NumBlossoms;

	// Token: 0x0400007F RID: 127
	public Vector2 BlossomScaleRange;

	// Token: 0x04000080 RID: 128
	public Vector2 FieldBounds;

	// Token: 0x04000081 RID: 129
	public int NumSpheresPerCircle;

	// Token: 0x04000082 RID: 130
	public int NumCircles;

	// Token: 0x04000083 RID: 131
	public float MaxCircleRadius;

	// Token: 0x04000084 RID: 132
	public float CircleSpeed;

	// Token: 0x04000085 RID: 133
	private List<GameObject> m_aSphere;

	// Token: 0x04000086 RID: 134
	private float m_basePhase;
}
