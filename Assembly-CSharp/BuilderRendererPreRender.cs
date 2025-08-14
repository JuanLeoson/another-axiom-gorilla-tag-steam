using System;
using UnityEngine;

// Token: 0x02000516 RID: 1302
public class BuilderRendererPreRender : MonoBehaviour
{
	// Token: 0x06001FB7 RID: 8119 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000A78C1 File Offset: 0x000A5AC1
	private void LateUpdate()
	{
		if (this.builderRenderer != null)
		{
			this.builderRenderer.PreRenderIndirect();
		}
	}

	// Token: 0x0400285C RID: 10332
	public BuilderRenderer builderRenderer;
}
