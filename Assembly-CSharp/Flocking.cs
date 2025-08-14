using System;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200058A RID: 1418
public class Flocking : MonoBehaviour
{
	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06002293 RID: 8851 RVA: 0x000BAE35 File Offset: 0x000B9035
	// (set) Token: 0x06002294 RID: 8852 RVA: 0x000BAE3D File Offset: 0x000B903D
	public FlockingManager.FishArea FishArea { get; set; }

	// Token: 0x06002295 RID: 8853 RVA: 0x000BAE46 File Offset: 0x000B9046
	private void Awake()
	{
		this.manager = base.GetComponentInParent<FlockingManager>();
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000BAE54 File Offset: 0x000B9054
	private void Start()
	{
		this.speed = Random.Range(this.minSpeed, this.maxSpeed);
		this.fishState = Flocking.FishState.patrol;
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000BAE74 File Offset: 0x000B9074
	private void OnDisable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Remove(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Remove(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.UnregisterFlocking(this);
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000BAED8 File Offset: 0x000B90D8
	public void InvokeUpdate()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		this.AvoidPlayerHands();
		this.MaybeTurn();
		switch (this.fishState)
		{
		case Flocking.FishState.flock:
			this.Flock(this.FishArea.nextWaypoint);
			this.SwitchState(Flocking.FishState.patrol);
			break;
		case Flocking.FishState.patrol:
			if (Random.Range(0, 10) < 2)
			{
				this.SwitchState(Flocking.FishState.flock);
			}
			break;
		case Flocking.FishState.followFood:
			if (this.isTurning)
			{
				return;
			}
			if (this.isRealFood)
			{
				if ((double)Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFoodStopDistance)
				{
					this.FollowFood();
				}
				else
				{
					this.followingFood = false;
					this.Flock(this.projectileGameObject.transform.position);
					this.feedingTimeStarted += Time.deltaTime;
					if (this.feedingTimeStarted > this.eatFoodDuration)
					{
						this.SwitchState(Flocking.FishState.patrol);
					}
				}
			}
			else if (Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFakeFoodStopDistance)
			{
				this.FollowFood();
			}
			else
			{
				this.followingFood = false;
				this.SwitchState(Flocking.FishState.patrol);
			}
			break;
		}
		if (!this.followingFood)
		{
			base.transform.Translate(0f, 0f, this.speed * Time.deltaTime);
		}
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000BB074 File Offset: 0x000B9274
	private void MaybeTurn()
	{
		if (!this.manager.IsInside(base.transform.position, this.FishArea))
		{
			this.Turn(this.FishArea.colliderCenter);
			if (Vector3.Angle(this.FishArea.colliderCenter - base.transform.position, Vector3.forward) > 5f)
			{
				this.isTurning = true;
				return;
			}
		}
		else
		{
			this.isTurning = false;
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000BB0EC File Offset: 0x000B92EC
	private void Turn(Vector3 towardPoint)
	{
		this.isTurning = true;
		Quaternion to = Quaternion.LookRotation(towardPoint - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000BB13F File Offset: 0x000B933F
	private void SwitchState(Flocking.FishState state)
	{
		this.fishState = state;
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000BB148 File Offset: 0x000B9348
	private void Flock(Vector3 nextGoal)
	{
		Vector3 a = Vector3.zero;
		Vector3 vector = Vector3.zero;
		float num = 1f;
		int num2 = 0;
		foreach (Flocking flocking in this.FishArea.fishList)
		{
			if (flocking.gameObject != base.gameObject)
			{
				float num3 = Vector3.Distance(flocking.transform.position, base.transform.position);
				if (num3 <= this.maxNeighbourDistance)
				{
					a += flocking.transform.position;
					num2++;
					if (num3 < this.flockingAvoidanceDistance)
					{
						vector += base.transform.position - flocking.transform.position;
					}
					num += flocking.speed;
				}
			}
		}
		if (num2 > 0)
		{
			this.fishState = Flocking.FishState.flock;
			a = a / (float)num2 + (nextGoal - base.transform.position);
			this.speed = num / (float)num2;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
			Vector3 vector2 = a + vector - base.transform.position;
			if (vector2 != Vector3.zero)
			{
				Quaternion to = Quaternion.LookRotation(vector2);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
			}
		}
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000BB2EC File Offset: 0x000B94EC
	private void HandleOnFoodDetected(FlockingManager.FishFood fishFood)
	{
		bool flag = false;
		foreach (BoxCollider y in this.FishArea.colliders)
		{
			if (fishFood.collider == y)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.followFood);
		this.feedingTimeStarted = 0f;
		this.projectileGameObject = fishFood.slingshotProjectile.gameObject;
		this.isRealFood = fishFood.isRealFood;
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000BB35C File Offset: 0x000B955C
	private void HandleOnFoodDestroyed(BoxCollider collider)
	{
		bool flag = false;
		foreach (BoxCollider y in this.FishArea.colliders)
		{
			if (collider == y)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.patrol);
		this.projectileGameObject = null;
		this.followingFood = false;
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000BB3B0 File Offset: 0x000B95B0
	private void FollowFood()
	{
		this.followingFood = true;
		Quaternion to = Quaternion.LookRotation(this.projectileGameObject.transform.position - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.projectileGameObject.transform.position, this.speed * this.followFoodSpeedMult * Time.deltaTime);
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000BB450 File Offset: 0x000B9650
	private void AvoidPlayerHands()
	{
		foreach (GameObject gameObject in FlockingManager.avoidPoints)
		{
			Vector3 position = gameObject.transform.position;
			if ((base.transform.position - position).IsShorterThan(this.avointPointRadius))
			{
				Vector3 randomPointInsideCollider = this.manager.GetRandomPointInsideCollider(this.FishArea);
				this.Turn(randomPointInsideCollider);
				this.speed = this.avoidHandSpeed;
			}
		}
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000BB4E8 File Offset: 0x000B96E8
	internal void SetSyncPosRot(Vector3 syncPos, Quaternion syncRot)
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		if (this.FishArea == null)
		{
			Debug.LogError("FISH AREA NULL");
		}
		if (syncRot.IsValid())
		{
			this.rot = syncRot;
		}
		float num = 10000f;
		if (syncPos.IsValid(num))
		{
			this.pos = this.manager.RestrictPointToArea(syncPos, this.FishArea);
		}
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000BB55C File Offset: 0x000B975C
	private void OnEnable()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Combine(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Combine(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.RegisterFlocking(this);
	}

	// Token: 0x04002C32 RID: 11314
	[Tooltip("Speed is randomly generated from min and max speed")]
	public float minSpeed = 2f;

	// Token: 0x04002C33 RID: 11315
	public float maxSpeed = 4f;

	// Token: 0x04002C34 RID: 11316
	public float rotationSpeed = 360f;

	// Token: 0x04002C35 RID: 11317
	[Tooltip("Maximum distance to the neighbours to form a flocking group")]
	public float maxNeighbourDistance = 4f;

	// Token: 0x04002C36 RID: 11318
	public float eatFoodDuration = 10f;

	// Token: 0x04002C37 RID: 11319
	[Tooltip("How fast should it follow the food? This value multiplies by the current speed")]
	public float followFoodSpeedMult = 3f;

	// Token: 0x04002C38 RID: 11320
	[Tooltip("How fast should it run away from players hand?")]
	public float avoidHandSpeed = 1.2f;

	// Token: 0x04002C39 RID: 11321
	[FormerlySerializedAs("avoidanceDistance")]
	[Tooltip("When flocking they will avoid each other if the distance between them is less than this value")]
	public float flockingAvoidanceDistance = 2f;

	// Token: 0x04002C3A RID: 11322
	[Tooltip("Follow the fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFood")]
	public double FollowFoodStopDistance = 0.20000000298023224;

	// Token: 0x04002C3B RID: 11323
	[Tooltip("Follow any fake fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFakeFood")]
	public float FollowFakeFoodStopDistance = 2f;

	// Token: 0x04002C3C RID: 11324
	private float speed;

	// Token: 0x04002C3D RID: 11325
	private Vector3 averageHeading;

	// Token: 0x04002C3E RID: 11326
	private Vector3 averagePosition;

	// Token: 0x04002C3F RID: 11327
	private float feedingTimeStarted;

	// Token: 0x04002C40 RID: 11328
	private GameObject projectileGameObject;

	// Token: 0x04002C41 RID: 11329
	private bool followingFood;

	// Token: 0x04002C42 RID: 11330
	private FlockingManager manager;

	// Token: 0x04002C43 RID: 11331
	private GameObjectManagerWithId _fishSceneGameObjectsManager;

	// Token: 0x04002C44 RID: 11332
	private UnityEvent<string, Transform> sendIdEvent;

	// Token: 0x04002C45 RID: 11333
	private Flocking.FishState fishState;

	// Token: 0x04002C46 RID: 11334
	[HideInInspector]
	public Vector3 pos;

	// Token: 0x04002C47 RID: 11335
	[HideInInspector]
	public Quaternion rot;

	// Token: 0x04002C48 RID: 11336
	private float velocity;

	// Token: 0x04002C49 RID: 11337
	private bool isTurning;

	// Token: 0x04002C4A RID: 11338
	private bool isRealFood;

	// Token: 0x04002C4B RID: 11339
	public float avointPointRadius = 0.5f;

	// Token: 0x04002C4C RID: 11340
	private float cacheSpeed;

	// Token: 0x0200058B RID: 1419
	public enum FishState
	{
		// Token: 0x04002C4F RID: 11343
		flock,
		// Token: 0x04002C50 RID: 11344
		patrol,
		// Token: 0x04002C51 RID: 11345
		followFood
	}
}
