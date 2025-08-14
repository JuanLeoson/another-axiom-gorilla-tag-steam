using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200033F RID: 831
public class TeleportTargetHandlerNavMesh : TeleportTargetHandler
{
	// Token: 0x060013DD RID: 5085 RVA: 0x0006A823 File Offset: 0x00068A23
	private void Awake()
	{
		this._path = new NavMeshPath();
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0006A830 File Offset: 0x00068A30
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, this.AimCollisionLayerMask, out this.AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * this.AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0006A89C File Offset: 0x00068A9C
	public override Vector3? ConsiderDestination(Vector3 location)
	{
		Vector3? result = base.ConsiderDestination(location);
		if (result != null)
		{
			Vector3 characterPosition = base.LocomotionTeleport.GetCharacterPosition();
			Vector3 valueOrDefault = result.GetValueOrDefault();
			NavMesh.CalculatePath(characterPosition, valueOrDefault, this.NavMeshAreaMask, this._path);
			if (this._path.status == NavMeshPathStatus.PathComplete)
			{
				return result;
			}
		}
		return null;
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("SHOW_PATH_RESULT")]
	private void OnDrawGizmos()
	{
	}

	// Token: 0x04001B54 RID: 6996
	public int NavMeshAreaMask = -1;

	// Token: 0x04001B55 RID: 6997
	private NavMeshPath _path;
}
