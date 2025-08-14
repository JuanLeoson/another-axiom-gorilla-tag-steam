using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020006FE RID: 1790
public class GorillaTagCompetitiveForcedLeaveRoomVolume : MonoBehaviour
{
	// Token: 0x06002CBB RID: 11451 RVA: 0x000EC8C0 File Offset: 0x000EAAC0
	private void Start()
	{
		this.VolumeCollider = base.GetComponent<Collider>();
		this.CompetitiveManager = (GameMode.GetGameModeInstance(GameModeType.InfectionCompetitive) as GorillaTagCompetitiveManager);
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.RegisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x000EC8FA File Offset: 0x000EAAFA
	private void OnDestroy()
	{
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.UnregisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x000EC918 File Offset: 0x000EAB18
	public bool ContainsPoint(Vector3 position)
	{
		SphereCollider sphereCollider = this.VolumeCollider as SphereCollider;
		if (sphereCollider != null)
		{
			return Vector3.SqrMagnitude(position - (sphereCollider.transform.position + sphereCollider.center)) <= sphereCollider.radius * sphereCollider.radius;
		}
		BoxCollider boxCollider = this.VolumeCollider as BoxCollider;
		return boxCollider != null && boxCollider.bounds.Contains(position);
	}

	// Token: 0x04003831 RID: 14385
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04003832 RID: 14386
	private Collider VolumeCollider;
}
