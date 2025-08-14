using System;
using UnityEngine;

// Token: 0x02000373 RID: 883
public class OverlayPassthrough : MonoBehaviour
{
	// Token: 0x060014DF RID: 5343 RVA: 0x000721A0 File Offset: 0x000703A0
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000721F0 File Offset: 0x000703F0
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
		{
			this.passthroughLayer.hidden = !this.passthroughLayer.hidden;
		}
		float x = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;
		this.passthroughLayer.textureOpacity = x * 0.5f + 0.5f;
	}

	// Token: 0x04001C79 RID: 7289
	private OVRPassthroughLayer passthroughLayer;
}
