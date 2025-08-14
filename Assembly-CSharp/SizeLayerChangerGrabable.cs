using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000777 RID: 1911
public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x06002FEA RID: 12266 RVA: 0x000FC11A File Offset: 0x000FA31A
	public bool MomentaryGrabOnly()
	{
		return this.momentaryGrabOnly;
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x0001D558 File Offset: 0x0001B758
	bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x000FC124 File Offset: 0x000FA324
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosiiton)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		grabbedObject = base.transform;
		grabbedLocalPosiiton = base.transform.InverseTransformPoint(g.transform.position);
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x000FC18C File Offset: 0x000FA38C
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x000139A7 File Offset: 0x00011BA7
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04003BFC RID: 15356
	[SerializeField]
	private bool grabChangesSizeLayer = true;

	// Token: 0x04003BFD RID: 15357
	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	// Token: 0x04003BFE RID: 15358
	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	// Token: 0x04003BFF RID: 15359
	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;

	// Token: 0x04003C00 RID: 15360
	[SerializeField]
	private bool momentaryGrabOnly = true;
}
