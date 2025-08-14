using System;
using System.Collections;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000298 RID: 664
public class LckWallCameraSpawner : MonoBehaviour
{
	// Token: 0x06000F4A RID: 3914 RVA: 0x0005A9F8 File Offset: 0x00058BF8
	private LckBodyCameraSpawner GetOrCreateBodyCameraSpawner()
	{
		if (LckWallCameraSpawner._bodySpawner != null)
		{
			return LckWallCameraSpawner._bodySpawner;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find Player!");
			return null;
		}
		LckWallCameraSpawner.AddGTag(Camera.main.gameObject, GtTagType.HMD);
		LckWallCameraSpawner.AddGTag(instance.gameObject, GtTagType.Player);
		Transform transform = instance.bodyCollider.transform;
		GameObject gameObject = Object.Instantiate<GameObject>(this._lckBodySpawnerPrefab, transform.parent);
		Transform transform2 = gameObject.transform;
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		transform2.localScale = Vector3.one;
		LckWallCameraSpawner._bodySpawner = gameObject.GetComponent<LckBodyCameraSpawner>();
		LckWallCameraSpawner._bodySpawner.SetFollowTransform(transform);
		GorillaTagger instance2 = GorillaTagger.Instance;
		if (instance2 != null)
		{
			LckWallCameraSpawner.AddGTag(instance2.leftHandTriggerCollider, GtTagType.LeftHand);
			LckWallCameraSpawner.AddGTag(instance2.rightHandTriggerCollider, GtTagType.RightHand);
		}
		else
		{
			Debug.LogError("Unable to find GorillaTagger!");
		}
		return LckWallCameraSpawner._bodySpawner;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x0005AADF File Offset: 0x00058CDF
	private static void AddGTag(GameObject go, GtTagType gtTagType)
	{
		if (go.GetComponent<GtTag>())
		{
			return;
		}
		GtTag gtTag = go.AddComponent<GtTag>();
		gtTag.gtTagType = gtTagType;
		gtTag.enabled = true;
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0005AB02 File Offset: 0x00058D02
	// (set) Token: 0x06000F4D RID: 3917 RVA: 0x0005AB0C File Offset: 0x00058D0C
	public LckWallCameraSpawner.WallSpawnerState wallSpawnerState
	{
		get
		{
			return this._wallSpawnerState;
		}
		set
		{
			switch (value)
			{
			case LckWallCameraSpawner.WallSpawnerState.CameraOnHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			case LckWallCameraSpawner.WallSpawnerState.CameraOffHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			}
			this._wallSpawnerState = value;
		}
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0005AB5C File Offset: 0x00058D5C
	private void Awake()
	{
		this.InitCameraStrap();
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0005AB64 File Offset: 0x00058D64
	private void OnEnable()
	{
		this._cameraHandleGrabbable.onGrabbed += this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased += this.OnReleased;
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0005AB9B File Offset: 0x00058D9B
	private void Start()
	{
		this.CreatePrewarmCamera();
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0005ABA4 File Offset: 0x00058DA4
	private void Update()
	{
		LckWallCameraSpawner.WallSpawnerState wallSpawnerState = this._wallSpawnerState;
		if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraOnHook)
		{
			if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraDragging)
			{
				return;
			}
			this.UpdateCameraStrap();
			if (this.ShouldSpawnCamera(this._cameraHandleGrabbable.grabber.transform))
			{
				this.SpawnCamera(this._cameraHandleGrabbable.grabber);
			}
		}
		else
		{
			if (this.GetOrCreateBodyCameraSpawner() == null)
			{
				Debug.LogError("Lck, Unable to find LckBodyCameraSpawner");
				base.gameObject.SetActive(false);
				return;
			}
			if (LckWallCameraSpawner._bodySpawner.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.isSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable.isGrabbed)
			{
				LckDirectGrabbable directGrabbable = LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable;
				GorillaGrabber grabber = directGrabbable.grabber;
				if (!this.ShouldSpawnCamera(grabber.transform))
				{
					directGrabbable.ForceRelease();
					LckWallCameraSpawner._bodySpawner.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
					this._cameraHandleGrabbable.target.SetPositionAndRotation(grabber.transform.position, grabber.transform.rotation * Quaternion.Euler(-55f, 180f, 0f));
					this._cameraHandleGrabbable.ForceGrab(grabber);
					return;
				}
			}
		}
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0005ACD9 File Offset: 0x00058ED9
	private void OnDisable()
	{
		this._cameraHandleGrabbable.onGrabbed -= this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased -= this.OnReleased;
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000F53 RID: 3923 RVA: 0x0005AD09 File Offset: 0x00058F09
	// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0005AD1B File Offset: 0x00058F1B
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.gameObject.SetActive(value);
		}
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0005AD3F File Offset: 0x00058F3F
	private void SpawnCamera(GorillaGrabber lastGorillaGrabber)
	{
		if (LckWallCameraSpawner._bodySpawner == null)
		{
			Debug.LogError("Lck, unable to spawn camera, body spawner is null!");
			return;
		}
		this.cameraVisible = false;
		this._cameraHandleGrabbable.ForceRelease();
		LckWallCameraSpawner._bodySpawner.SpawnCamera(lastGorillaGrabber, lastGorillaGrabber.transform);
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0005AD7C File Offset: 0x00058F7C
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0005ADA4 File Offset: 0x00058FA4
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
		this._cameraStrapRenderer.startColor = (this._cameraStrapRenderer.endColor = this._normalColor);
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0005AE46 File Offset: 0x00059046
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0005AE68 File Offset: 0x00059068
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 a = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 b = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(a - b) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0005AEBE File Offset: 0x000590BE
	private void OnGrabbed()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraDragging;
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0005AEC7 File Offset: 0x000590C7
	private void OnReleased()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0005AED0 File Offset: 0x000590D0
	private void CreatePrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("prewarm camera");
		gameObject.transform.SetParent(base.transform);
		LckWallCameraSpawner._prewarmCamera = gameObject.AddComponent<Camera>();
		Camera main = Camera.main;
		LckWallCameraSpawner._prewarmCamera.clearFlags = main.clearFlags;
		LckWallCameraSpawner._prewarmCamera.fieldOfView = main.fieldOfView;
		LckWallCameraSpawner._prewarmCamera.nearClipPlane = main.nearClipPlane;
		LckWallCameraSpawner._prewarmCamera.farClipPlane = main.farClipPlane;
		LckWallCameraSpawner._prewarmCamera.cullingMask = main.cullingMask;
		LckWallCameraSpawner._prewarmCamera.tag = "Untagged";
		LckWallCameraSpawner._prewarmCamera.stereoTargetEye = StereoTargetEyeMask.None;
		LckWallCameraSpawner._prewarmCamera.targetTexture = new RenderTexture(32, 32, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt);
		LckWallCameraSpawner._prewarmCamera.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
		base.StartCoroutine(this.DestroyPrewarmCameraDelayed());
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0005AFC8 File Offset: 0x000591C8
	private IEnumerator DestroyPrewarmCameraDelayed()
	{
		yield return new WaitForSeconds(1f);
		this.DestroyPrewarmCamera();
		yield break;
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0005AFD7 File Offset: 0x000591D7
	private void DestroyPrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera == null)
		{
			return;
		}
		RenderTexture targetTexture = LckWallCameraSpawner._prewarmCamera.targetTexture;
		LckWallCameraSpawner._prewarmCamera.targetTexture = null;
		targetTexture.Release();
		Object.Destroy(LckWallCameraSpawner._prewarmCamera.gameObject);
		LckWallCameraSpawner._prewarmCamera = null;
	}

	// Token: 0x0400180A RID: 6154
	[SerializeField]
	private GameObject _lckBodySpawnerPrefab;

	// Token: 0x0400180B RID: 6155
	[SerializeField]
	private LckDirectGrabbable _cameraHandleGrabbable;

	// Token: 0x0400180C RID: 6156
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x0400180D RID: 6157
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x0400180E RID: 6158
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x0400180F RID: 6159
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x04001810 RID: 6160
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x04001811 RID: 6161
	private Vector3[] _cameraStrapPositions;

	// Token: 0x04001812 RID: 6162
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x04001813 RID: 6163
	private static LckBodyCameraSpawner _bodySpawner;

	// Token: 0x04001814 RID: 6164
	private static Camera _prewarmCamera;

	// Token: 0x04001815 RID: 6165
	private LckWallCameraSpawner.WallSpawnerState _wallSpawnerState;

	// Token: 0x02000299 RID: 665
	public enum WallSpawnerState
	{
		// Token: 0x04001817 RID: 6167
		CameraOnHook,
		// Token: 0x04001818 RID: 6168
		CameraDragging,
		// Token: 0x04001819 RID: 6169
		CameraOffHook
	}
}
