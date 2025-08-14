using System;
using UnityEngine;

// Token: 0x02000493 RID: 1171
public class FirstPersonXRaySpecs : MonoBehaviour
{
	// Token: 0x06001CFD RID: 7421 RVA: 0x0009BF55 File Offset: 0x0009A155
	private void OnEnable()
	{
		GorillaBodyRenderer.SetAllSkeletons(true);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x0009BF5D File Offset: 0x0009A15D
	private void OnDisable()
	{
		GorillaBodyRenderer.SetAllSkeletons(false);
	}
}
