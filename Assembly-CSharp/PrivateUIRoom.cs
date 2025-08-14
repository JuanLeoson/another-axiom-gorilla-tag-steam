using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

// Token: 0x020003C8 RID: 968
public class PrivateUIRoom : MonoBehaviour
{
	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001664 RID: 5732 RVA: 0x000790C9 File Offset: 0x000772C9
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x00079C08 File Offset: 0x00077E08
	private void Awake()
	{
		if (PrivateUIRoom.instance == null)
		{
			PrivateUIRoom.instance = this;
			this.occluder.SetActive(false);
			this.leftHandObject.SetActive(false);
			this.rightHandObject.SetActive(false);
			this.ui = new List<Transform>();
			this.uiParents = new Dictionary<Transform, Transform>();
			this.backgroundDirectionPropertyID = Shader.PropertyToID(this.backgroundDirectionPropertyName);
			this._uiRoot = new GameObject("UIRoot").transform;
			this._uiRoot.parent = base.transform;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x00079CA0 File Offset: 0x00077EA0
	private void OnEnable()
	{
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Listen(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x00079CBD File Offset: 0x00077EBD
	private void OnDisable()
	{
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Remove(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x00079CDC File Offset: 0x00077EDC
	private static bool FindShoulderCamera()
	{
		if (PrivateUIRoom._shoulderCameraReference.IsNotNull())
		{
			return true;
		}
		if (GorillaTagger.Instance.IsNull())
		{
			return false;
		}
		PrivateUIRoom._shoulderCameraReference = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		if (PrivateUIRoom._shoulderCameraReference == null)
		{
			Debug.LogError("[PRIVATE_UI_ROOMS] Could not find Shoulder Camera");
			return false;
		}
		return true;
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x00079D34 File Offset: 0x00077F34
	private void ToggleHands(VREvent_t ev)
	{
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] Toggling hands visibility. Event: {0} ({1})", ev.eventType, (EVREventType)ev.eventType));
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] _handsShowing: {0}", PrivateUIRoom.instance.rightHandObject.activeSelf));
		if (PrivateUIRoom.instance.rightHandObject.activeSelf)
		{
			this.HideHands();
			return;
		}
		this.ShowHands();
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x00079DA7 File Offset: 0x00077FA7
	private void HideHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu shown, disabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x00079DD3 File Offset: 0x00077FD3
	private void ShowHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu hidden, re-enabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x00079E00 File Offset: 0x00078000
	private void ToggleLevelVisibility(bool levelShouldBeVisible)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (levelShouldBeVisible)
		{
			component.cullingMask = this.savedCullingLayers;
			if (this.savedCullingLayersShoudlerCam != null)
			{
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.savedCullingLayersShoudlerCam.Value;
				this.savedCullingLayersShoudlerCam = null;
				return;
			}
		}
		else
		{
			this.savedCullingLayers = component.cullingMask;
			component.cullingMask = this.visibleLayers;
			if (PrivateUIRoom.FindShoulderCamera())
			{
				this.savedCullingLayersShoudlerCam = new int?(PrivateUIRoom._shoulderCameraReference.cullingMask);
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.visibleLayers;
			}
		}
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x00079EAC File Offset: 0x000780AC
	private static void StopOverlay()
	{
		PrivateUIRoom.instance.localPlayer.inOverlay = false;
		PrivateUIRoom.instance.inOverlay = false;
		PrivateUIRoom.instance.localPlayer.disableMovement = false;
		PrivateUIRoom.instance.localPlayer.InReportMenu = false;
		PrivateUIRoom.instance.ToggleLevelVisibility(true);
		PrivateUIRoom.instance.occluder.SetActive(false);
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
		AudioListener.volume = PrivateUIRoom.instance._initialAudioVolume;
		Debug.Log("[PrivateUIRoom::StopOverlay] Re-enabling Audio Listener");
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x00079F48 File Offset: 0x00078148
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * Vector3.zero * scale.x;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x00079FD4 File Offset: 0x000781D4
	private static void AssignShoulderCameraToCanvases(Transform focus)
	{
		Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] setting up canvases with shoulder camera.");
		if (!PrivateUIRoom.FindShoulderCamera())
		{
			return;
		}
		Canvas componentInChildren = focus.GetComponentInChildren<Canvas>(true);
		if (componentInChildren != null)
		{
			componentInChildren.worldCamera = PrivateUIRoom._shoulderCameraReference;
			Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] Assigned shoulder camera to Canvas: " + componentInChildren.name);
			return;
		}
		Debug.LogError("[KID::PrivateUIRoom::CanvasCameraAssigner] No Canvas component found on this GameObject.");
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x0007A030 File Offset: 0x00078230
	public static void AddUI(Transform focus)
	{
		if (PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		PrivateUIRoom.AssignShoulderCameraToCanvases(focus);
		PrivateUIRoom.instance.uiParents.Add(focus, focus.parent);
		focus.gameObject.SetActive(false);
		focus.parent = PrivateUIRoom.instance._uiRoot;
		focus.localPosition = Vector3.zero;
		focus.localRotation = Quaternion.identity;
		PrivateUIRoom.instance.ui.Add(focus);
		if (PrivateUIRoom.instance.ui.Count == 1 && PrivateUIRoom.instance.focusTransform == null)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			if (!PrivateUIRoom.instance.inOverlay)
			{
				PrivateUIRoom.StartOverlay();
			}
		}
		PrivateUIRoom.instance.UpdateUIPositionAndRotation();
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x0007A11C File Offset: 0x0007831C
	public static void RemoveUI(Transform focus)
	{
		if (!PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		focus.gameObject.SetActive(false);
		PrivateUIRoom.instance.ui.Remove(focus);
		if (PrivateUIRoom.instance.focusTransform == focus)
		{
			PrivateUIRoom.instance.focusTransform = null;
		}
		if (PrivateUIRoom.instance.uiParents[focus] != null)
		{
			focus.parent = PrivateUIRoom.instance.uiParents[focus];
			PrivateUIRoom.instance.uiParents.Remove(focus);
		}
		else
		{
			Object.Destroy(focus.gameObject);
		}
		if (PrivateUIRoom.instance.ui.Count > 0)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			return;
		}
		if (!PrivateUIRoom.instance.overlayForcedActive)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x0007A215 File Offset: 0x00078415
	public static void ForceStartOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = true;
		if (PrivateUIRoom.instance.inOverlay)
		{
			return;
		}
		PrivateUIRoom.StartOverlay();
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x0007A242 File Offset: 0x00078442
	public static void StopForcedOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = false;
		if (PrivateUIRoom.instance.ui.Count == 0 && PrivateUIRoom.instance.inOverlay)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x0007A280 File Offset: 0x00078480
	private static void StartOverlay()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 localScale;
		PrivateUIRoom.instance.GetIdealScreenPositionRotation(out vector, out quaternion, out localScale);
		PrivateUIRoom.instance.leftHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.rightHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.occluder.transform.localScale = localScale;
		PrivateUIRoom.instance.localPlayer.InReportMenu = true;
		PrivateUIRoom.instance.localPlayer.disableMovement = true;
		PrivateUIRoom.instance.occluder.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.ToggleLevelVisibility(false);
		PrivateUIRoom.instance.localPlayer.inOverlay = true;
		PrivateUIRoom.instance.inOverlay = true;
		PrivateUIRoom.instance._initialAudioVolume = AudioListener.volume;
		AudioListener.volume = 0f;
		Debug.Log("[PrivateUIRoom::StartOverlay] Muting Audio Listener");
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x0007A378 File Offset: 0x00078578
	private void Update()
	{
		if (!this.localPlayer.InReportMenu)
		{
			return;
		}
		this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
		this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
		if (this.ShouldUpdateRotation())
		{
			this.UpdateUIPositionAndRotation();
			return;
		}
		if (this.ShouldUpdatePosition())
		{
			this.UpdateUIPosition();
		}
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x0007A434 File Offset: 0x00078634
	private bool ShouldUpdateRotation()
	{
		float magnitude = (GorillaTagger.Instance.mainCamera.transform.position - this.lastStablePosition).X_Z().magnitude;
		Quaternion b = Quaternion.Euler(0f, GorillaTagger.Instance.mainCamera.transform.rotation.eulerAngles.y, 0f);
		float num = Quaternion.Angle(this.lastStableRotation, b);
		return magnitude > this.lateralPlay || num >= this.rotationalPlay;
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x0007A4C1 File Offset: 0x000786C1
	private bool ShouldUpdatePosition()
	{
		return Mathf.Abs(GorillaTagger.Instance.mainCamera.transform.position.y - this.lastStablePosition.y) > this.verticalPlay;
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x0007A4F8 File Offset: 0x000786F8
	private void UpdateUIPositionAndRotation()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this.lastStableRotation = transform.rotation;
		Vector3 normalized = transform.forward.X_Z().normalized;
		this._uiRoot.SetPositionAndRotation(this.lastStablePosition + normalized * 0.02f, Quaternion.LookRotation(normalized));
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		PrivateUIRoom._shoulderCameraReference.transform.rotation = this._uiRoot.rotation;
		this.backgroundRenderer.material.SetVector(this.backgroundDirectionPropertyID, this.backgroundRenderer.transform.InverseTransformDirection(normalized));
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x0007A5C8 File Offset: 0x000787C8
	private void UpdateUIPosition()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this._uiRoot.position = this.lastStablePosition + this.lastStableRotation * new Vector3(0f, 0f, 0.02f);
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x0007A640 File Offset: 0x00078840
	public static bool GetInOverlay()
	{
		return !(PrivateUIRoom.instance == null) && PrivateUIRoom.instance.inOverlay;
	}

	// Token: 0x04001E2A RID: 7722
	[SerializeField]
	private GameObject occluder;

	// Token: 0x04001E2B RID: 7723
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x04001E2C RID: 7724
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04001E2D RID: 7725
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04001E2E RID: 7726
	[SerializeField]
	private MeshRenderer backgroundRenderer;

	// Token: 0x04001E2F RID: 7727
	[SerializeField]
	private string backgroundDirectionPropertyName = "_SpotDirection";

	// Token: 0x04001E30 RID: 7728
	private int backgroundDirectionPropertyID;

	// Token: 0x04001E31 RID: 7729
	private int savedCullingLayers;

	// Token: 0x04001E32 RID: 7730
	private Transform _uiRoot;

	// Token: 0x04001E33 RID: 7731
	private Transform focusTransform;

	// Token: 0x04001E34 RID: 7732
	private List<Transform> ui;

	// Token: 0x04001E35 RID: 7733
	private Dictionary<Transform, Transform> uiParents;

	// Token: 0x04001E36 RID: 7734
	private float _initialAudioVolume;

	// Token: 0x04001E37 RID: 7735
	private bool inOverlay;

	// Token: 0x04001E38 RID: 7736
	private bool overlayForcedActive;

	// Token: 0x04001E39 RID: 7737
	private static PrivateUIRoom instance;

	// Token: 0x04001E3A RID: 7738
	private Vector3 lastStablePosition;

	// Token: 0x04001E3B RID: 7739
	private Quaternion lastStableRotation;

	// Token: 0x04001E3C RID: 7740
	[SerializeField]
	private float verticalPlay = 0.1f;

	// Token: 0x04001E3D RID: 7741
	[SerializeField]
	private float lateralPlay = 0.5f;

	// Token: 0x04001E3E RID: 7742
	[SerializeField]
	private float rotationalPlay = 45f;

	// Token: 0x04001E3F RID: 7743
	private int? savedCullingLayersShoudlerCam;

	// Token: 0x04001E40 RID: 7744
	private static Camera _shoulderCameraReference;
}
