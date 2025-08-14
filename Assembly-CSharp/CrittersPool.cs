using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class CrittersPool : MonoBehaviour
{
	// Token: 0x0600028C RID: 652 RVA: 0x00010340 File Offset: 0x0000E540
	public static GameObject GetPooled(GameObject prefab)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return null;
		}
		return crittersPool.GetInstance(prefab);
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00010353 File Offset: 0x0000E553
	public static void Return(GameObject pooledGO)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return;
		}
		crittersPool.ReturnInstance(pooledGO);
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00010365 File Offset: 0x0000E565
	private void Awake()
	{
		if (CrittersPool.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		CrittersPool.instance = this;
		this.SetupPools();
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00010388 File Offset: 0x0000E588
	private void SetupPools()
	{
		this.pools = new Dictionary<GameObject, List<GameObject>>();
		this.poolParent = new GameObject("CrittersPool")
		{
			transform = 
			{
				parent = base.transform
			}
		}.transform;
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			CrittersPool.CrittersPoolSettings crittersPoolSettings = this.eventEffects[i];
			if (crittersPoolSettings.poolObject == null || crittersPoolSettings.poolSize <= 0)
			{
				GTDev.Log<string>("CrittersPool.SetupPools Failed. Pool has no poolObject or has size 0.", null);
			}
			else
			{
				List<GameObject> list = new List<GameObject>();
				for (int j = 0; j < crittersPoolSettings.poolSize; j++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(crittersPoolSettings.poolObject);
					gameObject.transform.SetParent(this.poolParent);
					GameObject gameObject2 = gameObject;
					gameObject2.name += j.ToString();
					gameObject.SetActive(false);
					list.Add(gameObject);
				}
				this.pools.Add(crittersPoolSettings.poolObject, list);
			}
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00010484 File Offset: 0x0000E684
	private GameObject GetInstance(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pools.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && !list[i].activeSelf)
				{
					list[i].SetActive(true);
					return list[i];
				}
			}
			GTDev.Log<string>("CrittersPool.GetInstance Failed. No available instance.", null);
			return null;
		}
		GTDev.LogError<string>("CrittersPool.GetInstance Failed. Prefab doesn't have a valid pool setup.", null);
		return null;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000104FD File Offset: 0x0000E6FD
	private void ReturnInstance(GameObject instance)
	{
		instance.transform.SetParent(this.poolParent);
		instance.SetActive(false);
	}

	// Token: 0x0400030D RID: 781
	private static CrittersPool instance;

	// Token: 0x0400030E RID: 782
	public CrittersPool.CrittersPoolSettings[] eventEffects;

	// Token: 0x0400030F RID: 783
	private Dictionary<GameObject, List<GameObject>> pools;

	// Token: 0x04000310 RID: 784
	public Transform poolParent;

	// Token: 0x02000069 RID: 105
	[Serializable]
	public class CrittersPoolSettings
	{
		// Token: 0x04000311 RID: 785
		public GameObject poolObject;

		// Token: 0x04000312 RID: 786
		public int poolSize = 20;
	}
}
