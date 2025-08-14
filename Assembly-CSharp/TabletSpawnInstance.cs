using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class TabletSpawnInstance : IDisposable
{
	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000EC3 RID: 3779 RVA: 0x00058E44 File Offset: 0x00057044
	// (remove) Token: 0x06000EC4 RID: 3780 RVA: 0x00058E7C File Offset: 0x0005707C
	public event Action onGrabbed;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000EC5 RID: 3781 RVA: 0x00058EB4 File Offset: 0x000570B4
	// (remove) Token: 0x06000EC6 RID: 3782 RVA: 0x00058EEC File Offset: 0x000570EC
	public event Action onReleased;

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x00058F21 File Offset: 0x00057121
	public LckDirectGrabbable directGrabbable
	{
		get
		{
			return this._lckSocialCameraManager.lckDirectGrabbable;
		}
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x00058F2E File Offset: 0x0005712E
	public bool ResetLocalPose()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.localPosition = Vector3.zero;
		this._cameraSpawnInstanceTransform.localRotation = Quaternion.identity;
		return true;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x00058F61 File Offset: 0x00057161
	public bool ResetParent()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(this._cameraSpawnParentTransform);
		return true;
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00058F85 File Offset: 0x00057185
	public bool SetParent(Transform transform)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(transform);
		return true;
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000ECB RID: 3787 RVA: 0x00058FA4 File Offset: 0x000571A4
	// (set) Token: 0x06000ECC RID: 3788 RVA: 0x00058FAC File Offset: 0x000571AC
	public bool cameraActive
	{
		get
		{
			return this._cameraActive;
		}
		set
		{
			this._cameraActive = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.cameraActive = this._cameraActive;
			}
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000ECD RID: 3789 RVA: 0x00058FD4 File Offset: 0x000571D4
	// (set) Token: 0x06000ECE RID: 3790 RVA: 0x00058FDC File Offset: 0x000571DC
	public bool uiVisible
	{
		get
		{
			return this._uiVisible;
		}
		set
		{
			this._uiVisible = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.uiVisible = this._uiVisible;
			}
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000ECF RID: 3791 RVA: 0x00059004 File Offset: 0x00057204
	public bool isSpawned
	{
		get
		{
			return this._cameraGameObjectInstance != null;
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x00059012 File Offset: 0x00057212
	public TabletSpawnInstance(GameObject cameraSpawnPrefab, Transform cameraSpawnParentTransform)
	{
		this._cameraSpawnPrefab = cameraSpawnPrefab;
		this._cameraSpawnParentTransform = cameraSpawnParentTransform;
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00059028 File Offset: 0x00057228
	public void SpawnCamera()
	{
		if (!this.isSpawned)
		{
			this._cameraGameObjectInstance = Object.Instantiate<GameObject>(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
			this._lckSocialCameraManager = this._cameraGameObjectInstance.GetComponent<LckSocialCameraManager>();
			this._lckSocialCameraManager.lckDirectGrabbable.onGrabbed += delegate()
			{
				Action action = this.onGrabbed;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._lckSocialCameraManager.lckDirectGrabbable.onReleased += delegate()
			{
				Action action = this.onReleased;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._cameraSpawnInstanceTransform = this._cameraGameObjectInstance.transform;
			this.Controller = this._cameraGameObjectInstance.GetComponent<GTLckController>();
		}
		this.uiVisible = this.uiVisible;
		this.cameraActive = this.cameraActive;
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x000590DA File Offset: 0x000572DA
	public Vector3 position
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Vector3.zero;
			}
			return this._cameraSpawnInstanceTransform.position;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x000590FB File Offset: 0x000572FB
	public Quaternion rotation
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Quaternion.identity;
			}
			return this._cameraSpawnInstanceTransform.rotation;
		}
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0005911C File Offset: 0x0005731C
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0005913A File Offset: 0x0005733A
	public void SetLocalScale(Vector3 scale)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.localScale = scale;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x00059157 File Offset: 0x00057357
	public void Dispose()
	{
		if (this._cameraGameObjectInstance != null)
		{
			Object.Destroy(this._cameraGameObjectInstance);
			this._cameraGameObjectInstance = null;
		}
	}

	// Token: 0x040017AD RID: 6061
	private GameObject _cameraGameObjectInstance;

	// Token: 0x040017AE RID: 6062
	private GameObject _cameraSpawnPrefab;

	// Token: 0x040017AF RID: 6063
	private GameEvents _GtCamera;

	// Token: 0x040017B0 RID: 6064
	private Transform _cameraSpawnParentTransform;

	// Token: 0x040017B1 RID: 6065
	private Transform _cameraSpawnInstanceTransform;

	// Token: 0x040017B2 RID: 6066
	public GTLckController Controller;

	// Token: 0x040017B3 RID: 6067
	private LckSocialCameraManager _lckSocialCameraManager;

	// Token: 0x040017B4 RID: 6068
	private bool _cameraActive;

	// Token: 0x040017B5 RID: 6069
	private bool _uiVisible;
}
