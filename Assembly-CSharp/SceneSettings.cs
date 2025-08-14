using System;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class SceneSettings : MonoBehaviour
{
	// Token: 0x06001618 RID: 5656 RVA: 0x000784C7 File Offset: 0x000766C7
	private void Awake()
	{
		Time.fixedDeltaTime = this.m_fixedTimeStep;
		Physics.gravity = Vector3.down * 9.81f * this.m_gravityScalar;
		Physics.defaultContactOffset = this.m_defaultContactOffset;
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000784FE File Offset: 0x000766FE
	private void Start()
	{
		SceneSettings.CollidersSetContactOffset(this.m_defaultContactOffset);
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x0007850C File Offset: 0x0007670C
	private static void CollidersSetContactOffset(float contactOffset)
	{
		Collider[] array = Object.FindObjectsOfType<Collider>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].contactOffset = contactOffset;
		}
	}

	// Token: 0x04001DCF RID: 7631
	[Header("Time")]
	[SerializeField]
	private float m_fixedTimeStep = 0.01f;

	// Token: 0x04001DD0 RID: 7632
	[Header("Physics")]
	[SerializeField]
	private float m_gravityScalar = 0.75f;

	// Token: 0x04001DD1 RID: 7633
	[SerializeField]
	private float m_defaultContactOffset = 0.001f;
}
