using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class CosmeticRefRegistry : MonoBehaviour
{
	// Token: 0x060009D7 RID: 2519 RVA: 0x00035F3C File Offset: 0x0003413C
	private void Awake()
	{
		foreach (CosmeticRefTarget cosmeticRefTarget in this.builtInRefTargets)
		{
			this.Register(cosmeticRefTarget.id, cosmeticRefTarget.gameObject);
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00035F74 File Offset: 0x00034174
	public void Register(CosmeticRefID partID, GameObject part)
	{
		this.partsTable[(int)partID] = part;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00035F7F File Offset: 0x0003417F
	public GameObject Get(CosmeticRefID partID)
	{
		return this.partsTable[(int)partID];
	}

	// Token: 0x04000BC5 RID: 3013
	private GameObject[] partsTable = new GameObject[8];

	// Token: 0x04000BC6 RID: 3014
	[SerializeField]
	private CosmeticRefTarget[] builtInRefTargets;
}
