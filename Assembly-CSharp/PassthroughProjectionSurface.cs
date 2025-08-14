using System;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class PassthroughProjectionSurface : MonoBehaviour
{
	// Token: 0x060014EC RID: 5356 RVA: 0x0007261C File Offset: 0x0007081C
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
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
		this.quadOutline = this.projectionObject.GetComponent<MeshRenderer>();
		this.quadOutline.enabled = false;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000726A0 File Offset: 0x000708A0
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
			this.quadOutline.enabled = true;
		}
		if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			OVRInput.Controller controllerType = OVRInput.Controller.RTouch;
			base.transform.position = OVRInput.GetLocalControllerPosition(controllerType);
			base.transform.rotation = OVRInput.GetLocalControllerRotation(controllerType);
		}
		if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
			this.quadOutline.enabled = false;
		}
	}

	// Token: 0x04001C88 RID: 7304
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x04001C89 RID: 7305
	public MeshFilter projectionObject;

	// Token: 0x04001C8A RID: 7306
	private MeshRenderer quadOutline;
}
