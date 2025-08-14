using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class LengthStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000041 RID: 65 RVA: 0x000028BF File Offset: 0x00000ABF
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x06000042 RID: 66 RVA: 0x000028CC File Offset: 0x00000ACC
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			foreach (Transform transform in array)
			{
				Vector3 position = transform.position;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
			transform2.rotation = Quaternion.AngleAxis(this.Tilt * (1f - num3), Vector3.right);
		}
	}

	// Token: 0x04000021 RID: 33
	public float Run = 11f;

	// Token: 0x04000022 RID: 34
	public float Tilt = 15f;

	// Token: 0x04000023 RID: 35
	public float Period = 3f;

	// Token: 0x04000024 RID: 36
	public float Rest = 3f;

	// Token: 0x04000025 RID: 37
	public Transform BonesA;

	// Token: 0x04000026 RID: 38
	public Transform BonesB;

	// Token: 0x04000027 RID: 39
	private float m_timer;
}
