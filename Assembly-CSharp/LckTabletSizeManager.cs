using System;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class LckTabletSizeManager : MonoBehaviour
{
	// Token: 0x06000F3F RID: 3903 RVA: 0x0005A6E5 File Offset: 0x000588E5
	private void Start()
	{
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Combine(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
		this._controller.OnHorizontalModeChanged += this.OnHorizontalModeChanged;
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x0005A725 File Offset: 0x00058925
	private void OnDestroy()
	{
		this._controller.OnHorizontalModeChanged -= this.OnHorizontalModeChanged;
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Remove(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0005A765 File Offset: 0x00058965
	private void OnHorizontalModeChanged(bool mode)
	{
		this.UpdateCustomNearClip(CameraMode.Selfie);
		this.UpdateCustomNearClip(CameraMode.FirstPerson);
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0005A775 File Offset: 0x00058975
	private void UpdateCustomNearClip(CameraMode mode)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		switch (mode)
		{
		case CameraMode.Selfie:
			this.SetCustomNearClip(this._selfieCamera);
			return;
		case CameraMode.FirstPerson:
			this.SetCustomNearClip(this._firstPersonCamera);
			break;
		case CameraMode.ThirdPerson:
		case CameraMode.Drone:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0005A7B4 File Offset: 0x000589B4
	private void SetCustomNearClip(Camera cam)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		Matrix4x4 projectionMatrix;
		if (this._controller.HorizontalMode)
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 1.777778f, this._customNearClip, cam.farClipPlane);
		}
		else
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 0.5625f, this._customNearClip, cam.farClipPlane);
		}
		cam.projectionMatrix = projectionMatrix;
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0005A81E File Offset: 0x00058A1E
	private void ClearCustomNearClip()
	{
		this._selfieCamera.ResetProjectionMatrix();
		this._firstPersonCamera.ResetProjectionMatrix();
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0005A838 File Offset: 0x00058A38
	private void PlayerBecameSmall()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamShrinkPosition;
		this._tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.SetCustomNearClip(this._selfieCamera);
		this.SetCustomNearClip(this._firstPersonCamera);
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0005A898 File Offset: 0x00058A98
	private void PlayerBecameDefaultSize()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamDefaultPosition;
		this._tabletFollower.SetPlayerSizeModifier(true, 1f);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.ClearCustomNearClip();
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0005A8E8 File Offset: 0x00058AE8
	private void SetCameraOnNeck()
	{
		GameObject gameObject = Camera.main.transform.Find("LCKBodyCameraSpawner(Clone)").gameObject;
		if (gameObject != null)
		{
			gameObject.GetComponent<LckBodyCameraSpawner>().ManuallySetCameraOnNeck();
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0005A924 File Offset: 0x00058B24
	private void Update()
	{
		if (!GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = false;
			this.PlayerBecameSmall();
			return;
		}
		if (GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = true;
			this.PlayerBecameDefaultSize();
		}
	}

	// Token: 0x040017FF RID: 6143
	[SerializeField]
	private GTLckController _controller;

	// Token: 0x04001800 RID: 6144
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x04001801 RID: 6145
	[SerializeField]
	private GtTabletFollower _tabletFollower;

	// Token: 0x04001802 RID: 6146
	[SerializeField]
	private Camera _firstPersonCamera;

	// Token: 0x04001803 RID: 6147
	[SerializeField]
	private Camera _selfieCamera;

	// Token: 0x04001804 RID: 6148
	private Vector3 _firstPersonCamShrinkPosition = new Vector3(0f, 0f, -0.78f);

	// Token: 0x04001805 RID: 6149
	private Vector3 _firstPersonCamDefaultPosition = Vector3.zero;

	// Token: 0x04001806 RID: 6150
	private float _shrinkSize = 0.06f;

	// Token: 0x04001807 RID: 6151
	private Vector3 _shrinkVector = new Vector3(0.06f, 0.06f, 0.06f);

	// Token: 0x04001808 RID: 6152
	private float _customNearClip = 0.0006f;

	// Token: 0x04001809 RID: 6153
	private bool _isDefaultScale = true;
}
