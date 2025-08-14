using System;
using UnityEngine;

// Token: 0x0200037E RID: 894
public class SelectivePassthroughExperience : MonoBehaviour
{
	// Token: 0x0600151B RID: 5403 RVA: 0x00072FE0 File Offset: 0x000711E0
	private void Update()
	{
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch || OVRInput.GetActiveController() == OVRInput.Controller.Touch;
		this.leftMaskObject.SetActive(flag);
		this.rightMaskObject.SetActive(flag);
		if (flag)
		{
			Vector3 position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward * 0.1f;
			Vector3 position2 = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward * 0.1f;
			this.leftMaskObject.transform.position = position;
			this.rightMaskObject.transform.position = position2;
			return;
		}
		if (OVRInput.GetActiveController() != OVRInput.Controller.LHand && OVRInput.GetActiveController() != OVRInput.Controller.RHand)
		{
			OVRInput.GetActiveController();
		}
	}

	// Token: 0x04001CB5 RID: 7349
	public GameObject leftMaskObject;

	// Token: 0x04001CB6 RID: 7350
	public GameObject rightMaskObject;
}
