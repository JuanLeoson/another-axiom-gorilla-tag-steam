using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class CrittersRigActorSetup : MonoBehaviour
{
	// Token: 0x060002A3 RID: 675 RVA: 0x00010737 File Offset: 0x0000E937
	public void OnEnable()
	{
		CrittersManager.RegisterRigActorSetup(this);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00010740 File Offset: 0x0000E940
	public void OnDisable()
	{
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			this.rigActors[i].actorSet = null;
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00010774 File Offset: 0x0000E974
	private CrittersActor RefreshActorForIndex(int index)
	{
		CrittersRigActorSetup.RigActor rigActor = this.rigActors[index];
		if (rigActor.actorSet.IsNotNull())
		{
			rigActor.actorSet.gameObject.SetActive(false);
		}
		CrittersActor crittersActor = CrittersManager.instance.SpawnActor(rigActor.type, rigActor.subIndex);
		if (crittersActor.IsNull())
		{
			return null;
		}
		crittersActor.isOnPlayer = true;
		crittersActor.rigIndex = index;
		crittersActor.rigPlayerId = this.myRig.Creator.ActorNumber;
		if (crittersActor.rigPlayerId == -1 && PhotonNetwork.InRoom)
		{
			crittersActor.rigPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
		}
		crittersActor.PlacePlayerCrittersActor();
		return crittersActor;
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001081C File Offset: 0x0000EA1C
	public void CheckUpdate(ref List<object> refActorData, bool forceCheck = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			CrittersRigActorSetup.RigActor rigActor = this.rigActors[i];
			RigContainer rigContainer;
			if (forceCheck || rigActor.actorSet == null || (rigActor.actorSet.rigPlayerId != this.myRig.Creator.ActorNumber && VRRigCache.Instance.TryGetVrrig(this.myRig.Creator, out rigContainer) && CrittersManager.instance.rigSetupByRig.ContainsKey(this.myRig)))
			{
				CrittersActor crittersActor = this.RefreshActorForIndex(i);
				if (crittersActor != null)
				{
					crittersActor.AddPlayerCrittersActorDataToList(ref refActorData);
				}
			}
		}
	}

	// Token: 0x0400031A RID: 794
	public CrittersRigActorSetup.RigActor[] rigActors;

	// Token: 0x0400031B RID: 795
	public List<object> rigActorData = new List<object>();

	// Token: 0x0400031C RID: 796
	public VRRig myRig;

	// Token: 0x0200006C RID: 108
	[Serializable]
	public struct RigActor
	{
		// Token: 0x0400031D RID: 797
		public Transform location;

		// Token: 0x0400031E RID: 798
		public CrittersActor.CrittersActorType type;

		// Token: 0x0400031F RID: 799
		public int subIndex;

		// Token: 0x04000320 RID: 800
		public CrittersActor actorSet;
	}
}
