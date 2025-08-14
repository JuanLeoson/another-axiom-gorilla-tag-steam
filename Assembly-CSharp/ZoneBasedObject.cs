using System;
using UnityEngine;

// Token: 0x02000858 RID: 2136
public class ZoneBasedObject : MonoBehaviour
{
	// Token: 0x060035B0 RID: 13744 RVA: 0x001196FC File Offset: 0x001178FC
	public bool IsLocalPlayerInZone()
	{
		GTZone[] array = this.zones;
		for (int i = 0; i < array.Length; i++)
		{
			if (ZoneManagement.IsInZone(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x0011972C File Offset: 0x0011792C
	public static ZoneBasedObject SelectRandomEligible(ZoneBasedObject[] objects, string overrideChoice = "")
	{
		if (overrideChoice != "")
		{
			foreach (ZoneBasedObject zoneBasedObject in objects)
			{
				if (zoneBasedObject.gameObject.name == overrideChoice)
				{
					return zoneBasedObject;
				}
			}
		}
		ZoneBasedObject result = null;
		int num = 0;
		foreach (ZoneBasedObject zoneBasedObject2 in objects)
		{
			if (zoneBasedObject2.gameObject.activeInHierarchy)
			{
				GTZone[] array = zoneBasedObject2.zones;
				for (int j = 0; j < array.Length; j++)
				{
					if (ZoneManagement.IsInZone(array[j]))
					{
						if (Random.Range(0, num) == 0)
						{
							result = zoneBasedObject2;
						}
						num++;
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x040042A1 RID: 17057
	public GTZone[] zones;
}
