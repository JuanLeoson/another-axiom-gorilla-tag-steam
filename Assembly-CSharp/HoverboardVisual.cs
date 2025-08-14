using System;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000759 RID: 1881
public class HoverboardVisual : MonoBehaviour, ICallBack
{
	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002F1D RID: 12061 RVA: 0x000F979E File Offset: 0x000F799E
	// (set) Token: 0x06002F1E RID: 12062 RVA: 0x000F97A6 File Offset: 0x000F79A6
	public Color boardColor { get; private set; }

	// Token: 0x06002F1F RID: 12063 RVA: 0x000F97B0 File Offset: 0x000F79B0
	private void Awake()
	{
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06002F20 RID: 12064 RVA: 0x000F97EC File Offset: 0x000F79EC
	// (set) Token: 0x06002F21 RID: 12065 RVA: 0x000F97F4 File Offset: 0x000F79F4
	public bool IsHeld { get; private set; }

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06002F22 RID: 12066 RVA: 0x000F97FD File Offset: 0x000F79FD
	// (set) Token: 0x06002F23 RID: 12067 RVA: 0x000F9805 File Offset: 0x000F7A05
	public bool IsLeftHanded { get; private set; }

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06002F24 RID: 12068 RVA: 0x000F980E File Offset: 0x000F7A0E
	// (set) Token: 0x06002F25 RID: 12069 RVA: 0x000F9816 File Offset: 0x000F7A16
	public Vector3 NominalLocalPosition { get; private set; }

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06002F26 RID: 12070 RVA: 0x000F981F File Offset: 0x000F7A1F
	// (set) Token: 0x06002F27 RID: 12071 RVA: 0x000F9827 File Offset: 0x000F7A27
	public Quaternion NominalLocalRotation { get; private set; }

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x06002F28 RID: 12072 RVA: 0x000F9830 File Offset: 0x000F7A30
	private Transform NominalParentTransform
	{
		get
		{
			if (!this.IsHeld)
			{
				return base.transform.parent;
			}
			return (this.IsLeftHanded ? this.parentRig.leftHand : this.parentRig.rightHand).rigTarget.transform;
		}
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x000F9870 File Offset: 0x000F7A70
	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!this.isCallbackActive)
		{
			this.parentRig.AddLateUpdateCallback(this);
			this.isCallbackActive = true;
		}
		this.IsHeld = true;
		base.gameObject.SetActive(true);
		this.IsLeftHanded = isHeldLeftHanded;
		this.NominalLocalPosition = localPosition;
		this.NominalLocalRotation = localRotation;
		Transform nominalParentTransform = this.NominalParentTransform;
		this.interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(base.transform.position);
		this.interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(base.transform.rotation);
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(true);
		}
		this.colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x000F9979 File Offset: 0x000F7B79
	public void SetNotHeld(bool isLeftHanded)
	{
		this.IsLeftHanded = isLeftHanded;
		this.SetNotHeld();
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x000F9988 File Offset: 0x000F7B88
	public void SetNotHeld()
	{
		bool isHeld = this.IsHeld;
		base.gameObject.SetActive(false);
		this.IsHeld = false;
		this.interpolatedLocalPosition = base.transform.localPosition;
		this.interpolatedLocalRotation = base.transform.localRotation;
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (!isHeld)
		{
			base.transform.position = base.transform.parent.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = base.transform.parent.TransformRotation(this.NominalLocalRotation);
		}
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(false);
		}
		this.hoverboardAudio.Stop();
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x000F9A90 File Offset: 0x000F7C90
	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = this.NominalParentTransform;
		if ((this.interpolatedLocalPosition - this.NominalLocalPosition).IsShorterThan(0.01f))
		{
			base.transform.position = nominalParentTransform.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.NominalLocalRotation);
			if (!this.IsHeld)
			{
				this.parentRig.RemoveLateUpdateCallback(this);
				this.isCallbackActive = false;
			}
		}
		else
		{
			this.interpolatedLocalPosition = Vector3.MoveTowards(this.interpolatedLocalPosition, this.NominalLocalPosition, this.positionLerpSpeed * Time.deltaTime);
			this.interpolatedLocalRotation = Quaternion.RotateTowards(this.interpolatedLocalRotation, this.NominalLocalRotation, this.rotationLerpSpeed * Time.deltaTime);
			base.transform.position = nominalParentTransform.TransformPoint(this.interpolatedLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.interpolatedLocalRotation);
		}
		if (this.IsHeld)
		{
			if (this.parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(base.transform.position, base.transform.rotation);
				return;
			}
			this.hoverboardAudio.UpdateAudioLoop(this.parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
		}
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x000F9BE6 File Offset: 0x000F7DE6
	public void PlayGrindHaptic()
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, this.grindHapticStrength, this.grindHapticDuration);
		}
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x000F9C0C File Offset: 0x000F7E0C
	public void PlayCarveHaptic(float carveForce)
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, carveForce * this.carveHapticStrength, this.carveHapticDuration);
		}
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x000F9C34 File Offset: 0x000F7E34
	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(this.handlePosition, isLeftHand);
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x000F9C49 File Offset: 0x000F7E49
	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(base.transform.position, base.transform.rotation, this.velocityEstimator.linearVelocity, this.velocityEstimator.angularVelocity, this.boardColor);
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x000F9C87 File Offset: 0x000F7E87
	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.racePositionReadout.gameObject.SetActive(false);
			return;
		}
		this.racePositionReadout.gameObject.SetActive(true);
		this.racePositionReadout.text = text;
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x000F9CC0 File Offset: 0x000F7EC0
	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.raceLapsReadout.gameObject.SetActive(false);
			return;
		}
		this.raceLapsReadout.gameObject.SetActive(true);
		this.raceLapsReadout.text = text;
	}

	// Token: 0x04003B21 RID: 15137
	[SerializeField]
	private VRRig parentRig;

	// Token: 0x04003B22 RID: 15138
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04003B23 RID: 15139
	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	// Token: 0x04003B24 RID: 15140
	[SerializeField]
	private HoverboardHandle handlePosition;

	// Token: 0x04003B25 RID: 15141
	[SerializeField]
	private float grindHapticStrength;

	// Token: 0x04003B26 RID: 15142
	[SerializeField]
	private float grindHapticDuration;

	// Token: 0x04003B27 RID: 15143
	[SerializeField]
	private float carveHapticStrength;

	// Token: 0x04003B28 RID: 15144
	[SerializeField]
	private float carveHapticDuration;

	// Token: 0x04003B29 RID: 15145
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04003B2A RID: 15146
	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	// Token: 0x04003B2B RID: 15147
	[SerializeField]
	private TextMeshPro racePositionReadout;

	// Token: 0x04003B2C RID: 15148
	[SerializeField]
	private TextMeshPro raceLapsReadout;

	// Token: 0x04003B2D RID: 15149
	private Material colorMaterial;

	// Token: 0x04003B33 RID: 15155
	private Vector3 interpolatedLocalPosition;

	// Token: 0x04003B34 RID: 15156
	private Quaternion interpolatedLocalRotation;

	// Token: 0x04003B35 RID: 15157
	[SerializeField]
	private float lerpIntoHandDuration;

	// Token: 0x04003B36 RID: 15158
	private float positionLerpSpeed;

	// Token: 0x04003B37 RID: 15159
	private float rotationLerpSpeed;

	// Token: 0x04003B38 RID: 15160
	private bool isCallbackActive;
}
