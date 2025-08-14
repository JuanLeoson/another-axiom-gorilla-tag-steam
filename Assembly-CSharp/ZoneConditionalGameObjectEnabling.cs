using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class ZoneConditionalGameObjectEnabling : MonoBehaviour
{
	// Token: 0x06000E76 RID: 3702 RVA: 0x00058076 File Offset: 0x00056276
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x000580A4 File Offset: 0x000562A4
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x000580CC File Offset: 0x000562CC
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.gameObjects != null)
			{
				for (int i = 0; i < this.gameObjects.Length; i++)
				{
					this.gameObjects[i].SetActive(!ZoneManagement.IsInZone(this.zone));
				}
				return;
			}
		}
		else if (this.gameObjects != null)
		{
			for (int j = 0; j < this.gameObjects.Length; j++)
			{
				this.gameObjects[j].SetActive(ZoneManagement.IsInZone(this.zone));
			}
		}
	}

	// Token: 0x04001754 RID: 5972
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001755 RID: 5973
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04001756 RID: 5974
	[SerializeField]
	private GameObject[] gameObjects;
}
