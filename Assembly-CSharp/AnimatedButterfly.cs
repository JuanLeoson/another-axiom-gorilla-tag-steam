using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000107 RID: 263
public struct AnimatedButterfly
{
	// Token: 0x0600068F RID: 1679 RVA: 0x000261D4 File Offset: 0x000243D4
	public void UpdateVisual(float syncTime, ButterflySwarmManager manager)
	{
		if (this.destinationCache == null)
		{
			return;
		}
		syncTime %= this.loopDuration;
		Vector3 vector;
		Vector3 vector2;
		this.GetPositionAndDestinationAtTime(syncTime, out vector, out vector2);
		Vector3 target = (vector2 - this.oldPosition).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * manager.BeeJitterDamping, target, manager.BeeAcceleration * Time.deltaTime);
		float sqrMagnitude = (this.oldPosition - vector2).sqrMagnitude;
		if (sqrMagnitude < manager.BeeNearDestinationRadius * manager.BeeNearDestinationRadius)
		{
			this.visual.transform.position = Vector3.MoveTowards(this.visual.transform.position, vector2, Time.deltaTime);
			this.visual.transform.rotation = this.destinationB.destination.transform.rotation;
			if (sqrMagnitude < 1E-07f && !this.wasPerched)
			{
				this.material.SetFloat(ShaderProps._VertexFlapSpeed, manager.PerchedFlapSpeed);
				this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, manager.PerchedFlapPhase);
				this.wasPerched = true;
			}
		}
		else
		{
			if (this.wasPerched)
			{
				this.material.SetFloat(ShaderProps._VertexFlapSpeed, this.baseFlapSpeed);
				this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, 0f);
				this.wasPerched = false;
			}
			this.velocity += Random.insideUnitSphere * manager.BeeJitterStrength * Time.deltaTime;
			Vector3 vector3 = this.oldPosition + this.velocity * Time.deltaTime;
			if ((vector3 - vector).IsLongerThan(manager.BeeMaxJitterRadius))
			{
				vector3 = vector + (vector3 - vector).normalized * manager.BeeMaxJitterRadius;
				this.velocity = (vector3 - this.oldPosition) / Time.deltaTime;
			}
			foreach (GameObject gameObject in BeeSwarmManager.avoidPoints)
			{
				Vector3 position = gameObject.transform.position;
				if ((vector3 - position).IsShorterThan(manager.AvoidPointRadius))
				{
					Vector3 normalized = Vector3.Cross(position - vector3, vector2 - vector3).normalized;
					Vector3 normalized2 = (vector2 - position).normalized;
					float num = Vector3.Dot(vector3 - position, normalized);
					Vector3 b = (manager.AvoidPointRadius - num) * normalized;
					vector3 += b;
					this.velocity += b;
				}
			}
			this.visual.transform.position = vector3;
			if ((vector2 - vector3).IsLongerThan(0.01f))
			{
				this.visual.transform.rotation = Quaternion.LookRotation(vector2 - vector3) * this.travellingLocalRotation;
			}
		}
		this.oldPosition = this.visual.transform.position;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002652C File Offset: 0x0002472C
	public void GetPositionAndDestinationAtTime(float syncTime, out Vector3 idealPosition, out Vector3 destination)
	{
		if (syncTime > this.destinationB.syncEndTime || syncTime < this.destinationA.syncTime || this.destinationA.destination == null || this.destinationB.destination == null)
		{
			int num = 0;
			int num2 = this.destinationCache.Count - 1;
			while (num + 1 < num2)
			{
				int num3 = (num + num2) / 2;
				float syncTime2 = this.destinationCache[num3].syncTime;
				float syncEndTime = this.destinationCache[num3].syncEndTime;
				if (syncTime2 <= syncTime && syncEndTime >= syncTime)
				{
					idealPosition = this.destinationCache[num3].destination.transform.position;
					destination = idealPosition;
				}
				if (syncEndTime < syncTime)
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			this.destinationA = this.destinationCache[num];
			this.destinationB = this.destinationCache[num2];
		}
		float t = Mathf.InverseLerp(this.destinationA.syncEndTime, this.destinationB.syncTime, syncTime);
		destination = this.destinationB.destination.transform.position;
		idealPosition = Vector3.Lerp(this.destinationA.destination.transform.position, destination, t);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00026677 File Offset: 0x00024877
	public void InitVisual(MeshRenderer prefab, ButterflySwarmManager manager)
	{
		this.visual = Object.Instantiate<MeshRenderer>(prefab, manager.transform);
		this.material = this.visual.material;
		this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, 0f);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000266B1 File Offset: 0x000248B1
	public void SetColor(Color color)
	{
		this.material.SetColor(ShaderProps._BaseColor, color);
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000266C4 File Offset: 0x000248C4
	public void SetFlapSpeed(float flapSpeed)
	{
		this.material.SetFloat(ShaderProps._VertexFlapSpeed, flapSpeed);
		this.baseFlapSpeed = flapSpeed;
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x000266E0 File Offset: 0x000248E0
	public void InitRoute(List<GameObject> route, List<float> holdTimes, ButterflySwarmManager manager)
	{
		this.speed = manager.BeeSpeed;
		this.maxTravelTime = manager.BeeMaxTravelTime;
		this.travellingLocalRotation = manager.TravellingLocalRotation;
		this.destinationCache = new List<AnimatedButterfly.TimedDestination>(route.Count + 1);
		this.destinationCache.Clear();
		this.destinationCache.Add(new AnimatedButterfly.TimedDestination
		{
			syncTime = 0f,
			syncEndTime = 0f,
			destination = route[0]
		});
		float num = 0f;
		for (int i = 1; i < route.Count; i++)
		{
			float num2 = (route[i].transform.position - route[i - 1].transform.position).magnitude / this.speed;
			num2 = Mathf.Min(num2, this.maxTravelTime);
			num += num2;
			float num3 = holdTimes[i];
			this.destinationCache.Add(new AnimatedButterfly.TimedDestination
			{
				syncTime = num,
				syncEndTime = num + num3,
				destination = route[i]
			});
			num += num3;
		}
		num += Mathf.Min((route[0].transform.position - route[route.Count - 1].transform.position).magnitude / this.speed, this.maxTravelTime);
		float num4 = holdTimes[0];
		this.destinationCache.Add(new AnimatedButterfly.TimedDestination
		{
			syncTime = num,
			syncEndTime = num + num4,
			destination = route[0]
		});
		this.loopDuration = num + (route[0].transform.position - route[route.Count - 1].transform.position).magnitude * manager.BeeSpeed + holdTimes[0];
	}

	// Token: 0x040007FE RID: 2046
	private List<AnimatedButterfly.TimedDestination> destinationCache;

	// Token: 0x040007FF RID: 2047
	private AnimatedButterfly.TimedDestination destinationA;

	// Token: 0x04000800 RID: 2048
	private AnimatedButterfly.TimedDestination destinationB;

	// Token: 0x04000801 RID: 2049
	private float loopDuration;

	// Token: 0x04000802 RID: 2050
	private Vector3 oldPosition;

	// Token: 0x04000803 RID: 2051
	private Vector3 velocity;

	// Token: 0x04000804 RID: 2052
	public MeshRenderer visual;

	// Token: 0x04000805 RID: 2053
	private Material material;

	// Token: 0x04000806 RID: 2054
	private float speed;

	// Token: 0x04000807 RID: 2055
	private float maxTravelTime;

	// Token: 0x04000808 RID: 2056
	private Quaternion travellingLocalRotation;

	// Token: 0x04000809 RID: 2057
	private float baseFlapSpeed;

	// Token: 0x0400080A RID: 2058
	private bool wasPerched;

	// Token: 0x02000108 RID: 264
	private struct TimedDestination
	{
		// Token: 0x0400080B RID: 2059
		public float syncTime;

		// Token: 0x0400080C RID: 2060
		public float syncEndTime;

		// Token: 0x0400080D RID: 2061
		public GameObject destination;
	}
}
