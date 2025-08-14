using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000697 RID: 1687
public class GRUIStationEmployeeBadges : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600294B RID: 10571 RVA: 0x000DE2D1 File Offset: 0x000DC4D1
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000DE2DC File Offset: 0x000DC4DC
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.registeredBadges = new List<GRBadge>();
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].index = i;
			this.badgeDispensers[i].actorNr = -1;
		}
		this.dispenserForActorNr = new Dictionary<int, int>();
		VRRigCache.OnRigActivated += this.UpdateRigs;
		VRRigCache.OnRigDeactivated += this.UpdateRigs;
		RoomSystem.JoinedRoomEvent += new Action(this.UpdateRigs);
		this.UpdateRigs();
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000DE384 File Offset: 0x000DC584
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VRRigCache.OnRigActivated -= this.UpdateRigs;
		VRRigCache.OnRigDeactivated -= this.UpdateRigs;
		RoomSystem.JoinedRoomEvent -= new Action(this.UpdateRigs);
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000DE3D5 File Offset: 0x000DC5D5
	public void UpdateRigs(RigContainer container)
	{
		this.UpdateRigs();
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000DE3DD File Offset: 0x000DC5DD
	public void UpdateRigs()
	{
		GRUIStationEmployeeBadges.tempRigs.Clear();
		GRUIStationEmployeeBadges.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRUIStationEmployeeBadges.tempRigs);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000DE408 File Offset: 0x000DC608
	public void RefreshBadgesAuthority()
	{
		for (int i = 0; i < GRUIStationEmployeeBadges.tempRigs.Count; i++)
		{
			NetPlayer netPlayer = GRUIStationEmployeeBadges.tempRigs[i].isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : GRUIStationEmployeeBadges.tempRigs[i].OwningNetPlayer;
			int num;
			if (netPlayer != null && netPlayer.ActorNumber != -1 && !this.dispenserForActorNr.TryGetValue(netPlayer.ActorNumber, out num))
			{
				for (int j = 0; j < this.badgeDispensers.Count; j++)
				{
					if (this.badgeDispensers[j].actorNr == -1)
					{
						this.badgeDispensers[j].CreateBadge(netPlayer, this.reactor.grManager.gameEntityManager);
						break;
					}
				}
			}
		}
		for (int k = this.registeredBadges.Count - 1; k >= 0; k--)
		{
			int num2;
			if (NetworkSystem.Instance.GetNetPlayerByID(this.registeredBadges[k].actorNr) == null || !this.dispenserForActorNr.TryGetValue(this.registeredBadges[k].actorNr, out num2) || num2 != this.registeredBadges[k].dispenserIndex)
			{
				this.reactor.grManager.gameEntityManager.RequestDestroyItem(this.registeredBadges[k].GetComponent<GameEntity>().id);
			}
		}
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000DE574 File Offset: 0x000DC774
	public void SliceUpdate()
	{
		if (!this.reactor.grManager.IsZoneActive())
		{
			return;
		}
		if (this.reactor.grManager.gameEntityManager.IsAuthority())
		{
			this.RefreshBadgesAuthority();
		}
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Refresh();
		}
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000DE5D8 File Offset: 0x000DC7D8
	public void RemoveBadge(GRBadge badge)
	{
		if (this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Remove(badge);
		}
		if (this.badgeDispensers[badge.dispenserIndex].idBadge == badge)
		{
			this.dispenserForActorNr.Remove(badge.actorNr);
			this.badgeDispensers[badge.dispenserIndex].ClearBadge();
		}
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000DE648 File Offset: 0x000DC848
	public void LinkBadgeToDispenser(GRBadge badge, long createData)
	{
		if (!this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Add(badge);
		}
		int num = (int)(createData % 100L);
		if (num > this.badgeDispensers.Count)
		{
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID((int)(createData / 100L));
		if (netPlayerByID != null)
		{
			this.dispenserForActorNr[netPlayerByID.ActorNumber] = num;
			this.badgeDispensers[num].AttachIDBadge(badge, netPlayerByID);
		}
	}

	// Token: 0x04003550 RID: 13648
	[SerializeField]
	public List<GRUIEmployeeBadgeDispenser> badgeDispensers;

	// Token: 0x04003551 RID: 13649
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003552 RID: 13650
	public Dictionary<int, int> dispenserForActorNr;

	// Token: 0x04003553 RID: 13651
	public List<GRBadge> registeredBadges;

	// Token: 0x04003554 RID: 13652
	private GhostReactor reactor;
}
