using System;
using UnityEngine;

// Token: 0x02000328 RID: 808
public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	// Token: 0x14000037 RID: 55
	// (add) Token: 0x06001367 RID: 4967 RVA: 0x00069114 File Offset: 0x00067314
	// (remove) Token: 0x06001368 RID: 4968 RVA: 0x0006914C File Offset: 0x0006734C
	public event Action CameraUpdated;

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x06001369 RID: 4969 RVA: 0x00069184 File Offset: 0x00067384
	// (remove) Token: 0x0600136A RID: 4970 RVA: 0x000691BC File Offset: 0x000673BC
	public event Action PreCharacterMove;

	// Token: 0x0600136B RID: 4971 RVA: 0x000691F1 File Offset: 0x000673F1
	private void Awake()
	{
		this._rigidbody = base.GetComponent<Rigidbody>();
		if (this.CameraRig == null)
		{
			this.CameraRig = base.GetComponentInChildren<OVRCameraRig>();
		}
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0006921C File Offset: 0x0006741C
	private void FixedUpdate()
	{
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
		}
		if (this.HMDRotatesPlayer)
		{
			this.RotatePlayerToHMD();
		}
		if (this.EnableLinearMovement)
		{
			this.StickMovement();
		}
		if (this.EnableRotation)
		{
			this.SnapTurn();
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x0006927C File Offset: 0x0006747C
	private void RotatePlayerToHMD()
	{
		Transform trackingSpace = this.CameraRig.trackingSpace;
		Transform centerEyeAnchor = this.CameraRig.centerEyeAnchor;
		Vector3 position = trackingSpace.position;
		Quaternion rotation = trackingSpace.rotation;
		base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
		trackingSpace.position = position;
		trackingSpace.rotation = rotation;
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x000692E8 File Offset: 0x000674E8
	private void StickMovement()
	{
		Vector3 eulerAngles = this.CameraRig.centerEyeAnchor.rotation.eulerAngles;
		eulerAngles.z = (eulerAngles.x = 0f);
		Quaternion rotation = Quaternion.Euler(eulerAngles);
		Vector3 a = Vector3.zero;
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
		a += rotation * (vector.x * Vector3.right);
		a += rotation * (vector.y * Vector3.forward);
		this._rigidbody.MovePosition(this._rigidbody.position + a * this.Speed * Time.fixedDeltaTime);
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x000693AC File Offset: 0x000675AC
	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, -this.RotationAngle);
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, this.RotationAngle);
				return;
			}
		}
		else
		{
			this.ReadyToSnapTurn = true;
		}
	}

	// Token: 0x04001AEE RID: 6894
	public bool EnableLinearMovement = true;

	// Token: 0x04001AEF RID: 6895
	public bool EnableRotation = true;

	// Token: 0x04001AF0 RID: 6896
	public bool HMDRotatesPlayer = true;

	// Token: 0x04001AF1 RID: 6897
	public bool RotationEitherThumbstick;

	// Token: 0x04001AF2 RID: 6898
	public float RotationAngle = 45f;

	// Token: 0x04001AF3 RID: 6899
	public float Speed;

	// Token: 0x04001AF4 RID: 6900
	public OVRCameraRig CameraRig;

	// Token: 0x04001AF5 RID: 6901
	private bool ReadyToSnapTurn;

	// Token: 0x04001AF6 RID: 6902
	private Rigidbody _rigidbody;
}
