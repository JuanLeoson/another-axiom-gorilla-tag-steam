using System;
using UnityEngine;

// Token: 0x02000A46 RID: 2630
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x0600405C RID: 16476 RVA: 0x001465C6 File Offset: 0x001447C6
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x001465DD File Offset: 0x001447DD
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04004C0C RID: 19468
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x04004C0D RID: 19469
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
