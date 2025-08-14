using System;
using Cinemachine;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004D1 RID: 1233
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x06001E4A RID: 7754 RVA: 0x000A107C File Offset: 0x0009F27C
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x000A1104 File Offset: 0x0009F304
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = GTPlayer.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x040026C9 RID: 9929
	public Transform playerHead;

	// Token: 0x040026CA RID: 9930
	public GameObject cameraParent;

	// Token: 0x040026CB RID: 9931
	public Vector3 headOffset;

	// Token: 0x040026CC RID: 9932
	public Vector3 eulerRotationOffset;

	// Token: 0x040026CD RID: 9933
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x040026CE RID: 9934
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x040026CF RID: 9935
	private float baseCameraRadius = 0.2f;

	// Token: 0x040026D0 RID: 9936
	private float baseFollowDistance = 2f;

	// Token: 0x040026D1 RID: 9937
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x040026D2 RID: 9938
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
