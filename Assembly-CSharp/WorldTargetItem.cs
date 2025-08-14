using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
public class WorldTargetItem
{
	// Token: 0x060017A2 RID: 6050 RVA: 0x0007F84C File Offset: 0x0007DA4C
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x0007F864 File Offset: 0x0007DA64
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(NetPlayer owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x0007F8AC File Offset: 0x0007DAAC
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x0007F8B6 File Offset: 0x0007DAB6
	private WorldTargetItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x0007F8DF File Offset: 0x0007DADF
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x04001F90 RID: 8080
	public readonly NetPlayer owner;

	// Token: 0x04001F91 RID: 8081
	public readonly int itemIdx;

	// Token: 0x04001F92 RID: 8082
	public readonly Transform targetObject;

	// Token: 0x04001F93 RID: 8083
	public readonly TransferrableObject transferrableObject;
}
