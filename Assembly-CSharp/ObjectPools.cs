using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ACD RID: 2765
public class ObjectPools : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000657 RID: 1623
	// (get) Token: 0x060042BD RID: 17085 RVA: 0x0014F946 File Offset: 0x0014DB46
	// (set) Token: 0x060042BE RID: 17086 RVA: 0x0014F94E File Offset: 0x0014DB4E
	public bool initialized { get; private set; }

	// Token: 0x060042BF RID: 17087 RVA: 0x0014F957 File Offset: 0x0014DB57
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x060042C0 RID: 17088 RVA: 0x0014F95F File Offset: 0x0014DB5F
	protected void Start()
	{
		this.InitializePools();
	}

	// Token: 0x060042C1 RID: 17089 RVA: 0x0014F968 File Offset: 0x0014DB68
	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x060042C2 RID: 17090 RVA: 0x0014FA9C File Offset: 0x0014DC9C
	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	// Token: 0x060042C3 RID: 17091 RVA: 0x0014FAAA File Offset: 0x0014DCAA
	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	// Token: 0x060042C4 RID: 17092 RVA: 0x0014FAB8 File Offset: 0x0014DCB8
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x060042C5 RID: 17093 RVA: 0x0014FAC8 File Offset: 0x0014DCC8
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	// Token: 0x060042C6 RID: 17094 RVA: 0x0014FAE3 File Offset: 0x0014DCE3
	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	// Token: 0x060042C7 RID: 17095 RVA: 0x0014FAF2 File Offset: 0x0014DCF2
	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	// Token: 0x060042C8 RID: 17096 RVA: 0x0014FB01 File Offset: 0x0014DD01
	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x060042C9 RID: 17097 RVA: 0x0014FB17 File Offset: 0x0014DD17
	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x060042CA RID: 17098 RVA: 0x0014FB2F File Offset: 0x0014DD2F
	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x060042CB RID: 17099 RVA: 0x0014FB45 File Offset: 0x0014DD45
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x060042CC RID: 17100 RVA: 0x0014FB5D File Offset: 0x0014DD5D
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x060042CD RID: 17101 RVA: 0x0014FB8C File Offset: 0x0014DD8C
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x060042CE RID: 17102 RVA: 0x0014FB9C File Offset: 0x0014DD9C
	public bool BuildValidationCheck()
	{
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x04004DBF RID: 19903
	public static ObjectPools instance;

	// Token: 0x04004DC1 RID: 19905
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04004DC2 RID: 19906
	private Dictionary<int, SinglePool> lookUp;
}
