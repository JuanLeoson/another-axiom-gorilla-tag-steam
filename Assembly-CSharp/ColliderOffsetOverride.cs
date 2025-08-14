using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class ColliderOffsetOverride : MonoBehaviour
{
	// Token: 0x060000B5 RID: 181 RVA: 0x00005108 File Offset: 0x00003308
	private void Awake()
	{
		if (this.autoSearch)
		{
			this.FindColliders();
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.contactOffset = 0.01f * this.targetScale;
			}
		}
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005180 File Offset: 0x00003380
	public void FindColliders()
	{
		foreach (Collider item in base.gameObject.GetComponents<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x000051F0 File Offset: 0x000033F0
	public void FindCollidersRecursively()
	{
		foreach (Collider item in base.gameObject.GetComponentsInChildren<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005260 File Offset: 0x00003460
	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005269 File Offset: 0x00003469
	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	// Token: 0x040000CB RID: 203
	public List<Collider> colliders;

	// Token: 0x040000CC RID: 204
	[HideInInspector]
	public bool autoSearch;

	// Token: 0x040000CD RID: 205
	public float targetScale = 1f;
}
