using System;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001E5D RID: 7773 RVA: 0x000A132C File Offset: 0x0009F52C
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000A1333 File Offset: 0x0009F533
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x040026E0 RID: 9952
	[OnEnterPlay_SetNull]
	private static GorillaPlaySpace _instance;

	// Token: 0x040026E1 RID: 9953
	public Collider headCollider;

	// Token: 0x040026E2 RID: 9954
	public Collider bodyCollider;

	// Token: 0x040026E3 RID: 9955
	public Transform rightHandTransform;

	// Token: 0x040026E4 RID: 9956
	public Transform leftHandTransform;

	// Token: 0x040026E5 RID: 9957
	public Vector3 headColliderOffset;

	// Token: 0x040026E6 RID: 9958
	public Vector3 bodyColliderOffset;

	// Token: 0x040026E7 RID: 9959
	private Vector3 lastLeftHandPosition;

	// Token: 0x040026E8 RID: 9960
	private Vector3 lastRightHandPosition;

	// Token: 0x040026E9 RID: 9961
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x040026EA RID: 9962
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x040026EB RID: 9963
	private Vector3 lastBodyPositionForTag;

	// Token: 0x040026EC RID: 9964
	private Vector3 lastHeadPositionForTag;

	// Token: 0x040026ED RID: 9965
	private Rigidbody playspaceRigidbody;

	// Token: 0x040026EE RID: 9966
	public Transform headsetTransform;

	// Token: 0x040026EF RID: 9967
	public Vector3 rightHandOffset;

	// Token: 0x040026F0 RID: 9968
	public Vector3 leftHandOffset;

	// Token: 0x040026F1 RID: 9969
	public VRRig vrRig;

	// Token: 0x040026F2 RID: 9970
	public VRRig offlineVRRig;

	// Token: 0x040026F3 RID: 9971
	public float vibrationCooldown = 0.1f;

	// Token: 0x040026F4 RID: 9972
	public float vibrationDuration = 0.05f;

	// Token: 0x040026F5 RID: 9973
	private float leftLastTouchedSurface;

	// Token: 0x040026F6 RID: 9974
	private float rightLastTouchedSurface;

	// Token: 0x040026F7 RID: 9975
	public VRRig myVRRig;

	// Token: 0x040026F8 RID: 9976
	private float bodyHeight;

	// Token: 0x040026F9 RID: 9977
	public float tagCooldown;

	// Token: 0x040026FA RID: 9978
	public float taggedTime;

	// Token: 0x040026FB RID: 9979
	public float disconnectTime = 60f;

	// Token: 0x040026FC RID: 9980
	public float maxStepVelocity = 2f;

	// Token: 0x040026FD RID: 9981
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x040026FE RID: 9982
	public float tapHapticDuration = 0.05f;

	// Token: 0x040026FF RID: 9983
	public float tapHapticStrength = 0.5f;

	// Token: 0x04002700 RID: 9984
	public float tagHapticDuration = 0.15f;

	// Token: 0x04002701 RID: 9985
	public float tagHapticStrength = 1f;

	// Token: 0x04002702 RID: 9986
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04002703 RID: 9987
	public float taggedHapticStrength = 1f;
}
