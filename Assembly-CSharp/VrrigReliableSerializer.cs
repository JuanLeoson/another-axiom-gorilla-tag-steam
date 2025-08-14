using System;
using Fusion;
using UnityEngine;

// Token: 0x020006B1 RID: 1713
[NetworkBehaviourWeaved(0)]
internal class VrrigReliableSerializer : GorillaWrappedSerializer
{
	// Token: 0x06002A4E RID: 10830 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnBeforeDespawn()
	{
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x000E1FD8 File Offset: 0x000E01D8
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (wrappedInfo.punInfo.Sender != wrappedInfo.punInfo.photonView.Owner || wrappedInfo.punInfo.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(wrappedInfo.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x000DFDA5 File Offset: 0x000DDFA5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x000DFDB1 File Offset: 0x000DDFB1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
