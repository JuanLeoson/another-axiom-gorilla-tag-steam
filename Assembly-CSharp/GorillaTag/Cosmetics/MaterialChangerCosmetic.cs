using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F4A RID: 3914
	public class MaterialChangerCosmetic : MonoBehaviour
	{
		// Token: 0x060060E5 RID: 24805 RVA: 0x001EDC0C File Offset: 0x001EBE0C
		public void ChangeMaterial(Material newMaterial)
		{
			if (this.targetRenderer == null || newMaterial == null || this.materialIndex < 0)
			{
				return;
			}
			Material[] materials = this.targetRenderer.materials;
			if (this.materialIndex >= materials.Length)
			{
				Debug.LogWarning(string.Format("Material index {0} is out of range.", this.materialIndex));
				return;
			}
			materials[this.materialIndex] = newMaterial;
			this.targetRenderer.materials = materials;
		}

		// Token: 0x060060E6 RID: 24806 RVA: 0x001EDC84 File Offset: 0x001EBE84
		public void ChangeAllMaterials(Material newMat)
		{
			if (this.targetRenderer == null || newMat == null)
			{
				return;
			}
			Material[] array = new Material[this.targetRenderer.materials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = newMat;
			}
			this.targetRenderer.materials = array;
		}

		// Token: 0x04006CE3 RID: 27875
		[SerializeField]
		private SkinnedMeshRenderer targetRenderer;

		// Token: 0x04006CE4 RID: 27876
		[SerializeField]
		private int materialIndex;
	}
}
