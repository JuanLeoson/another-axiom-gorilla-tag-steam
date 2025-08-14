using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class LckSocialCameraManager : MonoBehaviour
{
	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000F2E RID: 3886 RVA: 0x0005A422 File Offset: 0x00058622
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0005A42A File Offset: 0x0005862A
	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0005A431 File Offset: 0x00058631
	public void SetForceHidden(bool hidden)
	{
		this._forceHidden = hidden;
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0005A43A File Offset: 0x0005863A
	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0005A453 File Offset: 0x00058653
	public void SetLckSocialCamera(LckSocialCamera socialCamera)
	{
		this._socialCameraInstance = socialCamera;
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0005A45C File Offset: 0x0005865C
	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned(this);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0005A474 File Offset: 0x00058674
	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += this.OnRecordingStarted;
			service.Result.OnRecordingStopped += this.OnRecordingStopped;
		}
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0005A4D4 File Offset: 0x000586D4
	private void Update()
	{
		if (this._socialCameraInstance != null)
		{
			if (this._lckCamera != null)
			{
				Transform transform = this._lckCamera.transform;
				this._socialCameraInstance.transform.position = transform.position;
				this._socialCameraInstance.transform.rotation = transform.rotation;
				Camera main = Camera.main;
				if (main != null)
				{
					this._lckCamera.nearClipPlane = main.nearClipPlane;
					this._lckCamera.farClipPlane = main.farClipPlane;
				}
			}
			CameraMode lckActiveCameraMode = this._lckActiveCameraMode;
			if (lckActiveCameraMode == CameraMode.Selfie || lckActiveCameraMode - CameraMode.ThirdPerson <= 1)
			{
				this._socialCameraInstance.visible = (!this._forceHidden && this.cameraActive);
			}
			else
			{
				this._socialCameraInstance.visible = false;
			}
			this._socialCameraInstance.recording = this._recording;
		}
		if (this.CoconutCamera.gameObject.activeSelf)
		{
			CameraMode lckActiveCameraMode = this._lckActiveCameraMode;
			if (lckActiveCameraMode != CameraMode.Selfie)
			{
				if (lckActiveCameraMode - CameraMode.ThirdPerson <= 1)
				{
					this.CoconutCamera.SetVisualsActive(this.cameraActive);
				}
				else
				{
					this.CoconutCamera.SetVisualsActive(false);
				}
			}
			else
			{
				this.CoconutCamera.SetVisualsActive(false);
			}
			this.CoconutCamera.SetRecordingState(this._recording);
		}
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0005A614 File Offset: 0x00058814
	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= this.OnRecordingStarted;
			service.Result.OnRecordingStopped -= this.OnRecordingStopped;
		}
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000F37 RID: 3895 RVA: 0x0005A674 File Offset: 0x00058874
	// (set) Token: 0x06000F38 RID: 3896 RVA: 0x0005A681 File Offset: 0x00058881
	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			this._localCameras.SetActive(value);
			if (!value)
			{
				this._gtLckController.StopRecording();
			}
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000F39 RID: 3897 RVA: 0x0005A69E File Offset: 0x0005889E
	// (set) Token: 0x06000F3A RID: 3898 RVA: 0x0005A6AB File Offset: 0x000588AB
	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0005A6B9 File Offset: 0x000588B9
	private void OnRecordingStarted(LckResult result)
	{
		this._recording = result.Success;
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0005A6C7 File Offset: 0x000588C7
	private void OnRecordingStopped(LckResult result)
	{
		this._recording = false;
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0005A6D0 File Offset: 0x000588D0
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x040017F3 RID: 6131
	[SerializeField]
	private GameObject _localUi;

	// Token: 0x040017F4 RID: 6132
	[SerializeField]
	private GameObject _localCameras;

	// Token: 0x040017F5 RID: 6133
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x040017F6 RID: 6134
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x040017F7 RID: 6135
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x040017F8 RID: 6136
	private LckSocialCamera _socialCameraInstance;

	// Token: 0x040017F9 RID: 6137
	private Camera _lckCamera;

	// Token: 0x040017FA RID: 6138
	private CameraMode _lckActiveCameraMode;

	// Token: 0x040017FB RID: 6139
	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	// Token: 0x040017FC RID: 6140
	public static Action<LckSocialCameraManager> OnManagerSpawned;

	// Token: 0x040017FD RID: 6141
	private bool _recording;

	// Token: 0x040017FE RID: 6142
	private bool _forceHidden;
}
