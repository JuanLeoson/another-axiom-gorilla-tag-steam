using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020007B2 RID: 1970
public class SpawnPooledObject : MonoBehaviour
{
	// Token: 0x06003173 RID: 12659 RVA: 0x00101272 File Offset: 0x000FF472
	private void Awake()
	{
		if (this._pooledObject == null)
		{
			return;
		}
		this._pooledObjectHash = PoolUtils.GameObjHashCode(this._pooledObject);
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x00101294 File Offset: 0x000FF494
	public void SpawnObject()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (this._pooledObject == null || this._spawnLocation == null)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._pooledObjectHash, true);
		gameObject.transform.position = this.SpawnLocation();
		gameObject.transform.rotation = this.SpawnRotation();
		gameObject.transform.localScale = base.transform.lossyScale;
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x0010130F File Offset: 0x000FF50F
	private Vector3 SpawnLocation()
	{
		return this._spawnLocation.transform.position + this.offset;
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x0010132C File Offset: 0x000FF52C
	private Quaternion SpawnRotation()
	{
		Quaternion result = this._spawnLocation.transform.rotation;
		if (this.facePlayer)
		{
			result = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.position - this._spawnLocation.transform.position);
		}
		if (this.upright)
		{
			result.eulerAngles = new Vector3(0f, result.eulerAngles.y, 0f);
		}
		return result;
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x001013AC File Offset: 0x000FF5AC
	private bool ShouldSpawn()
	{
		return Random.Range(0, 100) < this.chanceToSpawn;
	}

	// Token: 0x04003D11 RID: 15633
	[SerializeField]
	private Transform _spawnLocation;

	// Token: 0x04003D12 RID: 15634
	[SerializeField]
	private GameObject _pooledObject;

	// Token: 0x04003D13 RID: 15635
	[FormerlySerializedAs("_offset")]
	public Vector3 offset;

	// Token: 0x04003D14 RID: 15636
	[FormerlySerializedAs("_upright")]
	public bool upright;

	// Token: 0x04003D15 RID: 15637
	[FormerlySerializedAs("_facePlayer")]
	public bool facePlayer;

	// Token: 0x04003D16 RID: 15638
	[FormerlySerializedAs("_chanceToSpawn")]
	[Range(0f, 100f)]
	public int chanceToSpawn = 100;

	// Token: 0x04003D17 RID: 15639
	private int _pooledObjectHash;
}
