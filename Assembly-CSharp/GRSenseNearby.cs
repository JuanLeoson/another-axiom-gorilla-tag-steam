using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000667 RID: 1639
[Serializable]
public class GRSenseNearby
{
	// Token: 0x06002828 RID: 10280 RVA: 0x000D89C3 File Offset: 0x000D6BC3
	public void Setup(Transform headTransform)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000D89D8 File Offset: 0x000D6BD8
	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 forward = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, forward, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x000D8A1F File Offset: 0x000D6C1F
	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x000D8A40 File Offset: 0x000D6C40
	public void AddNearby(Vector3 position, Vector3 forward, List<VRRig> allRigs)
	{
		float num = this.range * this.range;
		float num2 = Mathf.Cos(this.fov * 0.017453292f);
		for (int i = 0; i < allRigs.Count; i++)
		{
			VRRig vrrig = allRigs[i];
			if (vrrig.GetComponent<GRPlayer>().State != GRPlayer.GRPlayerState.Ghost && !this.rigsNearby.Contains(vrrig))
			{
				Vector3 a = vrrig.GetMouthPosition() - position;
				float sqrMagnitude = a.sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					if (sqrMagnitude > 0f)
					{
						float d = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(a / d, forward) < num2)
						{
							goto IL_9B;
						}
					}
					this.rigsNearby.Add(vrrig);
				}
			}
			IL_9B:;
		}
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000D8AF8 File Offset: 0x000D6CF8
	public void RemoveNotNearby(Vector3 position)
	{
		float num = this.exitRange * this.exitRange;
		int i = 0;
		while (i < this.rigsNearby.Count)
		{
			VRRig vrrig = this.rigsNearby[i];
			if (!(vrrig != null))
			{
				goto IL_50;
			}
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if ((vrrig.GetMouthPosition() - position).sqrMagnitude > num || component.State == GRPlayer.GRPlayerState.Ghost)
			{
				goto IL_50;
			}
			IL_60:
			i++;
			continue;
			IL_50:
			this.rigsNearby.RemoveAt(i);
			i--;
			goto IL_60;
		}
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000D8B78 File Offset: 0x000D6D78
	public void RemoveNoLineOfSight(Vector3 headPos, GRSenseLineOfSight senseLineOfSight)
	{
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			Vector3 mouthPosition = this.rigsNearby[i].GetMouthPosition();
			if (!senseLineOfSight.HasLineOfSight(headPos, mouthPosition))
			{
				this.rigsNearby.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000D8BC8 File Offset: 0x000D6DC8
	public VRRig PickClosest(out float outDistanceSq)
	{
		Vector3 position = this.headTransform.position;
		float num = float.MaxValue;
		VRRig result = null;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			float sqrMagnitude = (this.rigsNearby[i].GetMouthPosition() - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = this.rigsNearby[i];
			}
		}
		outDistanceSq = num;
		return result;
	}

	// Token: 0x040033A0 RID: 13216
	public float range;

	// Token: 0x040033A1 RID: 13217
	public float exitRange;

	// Token: 0x040033A2 RID: 13218
	public float fov;

	// Token: 0x040033A3 RID: 13219
	[ReadOnly]
	public List<VRRig> rigsNearby;

	// Token: 0x040033A4 RID: 13220
	private Transform headTransform;
}
