using System;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class RequestCaptureFlow : MonoBehaviour
{
	// Token: 0x060015B0 RID: 5552 RVA: 0x000767A2 File Offset: 0x000749A2
	private void Start()
	{
		this._sceneManager = Object.FindObjectOfType<OVRSceneManager>();
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x000767AF File Offset: 0x000749AF
	private void Update()
	{
		if (OVRInput.GetUp(this.RequestCaptureBtn, OVRInput.Controller.Active))
		{
			this._sceneManager.RequestSceneCapture();
		}
	}

	// Token: 0x04001D74 RID: 7540
	public OVRInput.Button RequestCaptureBtn = OVRInput.Button.Two;

	// Token: 0x04001D75 RID: 7541
	private OVRSceneManager _sceneManager;
}
