using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000292 RID: 658
[NetworkBehaviourWeaved(1)]
public class LckSocialCamera : NetworkComponent, IGorillaSliceableSimple
{
	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0005A135 File Offset: 0x00058335
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe ref LckSocialCamera.CameraData _networkedData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing LckSocialCamera._networkedData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ref *(LckSocialCamera.CameraData*)(this.Ptr + 0);
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0005A15A File Offset: 0x0005835A
	// (set) Token: 0x06000F17 RID: 3863 RVA: 0x0005A168 File Offset: 0x00058368
	private LckSocialCamera.CameraState currentState
	{
		get
		{
			return this._localData.currentState;
		}
		set
		{
			this._localData.currentState = value;
			if (base.IsLocallyOwned)
			{
				this.CoconutCamera.SetVisualsActive(false);
				this.CoconutCamera.SetRecordingState(false);
				return;
			}
			this.CoconutCamera.SetVisualsActive(this.visible);
			this.CoconutCamera.SetRecordingState(this.recording);
		}
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0005A1C4 File Offset: 0x000583C4
	private static bool GetFlag(LckSocialCamera.CameraState cameraState, LckSocialCamera.CameraState flag)
	{
		return (cameraState & flag) == flag;
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0005A1CC File Offset: 0x000583CC
	private static LckSocialCamera.CameraState SetFlag(LckSocialCamera.CameraState cameraState, LckSocialCamera.CameraState flag, bool value)
	{
		if (value)
		{
			cameraState |= flag;
		}
		else
		{
			cameraState &= ~flag;
		}
		return cameraState;
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0005A1DF File Offset: 0x000583DF
	// (set) Token: 0x06000F1B RID: 3867 RVA: 0x0005A1ED File Offset: 0x000583ED
	public bool visible
	{
		get
		{
			return LckSocialCamera.GetFlag(this.currentState, LckSocialCamera.CameraState.Visible);
		}
		set
		{
			this.currentState = LckSocialCamera.SetFlag(this.currentState, LckSocialCamera.CameraState.Visible, value);
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0005A202 File Offset: 0x00058402
	// (set) Token: 0x06000F1D RID: 3869 RVA: 0x0005A210 File Offset: 0x00058410
	public bool recording
	{
		get
		{
			return LckSocialCamera.GetFlag(this.currentState, LckSocialCamera.CameraState.Recording);
		}
		set
		{
			this.currentState = LckSocialCamera.SetFlag(this.currentState, LckSocialCamera.CameraState.Recording, value);
		}
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0005A225 File Offset: 0x00058425
	public unsafe override void WriteDataFusion()
	{
		*this._networkedData = new LckSocialCamera.CameraData(this._localData.currentState);
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0005A242 File Offset: 0x00058442
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this._networkedData.currentState);
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0005A255 File Offset: 0x00058455
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.currentState);
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0005A268 File Offset: 0x00058468
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		LckSocialCamera.CameraState newState = (LckSocialCamera.CameraState)stream.ReceiveNext();
		this.ReadDataShared(newState);
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0005A29C File Offset: 0x0005849C
	protected override void Start()
	{
		base.Start();
		this.visible = this.visible;
		if (base.IsLocallyOwned)
		{
			this.StoreRigReference();
			LckSocialCameraManager instance = LckSocialCameraManager.Instance;
			if (instance != null)
			{
				instance.SetLckSocialCamera(this);
				return;
			}
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Combine(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0005A300 File Offset: 0x00058500
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
		if (!info.photonView.IsMine)
		{
			this.StoreRigReference();
		}
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0005A31C File Offset: 0x0005851C
	private void StoreRigReference()
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(base.Owner, out rigContainer))
		{
			this._vrrig = rigContainer.Rig;
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0005A34C File Offset: 0x0005854C
	public void SliceUpdate()
	{
		if (this._vrrig == null)
		{
			this.StoreRigReference();
			if (this._vrrig.IsNull())
			{
				base.enabled = false;
				return;
			}
		}
		else
		{
			this.CoconutCamera.transform.localScale = Vector3.one * this._vrrig.scaleFactor;
		}
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0005A3A7 File Offset: 0x000585A7
	public new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0005A3BC File Offset: 0x000585BC
	public new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0005A3D1 File Offset: 0x000585D1
	private void OnManagerSpawned(LckSocialCameraManager manager)
	{
		manager.SetLckSocialCamera(this);
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0005A3DA File Offset: 0x000585DA
	private void ReadDataShared(LckSocialCamera.CameraState newState)
	{
		this.currentState = newState;
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0005A3E3 File Offset: 0x000585E3
	[WeaverGenerated]
	public unsafe override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		*this._networkedData = this.__networkedData;
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0005A400 File Offset: 0x00058600
	[WeaverGenerated]
	public unsafe override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this.__networkedData = *this._networkedData;
	}

	// Token: 0x040017E7 RID: 6119
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x040017E8 RID: 6120
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x040017E9 RID: 6121
	[SerializeField]
	private List<GameObject> _visualObjects;

	// Token: 0x040017EA RID: 6122
	[SerializeField]
	private VRRig _vrrig;

	// Token: 0x040017EB RID: 6123
	private LckSocialCamera.CameraDataLocal _localData;

	// Token: 0x040017EC RID: 6124
	[WeaverGenerated]
	[DefaultForProperty("_networkedData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private LckSocialCamera.CameraData __networkedData;

	// Token: 0x02000293 RID: 659
	private enum CameraState
	{
		// Token: 0x040017EE RID: 6126
		Empty,
		// Token: 0x040017EF RID: 6127
		Visible,
		// Token: 0x040017F0 RID: 6128
		Recording
	}

	// Token: 0x02000294 RID: 660
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct CameraData : INetworkStruct
	{
		// Token: 0x06000F2D RID: 3885 RVA: 0x0005A419 File Offset: 0x00058619
		public CameraData(LckSocialCamera.CameraState currentState)
		{
			this.currentState = currentState;
		}

		// Token: 0x040017F1 RID: 6129
		[FieldOffset(0)]
		public LckSocialCamera.CameraState currentState;
	}

	// Token: 0x02000295 RID: 661
	private struct CameraDataLocal
	{
		// Token: 0x040017F2 RID: 6130
		public LckSocialCamera.CameraState currentState;
	}
}
