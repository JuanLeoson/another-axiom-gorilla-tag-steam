using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B59 RID: 2905
public class ZoneBasedGameObjectActivator : MonoBehaviour
{
	// Token: 0x06004588 RID: 17800 RVA: 0x0015B426 File Offset: 0x00159626
	private void OnEnable()
	{
		ZoneManagement.OnZoneChange += this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x0015B439 File Offset: 0x00159639
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x0600458A RID: 17802 RVA: 0x0015B44C File Offset: 0x0015964C
	private void ZoneManagement_OnZoneChange(ZoneData[] zoneData)
	{
		HashSet<GTZone> hashSet = new HashSet<GTZone>(this.zones);
		bool flag = false;
		for (int i = 0; i < zoneData.Length; i++)
		{
			flag |= (zoneData[i].active && hashSet.Contains(zoneData[i].zone));
		}
		for (int j = 0; j < this.gameObjects.Length; j++)
		{
			this.gameObjects[j].SetActive(flag);
		}
	}

	// Token: 0x04005084 RID: 20612
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x04005085 RID: 20613
	[SerializeField]
	private GameObject[] gameObjects;
}
