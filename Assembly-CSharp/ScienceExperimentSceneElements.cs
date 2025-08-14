using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020007AB RID: 1963
public class ScienceExperimentSceneElements : MonoBehaviour
{
	// Token: 0x06003150 RID: 12624 RVA: 0x00100D3F File Offset: 0x000FEF3F
	private void Awake()
	{
		ScienceExperimentManager.instance.InitElements(this);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x00100D4E File Offset: 0x000FEF4E
	private void OnDestroy()
	{
		ScienceExperimentManager.instance.DeInitElements();
	}

	// Token: 0x04003CF4 RID: 15604
	public List<ScienceExperimentSceneElements.DisableByLiquidData> disableByLiquidList = new List<ScienceExperimentSceneElements.DisableByLiquidData>();

	// Token: 0x04003CF5 RID: 15605
	public ParticleSystem sodaFizzParticles;

	// Token: 0x04003CF6 RID: 15606
	public ParticleSystem sodaEruptionParticles;

	// Token: 0x020007AC RID: 1964
	[Serializable]
	public struct DisableByLiquidData
	{
		// Token: 0x04003CF7 RID: 15607
		public Transform target;

		// Token: 0x04003CF8 RID: 15608
		public float heightOffset;
	}
}
