using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class HandsActiveChecker : MonoBehaviour
{
	// Token: 0x06001458 RID: 5208 RVA: 0x0006D528 File Offset: 0x0006B728
	private void Awake()
	{
		this._notification = Object.Instantiate<GameObject>(this._notificationPrefab);
		base.StartCoroutine(this.GetCenterEye());
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0006D548 File Offset: 0x0006B748
	private void Update()
	{
		if (OVRPlugin.GetHandTrackingEnabled())
		{
			this._notification.SetActive(false);
			return;
		}
		this._notification.SetActive(true);
		if (this._centerEye)
		{
			this._notification.transform.position = this._centerEye.position + this._centerEye.forward * 0.5f;
			this._notification.transform.rotation = this._centerEye.rotation;
		}
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0006D5D2 File Offset: 0x0006B7D2
	private IEnumerator GetCenterEye()
	{
		if ((this._cameraRig = Object.FindObjectOfType<OVRCameraRig>()) != null)
		{
			while (!this._centerEye)
			{
				this._centerEye = this._cameraRig.centerEyeAnchor;
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x04001BE2 RID: 7138
	[SerializeField]
	private GameObject _notificationPrefab;

	// Token: 0x04001BE3 RID: 7139
	private GameObject _notification;

	// Token: 0x04001BE4 RID: 7140
	private OVRCameraRig _cameraRig;

	// Token: 0x04001BE5 RID: 7141
	private Transform _centerEye;
}
