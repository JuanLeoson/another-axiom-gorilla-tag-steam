using System;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class LocomotionController : MonoBehaviour
{
	// Token: 0x06001303 RID: 4867 RVA: 0x00068214 File Offset: 0x00066414
	private void Start()
	{
		if (this.CameraRig == null)
		{
			this.CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		}
	}

	// Token: 0x04001A9E RID: 6814
	public OVRCameraRig CameraRig;

	// Token: 0x04001A9F RID: 6815
	public CapsuleCollider CharacterController;

	// Token: 0x04001AA0 RID: 6816
	public SimpleCapsuleWithStickMovement PlayerController;
}
