using System;
using Cinemachine;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class MonitorOutputController : MonoBehaviour
{
	// Token: 0x06000F66 RID: 3942 RVA: 0x0005B09E File Offset: 0x0005929E
	private void Awake()
	{
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0005B0B1 File Offset: 0x000592B1
	private void OnEnable()
	{
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		LckBodyCameraSpawner.OnCameraStateChange += this.CameraStateChanged;
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0005B0DC File Offset: 0x000592DC
	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Object.Destroy(this);
		}
		if (this._shoulderCamera == null)
		{
			this.FindShoulderCamera();
		}
		if (this._lckCamera != null)
		{
			this._shoulderCamera.transform.position = this._lckCamera.transform.position;
			this._shoulderCamera.transform.rotation = this._lckCamera.transform.rotation;
			this._shoulderCamera.fieldOfView = this._lckCamera.fieldOfView;
			return;
		}
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0005B182 File Offset: 0x00059382
	private void CameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this.RestoreShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this.TakeOverShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this.TakeOverShoulderCamera();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0005B1AB File Offset: 0x000593AB
	private void OnDisable()
	{
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		LckBodyCameraSpawner.OnCameraStateChange -= this.CameraStateChanged;
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0005B1EB File Offset: 0x000593EB
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0005B200 File Offset: 0x00059400
	private void TakeOverShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = false;
		this._shoulderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LCKHide"));
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0005B240 File Offset: 0x00059440
	private void RestoreShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		this._shoulderCamera.cullingMask |= 1 << LayerMask.NameToLayer("LCKHide");
		this._shoulderCamera.fieldOfView = this._shoulderCameraFov;
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0005B29C File Offset: 0x0005949C
	private void FindShoulderCamera()
	{
		if (this._shoulderCamera != null)
		{
			return;
		}
		if (!GorillaTagger.hasInstance || !base.isActiveAndEnabled)
		{
			return;
		}
		this._shoulderCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		this._shoulderCameraFov = this._shoulderCamera.fieldOfView;
	}

	// Token: 0x0400181D RID: 6173
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x0400181E RID: 6174
	private Camera _lckCamera;

	// Token: 0x0400181F RID: 6175
	private CameraMode _lckActiveCameraMode;

	// Token: 0x04001820 RID: 6176
	private Camera _shoulderCamera;

	// Token: 0x04001821 RID: 6177
	private float _shoulderCameraFov;
}
