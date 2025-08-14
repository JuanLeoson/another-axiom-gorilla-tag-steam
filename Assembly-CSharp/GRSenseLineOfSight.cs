using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000668 RID: 1640
[Serializable]
public class GRSenseLineOfSight
{
	// Token: 0x06002830 RID: 10288 RVA: 0x000D8C3A File Offset: 0x000D6E3A
	public bool HasLineOfSight(Vector3 headPos, Vector3 targetPos)
	{
		return GRSenseLineOfSight.HasLineOfSight(headPos, targetPos, this.sightDist, this.visibilityMask.value, this.rayCastMode);
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000D8C5C File Offset: 0x000D6E5C
	public static bool HasLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask, GRSenseLineOfSight.RaycastMode rayCastMode = GRSenseLineOfSight.RaycastMode.Geometry)
	{
		switch (rayCastMode)
		{
		case GRSenseLineOfSight.RaycastMode.Geometry:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		case GRSenseLineOfSight.RaycastMode.Navmesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryAndNavMesh:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask) && GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryOrNavMesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist) || GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		default:
			return false;
		}
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000D8CC2 File Offset: 0x000D6EC2
	public static bool HasGeoLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask)
	{
		return Physics.RaycastNonAlloc(new Ray(headPos, targetPos - headPos), GRSenseLineOfSight.visibilityHits, Mathf.Min(Vector3.Distance(targetPos, headPos), sightDist), layerMask, QueryTriggerInteraction.Ignore) < 1;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000D8CF0 File Offset: 0x000D6EF0
	public static bool HasNavmeshLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist)
	{
		NavMeshHit navMeshHit;
		NavMeshHit navMeshHit2;
		return (targetPos - headPos).sqrMagnitude <= sightDist * sightDist && NavMesh.SamplePosition(headPos, out navMeshHit, 1f, -1) && !NavMesh.Raycast(navMeshHit.position, targetPos, out navMeshHit2, -1);
	}

	// Token: 0x040033A5 RID: 13221
	public float sightDist;

	// Token: 0x040033A6 RID: 13222
	public LayerMask visibilityMask;

	// Token: 0x040033A7 RID: 13223
	public GRSenseLineOfSight.RaycastMode rayCastMode;

	// Token: 0x040033A8 RID: 13224
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x02000669 RID: 1641
	public enum RaycastMode
	{
		// Token: 0x040033AA RID: 13226
		Geometry,
		// Token: 0x040033AB RID: 13227
		Navmesh,
		// Token: 0x040033AC RID: 13228
		GeometryAndNavMesh,
		// Token: 0x040033AD RID: 13229
		GeometryOrNavMesh
	}
}
