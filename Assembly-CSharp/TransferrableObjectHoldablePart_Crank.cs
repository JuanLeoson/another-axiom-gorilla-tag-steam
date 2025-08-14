using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003F4 RID: 1012
public class TransferrableObjectHoldablePart_Crank : TransferrableObjectHoldablePart
{
	// Token: 0x0600179A RID: 6042 RVA: 0x0007F387 File Offset: 0x0007D587
	public void SetOnCrankedCallback(Action<float> onCrankedCallback)
	{
		this.onCrankedCallback = onCrankedCallback;
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0007F390 File Offset: 0x0007D590
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x);
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x0007F478 File Offset: 0x0007D678
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		Vector3 a;
		if (rig.isOfflineVRRig)
		{
			Transform transform = isHeldLeftHand ? GTPlayer.Instance.leftControllerTransform : GTPlayer.Instance.rightControllerTransform;
			Vector3 vector = this.rotatingPart.InverseTransformPoint(transform.position);
			Vector3 position = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
			Vector3 vector2 = this.rotatingPart.TransformPoint(position);
			if (this.maxHandSnapDistance > 0f && (transform.position - vector2).IsLongerThan(this.maxHandSnapDistance))
			{
				this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
				return;
			}
			transform.position = vector2;
			a = transform.position;
		}
		else
		{
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			a = vrmap.GetExtrapolatedControllerPosition();
			a -= vrmap.rigTarget.rotation * (isHeldLeftHand ? GTPlayer.Instance.leftHandOffset : GTPlayer.Instance.rightHandOffset) * rig.scaleFactor;
		}
		Vector3 vector3 = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (a - this.rotatingPart.position);
		float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		if (num2 != 0f)
		{
			if (this.onCrankedCallback != null)
			{
				this.onCrankedCallback(num2);
			}
			for (int i = 0; i < this.thresholds.Length; i++)
			{
				this.thresholds[i].OnCranked(num2);
			}
		}
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(num - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x0007F694 File Offset: 0x0007D894
	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04001F7C RID: 8060
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04001F7D RID: 8061
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04001F7E RID: 8062
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04001F7F RID: 8063
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04001F80 RID: 8064
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04001F81 RID: 8065
	private float crankAngleOffset;

	// Token: 0x04001F82 RID: 8066
	private float crankRadius;

	// Token: 0x04001F83 RID: 8067
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04001F84 RID: 8068
	private float lastAngle;

	// Token: 0x04001F85 RID: 8069
	private Quaternion baseLocalAngle;

	// Token: 0x04001F86 RID: 8070
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04001F87 RID: 8071
	private Action<float> onCrankedCallback;

	// Token: 0x04001F88 RID: 8072
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank.CrankThreshold[] thresholds;

	// Token: 0x020003F5 RID: 1013
	[Serializable]
	private struct CrankThreshold
	{
		// Token: 0x0600179F RID: 6047 RVA: 0x0007F70F File Offset: 0x0007D90F
		public void OnCranked(float deltaAngle)
		{
			this.currentAngle += deltaAngle;
			if (Mathf.Abs(this.currentAngle) > this.angleThreshold)
			{
				this.currentAngle = 0f;
				this.onReached.Invoke();
			}
		}

		// Token: 0x04001F89 RID: 8073
		public float angleThreshold;

		// Token: 0x04001F8A RID: 8074
		public UnityEvent onReached;

		// Token: 0x04001F8B RID: 8075
		[HideInInspector]
		public float currentAngle;
	}
}
