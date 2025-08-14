using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
public class ParentedObjectStressTestMain : MonoBehaviour
{
	// Token: 0x0600003C RID: 60 RVA: 0x000026F4 File Offset: 0x000008F4
	public void Start()
	{
		for (int i = 0; i < (int)this.NumObjects.x; i++)
		{
			for (int j = 0; j < (int)this.NumObjects.y; j++)
			{
				for (int k = 0; k < (int)this.NumObjects.z; k++)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.Object).transform.position = new Vector3(2f * ((float)i / (this.NumObjects.x - 1f) - 0.5f) * this.NumObjects.x * this.Spacing.x, 2f * ((float)j / (this.NumObjects.y - 1f) - 0.5f) * this.NumObjects.y * this.Spacing.y, 2f * ((float)k / (this.NumObjects.z - 1f) - 0.5f) * this.NumObjects.z * this.Spacing.z);
				}
			}
		}
	}

	// Token: 0x0400001C RID: 28
	public GameObject Object;

	// Token: 0x0400001D RID: 29
	public Vector3 NumObjects;

	// Token: 0x0400001E RID: 30
	public Vector3 Spacing;
}
