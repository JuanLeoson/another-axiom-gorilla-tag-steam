using System;
using Critters.Scripts;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class ReleaseFoodWhenUpsideDown : MonoBehaviour
{
	// Token: 0x0600034C RID: 844 RVA: 0x00013C31 File Offset: 0x00011E31
	private void Awake()
	{
		this.latch = false;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00013C3C File Offset: 0x00011E3C
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.dispenser.heldByPlayer)
		{
			return;
		}
		if (Vector3.Angle(base.transform.up, Vector3.down) < this.angle)
		{
			if (this.latch)
			{
				return;
			}
			this.latch = true;
			if (this.nextSpawnTime > (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
			{
				return;
			}
			this.nextSpawnTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) + (double)this.spawnDelay;
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.Food, this.foodSubIndex);
			if (!crittersActor.IsNull())
			{
				CrittersFood crittersFood = (CrittersFood)crittersActor;
				crittersFood.MoveActor(this.spawnPoint.position, this.spawnPoint.rotation, false, true, true);
				crittersFood.SetImpulseVelocity(Vector3.zero, Vector3.zero);
				crittersFood.SpawnData(this.maxFood, this.startingFood, this.startingSize);
				return;
			}
		}
		else
		{
			this.latch = false;
		}
	}

	// Token: 0x040003DF RID: 991
	public CrittersFoodDispenser dispenser;

	// Token: 0x040003E0 RID: 992
	public float angle = 30f;

	// Token: 0x040003E1 RID: 993
	private bool latch;

	// Token: 0x040003E2 RID: 994
	public Transform spawnPoint;

	// Token: 0x040003E3 RID: 995
	public float maxFood;

	// Token: 0x040003E4 RID: 996
	public float startingFood;

	// Token: 0x040003E5 RID: 997
	public float startingSize;

	// Token: 0x040003E6 RID: 998
	public int foodSubIndex;

	// Token: 0x040003E7 RID: 999
	public float spawnDelay = 0.6f;

	// Token: 0x040003E8 RID: 1000
	private double nextSpawnTime;
}
