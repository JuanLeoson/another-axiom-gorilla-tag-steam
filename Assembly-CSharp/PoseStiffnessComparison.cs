using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class PoseStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000044 RID: 68 RVA: 0x00002AAC File Offset: 0x00000CAC
	private void Start()
	{
		this.m_timer = 0f;
		this.m_yA = this.BonesA.position.y;
		this.m_yB = this.BonesB.position.y;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00002AE8 File Offset: 0x00000CE8
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] source = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		float[] source2 = new float[]
		{
			this.m_yA,
			this.m_yB
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			for (int i = 0; i < 2; i++)
			{
				Transform transform = source.ElementAt(i);
				float y = source2.ElementAt(i);
				Vector3 position = transform.position;
				position.y = y;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 1.5f);
		for (int j = 0; j < 2; j++)
		{
			Transform transform2 = source.ElementAt(j);
			float num4 = source2.ElementAt(j);
			Vector3 position2 = transform2.position;
			position2.y = num4 + 2f * Mathf.Sin(12.566371f * num3);
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
		}
	}

	// Token: 0x04000028 RID: 40
	public float Run = 11f;

	// Token: 0x04000029 RID: 41
	public float Tilt = 15f;

	// Token: 0x0400002A RID: 42
	public float Period = 3f;

	// Token: 0x0400002B RID: 43
	public float Rest = 3f;

	// Token: 0x0400002C RID: 44
	public Transform BonesA;

	// Token: 0x0400002D RID: 45
	public Transform BonesB;

	// Token: 0x0400002E RID: 46
	private float m_yA;

	// Token: 0x0400002F RID: 47
	private float m_yB;

	// Token: 0x04000030 RID: 48
	private float m_timer;
}
