using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000289 RID: 649
public class LckBodyCameraSpawner : MonoBehaviour
{
	// Token: 0x06000ED9 RID: 3801 RVA: 0x0005919D File Offset: 0x0005739D
	public void SetFollowTransform(Transform transform)
	{
		this._followTransform = transform;
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000EDA RID: 3802 RVA: 0x000591A6 File Offset: 0x000573A6
	public TabletSpawnInstance tabletSpawnInstance
	{
		get
		{
			return this._tabletSpawnInstance;
		}
	}

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000EDB RID: 3803 RVA: 0x000591B0 File Offset: 0x000573B0
	// (remove) Token: 0x06000EDC RID: 3804 RVA: 0x000591E4 File Offset: 0x000573E4
	public static event LckBodyCameraSpawner.CameraStateDelegate OnCameraStateChange;

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x00059217 File Offset: 0x00057417
	// (set) Token: 0x06000EDE RID: 3806 RVA: 0x00059220 File Offset: 0x00057420
	public LckBodyCameraSpawner.CameraState cameraState
	{
		get
		{
			return this._cameraState;
		}
		set
		{
			switch (value)
			{
			case LckBodyCameraSpawner.CameraState.CameraDisabled:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.NotVisible;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = false;
				this.ResetCameraModel();
				this.cameraVisible = false;
				this._shouldMoveCameraToNeck = false;
				break;
			case LckBodyCameraSpawner.CameraState.CameraOnNeck:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = true;
				this.ResetCameraModel();
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(false);
				}
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetTabletIsSpawned(false);
				break;
			case LckBodyCameraSpawner.CameraState.CameraSpawned:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = true;
				this._tabletSpawnInstance.cameraActive = true;
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(true);
				}
				this.ResetCameraModel();
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetTabletIsSpawned(true);
				break;
			}
			this._cameraState = value;
			LckBodyCameraSpawner.CameraStateDelegate onCameraStateChange = LckBodyCameraSpawner.OnCameraStateChange;
			if (onCameraStateChange == null)
			{
				return;
			}
			onCameraStateChange(this._cameraState);
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00059338 File Offset: 0x00057538
	private void SetPreviewActive(bool isActive)
	{
		LckResult<LckService> service = LckService.GetService();
		if (!service.Success)
		{
			Debug.LogError("LCK Could not get Service" + service.Error.ToString());
			return;
		}
		LckService result = service.Result;
		if (result == null)
		{
			return;
		}
		result.SetPreviewActive(isActive);
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x00059389 File Offset: 0x00057589
	// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x00059394 File Offset: 0x00057594
	public LckBodyCameraSpawner.CameraPosition cameraPosition
	{
		get
		{
			return this._cameraPosition;
		}
		set
		{
			if (this._cameraModelTransform != null && this._cameraPosition != value)
			{
				switch (value)
				{
				case LckBodyCameraSpawner.CameraPosition.CameraDefault:
					this.ChangeCameraModelParent(this._cameraPositionDefault);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
					return;
				case LckBodyCameraSpawner.CameraPosition.CameraSlingshot:
					this.ChangeCameraModelParent(this._cameraPositionSlingshot);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
					break;
				case LckBodyCameraSpawner.CameraPosition.NotVisible:
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x000593F2 File Offset: 0x000575F2
	// (set) Token: 0x06000EE3 RID: 3811 RVA: 0x00059404 File Offset: 0x00057604
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.enabled = value;
		}
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00059423 File Offset: 0x00057623
	private void Awake()
	{
		this._tabletSpawnInstance = new TabletSpawnInstance(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0005943C File Offset: 0x0005763C
	private void OnEnable()
	{
		this.InitCameraStrap();
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
		if (this._tabletSpawnInstance.Controller != null)
		{
			this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
		}
		ZoneManagement.OnZoneChange += this.OnZoneChanged;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x00059498 File Offset: 0x00057698
	private void Update()
	{
		if (this._followTransform != null && base.transform.parent != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.parent.localToWorldMatrix;
			Vector3 position = localToWorldMatrix.MultiplyPoint(this._followTransform.localPosition + this._followTransform.localRotation * new Vector3(0f, -0.05f, 0.1f));
			Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.forward), localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.up));
			base.transform.SetPositionAndRotation(position, rotation);
		}
		LckBodyCameraSpawner.CameraState cameraState = this._cameraState;
		if (cameraState != LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			if (cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned)
			{
				this.UpdateCameraStrap();
				if (this._cameraModelGrabbable.isGrabbed)
				{
					GorillaGrabber grabber = this._cameraModelGrabbable.grabber;
					Transform transform = grabber.transform;
					if (this.ShouldSpawnCamera(transform))
					{
						this.SpawnCamera(grabber, transform);
					}
				}
				else
				{
					this.ResetCameraModel();
				}
				if (this._tabletSpawnInstance.isSpawned)
				{
					Transform transform3;
					if (this._tabletSpawnInstance.directGrabbable.isGrabbed)
					{
						GorillaGrabber grabber2 = this._tabletSpawnInstance.directGrabbable.grabber;
						Transform transform2 = grabber2.transform;
						if (!this.ShouldSpawnCamera(transform2))
						{
							this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
							this._cameraModelGrabbable.target.SetPositionAndRotation(transform2.position, transform2.rotation * Quaternion.Euler(0f, 180f, 0f));
							this._tabletSpawnInstance.directGrabbable.ForceRelease();
							this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
							this._tabletSpawnInstance.ResetLocalPose();
							this._cameraModelGrabbable.ForceGrab(grabber2);
							this._cameraModelGrabbable.onReleased += this.OnCameraModelReleased;
							this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
							if (this._previousMode == CameraMode.Selfie)
							{
								this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
							}
						}
					}
					else if (this._shouldMoveCameraToNeck && GtTag.TryGetTransform(GtTagType.HMD, out transform3) && Vector3.SqrMagnitude(transform3.position - this.tabletSpawnInstance.position) >= this._snapToNeckDistance * this._snapToNeckDistance)
					{
						this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
						this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
						this._tabletSpawnInstance.ResetLocalPose();
						this._shouldMoveCameraToNeck = false;
					}
				}
			}
		}
		else
		{
			this.UpdateCameraStrap();
			if (this._cameraModelGrabbable.isGrabbed)
			{
				GorillaGrabber grabber3 = this._cameraModelGrabbable.grabber;
				Transform transform4 = grabber3.transform;
				if (this.ShouldSpawnCamera(transform4))
				{
					this.SpawnCamera(grabber3, transform4);
				}
			}
			else
			{
				this.ResetCameraModel();
			}
		}
		if (!this.IsSlingshotActiveInHierarchy())
		{
			this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
			return;
		}
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0005979B File Offset: 0x0005799B
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.OnZoneChanged;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x000597AE File Offset: 0x000579AE
	private void OnDestroy()
	{
		this._tabletSpawnInstance.Dispose();
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000597BC File Offset: 0x000579BC
	public void ManuallySetCameraOnNeck()
	{
		if (this.cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			return;
		}
		if (this._tabletSpawnInstance.isSpawned)
		{
			this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
			this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
			this._tabletSpawnInstance.ResetLocalPose();
			this._shouldMoveCameraToNeck = false;
			this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
			if (this._previousMode == CameraMode.Selfie)
			{
				this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
			}
		}
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0005983B File Offset: 0x00057A3B
	private void OnZoneChanged(ZoneData[] zones)
	{
		if (!this._tabletSpawnInstance.isSpawned || this._tabletSpawnInstance.directGrabbable.isGrabbed)
		{
			return;
		}
		this.ManuallySetCameraOnNeck();
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x00059863 File Offset: 0x00057A63
	private void OnCameraModelReleased()
	{
		this._cameraModelGrabbable.onReleased -= this.OnCameraModelReleased;
		this.ResetCameraModel();
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x00059884 File Offset: 0x00057A84
	public void SpawnCamera(GorillaGrabber overrideGorillaGrabber, Transform transform)
	{
		if (!this._tabletSpawnInstance.isSpawned)
		{
			this._tabletSpawnInstance.SpawnCamera();
		}
		Vector3 zero = Vector3.zero;
		XRNode xrNode = overrideGorillaGrabber.XrNode;
		if (xrNode != XRNode.LeftHand)
		{
			if (xrNode == XRNode.RightHand)
			{
				zero = new Vector3(0.25f, -0.12f, 0.03f);
			}
		}
		else
		{
			zero = new Vector3(-0.25f, -0.12f, 0.03f);
		}
		if (this._previousMode == CameraMode.Selfie)
		{
			this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.Selfie);
			this._previousMode = CameraMode.Selfie;
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraSpawned;
		this._cameraModelGrabbable.ForceRelease();
		this._tabletSpawnInstance.ResetParent();
		Matrix4x4 lhs = transform.localToWorldMatrix;
		Quaternion rotation = transform.rotation * Quaternion.Euler(-55f, 180f, 0f);
		lhs *= Matrix4x4.Rotate(Quaternion.Euler(-55f, 180f, 0f));
		this._tabletSpawnInstance.SetPositionAndRotation(lhs.MultiplyPoint(zero), rotation);
		this._tabletSpawnInstance.directGrabbable.ForceGrab(overrideGorillaGrabber);
		this._tabletSpawnInstance.SetLocalScale(Vector3.one);
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x000599A8 File Offset: 0x00057BA8
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 a = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 b = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(a - b) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x00059A00 File Offset: 0x00057C00
	private void ChangeCameraModelParent(Transform transform)
	{
		if (this._cameraModelTransform != null)
		{
			this._cameraModelGrabbable.SetOriginalTargetParent(transform);
			if (!this._cameraModelGrabbable.isGrabbed)
			{
				this._cameraModelTransform.transform.parent = transform;
				this._cameraModelTransform.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x00059A5A File Offset: 0x00057C5A
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00059A84 File Offset: 0x00057C84
	private void UpdateCameraStrap()
	{
		for (int i = 0; i < this._cameraStrapPoints.Length; i++)
		{
			this._cameraStrapPositions[i] = this._cameraStrapPoints[i].position;
		}
		this._cameraStrapRenderer.SetPositions(this._cameraStrapPositions);
		Vector3 lossyScale = base.transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.3333333f;
		this._cameraStrapRenderer.widthMultiplier = num * 0.02f;
		Color color = (this.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned) ? this._ghostColor : this._normalColor;
		this._cameraStrapRenderer.startColor = color;
		this._cameraStrapRenderer.endColor = color;
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00059B37 File Offset: 0x00057D37
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x00059B59 File Offset: 0x00057D59
	private VRRig GetLocalRig()
	{
		if (this._localRig == null)
		{
			this._localRig = VRRigCache.Instance.localRig.Rig;
		}
		return this._localRig;
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00059B84 File Offset: 0x00057D84
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig localRig = this.GetLocalRig();
		if (localRig == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = localRig.projectileWeapon.InLeftHand();
		rightHand = localRig.projectileWeapon.InRightHand();
		return localRig.projectileWeapon.InHand();
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x00059BD0 File Offset: 0x00057DD0
	private bool IsSlingshotActiveInHierarchy()
	{
		VRRig localRig = this.GetLocalRig();
		return !(localRig == null) && !(localRig.projectileWeapon == null) && localRig.projectileWeapon.gameObject.activeInHierarchy;
	}

	// Token: 0x040017B6 RID: 6070
	[SerializeField]
	private GameObject _cameraSpawnPrefab;

	// Token: 0x040017B7 RID: 6071
	[SerializeField]
	private Transform _cameraSpawnParentTransform;

	// Token: 0x040017B8 RID: 6072
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x040017B9 RID: 6073
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x040017BA RID: 6074
	[SerializeField]
	private LckDirectGrabbable _cameraModelGrabbable;

	// Token: 0x040017BB RID: 6075
	[SerializeField]
	private Transform _cameraPositionDefault;

	// Token: 0x040017BC RID: 6076
	[SerializeField]
	private Transform _cameraPositionSlingshot;

	// Token: 0x040017BD RID: 6077
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x040017BE RID: 6078
	[SerializeField]
	private float _snapToNeckDistance = 15f;

	// Token: 0x040017BF RID: 6079
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x040017C0 RID: 6080
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x040017C1 RID: 6081
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x040017C2 RID: 6082
	[SerializeField]
	private Color _ghostColor = Color.gray;

	// Token: 0x040017C3 RID: 6083
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x040017C4 RID: 6084
	private Transform _followTransform;

	// Token: 0x040017C5 RID: 6085
	private Vector3[] _cameraStrapPositions;

	// Token: 0x040017C6 RID: 6086
	private TabletSpawnInstance _tabletSpawnInstance;

	// Token: 0x040017C7 RID: 6087
	private VRRig _localRig;

	// Token: 0x040017C8 RID: 6088
	private bool _shouldMoveCameraToNeck;

	// Token: 0x040017C9 RID: 6089
	private CameraMode _previousMode;

	// Token: 0x040017CB RID: 6091
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x040017CC RID: 6092
	private LckBodyCameraSpawner.CameraPosition _cameraPosition;

	// Token: 0x0200028A RID: 650
	public enum CameraState
	{
		// Token: 0x040017CE RID: 6094
		CameraDisabled,
		// Token: 0x040017CF RID: 6095
		CameraOnNeck,
		// Token: 0x040017D0 RID: 6096
		CameraSpawned
	}

	// Token: 0x0200028B RID: 651
	public enum CameraPosition
	{
		// Token: 0x040017D2 RID: 6098
		CameraDefault,
		// Token: 0x040017D3 RID: 6099
		CameraSlingshot,
		// Token: 0x040017D4 RID: 6100
		NotVisible
	}

	// Token: 0x0200028C RID: 652
	// (Invoke) Token: 0x06000EF7 RID: 3831
	public delegate void CameraStateDelegate(LckBodyCameraSpawner.CameraState state);
}
