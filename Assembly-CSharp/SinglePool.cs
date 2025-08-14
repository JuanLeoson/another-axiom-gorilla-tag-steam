using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ACC RID: 2764
[Serializable]
public class SinglePool
{
	// Token: 0x060042B4 RID: 17076 RVA: 0x0014F744 File Offset: 0x0014D944
	private void PrivAllocPooledObjects()
	{
		int count = this.inactivePool.Count;
		for (int i = count; i < count + this.initAmountToPool; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
			gameObject.name = this.objectToPool.name + "(PoolIndex=" + i.ToString() + ")";
			gameObject.SetActive(false);
			this.inactivePool.Push(gameObject);
			int instanceID = gameObject.GetInstanceID();
			this.pooledObjects.Add(instanceID);
		}
	}

	// Token: 0x060042B5 RID: 17077 RVA: 0x0014F7D6 File Offset: 0x0014D9D6
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		this.PrivAllocPooledObjects();
	}

	// Token: 0x060042B6 RID: 17078 RVA: 0x0014F814 File Offset: 0x0014DA14
	public GameObject Instantiate(bool setActive = true)
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(setActive);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x060042B7 RID: 17079 RVA: 0x0014F87C File Offset: 0x0014DA7C
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the activePool");
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the pooledObjects");
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x060042B8 RID: 17080 RVA: 0x0014F902 File Offset: 0x0014DB02
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x060042B9 RID: 17081 RVA: 0x0014F90F File Offset: 0x0014DB0F
	public int GetTotalCount()
	{
		return this.pooledObjects.Count;
	}

	// Token: 0x060042BA RID: 17082 RVA: 0x0014F91C File Offset: 0x0014DB1C
	public int GetActiveCount()
	{
		return this.activePool.Count;
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x0014F929 File Offset: 0x0014DB29
	public int GetInactiveCount()
	{
		return this.inactivePool.Count;
	}

	// Token: 0x04004DB9 RID: 19897
	public GameObject objectToPool;

	// Token: 0x04004DBA RID: 19898
	public int initAmountToPool = 32;

	// Token: 0x04004DBB RID: 19899
	private HashSet<int> pooledObjects;

	// Token: 0x04004DBC RID: 19900
	private Stack<GameObject> inactivePool;

	// Token: 0x04004DBD RID: 19901
	private Dictionary<int, GameObject> activePool;

	// Token: 0x04004DBE RID: 19902
	private GameObject gameObject;
}
