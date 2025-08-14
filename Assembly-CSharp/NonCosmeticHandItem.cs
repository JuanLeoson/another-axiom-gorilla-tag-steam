using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class NonCosmeticHandItem : MonoBehaviour
{
	// Token: 0x060012BB RID: 4795 RVA: 0x00066E8B File Offset: 0x0006508B
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x060012BC RID: 4796 RVA: 0x00066EAB File Offset: 0x000650AB
	public bool IsEnabled
	{
		get
		{
			return this.itemPrefab && this.itemPrefab.gameObject.activeSelf;
		}
	}

	// Token: 0x04001A57 RID: 6743
	public CosmeticsController.CosmeticSlots cosmeticSlots;

	// Token: 0x04001A58 RID: 6744
	public GameObject itemPrefab;
}
