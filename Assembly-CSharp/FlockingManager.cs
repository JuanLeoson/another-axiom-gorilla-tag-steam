using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200058D RID: 1421
[NetworkBehaviourWeaved(337)]
public class FlockingManager : NetworkComponent
{
	// Token: 0x060022AC RID: 8876 RVA: 0x000BB75C File Offset: 0x000B995C
	protected override void Awake()
	{
		base.Awake();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			Flocking[] componentsInChildren = gameObject.GetComponentsInChildren<Flocking>(false);
			FlockingManager.FishArea fishArea = new FlockingManager.FishArea();
			fishArea.id = gameObject.name;
			fishArea.colliders = gameObject.GetComponentsInChildren<BoxCollider>();
			fishArea.colliderCenter = fishArea.colliders[0].bounds.center;
			fishArea.fishList.AddRange(componentsInChildren);
			fishArea.zoneBasedObject = gameObject.GetComponent<ZoneBasedObject>();
			this.areaToWaypointDict[fishArea.id] = Vector3.zero;
			Flocking[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FishArea = fishArea;
			}
			this.fishAreaList.Add(fishArea);
			this.allFish.AddRange(fishArea.fishList);
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				component.OnProjectileTriggerExit += this.ProjectileHitExit;
			}
			else
			{
				Debug.LogError("Needs SlingshotProjectileHitNotifier added to each fish area");
			}
		}
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x0001AF23 File Offset: 0x00019123
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000BB8B4 File Offset: 0x000B9AB4
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.fishAreaList.Clear();
		this.areaToWaypointDict.Clear();
		this.allFish.Clear();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerExit -= this.ProjectileHitExit;
				component.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
			}
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000BB95C File Offset: 0x000B9B5C
	private void Update()
	{
		if (Random.Range(0, 10000) < 50)
		{
			foreach (FlockingManager.FishArea fishArea in this.fishAreaList)
			{
				if (fishArea.zoneBasedObject != null)
				{
					fishArea.zoneBasedObject.gameObject.SetActive(fishArea.zoneBasedObject.IsLocalPlayerInZone());
				}
				fishArea.nextWaypoint = this.GetRandomPointInsideCollider(fishArea);
				this.areaToWaypointDict[fishArea.id] = fishArea.nextWaypoint;
				Debug.DrawLine(fishArea.nextWaypoint, Vector3.forward * 5f, Color.magenta);
			}
		}
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000BBA28 File Offset: 0x000B9C28
	public Vector3 GetRandomPointInsideCollider(FlockingManager.FishArea fishArea)
	{
		int num = Random.Range(0, fishArea.colliders.Length);
		BoxCollider boxCollider = fishArea.colliders[num];
		Vector3 vector = boxCollider.size / 2f;
		Vector3 position = new Vector3(Random.Range(-vector.x, vector.x), Random.Range(-vector.y, vector.y), Random.Range(-vector.z, vector.z));
		return boxCollider.transform.TransformPoint(position);
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000BBAA8 File Offset: 0x000B9CA8
	public bool IsInside(Vector3 point, FlockingManager.FishArea fish)
	{
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			vector -= center;
			Vector3 size = boxCollider.size;
			if (Mathf.Abs(vector.x) < size.x / 2f && Mathf.Abs(vector.y) < size.y / 2f && Mathf.Abs(vector.z) < size.z / 2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000BBB44 File Offset: 0x000B9D44
	public Vector3 RestrictPointToArea(Vector3 point, FlockingManager.FishArea fish)
	{
		Vector3 result = default(Vector3);
		float num = float.MaxValue;
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector2 = vector - center;
			Vector3 size = boxCollider.size;
			float num2 = size.x / 2f;
			float num3 = size.y / 2f;
			float num4 = size.z / 2f;
			if (Mathf.Abs(vector2.x) < num2 && Mathf.Abs(vector2.y) < num3 && Mathf.Abs(vector2.z) < num4)
			{
				return point;
			}
			Vector3 vector3 = new Vector3(center.x - num2, center.y - num3, center.z - num4);
			Vector3 vector4 = new Vector3(center.x + num2, center.y + num3, center.z + num4);
			Vector3 vector5 = new Vector3(Mathf.Clamp(vector.x, vector3.x, vector4.x), Mathf.Clamp(vector.y, vector3.y, vector4.y), Mathf.Clamp(vector.z, vector3.z, vector4.z));
			float num5 = Vector3.Distance(vector, vector5);
			if (num5 < num)
			{
				num = num5;
				if (num5 > 1f)
				{
					Vector3 a = Vector3.Normalize(vector - vector5);
					result = boxCollider.transform.TransformPoint(vector5 + a * 1f);
				}
				else
				{
					result = point;
				}
			}
		}
		return result;
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000BBCF4 File Offset: 0x000B9EF4
	private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider1)
	{
		bool isRealFood = projectile.CompareTag(this.foodProjectileTag);
		FlockingManager.FishFood arg = new FlockingManager.FishFood
		{
			collider = (collider1 as BoxCollider),
			isRealFood = isRealFood,
			slingshotProjectile = projectile
		};
		UnityAction<FlockingManager.FishFood> unityAction = this.onFoodDetected;
		if (unityAction == null)
		{
			return;
		}
		unityAction(arg);
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000BBD3F File Offset: 0x000B9F3F
	private void ProjectileHitExit(SlingshotProjectile projectile, Collider collider2)
	{
		UnityAction<BoxCollider> unityAction = this.onFoodDestroyed;
		if (unityAction == null)
		{
			return;
		}
		unityAction(collider2 as BoxCollider);
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x060022B5 RID: 8885 RVA: 0x000BBD57 File Offset: 0x000B9F57
	// (set) Token: 0x060022B6 RID: 8886 RVA: 0x000BBD81 File Offset: 0x000B9F81
	[Networked]
	[NetworkedWeaved(0, 337)]
	public unsafe FlockingData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(FlockingData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(FlockingData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000BBDAC File Offset: 0x000B9FAC
	public override void WriteDataFusion()
	{
		this.Data = new FlockingData(this.allFish);
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000BBDC0 File Offset: 0x000B9FC0
	public override void ReadDataFusion()
	{
		for (int i = 0; i < this.Data.count; i++)
		{
			Vector3 syncPos = this.Data.Positions[i];
			Quaternion syncRot = this.Data.Rotations[i];
			this.allFish[i].SetSyncPosRot(syncPos, syncRot);
		}
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000BBE2B File Offset: 0x000BA02B
	public static void RegisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Add(obj);
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000BBE38 File Offset: 0x000BA038
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Remove(obj);
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000BBE86 File Offset: 0x000BA086
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000BBE9E File Offset: 0x000BA09E
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04002C55 RID: 11349
	public List<GameObject> fishAreaContainer;

	// Token: 0x04002C56 RID: 11350
	public string foodProjectileTag = "WaterBalloonProjectile";

	// Token: 0x04002C57 RID: 11351
	private Dictionary<string, Vector3> areaToWaypointDict = new Dictionary<string, Vector3>();

	// Token: 0x04002C58 RID: 11352
	private List<FlockingManager.FishArea> fishAreaList = new List<FlockingManager.FishArea>();

	// Token: 0x04002C59 RID: 11353
	private List<Flocking> allFish = new List<Flocking>();

	// Token: 0x04002C5A RID: 11354
	public UnityAction<FlockingManager.FishFood> onFoodDetected;

	// Token: 0x04002C5B RID: 11355
	public UnityAction<BoxCollider> onFoodDestroyed;

	// Token: 0x04002C5C RID: 11356
	private bool hasBeenSerialized;

	// Token: 0x04002C5D RID: 11357
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();

	// Token: 0x04002C5E RID: 11358
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 337)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private FlockingData _Data;

	// Token: 0x0200058E RID: 1422
	public class FishArea
	{
		// Token: 0x04002C5F RID: 11359
		public string id;

		// Token: 0x04002C60 RID: 11360
		public List<Flocking> fishList = new List<Flocking>();

		// Token: 0x04002C61 RID: 11361
		public Vector3 colliderCenter;

		// Token: 0x04002C62 RID: 11362
		public BoxCollider[] colliders;

		// Token: 0x04002C63 RID: 11363
		public Vector3 nextWaypoint = Vector3.zero;

		// Token: 0x04002C64 RID: 11364
		public ZoneBasedObject zoneBasedObject;
	}

	// Token: 0x0200058F RID: 1423
	public class FishFood
	{
		// Token: 0x04002C65 RID: 11365
		public BoxCollider collider;

		// Token: 0x04002C66 RID: 11366
		public bool isRealFood;

		// Token: 0x04002C67 RID: 11367
		public SlingshotProjectile slingshotProjectile;
	}
}
