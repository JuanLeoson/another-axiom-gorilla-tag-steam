using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class RandomizeWavePhaseOffset : MonoBehaviour
{
	// Token: 0x060006E5 RID: 1765 RVA: 0x000274A0 File Offset: 0x000256A0
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		UberShader.VertexWavePhaseOffset.SetValue<float>(material, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	// Token: 0x04000849 RID: 2121
	[SerializeField]
	private float minPhaseOffset;

	// Token: 0x0400084A RID: 2122
	[SerializeField]
	private float maxPhaseOffset;
}
