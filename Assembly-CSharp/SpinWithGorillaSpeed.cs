using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class SpinWithGorillaSpeed : MonoBehaviour
{
	// Token: 0x06000B29 RID: 2857 RVA: 0x0003B537 File Offset: 0x00039737
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.initialRotation = base.transform.localRotation;
		this.spinAxis = this.initialRotation * this.axisOfRotation * Vector3.forward;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0003B578 File Offset: 0x00039778
	private void Update()
	{
		Vector3 vector = (this.optionalVelocityEstimator != null) ? this.optionalVelocityEstimator.linearVelocity : this.rig.LatestVelocity();
		vector.y *= this.verticalSpeedInfluence;
		float time = vector.magnitude / this.maxSpeed;
		float num = Time.deltaTime * this.degreesPerSecondAtSpeed.Evaluate(time) * (this.clockwise ? -1f : 1f);
		this.currentAngle = Mathf.Repeat(this.currentAngle + num, 360f);
		Quaternion quaternion = this.initialRotation * Quaternion.AngleAxis(this.currentAngle, this.spinAxis);
		base.transform.SetLocalPositionAndRotation(quaternion * this.centerOfRotation, quaternion);
		if (this.tickSound != null && this.tickClips.Length != 0)
		{
			this.tickAngle += num;
			if (this.tickAngle >= this.tickSoundDegrees)
			{
				this.tickSound.pitch = this.tickPitchAtSpeed.Evaluate(time);
				this.tickSound.volume = this.tickVolumeAtSpeed.Evaluate(time);
				this.tickSound.clip = this.tickClips.GetRandomItem<AudioClip>();
				this.tickSound.GTPlay();
				this.tickAngle = Mathf.Repeat(this.tickAngle, this.tickSoundDegrees);
			}
		}
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0003B6E0 File Offset: 0x000398E0
	private void OnDisable()
	{
		this.currentAngle = 0f;
		this.tickAngle = 0f;
	}

	// Token: 0x04000DA5 RID: 3493
	[Tooltip("Get the velocity from this component when determining the spin speed. If this is unset, it will use the unsmoothed velocity of the parent VRRig component.")]
	[SerializeField]
	private GorillaVelocityEstimator optionalVelocityEstimator;

	// Token: 0x04000DA6 RID: 3494
	[SerializeField]
	private Quaternion axisOfRotation = Quaternion.identity;

	// Token: 0x04000DA7 RID: 3495
	[SerializeField]
	private Vector3 centerOfRotation = Vector3.zero;

	// Token: 0x04000DA8 RID: 3496
	[Tooltip("The reported speed will be divided by this value before being used to sample AnimationCurves, to allow them to be in the range 0-1.")]
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04000DA9 RID: 3497
	[SerializeField]
	private AnimationCurve degreesPerSecondAtSpeed;

	// Token: 0x04000DAA RID: 3498
	[SerializeField]
	private bool clockwise;

	// Token: 0x04000DAB RID: 3499
	[Tooltip("The Y component of the reported speed will be multiplied by this value. At 0, falling will have no effect on the rotation speed.")]
	[SerializeField]
	private float verticalSpeedInfluence = 1f;

	// Token: 0x04000DAC RID: 3500
	[Header("Ticking sound")]
	[Tooltip("After this many degrees of rotation, a \"tick\" sound will play.")]
	[SerializeField]
	private float tickSoundDegrees = 360f;

	// Token: 0x04000DAD RID: 3501
	[SerializeField]
	private AnimationCurve tickVolumeAtSpeed;

	// Token: 0x04000DAE RID: 3502
	[SerializeField]
	private AnimationCurve tickPitchAtSpeed;

	// Token: 0x04000DAF RID: 3503
	[SerializeField]
	private AudioSource tickSound;

	// Token: 0x04000DB0 RID: 3504
	[SerializeField]
	private AudioClip[] tickClips;

	// Token: 0x04000DB1 RID: 3505
	private VRRig rig;

	// Token: 0x04000DB2 RID: 3506
	private Quaternion initialRotation;

	// Token: 0x04000DB3 RID: 3507
	private Vector3 spinAxis;

	// Token: 0x04000DB4 RID: 3508
	private float currentAngle;

	// Token: 0x04000DB5 RID: 3509
	private float tickAngle;
}
