using System;
using System.Collections.Generic;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x02000AC6 RID: 2758
[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour, IBuildValidation
{
	// Token: 0x06004286 RID: 17030 RVA: 0x0014E3A8 File Offset: 0x0014C5A8
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06004287 RID: 17031 RVA: 0x0014E3D4 File Offset: 0x0014C5D4
	public void SetUVOffset(int listIndex)
	{
		if (listIndex >= this.uvOffsetList.Count || listIndex < 0)
		{
			Debug.LogError("Invalid uv offset list index provided.");
			return;
		}
		if (this.matPropertyBlock == null || this.meshRenderer == null)
		{
			Debug.LogError("MaterialUVOffsetListSetter settings are incorrect somehow, please fix", base.gameObject);
			this.Awake();
			return;
		}
		Vector2 vector = this.uvOffsetList[listIndex];
		this.matPropertyBlock.SetVector(ShaderProps._BaseMap_ST, new Vector4(1f, 1f, vector.x, vector.y));
		this.meshRenderer.SetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x0014E474 File Offset: 0x0014C674
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() == null)
		{
			Debug.LogError("missing a mesh renderer for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		if (base.GetComponentInParent<EdMeshCombinerMono>() != null && base.GetComponentInParent<EdDoNotMeshCombine>() == null)
		{
			Debug.LogError("the meshrenderer is going to getcombined, that will likely cause issues for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x04004DB1 RID: 19889
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	// Token: 0x04004DB2 RID: 19890
	private MeshRenderer meshRenderer;

	// Token: 0x04004DB3 RID: 19891
	private MaterialPropertyBlock matPropertyBlock;
}
