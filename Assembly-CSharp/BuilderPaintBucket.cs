using System;
using UnityEngine;

// Token: 0x02000510 RID: 1296
public class BuilderPaintBucket : MonoBehaviour
{
	// Token: 0x06001FA3 RID: 8099 RVA: 0x000A759C File Offset: 0x000A579C
	private void Awake()
	{
		if (string.IsNullOrEmpty(this.materialId))
		{
			return;
		}
		this.materialType = this.materialId.GetHashCode();
		if (this.bucketMaterialOptions != null && this.paintBucketRenderer != null)
		{
			Material material;
			int num;
			this.bucketMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.paintBucketRenderer.material = material;
			}
		}
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000A7610 File Offset: 0x000A5810
	private void OnTriggerEnter(Collider other)
	{
		if (this.materialType == -1)
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			BuilderPaintBrush component = attachedRigidbody.GetComponent<BuilderPaintBrush>();
			if (component != null)
			{
				component.SetBrushMaterial(this.materialType);
			}
		}
	}

	// Token: 0x04002849 RID: 10313
	[SerializeField]
	private BuilderMaterialOptions bucketMaterialOptions;

	// Token: 0x0400284A RID: 10314
	[SerializeField]
	private MeshRenderer paintBucketRenderer;

	// Token: 0x0400284B RID: 10315
	[SerializeField]
	private string materialId;

	// Token: 0x0400284C RID: 10316
	private int materialType = -1;
}
