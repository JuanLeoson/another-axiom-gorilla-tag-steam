using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000317 RID: 791
public class HandedInputSelector : MonoBehaviour
{
	// Token: 0x060012EB RID: 4843 RVA: 0x00067A0A File Offset: 0x00065C0A
	private void Start()
	{
		this.m_CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		this.m_InputModule = Object.FindObjectOfType<OVRInputModule>();
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00067A22 File Offset: 0x00065C22
	private void Update()
	{
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.SetActiveController(OVRInput.Controller.LTouch);
			return;
		}
		this.SetActiveController(OVRInput.Controller.RTouch);
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00067A3C File Offset: 0x00065C3C
	private void SetActiveController(OVRInput.Controller c)
	{
		Transform rayTransform;
		if (c == OVRInput.Controller.LTouch)
		{
			rayTransform = this.m_CameraRig.leftHandAnchor;
		}
		else
		{
			rayTransform = this.m_CameraRig.rightHandAnchor;
		}
		this.m_InputModule.rayTransform = rayTransform;
	}

	// Token: 0x04001A85 RID: 6789
	private OVRCameraRig m_CameraRig;

	// Token: 0x04001A86 RID: 6790
	private OVRInputModule m_InputModule;
}
