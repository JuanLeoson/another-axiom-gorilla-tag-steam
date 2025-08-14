using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200068F RID: 1679
public class GRUIBuyItem : MonoBehaviour
{
	// Token: 0x0600291E RID: 10526 RVA: 0x000DD5D2 File Offset: 0x000DB7D2
	public void Setup(int standId)
	{
		this.standId = standId;
		this.buyItemButton.onPressButton.AddListener(new UnityAction(this.OnBuyItem));
		this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnBuyItem()
	{
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000DD612 File Offset: 0x000DB812
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x0400350A RID: 13578
	[SerializeField]
	private GorillaPressableButton buyItemButton;

	// Token: 0x0400350B RID: 13579
	[SerializeField]
	private Text itemInfoLabel;

	// Token: 0x0400350C RID: 13580
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x0400350D RID: 13581
	[SerializeField]
	private GameEntity entityPrefab;

	// Token: 0x0400350E RID: 13582
	private int entityTypeId;

	// Token: 0x0400350F RID: 13583
	private int standId;
}
