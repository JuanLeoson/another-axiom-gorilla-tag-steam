using System;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class CharacterCameraConstraint : MonoBehaviour
{
	// Token: 0x060012FC RID: 4860 RVA: 0x000026E9 File Offset: 0x000008E9
	private CharacterCameraConstraint()
	{
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x00067D5F File Offset: 0x00065F5F
	private void Awake()
	{
		this._character = base.GetComponent<CapsuleCollider>();
		this._simplePlayerController = base.GetComponent<SimpleCapsuleWithStickMovement>();
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x00067D79 File Offset: 0x00065F79
	private void OnEnable()
	{
		this._simplePlayerController.CameraUpdated += this.CameraUpdate;
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x00067D92 File Offset: 0x00065F92
	private void OnDisable()
	{
		this._simplePlayerController.CameraUpdated -= this.CameraUpdate;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00067DAC File Offset: 0x00065FAC
	private void CameraUpdate()
	{
		float value = 0f;
		if (this.CheckCameraOverlapped())
		{
			OVRScreenFade.instance.SetExplicitFade(1f);
		}
		else if (this.CheckCameraNearClipping(out value))
		{
			float t = Mathf.InverseLerp(0f, 0.1f, value);
			float explicitFade = Mathf.Lerp(0f, 1f, t);
			OVRScreenFade.instance.SetExplicitFade(explicitFade);
		}
		else
		{
			OVRScreenFade.instance.SetExplicitFade(0f);
		}
		float num = 0.25f;
		float value2 = this.CameraRig.centerEyeAnchor.localPosition.y + this.HeightOffset + num;
		float num2 = this.MinimumHeight;
		num2 = Mathf.Min(this._character.height, num2);
		float num3 = this.MaximumHeight;
		RaycastHit raycastHit;
		if (Physics.SphereCast(this._character.transform.position, this._character.radius * 0.2f, Vector3.up, out raycastHit, this.MaximumHeight - this._character.transform.position.y, this.CollideLayers, QueryTriggerInteraction.Ignore))
		{
			num3 = raycastHit.point.y;
		}
		num3 = Mathf.Max(this._character.height, num3);
		this._character.height = Mathf.Clamp(value2, num2, num3);
		float y = this.HeightOffset - this._character.height * 0.5f - num;
		this.CameraRig.transform.localPosition = new Vector3(0f, y, 0f);
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x00067F34 File Offset: 0x00066134
	private bool CheckCameraOverlapped()
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3 position = this._character.transform.position;
		float num = Mathf.Max(0f, this._character.height * 0.5f - component.nearClipPlane - 0.01f);
		position.y = Mathf.Clamp(this.CameraRig.centerEyeAnchor.position.y, this._character.transform.position.y - num, this._character.transform.position.y + num);
		Vector3 a = this.CameraRig.centerEyeAnchor.position - position;
		float magnitude = a.magnitude;
		Vector3 direction = a / magnitude;
		RaycastHit raycastHit;
		return Physics.SphereCast(position, component.nearClipPlane, direction, out raycastHit, magnitude, this.CollideLayers, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00068024 File Offset: 0x00066224
	private bool CheckCameraNearClipping(out float result)
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3[] array = new Vector3[4];
		component.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), component.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, array);
		Vector3 vector = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[0])) * 0.25f;
		Vector3 vector2 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[1])) * 0.25f;
		Vector3 vector3 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[2])) * 0.25f;
		Vector3 vector4 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[3])) * 0.25f;
		Vector3 vector5 = (vector2 + vector4) / 2f;
		bool result2 = false;
		result = 0f;
		foreach (Vector3 vector6 in new Vector3[]
		{
			vector,
			vector2,
			vector3,
			vector4,
			vector5
		})
		{
			RaycastHit raycastHit;
			if (Physics.Linecast(this.CameraRig.centerEyeAnchor.position, vector6, out raycastHit, this.CollideLayers, QueryTriggerInteraction.Ignore))
			{
				result2 = true;
				result = Mathf.Max(result, Vector3.Distance(raycastHit.point, vector6));
			}
		}
		return result2;
	}

	// Token: 0x04001A94 RID: 6804
	private const float FADE_RAY_LENGTH = 0.25f;

	// Token: 0x04001A95 RID: 6805
	private const float FADE_OVERLAP_MAXIMUM = 0.1f;

	// Token: 0x04001A96 RID: 6806
	private const float FADE_AMOUNT_MAXIMUM = 1f;

	// Token: 0x04001A97 RID: 6807
	[Tooltip("This should be a reference to the OVRCameraRig that is usually a child of the PlayerController.")]
	public OVRCameraRig CameraRig;

	// Token: 0x04001A98 RID: 6808
	[Tooltip("Collision layers to be used for the purposes of fading out the screen when the HMD is inside world geometry and adjusting the capsule height.")]
	public LayerMask CollideLayers;

	// Token: 0x04001A99 RID: 6809
	[Tooltip("Offset is added to camera's real world height, effectively treating it as though the player was taller/standing higher.")]
	public float HeightOffset;

	// Token: 0x04001A9A RID: 6810
	[Tooltip("Minimum height that the character capsule can shrink to.  To disable, set to capsule's height.")]
	public float MinimumHeight;

	// Token: 0x04001A9B RID: 6811
	[Tooltip("Maximum height that the character capsule can grow to.  To disable, set to capsule's height.")]
	public float MaximumHeight;

	// Token: 0x04001A9C RID: 6812
	private CapsuleCollider _character;

	// Token: 0x04001A9D RID: 6813
	private SimpleCapsuleWithStickMovement _simplePlayerController;
}
